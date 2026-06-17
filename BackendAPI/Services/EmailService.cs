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

            // Thiết lập phong bì thư MimeMessage (của MailKit/MimeKit)
            email.From.Add(new MailboxAddress("NihonManabu System", "dnam2886@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // Sử dụng MailKit SmtpClient
            using var smtp = new SmtpClient();
            try
            {
                // 1. Kết nối tới Server SMTP của Gmail
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // 2. ĐĂNG NHẬP: Dùng email và mật khẩu ứng dụng xịn
                await smtp.AuthenticateAsync("dnam2886@gmail.com", "ozob xwsy ybts lsbp");

                // 3. Gửi email đi
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gửi email: {ex.Message}");
            }
            finally
            {
                // 4. Ngắt kết nối an toàn
                await smtp.DisconnectAsync(true);
            }
        }
    }
}