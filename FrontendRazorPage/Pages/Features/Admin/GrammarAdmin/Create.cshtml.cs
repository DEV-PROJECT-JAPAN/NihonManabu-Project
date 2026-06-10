using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class CreateModel : PageModel
    {
        private readonly GrammarClientService _grammarService;

        public CreateModel(GrammarClientService grammarService)
        {
            _grammarService = grammarService;
        }

        [BindProperty]
        public GrammarAdminModel Grammar { get; set; } = new();

        public void OnGet() { }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid) return Page();

        //    var success = await _grammarService.CreateAsync(Grammar);
        //    if (success)
        //    {
        //        return RedirectToPage("./Index");
        //    }

        //    ModelState.AddModelError(string.Empty, "Lỗi khi thêm mới dữ liệu vào hệ thống.");
        //    return Page();
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Kiểm tra nếu Form bị lỗi Validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"[VALIDATION ERROR]: {error}");
                }
                return Page();
            }

            // 2. Gọi service và kiểm tra kết quả
            var success = await _grammarService.CreateAsync(Grammar);
            if (success)
            {
                return RedirectToPage("/Features/Admin/GrammarAdmin/Index");
            }

            System.Diagnostics.Debug.WriteLine("[SERVICE ERROR]: Gọi API thất bại hoặc Database không lưu được.");
            ModelState.AddModelError(string.Empty, "Lỗi khi thêm mới dữ liệu vào hệ thống.");
            return Page();
        }
    }
}