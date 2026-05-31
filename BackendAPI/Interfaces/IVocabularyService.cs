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
    }
}