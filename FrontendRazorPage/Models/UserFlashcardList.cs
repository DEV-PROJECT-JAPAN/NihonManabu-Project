using BackendAPI.DTOs;


namespace BackendAPI.DTOs
{
    public class UserFlashcardList
    {
        public int Id { get; set; }
        public string Name { get; set; } // Tên danh sách flashcard
        public string Description { get; set; } // Mô tả về danh sách flashcard
    }
}
