using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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



        #region ================= ADMIN LEVEL CRUD =================

        public async Task<bool> CreateLevelAsync(LevelDTO input)
        {
            // 1. Chuyển đổi từ dữ liệu truyền vào (DTO) sang thực thể Database (Model)
            var newLevel = new Level
            {
                Name = input.Name,
                Description = input.Description
            };

            // 2. Thêm vào bảng Levels và lưu thay đổi
            _context.Levels.Add(newLevel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<LevelDTO> GetLevelByIdAsync(int id)
        {
            // Lấy 1 dòng dữ liệu dựa vào khóa chính ID
            var level = await _context.Levels.FindAsync(id);

            if (level == null) return null;

            // Map ngược từ Model ra DTO để trả về cho Frontend
            return new LevelDTO
            {
                Id = level.Id,
                Name = level.Name,
                Description = level.Description
            };
        }

        public async Task<bool> UpdateLevelAsync(LevelDTO input)
        {
            // 1. Tìm xem cấp độ đó có tồn tại trong DB không
            var level = await _context.Levels.FindAsync(input.Id);
            if (level == null) return false;

            // 2. Cập nhật các trường dữ liệu
            level.Name = input.Name;
            level.Description = input.Description;

            // 3. Lưu xuống Database
            _context.Levels.Update(level);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLevelAsync(int id)
        {
            // 1. Tìm dòng dữ liệu cần xóa
            var level = await _context.Levels.FindAsync(id);
            if (level == null) return false;

            // 2. Đánh dấu xóa và lưu lại
            _context.Levels.Remove(level);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region ================= ADMIN LESSON CRUD =================

        public async Task<bool> CreateLessonAsync(LessonDTO input)
        {
            var newLesson = new Lesson
            {
                LevelId = input.LevelId, // Nhớ map Khóa ngoại
                Title = input.Title,
                Order = input.Order
            };

            _context.Lessons.Add(newLesson);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<LessonDTO> GetLessonByIdAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return null;

            return new LessonDTO
            {
                Id = lesson.Id,
                LevelId = lesson.LevelId,
                Title = lesson.Title,
                Order = lesson.Order
            };
        }

        public async Task<bool> UpdateLessonAsync(LessonDTO input)
        {
            var lesson = await _context.Lessons.FindAsync(input.Id);
            if (lesson == null) return false;

            lesson.LevelId = input.LevelId;
            lesson.Title = input.Title;
            lesson.Order = input.Order;

            _context.Lessons.Update(lesson);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return false;

            _context.Lessons.Remove(lesson);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
        #region ================= ADMIN VOCABULARY CRUD =================

        public async Task<List<LessonDTO>> GetAllLessonsAsync()
        {
            return await _context.Lessons
                .OrderBy(l => l.LevelId).ThenBy(l => l.Order)
                .Select(l => new LessonDTO
                {
                    Id = l.Id,
                    LevelId = l.LevelId,
                    Title = l.Title,
                    Order = l.Order
                }).ToListAsync();
        }

        public async Task<bool> CreateVocabularyAsync(VocabularyDTO input)
        {
            var newVocab = new Vocabulary
            {
                LessonId = input.LessonId,
                Kanji = input.Kanji,
                Hiragana = input.Hiragana,
                Romaji = input.Romaji,
                Meaning = input.Meaning,
                ExampleSentence = input.ExampleSentence
            };

            _context.Vocabularies.Add(newVocab);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<VocabularyDTO> GetVocabularyByIdAsync(int id)
        {
            var vocab = await _context.Vocabularies.FindAsync(id);
            if (vocab == null) return null;

            return new VocabularyDTO
            {
                Id = vocab.Id,
                LessonId = vocab.LessonId,
                Kanji = vocab.Kanji,
                Hiragana = vocab.Hiragana,
                Romaji = vocab.Romaji,
                Meaning = vocab.Meaning,
                ExampleSentence = vocab.ExampleSentence
            };
        }

        public async Task<bool> UpdateVocabularyAsync(VocabularyDTO input)
        {
            var vocab = await _context.Vocabularies.FindAsync(input.Id);
            if (vocab == null) return false;

            vocab.LessonId = input.LessonId;
            vocab.Kanji = input.Kanji;
            vocab.Hiragana = input.Hiragana;
            vocab.Romaji = input.Romaji;
            vocab.Meaning = input.Meaning;
            vocab.ExampleSentence = input.ExampleSentence;

            _context.Vocabularies.Update(vocab);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVocabularyAsync(int id)
        {
            var vocab = await _context.Vocabularies.FindAsync(id);
            if (vocab == null) return false;

            _context.Vocabularies.Remove(vocab);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
        public async Task<bool> ImportVocabulariesAsync(int lessonId, IFormFile file)
        {
            if (file == null || file.Length == 0) return false;

            var vocabulariesToAdd = new List<Vocabulary>();
            var extension = Path.GetExtension(file.FileName).ToLower();

            using var stream = file.OpenReadStream();

            try
            {
                if (extension == ".json")
                {
                    // XỬ LÝ FILE JSON
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var importedData = await JsonSerializer.DeserializeAsync<List<VocabularyDTO>>(stream, options);

                    if (importedData != null)
                    {
                        foreach (var item in importedData)
                        {
                            vocabulariesToAdd.Add(new Vocabulary
                            {
                                LessonId = lessonId, // Ép vào bài học mà Admin đã chọn
                                Kanji = item.Kanji,
                                Hiragana = item.Hiragana,
                                Romaji = item.Romaji,
                                Meaning = item.Meaning,
                                ExampleSentence = item.ExampleSentence
                            });
                        }
                    }
                }
                else if (extension == ".csv")
                {
                    // XỬ LÝ FILE CSV (Cấu trúc mong đợi: Kanji,Hiragana,Romaji,Meaning,ExampleSentence)
                    using var reader = new StreamReader(stream);
                    bool isHeader = true; // Bỏ qua dòng tiêu đề

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        var values = line.Split(',');

                        // Cần ít nhất 4 cột (Hiragana, Romaji, Meaning là bắt buộc)
                        if (values.Length >= 4)
                        {
                            vocabulariesToAdd.Add(new Vocabulary
                            {
                                LessonId = lessonId,
                                Kanji = string.IsNullOrWhiteSpace(values[0]) ? null : values[0].Trim(),
                                Hiragana = values[1].Trim(),
                                Romaji = values[2].Trim(),
                                Meaning = values[3].Trim(),
                                ExampleSentence = values.Length >= 5 ? values[4].Trim() : null
                            });
                        }
                    }
                }
                else
                {
                    // Không hỗ trợ định dạng khác
                    return false;
                }

                if (vocabulariesToAdd.Any())
                {
                    _context.Vocabularies.AddRange(vocabulariesToAdd);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false; // Lỗi đọc file hoặc sai định dạng
            }
        }
    }
}