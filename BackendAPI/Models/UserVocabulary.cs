using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    //=========================================================
    // Bảng lưu các từ vựng mà người dùng cần ôn tập
    //=========================================================
    [Table("UserVocabularies")]
    public class UserVocabulary: BaseModels
    {
      
        // Khóa ngoại nối sang bảng UserFlashcardLists (Thay thế hoàn toàn ListId của thằng Item cũ)
        [Required]
        public int ListId { get; set; }

        [Required]
        [StringLength(100)]
        public string Kanji { get; set; }

        [Required]
        [StringLength(100)]
        public string Hiragana { get; set; }

        [Required]
        [StringLength(500)]
        public string Meaning { get; set; }

        [StringLength(100)]
        public string Romaji { get; set; }

        [StringLength(500)]
        public string AudioUrl { get; set; }

        [StringLength(1000)]
        public string ExampleSentence { get; set; }

        // =======================================================
        // CỤM TRƯỜNG QUẢN LÝ TIẾN TRÌNH & THUẬT TOÁN NGẮT QUÃNG (SPACED REPETITION)
        // =======================================================

        [Required]
        public bool IsMastered { get; set; } = false; // Mặc định từ mới nạp vào là Chưa thuộc

        [Required]
        public int ReviewCount { get; set; } = 0; // Số lần bấm ôn tập

        [Required]
        public DateTime LastReviewed { get; set; } = DateTime.UtcNow; // Ngày giờ vừa bấm ôn tập gần nhất

       
        // =======================================================
        // NAVIGATION PROPERTY (Mối quan hệ trong EF Core)
        // =======================================================

        [ForeignKey("ListId")]
        public virtual UserFlashcardList UserFlashcardList { get; set; }
    }
}