using BackendAPI.Interfaces;
using BackendAPI.DTOs;

namespace BackendAPI.Services
{
    public class PracticeService : IPracticeService
    {
        private readonly List<PracticeDTO> _practices;

        public async Task<List<PracticeDTO>> GetAllPracticesAsync(int IdLesson)
        {
            // Trả về danh sách bài tập mẫu
            //thuoc tinh: Id, Kanji, Hiragana, Romaji, Meaning
            var practices = new List<PracticeDTO>
            {
                new PracticeDTO { Id = 1, IdLesson = 1, Kanji = "日", Hiragana = "にち", Romaji = "nichi", Meaning = "day/sun" },
                new PracticeDTO { Id = 2, IdLesson = 1, Kanji = "月", Hiragana = "げつ", Romaji = "getsu", Meaning = "month/moon" },
                new PracticeDTO { Id = 3, IdLesson = 1, Kanji = "火", Hiragana = "か", Romaji = "ka", Meaning = "fire" },
                new PracticeDTO { Id = 4, IdLesson = 1, Kanji = "水", Hiragana = "すい", Romaji = "sui", Meaning = "water" },
                new PracticeDTO { Id = 5, IdLesson = 1, Kanji = "木", Hiragana = "もく", Romaji = "moku", Meaning = "tree/wood" }
            };

            var filteredPractices = practices.Where(p => p.IdLesson == IdLesson).ToList();

            return await Task.FromResult(filteredPractices);
        }
    }
}
