using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Vocabulary
{
    public class CreateModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public CreateModel(VocabularyClientService service) => _service = service;

        [BindProperty]
        public VocabularyModel NewVocab { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        public async Task OnGetAsync()
        {
            Lessons = await _service.GetAllLessonsAdminAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Lessons = await _service.GetAllLessonsAdminAsync();
                return Page();
            }

            bool success = await _service.CreateVocabularyAsync(NewVocab);
            if (success) return RedirectToPage("/Admin/Vocabulary/Index", new { lessonId = NewVocab.LessonId });

            ModelState.AddModelError(string.Empty, "Thêm từ vựng thất bại.");
            Lessons = await _service.GetAllLessonsAdminAsync();
            return Page();
        }
    }
}