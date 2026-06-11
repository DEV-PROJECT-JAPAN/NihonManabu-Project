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
    public class IndexModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly VocabularyClientService _vocabularyService;
        private readonly LevelClientService _levelClientService;
        private readonly LessonClientService _lessonClientService;
        public IndexModel(GrammarClientService grammarService, VocabularyClientService vocabularyService, LevelClientService levelClientService,LessonClientService lessonClientService)
        {
            _grammarService = grammarService;
            _vocabularyService = vocabularyService;
            _levelClientService = levelClientService;
            _lessonClientService = lessonClientService;
        }

        [BindProperty(SupportsGet = true, Name = "levelId")]
        public int? LevelId { get; set; }

        [BindProperty(SupportsGet = true, Name = "lessonId")]
        public int? LessonId { get; set; }

        [BindProperty(SupportsGet = true, Name = "selectedGrammarId")]
        public int? SelectedGrammarId { get; set; }

        [BindProperty(SupportsGet = true, Name = "questionType")]
        public int QuestionType { get; set; } = 0; // 0: Tổng hợp, 1: Trắc nghiệm, 2: Sắp xếp

        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();
        public List<GrammarModel> Grammars { get; set; } = new();
        public List<QuestionModel> Questions { get; set; } = new();

        public GrammarModel currentGrammar { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // TẦNG 5: Nếu bấm chọn bài tập Mẫu ngữ pháp cụ thể -> Lấy danh sách câu hỏi theo type đã chọn
            if (SelectedGrammarId.HasValue && SelectedGrammarId > 0 && QuestionType != null) 
            { 
                Questions = await _grammarService.GetQuestionsByGrammarAsync(SelectedGrammarId.Value, QuestionType);
                
                // TẦNG 4: Nếu bấm chọn 1 Mẫu ngữ pháp cụ thể -> Lấy danh sách câu hỏi
                currentGrammar = await _grammarService.GetGrammarByIdAsync(SelectedGrammarId.Value);
            }
            // TẦNG 3: Nếu đã chọn Bài học -> Lấy danh sách Ngữ pháp
            else if (LessonId.HasValue && LessonId > 0)
            {
                Grammars = await _grammarService.GetGrammarByLessonAsync(LessonId.Value);
            }
            // TẦNG 2: Nếu đã chọn Cấp độ -> Lấy danh sách Bài học
            else if (LevelId.HasValue && LevelId > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelAsync(LevelId.Value);
            } else // TẦNG 1: Mặc định -> Lấy danh sách Cấp độ
            {
                Levels = await _levelClientService.GetLevelsAsync();
            }



            return Page();
        }
    }
}