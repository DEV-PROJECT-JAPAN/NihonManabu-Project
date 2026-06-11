using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.Vocabularies
{
    public class EditModel : PageModel
    {
        private readonly VocabularyClientService _vocabClientService;

        public EditModel(VocabularyClientService vocabClientService)
        {
            _vocabClientService = vocabClientService;
        }

        // Hứng bộ lọc từ URL để khi sửa xong quay lại đúng bài học cũ
        [BindProperty(SupportsGet = true)]
        public int SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedLessonId { get; set; }

        // Đối tượng chứa dữ liệu từ vựng cần chỉnh sửa
        [BindProperty]
        public VocabularyAdminModel VocabularyInput { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 1. TỰ ĐỘNG ĐỔ DỮ LIỆU CŨ LÊN FORM KHI VỪA VÀO TRANG
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Gọi Service lấy chi tiết từ vựng theo ID từ API Backend
            var vocab = await _vocabClientService.GetVocabularyByIdAsync(id);

            if (vocab == null)
            {
                ErrorMessage = "Không tìm thấy từ vựng này trong hệ thống.";
                return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
            }

            // Điền dữ liệu bốc được vào Form để Admin nhìn thấy dữ liệu cũ
            VocabularyInput = vocab;
            return Page();
        }

        /// <summary>
        /// 2. XỬ LÝ LƯU DỮ LIỆU KHI ADMIN BẤM NÚT CẬP NHẬT
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng kiểm tra lại thông tin nhập vào.";
                return Page();
            }

            // Gọi API Update đè dữ liệu mới lên bản ghi cũ dưới Database
            var success = await _vocabClientService.UpdateVocabularyAsync(VocabularyInput.Id, VocabularyInput);

            if (success)
            {
                Message = $"Cập nhật từ vựng '{VocabularyInput.Hiragana}' thành công!";
                // Quay về trang danh sách và giữ nguyên bộ lọc bài học cho Admin
                return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
            }

            ErrorMessage = "Đã xảy ra lỗi trong quá trình cập nhật từ vựng.";
            return Page();
        }
    }
}