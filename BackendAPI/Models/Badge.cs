using BackendAPI.Models.Base;

namespace BackendAPI.Models
{
    public class Badge : BaseModels
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;           // Tên huy hiệu (Ví dụ: 'Vua Từ Vựng')
        public string? Description { get; set; }            // Mô tả điều kiện để đạt được
        public string? IconUrl { get; set; }                // Đường dẫn tới file ảnh/icon của huy hiệu

        // Navigation Property: Một huy hiệu có thể được sở hữu bởi nhiều người dùng khác nhau
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
