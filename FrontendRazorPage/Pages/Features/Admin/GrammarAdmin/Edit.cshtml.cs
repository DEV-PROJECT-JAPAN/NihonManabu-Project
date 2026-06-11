using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models.AdminModel;
using FrontendRazorPage.Services;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class EditModel : PageModel
    {
        private readonly GrammarClientService _grammarService;

        public EditModel(GrammarClientService grammarService)
        {
            _grammarService = grammarService;
        }

        [BindProperty]
        public GrammarAdminModel Grammar { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var data = await _grammarService.GetByIdForAdminAsync(id);
            if (data == null) return NotFound();

            Grammar = data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _grammarService.UpdateAsync(Grammar.Id, Grammar);
            if (success)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Cập nhật thất bại, vui lòng kiểm tra lại API.");
            return Page();
        }
    }
}