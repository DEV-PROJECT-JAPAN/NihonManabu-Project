using FrontendRazorPage.Models; // Đổi đúng tên Model bên bạn
using FrontendRazorPage.Services; // Đổi đúng tên Service bên bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Grammar
{
    public class IndexModel : PageModel
    {
        private readonly GrammarClientService _grammarService;

        public IndexModel(GrammarClientService grammarService)
        {
            _grammarService = grammarService;
        }

        [BindProperty(SupportsGet = true, Name = "levelId")]
        public int? LevelId { get; set; }

        [BindProperty(SupportsGet = true, Name = "lessonId")]
        public int? LessonId { get; set; }

        [BindProperty(SupportsGet = true, Name = "selectedGrammarId")]
        public int? SelectedGrammarId { get; set; }

        [BindProperty(SupportsGet = true, Name = "questionType")]
        public int QuestionType { get; set; } = 0; // 0: Tổng hợp, 1: Trắc nghiệm, 2: Sắp xếp

        public List<GrammarModel> Grammars { get; set; } = new();
        public List<QuestionModel> Questions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Nếu không có LevelId hoặc LessonId, giữ nguyên giao diện để hiện hộp thông báo chọn bài, không lỗi 404
            if (!LessonId.HasValue || LessonId.Value <= 0)
            {
                return Page();
            }

            // 1. Tải TOÀN BỘ danh sách ngữ pháp của bài học lên trước
            Grammars = await _grammarService.GetGrammarByLessonAsync(LessonId.Value);

            // 2. Nếu người dùng ĐÃ BẤM CHỌN một mẫu ngữ pháp cụ thể
            if (SelectedGrammarId.HasValue && SelectedGrammarId.Value > 0)
            {
                // Bốc ngân hàng câu hỏi kiểm tra dựa theo loại bài tập (1, 2, 0)
                Questions = await _grammarService.GetQuestionsByGrammarAsync(SelectedGrammarId.Value, QuestionType);
            }

            return Page();
        }
    }
}