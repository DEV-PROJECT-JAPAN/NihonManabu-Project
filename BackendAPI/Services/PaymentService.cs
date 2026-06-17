using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BackendAPI.Interfaces;

namespace BackendAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly JapaneseDbContext _context;
        private readonly IConfiguration _config;

        public PaymentService(JapaneseDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> GenerateVipPaymentQrAsync(int userId)
        {
            decimal vipPrice = 1000;

            // Tìm hoặc tạo bill Pending
            var pendingTx = await _context.Transactions
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Status == "Pending" && t.Amount == vipPrice);

            if (pendingTx == null)
            {
                _context.Transactions.Add(new Transaction
                {
                    UserId = userId,
                    Amount = vipPrice,
                    Status = "Pending",
                    PaymentMethod = "VietQR",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            string bankId = _config["VietQRConfig:BankId"];
            string accountNo = _config["VietQRConfig:AccountNo"];
            string accountName = Uri.EscapeDataString(_config["VietQRConfig:AccountName"]);
            string template = _config["VietQRConfig:Template"];
            string addInfo = Uri.EscapeDataString($"VIP {userId}");

            return $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png?amount={vipPrice}&addInfo={addInfo}&accountName={accountName}";
        }
        public async Task<bool> CheckIsVipAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null && user.Role == "VIP";
        }
    }
}