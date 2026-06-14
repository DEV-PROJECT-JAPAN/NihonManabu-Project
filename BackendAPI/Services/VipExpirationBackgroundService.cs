using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
namespace BackendAPI.Services
{
    public class VipExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<VipExpirationBackgroundService> _logger;

        public VipExpirationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<VipExpirationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🤖 Quản đốc kiểm tra hạn sử dụng VIP đã khởi động!");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndDowngradeVipAsync();

                // Quét 1 giờ 1 lần. (Nếu test anh có thể đổi thành TimeSpan.FromMinutes(1))
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckAndDowngradeVipAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<JapaneseDbContext>();

                // 1. Tìm những gói VIP đang Active nhưng thời gian đã vượt quá hiện tại
                var expiredSubs = await context.UserSubscriptions
                    .Where(s => s.IsActive == true && s.ExpiresAt <= DateTime.Now)
                    .ToListAsync();

                if (!expiredSubs.Any()) return; // Không có ai hết hạn thì đi ngủ tiếp

                // 2. Tước quyền từng người
                foreach (var sub in expiredSubs)
                {
                    sub.IsActive = false; // Tắt trạng thái gói

                    var user = await context.Users.FindAsync(sub.UserId);
                    if (user != null && user.Role == "VIP")
                    {
                        user.Role = "User"; // Giáng cấp về dân thường
                        _logger.LogInformation($"⬇️ Đã giáng cấp User '{user.UserName}' về tài khoản thường do hết hạn VIP.");
                    }
                }

                // 3. Lưu xuống Database
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi quét hạn VIP: {ex.Message}");
            }
        }
    }
}
