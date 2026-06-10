using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Grammar : BaseModels
    {
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }

        [Required, Column(TypeName = "nvarchar(200)")]
        public string Structure { get; set; } = string.Empty;// Cấu trúc ngữ pháp

        [Column(TypeName = "nvarchar(max)")]
        public string Explanation { get; set; } = string.Empty;// Giải thích ngữ pháp

        public virtual ICollection<Question>? Questions { get; set; } // Câu hỏi liên quan đến ngữ pháp
    }
}
