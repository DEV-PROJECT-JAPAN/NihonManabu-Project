using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Vocabulary
{
    public class EditModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public EditModel(VocabularyClientService service) => _service = service;

        [BindProperty]
        public VocabularyModel VocabData { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Lessons = await _service.GetAllLessonsAdminAsync();
            var data = await _service.GetVocabularyByIdAsync(id);
            if (data == null) return RedirectToPage("/Admin/Vocabulary/Index");

            VocabData = data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Lessons = await _service.GetAllLessonsAdminAsync();
                return Page();
            }

            bool success = await _service.UpdateVocabularyAsync(VocabData);
            if (success) return RedirectToPage("/Admin/Vocabulary/Index", new { lessonId = VocabData.LessonId });

            ModelState.AddModelError(string.Empty, "Cập nhật từ vựng thất bại.");
            Lessons = await _service.GetAllLessonsAdminAsync();
            return Page();
        }
    }
}