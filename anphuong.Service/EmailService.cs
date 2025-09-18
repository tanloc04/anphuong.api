using System.Management;
using anphuong.Core.Constants;
using anphuong.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace anphuong.Service
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailService()
        {
            _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER")
                ?? throw new InvalidOperationException("SMTP_SERVER environment variable is not set.");

            var smtpPortString = Environment.GetEnvironmentVariable("SMTP_PORT")
                ?? throw new InvalidOperationException("SMTP_PORT environment variable is not set.");
            if (!int.TryParse(smtpPortString, out _smtpPort))
                throw new InvalidOperationException("SMTP_PORT environment variable is not a valid number.");

            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")
                ?? throw new InvalidOperationException("SMTP_USER environment variable is not set.");

            _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                ?? throw new InvalidOperationException("SMTP_PASSWORD environment variable is not set.");
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage()
            {
                From = { new MailboxAddress(Consts.COMPANY_NAME, _smtpUser) },
                To = { new MailboxAddress(string.Empty, email) },
                Subject = subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message }
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}