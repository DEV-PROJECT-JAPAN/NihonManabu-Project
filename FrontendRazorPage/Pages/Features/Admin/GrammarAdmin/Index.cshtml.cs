using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class IndexModel : PageModel
    {
        private readonly GrammarClientService _grammarService;

        public IndexModel(GrammarClientService grammarService)
        {
            _grammarService = grammarService;
        }

        public List<GrammarAdminModel> Grammars { get; set; } = new();

        public async Task OnGetAsync()
        {
            Grammars = await _grammarService.GetAllForAdminAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _grammarService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}