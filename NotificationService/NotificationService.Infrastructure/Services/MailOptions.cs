namespace NotificationService.Infrastructure.Services;

public class MailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public string From { get; set; } = "noreply@applicant-system.local";
}