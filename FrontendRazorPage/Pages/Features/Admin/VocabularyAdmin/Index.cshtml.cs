using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.Vocabularies
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _vocabClientService;
        private readonly LevelClientService _levelClientService;
        private readonly LessonClientService _lessonClientService;

        public IndexModel(
            VocabularyClientService vocabClientService,
            LevelClientService levelClientService,
            LessonClientService lessonClientService)
        {
            _vocabClientService = vocabClientService;
            _levelClientService = levelClientService;
            _lessonClientService = lessonClientService;
        }

        // Danh sách hiển thị lên bảng dữ liệu
        public List<VocabularyAdminModel> Vocabularies { get; set; } = new();

        // Dữ liệu nạp vào các thanh bộ lọc Dropdown
        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonAdminModel> Lessons { get; set; } = new();

        // Hai thuộc tính hứng giá trị lọc gửi từ URL (Phương thức GET)
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedLessonId { get; set; }

        [TempData]
        public string Message { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // 1. Luôn nạp danh sách Cấp độ (N5, N4...) cho Dropdown số 1
            Levels = await _levelClientService.GetLevelsAsync();

            // 2. Nếu Admin đã chọn một Cấp độ cụ thể -> Load các Bài học tương ứng của cấp độ đó
            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelForAdminAsync(SelectedLevelId.Value);
            }

            // 3. Logic lấy từ vựng: Chỉ lôi từ vựng ra khi Admin đã chọn đích danh một Bài học cụ thể
            if (SelectedLessonId.HasValue && SelectedLessonId.Value > 0)
            {
                // Gọi API Admin để lấy từ vựng đầy đủ trường ngày tháng
                Vocabularies = await _vocabClientService.GetVocabulariesByLessonForAdminAsync(SelectedLessonId.Value);
            }
            else
            {
                // Nếu chưa chọn Bài học -> Để bảng trống, hướng dẫn Admin chọn bài học (Tránh quá tải dữ liệu)
                Vocabularies = new List<VocabularyAdminModel>();
            }
        }

        // Xử lý xóa từ vựng
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _vocabClientService.DeleteVocabularyAsync(id);
            Message = success ? "Xóa từ vựng thành công!" : "Xóa từ vựng thất bại.";

            // Reload lại trang và giữ nguyên bộ lọc cũ để Admin không phải chọn lại từ đầu
            return RedirectToPage(new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
        }
    }
}