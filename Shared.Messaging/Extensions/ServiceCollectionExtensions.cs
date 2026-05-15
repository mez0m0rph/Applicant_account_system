using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Interfaces;
using Shared.Messaging.RabbitMQ;
using Shared.Messaging.Services;

namespace Shared.Messaging.Extensions;

public static class ServiceCollectionExtensions  // расширение DI-контейнера 
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}