using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Services
{
    public class VocabularyService : IVocabularyService
    {
        private readonly JapaneseDbContext _context;
        public VocabularyService(JapaneseDbContext context) => _context = context;

        public async Task<List<LevelDTO>> GetAllLevelsAsync()
        {
           return await _context.Levels.Select(l => new LevelDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description
                })
                .ToListAsync();
        }

        public async Task<List<LessonDTO>> GetLessonsByLevelAsync(int levelId)
        {
            return await _context.Lessons
                .Where(l => l.LevelId == levelId)
                .OrderBy(l => l.Order)
                .Select(l => new LessonDTO
                {
                    Id = l.Id,
                    LevelId = l.LevelId,
                    Title = l.Title,
                    Order = l.Order
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<VocabularyDTO>> GetVocabulariesByLessonAsync(int lessonId)
        {
            return await _context.Vocabularies
                 .Where(v => v.LessonId == lessonId)
                 .Select(v => new VocabularyDTO
                 {
                     Id = v.Id,
                     LessonId = v.LessonId,
                     Kanji = v.Kanji,
                     Hiragana = v.Hiragana,
                     Meaning = v.Meaning,
                     Romaji = v.Romaji,
                     ExampleSentence = v.ExampleSentence
                 })
                 .ToListAsync();
        }

         

        public async Task<bool> UpdateProgressAsync(int userId, UpdateLearningProgresByUserDTO input)
        {
            var progress = await _context.LearningProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.VocabularyId == input.VocabularyId);

            if (progress == null)
            {
                _context.LearningProgresses.Add(new LearningProgress
                {
                    UserId = userId,
                    VocabularyId = input.VocabularyId,
                    IsMasstered = input.IsMastered,
                    LastReviewed = DateTime.Now,
                    ReviewCount = 1 // Lần đầu tiên tương tác thì khởi tạo bằng 1
                });
            }
            else
            {
                progress.IsMasstered = input.IsMastered;
                progress.LastReviewed = DateTime.Now;
                progress.ReviewCount += 1; // Các lần sau bấm lại thì tự động cộng dồn lên 1 đơn vị
                _context.LearningProgresses.Update(progress);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}