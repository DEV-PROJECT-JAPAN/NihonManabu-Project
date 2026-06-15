using System.ComponentModel.DataAnnotations;

namespace FrontendRazorPage.Models.AdminModel
{
    public class AnswerAdminModel
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Nội dung câu trả lời không được để trống!")]
        [StringLength(500, ErrorMessage = "Nội dung không được vượt quá 500 ký tự!")]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public string DisplayOrder { get; set; } = string.Empty; // Thứ tự hiển thị (A, B, C, D...)

        public int OrderId { get; set; }

        // 📅 Các thuộc tính kế thừa từ BaseModels hệ thống (để hiển thị nếu cần)
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


