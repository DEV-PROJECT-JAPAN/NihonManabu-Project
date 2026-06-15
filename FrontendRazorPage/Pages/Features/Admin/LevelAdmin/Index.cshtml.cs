using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Admin.Levels
{
    public class IndexModel : PageModel
    {
        private readonly LevelClientService _levelClientService;

        public IndexModel(LevelClientService levelClientService)
        {
            _levelClientService = levelClientService;
        }

        public List<LevelAdminModel> Levels { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Levels = await _levelClientService.GetLevelsForAdminAsync();
        }

        // Xử lý xóa trực tiếp tại trang danh sách
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _levelClientService.DeleteLevelAsync(id);
            Message = success ? "Xóa cấp độ thành công!" : "Xóa thất bại.";
            return RedirectToPage("./Index");
        }
    }
}