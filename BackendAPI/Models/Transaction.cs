using BackendAPI.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Transaction : BaseModels
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }                  // Số tiền nâng cấp VIP
        public string Status { get; set; } = "Pending";       // 'Pending', 'Completed', 'Failed'
        public string? PaymentMethod { get; set; }           // 'VNPAY', 'Momo', 'BankTransfer'

        // Navigation Property: Mỗi giao dịch phải thuộc về một User cụ thể
    public virtual User User { get; set; } = null!;
    }
}
