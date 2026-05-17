using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Shared.Messaging.Interfaces;

namespace Shared.Messaging.Services;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConfiguration _configuration;
    private readonly string _queueName;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        _queueName = configuration["RabbitMq:QueueName"] ?? "notification-queue";
    }

    public Task PublishAsync<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"] ?? "localhost",
            UserName = _configuration["RabbitMq:UserName"] ?? "guest",
            Password = _configuration["RabbitMq:Password"] ?? "guest",
            Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }
}
