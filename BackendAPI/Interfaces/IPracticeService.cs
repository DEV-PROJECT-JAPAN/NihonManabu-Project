using BackendAPI.DTOs;

namespace BackendAPI.Interfaces
{
    public interface IPracticeService
    {
        Task<List<PracticeDTO>> GetVocabularySystemAsync(int LessonId, int UserId);
        Task<List<PracticeDTO>> GetVocabularyUserAsync(int FolderId, int UserId);
        Task<List<UserFlashcardListDTO>> GetUserFoldersAsync(int UserId);
    }
}
