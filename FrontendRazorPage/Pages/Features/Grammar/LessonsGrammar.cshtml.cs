using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Grammar
{
    public class LessonsGrammarModel : PageModel
    {
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;

        public LessonsGrammarModel(LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        [BindProperty(SupportsGet = true, Name = "levelId")]
        public int LevelId { get; set; }

        public LevelModel CurrentLevel { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (LevelId <= 0) return RedirectToPage("./Index");

            var levels = await _levelClientService.GetLevelsAsync();
            CurrentLevel = levels.FirstOrDefault(l => l.Id == LevelId) ?? new LevelModel { Name = "Trình độ" };

            Lessons = await _lessonClientService.GetLessonsByLevelAsync(LevelId);
            return Page();
        }
    }
}