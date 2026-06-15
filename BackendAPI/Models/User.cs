using BackendAPI.Models.Base;

namespace BackendAPI.Models
{
    public class User :BaseModels
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // --- CÁC TRƯỜNG NÂNG CẤP MỚI KHỚP VỚI DATABASE ---
        public string Role { get; set; } = "User";            // 'User', 'VIP', 'Admin'
        public int TotalExp { get; set; } = 0;                // Điểm kinh nghiệm tích lũy để xếp hạng
        public int CurrentStreak { get; set; } = 0;           // Chuỗi ngày học liên tục (Duolingo style)
        public DateTime? LastActiveDate { get; set; }         // Ngày cuối cùng học bài để check Streak

        public string? VerificationToken { get; set; }   // Lưu mã OTP (Ví dụ: "193582") hoặc chuỗi GUID bảo mật
        public DateTime? TokenExpiresAt { get; set; }   // Thời gian hết hạn của mã (thường là 5 - 10 phút)
        public bool IsEmailConfirmed { get; set; } = false; // Đánh dấu xem Email này đã được xác thực hay chưa

        public DateTime? LastTokenSentAt { get; set; }  // Thời gian gửi mã cuối cùng (Dùng để chặn user bấm gửi lại liên tục - Anti Spam)

        // --- NAVIGATION PROPERTIES (Mối quan hệ giữa các bảng) ---

        // Một User có thể tự tạo nhiều từ vựng cá nhân (Sổ tay chủ động)
        public virtual ICollection<UserFlashcardList> UserFlashcardLists { get; set; } = new List<UserFlashcardList>();

        // Một User có nhiều dòng ghi nhận tiến độ học của các từ vựng trong giáo trình
        public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();

        // Một User có thể sở hữu nhiều Huy hiệu khác nhau qua bảng trung gian
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

    }
}
