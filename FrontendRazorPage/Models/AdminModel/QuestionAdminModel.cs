using FrontendRazorPage.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontendRazorPage.Models.AdminModel
{
    public class QuestionAdminModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng liên kết với một mẫu ngữ pháp!")]
        public int? GrammarId { get; set; }

        [Required(ErrorMessage = "Nội dung câu hỏi không được để trống!")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại câu hỏi!")]
        public int QuestionType { get; set; } = 1; // 1: Trắc nghiệm, 2: Điền vào chỗ trống, 3: Tự luận

        // 🔄 Danh sách các đáp án đi kèm (Đồng bộ trực tiếp dạng lồng nhau)
        public List<AnswerAdminModel> Answers { get; set; } = new List<AnswerAdminModel>();

        // 📅 Các thuộc tính kế thừa từ BaseModels hệ thống
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
