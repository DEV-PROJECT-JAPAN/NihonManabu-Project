using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Question : BaseModels
    {
        public int? GrammarId { get; set; }
        [ForeignKey("GrammarId")]
        public virtual Grammar Grammar { get; set; }
        public string Content { get; set; } // Nội dung câu hỏi

        public int QuestionType { get; set; } // Loại câu hỏi: 1 - Trắc nghiệm, 2 - Điền vào chỗ trống, 3 - Tự luận
        
        public virtual ICollection<Answer> Answers { get; set; }


    }
}
