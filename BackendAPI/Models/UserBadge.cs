using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class UserBadge : BaseModels
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BadgeId { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.Now; // Ngày đạt được huy hiệu

        // Navigation Properties: Phục vụ cho việc JOIN dữ liệu dễ dàng qua EF Core
        public virtual User User { get; set; } = null!;
        public virtual Badge Badge { get; set; } = null!;
    }
}
