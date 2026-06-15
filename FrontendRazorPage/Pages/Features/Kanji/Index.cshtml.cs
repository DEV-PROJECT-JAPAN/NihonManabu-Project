using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages
{
    public class KanjiNotebookModel : PageModel
    {
        private readonly VocabularyClientService _vocabularyService;

        // Bạn có thể inject thêm LessonClientService nếu có, 
        // ở đây mình dùng tạm hàm GetVocabulariesByLesson hoặc xử lý lặp để lấy dữ liệu động
        public KanjiNotebookModel(VocabularyClientService vocabularyService)
        {
            _vocabularyService = vocabularyService;
        }

        // Danh sách chữ Hán động
        public List<VocabularyModel> KanjiList { get; set; } = new List<VocabularyModel>();

        // Danh sách Loại từ động lấy từ DB dựa trên các Lesson phát sinh
        public List<dynamic> CategoryList { get; set; } = new List<dynamic>();

        public async Task OnGetAsync()
        {
            // Các ID bài học (Loại từ) thực tế trong database của bạn
            var kanjiLessons = new int[] { 209, 210, 211, 212, 213 };

            // Giả lập tên Category động tương ứng với ID để render ra giao diện (hoặc map từ thực thể Lesson nếu bạn có)
            var lessonNames = new Dictionary<int, string>
            {
                { 209, "Thiên nhiên" },
                { 210, "Con người" },
                { 211, "Số đếm" },
                { 212, "Vị trí & Kích thước" },
                { 213, "Học đường" }
            };

            foreach (var lessonId in kanjiLessons)
            {
                var cards = await _vocabularyService.GetCardsAsync(lessonId);
                if (cards != null && cards.Count > 0)
                {
                    
                    KanjiList.AddRange(cards);

                    // Thêm động danh mục vào list nếu bài học đó có dữ liệu
                    if (lessonNames.ContainsKey(lessonId))
                    {
                        CategoryList.Add(new { key = lessonId.ToString(), label = lessonNames[lessonId] });
                    }
                }
            }
        }
    }
}