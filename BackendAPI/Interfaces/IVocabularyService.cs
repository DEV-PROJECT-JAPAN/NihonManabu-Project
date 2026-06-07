using BackendAPI.DTOs;
using BackendAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace BackendAPI.Interfaces
{
    public interface IVocabularyService
    {
        Task<IEnumerable<VocabularyDTO>> GetVocabulariesByLessonAsync(int lessonId);

        Task<List<LevelDTO>> GetAllLevelsAsync();
        Task<List<LessonDTO>> GetLessonsByLevelAsync(int levelId);
        Task<bool> UpdateProgressAsync(int userId, UpdateLearningProgresByUserDTO input);



        Task<bool> CreateLevelAsync(LevelDTO input);
        Task<LevelDTO> GetLevelByIdAsync(int id);
        Task<bool> UpdateLevelAsync(LevelDTO input);
        Task<bool> DeleteLevelAsync(int id);

        // Bổ sung 4 hàm CRUD cho Lesson
        Task<bool> CreateLessonAsync(LessonDTO input);
        Task<LessonDTO> GetLessonByIdAsync(int id);
        Task<bool> UpdateLessonAsync(LessonDTO input);
        Task<bool> DeleteLessonAsync(int id);

        // --- Bổ sung vào IVocabularyService ---
        Task<List<LessonDTO>> GetAllLessonsAsync(); // Hàm mới để lấy tất cả bài học đổ ra Dropdown
        Task<bool> CreateVocabularyAsync(VocabularyDTO input);
        Task<VocabularyDTO> GetVocabularyByIdAsync(int id);
        Task<bool> UpdateVocabularyAsync(VocabularyDTO input);
        Task<bool> DeleteVocabularyAsync(int id);
        Task<bool> ImportVocabulariesAsync(int lessonId, IFormFile file);
    }
}