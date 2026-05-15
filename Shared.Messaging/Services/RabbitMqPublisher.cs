using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Shared.Messaging.Services;

public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly ConnectionFactory _factory;
    private readonly string _queueName;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _queueName = configuration["RabbitMq:QueueName"] ?? "notification-queue";

        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };

        _connection = _factory.CreateConnection();
    }

    public Task PublishAsync<T>(T message)
    {
        using var channel = _connection.CreateModel();

        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: _queueName,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}