using BackendAPI.Interfaces;
using System.Net;
using System.Net.Mail;
namespace BackendAPI.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
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
        }
    }
}
