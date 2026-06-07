using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Vocabulary
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _service;
        public IndexModel(VocabularyClientService service) => _service = service;

        public List<LessonModel> Lessons { get; set; } = new();
        public List<VocabularyModel> Vocabularies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? LessonId { get; set; }

        public async Task OnGetAsync()
        {
            // Nạp danh sách Bài học cho Dropdown lọc
            Lessons = await _service.GetAllLessonsAdminAsync();

            if (Lessons.Any())
            {
                LessonId ??= Lessons.First().Id;
                // Lấy từ vựng theo ID Bài học được chọn
                Vocabularies = await _service.GetCardsAsync(LessonId.Value);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int currentLessonId)
        {
            bool success = await _service.DeleteVocabularyAsync(id);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi xóa từ vựng.");
                Lessons = await _service.GetAllLessonsAdminAsync();
                Vocabularies = await _service.GetCardsAsync(currentLessonId);
                return Page();
            }
            // Xóa xong giữ nguyên bộ lọc Bài học
            return RedirectToPage("/Admin/Vocabulary/Index", new { lessonId = currentLessonId });
        }


        // Bổ sung thuộc tính để hứng file từ HTML
        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        // Thêm hàm hứng sự kiện Import
        public async Task<IActionResult> OnPostImportAsync(int currentLessonId)
        {
            if (UploadFile == null || UploadFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn một file hợp lệ!");
            }
            else
            {
                bool success = await _service.ImportVocabulariesAsync(currentLessonId, UploadFile);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Import thất bại! Hãy chắc chắn file là .csv hoặc .json và đúng cấu trúc.");
                }
            }

            // Load lại dữ liệu và giữ nguyên bài học
            Lessons = await _service.GetAllLessonsAdminAsync();
            Vocabularies = await _service.GetCardsAsync(currentLessonId);
            LessonId = currentLessonId;
            return Page();
        }
    }
}
