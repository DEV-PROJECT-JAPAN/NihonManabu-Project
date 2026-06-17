using BackendAPI.Interfaces;
<<<<<<< HEAD
// 🚨 ĐÃ XÓA BỎ "using System.Net.Mail;" để triệt hạ tận gốc lỗi tranh chấp SmtpClient
=======
>>>>>>> feature/login
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BackendAPI.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
<<<<<<< HEAD
            // =========================================================================
            // CHUẨN HÓA: SỬ DỤNG 100% THƯ VIỆN MAILKIT (BỎ LUỒNG TRÙNG LẶP CŨ)
            // =========================================================================

            var email = new MimeMessage();

            // Thiết lập thông tin người gửi (Hiện tại đang dùng tài khoản dnam2886)
=======
            // Thiết lập phong bì thư MimeMessage (của MailKit/MimeKit)
            var email = new MimeMessage();
>>>>>>> feature/login
            email.From.Add(new MailboxAddress("NihonManabu System", "dnam2886@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // Thiết lập nội dung Email dạng HTML
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

<<<<<<< HEAD
            // Lúc này SmtpClient chắc chắn 100% là của MailKit, không còn ai tranh chấp nữa!
            using var smtp = new SmtpClient();
            try
            {
                // 1. Kết nối tới Server SMTP của Gmail qua cổng 587 bằng phương thức bảo mật StartTls
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // 2. ĐĂNG NHẬP: Điền Email và chuỗi 16 ký tự Mật khẩu ứng dụng xịn của anh vào đây
=======
            // Sử dụng MailKit SmtpClient
            using var smtp = new SmtpClient();
            try
            {
                // Kết nối tới Server SMTP của Gmail
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // ĐĂNG NHẬP: Dùng mật khẩu ứng dụng
>>>>>>> feature/login
                await smtp.AuthenticateAsync("dnam2886@gmail.com", "ozob xwsy ybts lsbp");

                // 3. Thực hiện gửi bất đồng bộ
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gửi email: {ex.Message}");
            }
            finally
            {
                // 4. Ngắt kết nối an toàn để giải phóng tài nguyên hệ thống
                await smtp.DisconnectAsync(true);
            }
        }
    }
}