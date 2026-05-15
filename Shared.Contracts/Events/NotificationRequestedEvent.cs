namespace Shared.Contracts.Events;

public class NotificationRequestedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}

// в системе произошло событие - кто-то запросил уведомление; внутри лежит минимальный payload (кому отправить, тема, текст)

// publisher (допустим сервис заказов создал новый заказ). Он не вызывает сервис уведомлений и не отправляет уведомление сам,
    // он просто публикует объект NotificationRequestedEvent в рэббите

// Shared.Contracts - как бы документы, которые пересылаются сервисами, а Shared.Messaging - типо почтового сервиса (службы)