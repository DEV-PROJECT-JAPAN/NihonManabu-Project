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
            // Thay "Tên Ứng Dụng" và Email của bạn vào đây
            email.From.Add(new MailboxAddress("NihonManabu System", "dnam2886@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // Thiết lập nội dung Email dạng HTML cho đẹp
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                // Kết nối tới Server SMTP của Gmail qua cổng 587
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // ĐĂNG NHẬP: Điền Email và chuỗi 16 ký tự Mật khẩu ứng dụng vừa tạo ở Bước 1 vào đây
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
