using BackendAPI.Interfaces;
using BackendAPI.DTOs;
using BackendAPI.Models.Data;

namespace BackendAPI.Services
{
    public class PracticeService : IPracticeService
    {
        private readonly JapaneseDbContext _context;

        public PracticeService(JapaneseDbContext context)
        {
            _context = context;
        }

        //danh sách folder mà user đã tạo
        public async Task<List<UserFlashcardListDTO>> GetUserFoldersAsync(int UserId)
        {
            return await Task.Run(() =>
            {
                var userFlashcardLists = _context.UserFlashcardLists.Where(p => p.UserId == UserId).ToList();
                return userFlashcardLists.Select(p => new UserFlashcardListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToList();
            });
        }

        // ôn tập : lấy tất cả từ vựng mà user đã thêm vào flashcard list của họ
        public async Task<List<PracticeDTO>> GetVocabularyUserAsync(int FolderId, int UserId)
        {
            return await Task.Run(() =>
            {
                var listId = _context.UserFlashcardLists.Where(p => p.Id == FolderId && p.UserId == UserId).Select(p => p.Id);
                var practicesUserVocabulary = _context.UserVocabularies.Where(p => listId.Contains(p.ListId)).ToList();
                return practicesUserVocabulary.Select(p => new PracticeDTO
                {
                    Id = p.Id,
                    ListId = p.ListId,
                    Kanji = p.Kanji,
                    Hiragana = p.Hiragana,
                    Romaji = p.Romaji,
                    Meaning = p.Meaning
                }).ToList();
            });
        }

        //ôn tập : lấy tất cả từ vựng mà user đã học trong phần Learning Progress
        public async Task<List<PracticeDTO>> GetVocabularySystemAsync(int LessonId, int UserId)
        {
            return await Task.Run(() =>
            {
                var listId = _context.UserFlashcardLists.Where(p => p.Id == LessonId && p.UserId == UserId).Select(p => p.Id);
                var practicesSystemVocabulary = _context.UserVocabularies.Where(p => listId.Contains(p.ListId)).ToList();
                return practicesSystemVocabulary.Select(p => new PracticeDTO
                {
                    Id = p.Id,
                    ListId = p.ListId,
                    Kanji = p.Kanji,
                    Hiragana = p.Hiragana,
                    Romaji = p.Romaji,
                    Meaning = p.Meaning
                }).ToList();
            });
        }
    }
}
