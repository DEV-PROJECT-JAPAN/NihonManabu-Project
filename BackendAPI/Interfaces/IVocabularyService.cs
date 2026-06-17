using BackendAPI.DTOs;
using BackendAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendAPI.Interfaces
{
    public interface IVocabularyService
    {
        Task<IEnumerable<VocabularyDTO>> GetVocabulariesByLessonAsync(int lessonId);
        Task<List<LevelDTO>> GetAllLevelsAsync();
        Task<List<LessonDTO>> GetLessonsByLevelAsync(int levelId);
        Task<bool> UpdateProgressAsync(int userId, UpdateLearningProgresByUserDTO input);

        // ==================== 🟢 BỔ SUNG THÊM CÁC HÀM CRUD DÀNH CHO ADMIN ====================

      
        Task<IEnumerable<Vocabulary>> GetVocabulariesByLessonForAdminAsync(int lessonId);
        Task<Vocabulary?> GetVocabByIdAsync(int id);
        Task<Vocabulary> CreateVocabularyAsync(Vocabulary vocabData);
        Task<bool> UpdateVocabularyAsync(int id, Vocabulary vocabData);

    
        Task<bool> DeleteVocabularyAsync(int id);
        Task<byte[]> ExportVocabularyPdfAsync(int lessonId);
    }
}