using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;

namespace FrontendRazorPage.Pages.Admin.Level
{
    public class EditModel : PageModel
    {
        private readonly VocabularyClientService _service;

        public EditModel(VocabularyClientService service)
        {
            _service = service;
        }

        [BindProperty]
        public LevelModel LevelData { get; set; } = new();

        // Hàm này ch?y khi ng??i dùng V?A M? TRANG LÊN
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var data = await _service.GetLevelByIdAsync(id);
            if (data == null)
            {
                // N?u ID tào lao không t?n t?i, ?á v? trang danh sách
                return RedirectToPage("/Admin/Level/Index");
            }

            // ?? d? li?u c? vào bi?n ?? hi?n th? lên Form HTML
            LevelData = data;
            return Page();
        }

        // Hàm này ch?y khi ng??i dùng B?M NÚT L?U THAY ??I
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            bool success = await _service.UpdateLevelAsync(LevelData);

            if (success)
            {
                // C?p nh?t thành công thì ?á v? trang danh sách (?ã g?n /Index chu?n ch?)
                return RedirectToPage("/Admin/Level/Index");
            }

            ModelState.AddModelError(string.Empty, "C?p nh?t th?t b?i. Vui lòng ki?m tra l?i.");
            return Page();
        }
    }
}