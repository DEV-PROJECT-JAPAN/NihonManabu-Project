using BackendAPI.Models.Base;


namespace BackendAPI.Models
{
    public class UserFlashcardList : BaseModels
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Name { get; set; } // Tên danh sách flashcard
        public string Description { get; set; } // Mô tả về danh sách flashcard
        public virtual ICollection<FolderVocabulary> FolderVocabularies { get; set; } = new List<FolderVocabulary>();
    }
}