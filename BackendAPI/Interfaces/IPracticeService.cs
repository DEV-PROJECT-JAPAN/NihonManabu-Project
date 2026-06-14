using BackendAPI.DTOs;
using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface IPracticeService
    {
        Task<List<PracticeDTO>> GetVocabularyUserAsync(int FolderId, int UserId);
        Task<List<UserFlashcardListDTO>> GetUserFoldersAsync(int UserId);
        Task<List<VocabularyDTO>> GetVocabularySystemAsync(int inUserId);
        Task<(bool isSuccess, string errorMessage)> UploadFolderExcelAsync(int userId, string folderName, string description, IFormFile file);
        Task<bool> DeleteFolderAsync(int folderId, int userId);
    }
}
