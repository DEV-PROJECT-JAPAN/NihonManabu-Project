using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class LearningProgress : BaseModels
    {

        public virtual User User { get; set; }             
        public virtual Vocabulary Vocabulary { get; set; }
        
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("VocabularyId")]
        public int VocabularyId { get; set; }
        public bool IsMasstered { get; set; } // Đánh dấu đã học thuộc hay chưa
        public int ReviewCount { get; set; } // Số lần ôn tập
        public DateTime LastReviewed { get; set; } // Lần cuối ôn tập
    }
}
