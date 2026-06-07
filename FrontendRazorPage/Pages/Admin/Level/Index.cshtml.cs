using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services; // Th? m?c ch?a service g?i API c?a b?n
using FrontendRazorPage.Models; // Th? m?c ch?a model/DTO c?a b?n

namespace FrontendRazorPage.Pages.Admin.Level
{
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _service;

        // Tiêm service vào ?? g?i API
        public IndexModel(VocabularyClientService service)
        {
            _service = service;
        }

        // Bi?n ch?a d? li?u th?t ?? ??y ra View
        public List<LevelModel> Levels { get; set; } = new();

        // Hàm này ch?y t? ??ng khi Admin m? trang
        public async Task OnGetAsync()
        {
            // G?i API GET /api/vocabulary/levels thông qua Service
            Levels = await _service.GetLevelsAsync();
        }

        // HÀM MỚI BỔ SUNG: Hứng sự kiện bấm nút Xóa
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            bool success = await _service.DeleteLevelAsync(id);

            if (success)
            {
                // Xóa thành công thì load lại trang Index này
                return RedirectToPage("/Admin/Level/Index");
            }

            // Nếu Backend từ chối xóa (thường do dính khóa ngoại với Bài học)
            ModelState.AddModelError(string.Empty, "Không thể xóa Cấp độ này. Có thể nó đang chứa các Bài học bên trong.");

            // Phải load lại danh sách để lấp đầy bảng, nếu không giao diện sẽ bị trống
            Levels = await _service.GetLevelsAsync();
            return Page();
        }
    }
}