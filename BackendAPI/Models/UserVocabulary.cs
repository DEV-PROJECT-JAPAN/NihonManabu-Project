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
        public string? Romaji { get; set; } // Thêm dấu ?

        [StringLength(500)]
        public string? AudioUrl { get; set; } = null; // Thêm dấu ?

        [StringLength(1000)]
        public string? ExampleSentence { get; set; } = null; // Thêm dấu ?

        // =======================================================
        // CỤM TRƯỜNG QUẢN LÝ TIẾN TRÌNH & THUẬT TOÁN NGẮT QUÃNG (SPACED REPETITION)
        // =======================================================

        [Required]
        public bool IsMastered { get; set; } = false; // Mặc định từ mới nạp vào là Chưa thuộc

        [Required]
        public int ReviewCount { get; set; } = 0; // Số lần bấm ôn tập

        [Required]
        public DateTime LastReviewed { get; set; } = DateTime.UtcNow; // Ngày giờ vừa bấm ôn tập gần nhất

        public virtual ICollection<FolderVocabulary> FolderVocabularies { get; set; } = new List<FolderVocabulary>();

    }
}