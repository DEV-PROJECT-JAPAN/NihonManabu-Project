using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Lesson
{
    public class CreateModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public CreateModel(VocabularyClientService service) => _service = service;

        [BindProperty]
        public LessonModel NewLesson { get; set; } = new();

        // Biến này để chứa danh sách Cấp độ hiển thị lên màn hình
        public List<LevelModel> Levels { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Lấy danh sách Level về để lát nữa nhét vào cái menu thả xuống
            Levels = await _service.GetLevelsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Levels = await _service.GetLevelsAsync(); // Lỗi thì cũng phải load lại menu thả xuống
                return Page();
            }

            bool success = await _service.CreateLessonAsync(NewLesson);
            if (success)
            {
                return RedirectToPage("/Admin/Lesson/Index");
            }

            ModelState.AddModelError(string.Empty, "Them moi Bai hoc that bai.");
            Levels = await _service.GetLevelsAsync();
            return Page();
        }
    }
}