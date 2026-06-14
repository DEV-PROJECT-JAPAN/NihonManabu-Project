using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly JapaneseDbContext _context;

        public ReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ReminderBackgroundService> logger, JapaneseDbContext context)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🤖 Quản đốc tuần tra nhắc nhở học tập đã kích hoạt thành công!");

            // Vòng lặp vô tận duy trì vòng đời của Bot chạy ngầm cùng Server
            while (!stoppingToken.IsCancellationRequested)
            {
                await ScanAndSendRemindersAsync();

                // ⏱️ THỜI GIAN TEST CHỨC NĂNG: Cứ đúng 1 phút con Bot sẽ tự động đi quét DB một lần
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ScanAndSendRemindersAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // 1. Mốc thời gian test: Quá 1 phút không online
                var timeThreshold = DateTime.Now.AddMinutes(-1);

                // 2. CHỈ QUÉT ĐÚNG BẢNG USERS: Lấy những ai vắng mặt quá 1 phút
                var lazyUsers = await _context.Users
                    .Where(u => u.LastActiveDate <= timeThreshold)
                    .ToListAsync();

                // Nếu ai cũng chăm chỉ online thì đi ngủ tiếp
                if (!lazyUsers.Any()) return;

                // 3. Duyệt qua từng User và gửi mail
                foreach (var user in lazyUsers)
                {
                    if (string.IsNullOrEmpty(user.Email)) continue;

                    string subject = "🔔 [Nihongo] Đã đến giờ ôn tập tiếng Nhật rồi!";

                    // Nội dung mail chung chung vì không còn gọi bảng từ vựng nữa
                    string body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 25px; border: 1px solid #e0e0e0; border-radius: 12px; max-width: 550px; margin: auto; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
                            <h2 style='color: #0dcaf0; margin-top: 0;'>Chào {user.UserName},</h2>
                            <p style='color: #555; line-height: 1.6;'>Hệ thống nhận thấy đã lâu bạn chưa truy cập vào ứng dụng.</p>
                            <p style='color: #555; line-height: 1.6;'>Việc học ngoại ngữ cần sự kiên trì mỗi ngày. Hãy dành ra 2 phút vào ứng dụng lắc thẻ Gacha để duy trì chuỗi ngày học <b>Streak {user.CurrentStreak} ngày</b> nhé!</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='#' style='background-color: #0dcaf0; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px; display: inline-block;'>Vào Học Ngay</a>
                            </div>
                            <hr style='border: none; border-top: 1px solid #eeeeee; margin: 25px 0;'>
                            <small style='color: #999; display: block; text-align: center;'>Đây là thông báo tự động từ hệ thống học tiếng Nhật Alpha PGR.</small>
                        </div>";

                    // Bắn email
                    await emailService.SendEmailAsync(user.Email, subject, body);

                    _logger.LogInformation($"➡️ Hệ thống đã bắn email nhắc nhở thành công tới: {user.Email}");

                    // 🚨 [QUAN TRỌNG - CHỐNG SPAM LOOP] 🚨
                    // Để tránh việc vòng lặp 1 phút chạy lại gửi mail y chang cho người này (khiến họ nhận hàng chục mail),
                   
                    user.LastActiveDate = DateTime.Now;
                }

                // Lưu lại việc reset thời gian vào DB
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi thực thi tuần tra: {ex.Message}");
            }
        }
    }
}
