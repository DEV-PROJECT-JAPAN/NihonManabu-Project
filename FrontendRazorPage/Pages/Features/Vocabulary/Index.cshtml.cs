using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Vocabulary
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _service;
        private readonly LevelClientService _levelClientService;
        public IndexModel(VocabularyClientService service, LevelClientService levelClientService)
        {
            _service = service;
            _levelClientService = levelClientService;
        }


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

            Levels = await _levelClientService.GetLevelsAsync();

        }
        //Nhiệm vụ chính của nó là nhận dữ liệu tiến độ học từ vựng
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