using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Admin.Levels
{
    public class EditModel : PageModel
    {
        private readonly LevelClientService _levelClientService;

        public EditModel(LevelClientService levelClientService)
        {
            _levelClientService = levelClientService;
        }

        [BindProperty]
        public LevelAdminModel InputLevel { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var level = await _levelClientService.GetLevelByIdForAdminAsync(id);
            if (level == null)
            {
                return RedirectToPage("./Index");
            }
            InputLevel = level;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _levelClientService.UpdateLevelAsync(InputLevel.Id, InputLevel);
            if (success)
            {
                Message = "Cập nhật cấp độ thành công!";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật dữ liệu.");
            return Page();
        }
    }
}