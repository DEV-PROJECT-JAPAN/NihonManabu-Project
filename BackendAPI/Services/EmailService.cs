using BackendAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BackendAPI.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("NihonManabu System", "dnam2886@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // CHỈ DÙNG 1 KHỐI USING Ở ĐÂY
            using var smtp = new SmtpClient();
            try
            {
                // 1. Kết nối
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // 2. Đăng nhập
                await smtp.AuthenticateAsync("dnam2886@gmail.com", "ozob xwsy ybts lsbp");

                // 3. Gửi
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gửi email: {ex.Message}");
            }
            finally
            {
                // 4. Ngắt kết nối
                await smtp.DisconnectAsync(true);
            }
        }
    }
}