using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BackendAPI.Models
{
    public class UserSubscription : BaseModels
    {
        [Required]
        public int UserId { get; set; } // Trỏ về User nhưng không thèm Navigation để tránh đụng file User.cs

        [Required]
        public DateTime ExpiresAt { get; set; } // Ngày giờ hết hạn VIP

        public bool IsActive { get; set; } = false; // Trạng thái gói VIP (Còn hạn hay không)

        //foreign key thì sẽ có UserId nhưng không cần navigation property vì đã có UserId rồi, tránh đụng file User.cs
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
