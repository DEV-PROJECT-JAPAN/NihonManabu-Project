using BackendAPI.Interfaces;
<<<<<<< HEAD
using System.Net;
using System.Net.Mail;
=======
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


>>>>>>> develop
namespace BackendAPI.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
<<<<<<< HEAD
            // 1. Điền thông tin Email Support mới và Mật khẩu ứng dụng 16 ký tự vào đây
            var fromEmail = "nihongolearning.support@gmail.com";
            //var fromEmail = "support.vankhoi@gmail.com";
            var appPassword = "jzzoyzquvkqoikzq"; // Chuỗi viết liền không dấu cách

            // 2. Thiết lập xe tải vận chuyển SMTP kết nối đến máy chủ Google
            // 2. Thiết lập xe tải vận chuyển SMTP
            using var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;

            // MẤU CHỐT LÀ DÒNG NÀY: Phải tắt mặc định trước khi nạp mật khẩu ứng dụng
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromEmail, appPassword);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            // 3. Đóng gói phong bì thư (Hỗ trợ hiển thị giao diện HTML)
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "Nihongo Learning Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            // 4. Bắn mail đi bất đồng bộ
            await smtpClient.SendMailAsync(mailMessage);
=======
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
>>>>>>> develop
        }
    }
}
