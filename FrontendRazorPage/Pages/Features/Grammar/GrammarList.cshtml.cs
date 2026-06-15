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
    public class GrammarListModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;

        public GrammarListModel(GrammarClientService grammarService, LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _grammarService = grammarService;
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        [BindProperty(SupportsGet = true, Name = "levelId")]
        public int LevelId { get; set; }

        [BindProperty(SupportsGet = true, Name = "lessonId")]
        public int LessonId { get; set; }

        public LevelModel CurrentLevel { get; set; } = new();
        public LessonModel CurrentLesson { get; set; } = new();
        public List<GrammarModel> Grammars { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (LessonId <= 0) return RedirectToPage("./Index");

            var levels = await _levelClientService.GetLevelsAsync();
            CurrentLevel = levels.FirstOrDefault(l => l.Id == LevelId) ?? new LevelModel { Name = "Trình độ" };

            var lessons = await _lessonClientService.GetLessonsByLevelAsync(LevelId);
            CurrentLesson = lessons.FirstOrDefault(l => l.Id == LessonId) ?? new LessonModel { Order = 0 };

            Grammars = await _grammarService.GetGrammarByLessonAsync(LessonId);
            return Page();
        }
    }
}