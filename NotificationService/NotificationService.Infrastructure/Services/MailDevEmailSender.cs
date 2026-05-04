using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class MailDevEmailSender : IEmailSender
{
    private readonly MailOptions _options;

    public MailDevEmailSender(IOptions<MailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string to, string subject, string message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_options.From));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("plain")
        {
            Text = message
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.None);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}