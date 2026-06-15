using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Grammar
{
    public class IndexModel : PageModel
    {
        private readonly LevelClientService _levelClientService;

        public IndexModel(LevelClientService levelClientService)
        {
            _levelClientService = levelClientService;
        }

        public List<LevelModel> Levels { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Levels = await _levelClientService.GetLevelsAsync();
            return Page();
        }
    }
}