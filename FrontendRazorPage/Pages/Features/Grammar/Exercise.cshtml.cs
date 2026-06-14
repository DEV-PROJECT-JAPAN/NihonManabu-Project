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
    public class ExerciseModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;
        private readonly QuestionClientService _questionService;

        public ExerciseModel(GrammarClientService grammarService, LevelClientService levelClientService, LessonClientService lessonClientService, QuestionClientService questionService)
        {
            _grammarService = grammarService;
            _levelClientService = levelClientService;
            _lessonClientService = lessonClientService;
            _questionService = questionService;
        }

        [BindProperty(SupportsGet = true, Name = "levelId")]
        public int LevelId { get; set; }

        [BindProperty(SupportsGet = true, Name = "lessonId")]
        public int LessonId { get; set; }

        [BindProperty(SupportsGet = true, Name = "selectedGrammarId")]
        public int SelectedGrammarId { get; set; }

        [BindProperty(SupportsGet = true, Name = "questionType")]
        public int? QuestionType { get; set; } // Nullable để hiển thị giao diện chọn chế độ trước

        public LevelModel CurrentLevel { get; set; } = new();
        public LessonModel CurrentLesson { get; set; } = new();
        public GrammarModel CurrentGrammar { get; set; } = new();
        public List<QuestionModel> Questions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (SelectedGrammarId <= 0) return RedirectToPage("./Index");

            var levels = await _levelClientService.GetLevelsAsync();
            CurrentLevel = levels.FirstOrDefault(l => l.Id == LevelId) ?? new LevelModel { Name = "Trình độ" };

            var lessons = await _lessonClientService.GetLessonsByLevelAsync(LevelId);
            CurrentLesson = lessons.FirstOrDefault(l => l.Id == LessonId) ?? new LessonModel { Order = 0 };

            CurrentGrammar = await _grammarService.GetGrammarByIdAsync(SelectedGrammarId);

            if (QuestionType.HasValue)
            {
                Questions = await _questionService.GetQuestionsByGrammarAsync(SelectedGrammarId, QuestionType.Value);
            }

            return Page();
        }
    }
}