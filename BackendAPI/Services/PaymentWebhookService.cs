using BackendAPI.DTOs;
using BackendAPI.Models.Data;
using BackendAPI.Models;
using BackendAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BackendAPI.Services
{
    public class PaymentWebhookService : IPaymentWebhookService
    {
        private readonly JapaneseDbContext _context;
        private readonly ILogger<PaymentWebhookService> _logger;

        public PaymentWebhookService(JapaneseDbContext context, ILogger<PaymentWebhookService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ProcessWebhookAsync(BankWebhookDTO data)
        {
            // 1. DÙNG REGEX TÌM ID NGƯỜI DÙNG TỪ NỘI DUNG CHUYỂN KHOẢN
            string memo = data.Content.ToUpper();
            var match = Regex.Match(memo, @"VIP\s*(\d+)");

            if (!match.Success)
            {
                _logger.LogWarning($"Bỏ qua giao dịch không chứa mã ALPHAVIP. Nội dung: {data.Content}");
                return false;
            }

            int userId = int.Parse(match.Groups[1].Value);

            // 2. MỞ KHÓA DATABASE BẰNG TRANSACTION (Chống lỗi nửa vời)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // BẢNG 1: Kiểm tra User có tồn tại không
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Nhận tiền nhưng không tìm thấy User ID {userId} trong hệ thống!");
                    return false;
                }

                // BẢNG 2: Xử lý bảng Transactions (Tìm giao dịch Pending)
                var pendingTx = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.Status == "Pending");

                if (pendingTx != null)
                {
                    // Cập nhật giao dịch đã có
                    pendingTx.Status = "Success";
                    pendingTx.Amount = data.Amount; // Cập nhật đúng số tiền thực tế nhận được
                    pendingTx.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Lỡ hệ thống không có Pending, tạo luôn bản ghi mới để đối soát
                    _context.Transactions.Add(new Transaction
                    {
                        UserId = userId,
                        Amount = data.Amount,
                        Status = "Success",
                        PaymentMethod = "VietQR",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }

                // KIỂM TRA SỐ TIỀN THỰC TẾ NHẬN ĐƯỢC
                // Nếu khách chuyển thiếu tiền (Ví dụ gói VIP là 50k mà chuyển có 20k)
                if (data.Amount < 50000)
                {
                    _logger.LogWarning($"User {userId} chuyển thiếu tiền ({data.Amount}đ). Đã ghi nhận Transaction nhưng KHÔNG thăng cấp VIP.");
                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return true; // Vẫn trả về true cho Bank biết là đã nhận tín hiệu
                }

                // NẾU ĐỦ TIỀN -> BẢNG 1: Thăng cấp Role
                user.Role = "VIP";
                user.UpdatedAt = DateTime.Now;

                // BẢNG 3: Xử lý bảng UserSubscriptions (Thêm hoặc gia hạn ngày)
                var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId);
                if (subscription != null)
                {
                    // Đã từng là VIP: Cộng dồn nếu còn hạn, hoặc tính từ hôm nay nếu đã hết hạn
                    subscription.ExpiresAt = subscription.ExpiresAt > DateTime.Now
                        ? subscription.ExpiresAt.AddMonths(1)
                        : DateTime.Now.AddMonths(1);
                    subscription.IsActive = true;
                    subscription.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Lần đầu mua VIP
                    _context.Subscriptions.Add(new UserSubscription
                    {
                        UserId = userId,
                        ExpiresAt = DateTime.Now.AddMonths(1),
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }

                // 3. LƯU TOÀN BỘ THAY ĐỔI
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation($"🎉 THÀNH CÔNG: Đã kích hoạt VIP tự động cho User {user.UserName} (ID: {userId})");
                return true;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync(); // Rút lại toàn bộ thay đổi nếu có lỗi
                _logger.LogError($"Lỗi Database khi cấp VIP: {ex.Message}");
                return false;
            }
        }
    }
}