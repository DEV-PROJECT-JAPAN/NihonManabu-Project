using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Vocabulary : BaseModels
    {
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson ? Lesson { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string ? Kanji { get; set; } // Chứ Hán 

        [Required, Column(TypeName = "nvarchar(100)")]
        public string Hiragana { get; set; } // Phiên âm Hiragana

        [Required, Column(TypeName = "nvarchar(255)")]
        public string Meaning { get; set; } // Nghĩa tiếng Việt
    
        public string Romaji { get; set; } // Phiên âm Romaji chữ latin
        public string ?AudioUrl { get; set; } // URL đến file âm thanh phát âm từ vựng
        public string ? ExampleSentence { get; set; } // Câu ví dụ sử dụng từ vựng


    }
}
