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
            // Thiết lập phong bì thư MimeMessage (của MailKit/MimeKit)
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("NihonManabu System", "dnam2886@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // Thiết lập nội dung Email dạng HTML
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // Sử dụng MailKit SmtpClient
            using var smtp = new SmtpClient();
            try
            {
                // Kết nối tới Server SMTP của Gmail
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // ĐĂNG NHẬP: Dùng mật khẩu ứng dụng
                await smtp.AuthenticateAsync("dnam2886@gmail.com", "ozob xwsy ybts lsbp");

                // Thực hiện gửi
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gửi email: {ex.Message}");
            }
            finally
            {
                // Ngắt kết nối an toàn
                await smtp.DisconnectAsync(true);
            }
        }
    }
}