using BackendAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace BackendAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            var senderEmail =
                _configuration["EmailSettings:SenderEmail"];

            var senderName =
                _configuration["EmailSettings:SenderName"];

            var appPassword =
                _configuration["EmailSettings:AppPassword"];

            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    senderName,
                    senderEmail));

            email.To.Add(
                MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                senderEmail,
                appPassword);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}

