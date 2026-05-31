using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Vocabulary
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public IndexModel(VocabularyClientService service) => _service = service;

        // "State" của Component
        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();
        public List<VocabularyModel> Cards { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? LevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? LessonId { get; set; }

        // ngOnInit của Angular
        public async Task OnGetAsync()
        {
            if (LevelId.HasValue && LessonId.HasValue)
            {
                Cards = await _service.GetCardsAsync(LessonId.Value);
            }
            else if (LevelId.HasValue)
            {
                Lessons = await _service.GetLessonsAsync(LevelId.Value);
            }
            else
            {
                Levels = await _service.GetLevelsAsync();
            }
        }

        public async Task<JsonResult> OnPostUpdateProgressAsync([FromBody] UpdateLearningProgresByUserModel input)
        {
            if (input == null || input.VocabularyId <= 0)
                return new JsonResult(new { success = false });

            // Đẩy việc kết nối mạng HttpClient xuống cho tầng Service gánh vác
            bool success = await _service.UpdateProgressAsync(input);
            return new JsonResult(new { success = success });
        }
    }
}