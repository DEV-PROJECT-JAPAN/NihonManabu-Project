using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Admin.Levels
{
    public class CreateModel : PageModel
    {
        private readonly LevelClientService _levelClientService;

        public CreateModel(LevelClientService levelClientService)
        {
            _levelClientService = levelClientService;
        }

        [BindProperty]
        public LevelAdminModel InputLevel { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _levelClientService.CreateLevelAsync(InputLevel);
            if (success)
            {
                Message = "Thêm mới cấp độ thành công!";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo mới dữ liệu.");
            return Page();
        }
    }
}