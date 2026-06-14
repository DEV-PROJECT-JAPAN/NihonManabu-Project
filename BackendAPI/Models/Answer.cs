using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Answer : BaseModels
    {
        public int  QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question? Question { get; set; }

        [Required, Column(TypeName = "nvarchar(500)")]
        public string Text { get; set; } // Nội dung câu trả lời
        public bool IsCorrect { get; set; } // Đánh dấu câu trả lời đúng hay sai
        public string DisplayOrder { get; set; } // Thứ tự hiển thị câu trả lời (A, B, C, D...)
        public int OrderId { get; internal set; }
    }
}
