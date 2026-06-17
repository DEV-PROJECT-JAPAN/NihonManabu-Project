using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.QuestionAdmin
{
    public class IndexModel : PageModel
    {
        private readonly QuestionClientService _questionService;
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;
        private readonly GrammarClientService _grammarService; // Bổ sung service ngữ pháp

        // Ràng buộc dữ liệu bộ lọc 3 tầng qua URL (SupportsGet = true)
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedLessonId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedGrammarId { get; set; } // Thêm biến ràng buộc cho ngữ pháp

        // Danh sách hiển thị ở các Dropdown bộ lọc
        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();
        public List<GrammarAdminModel> Grammars { get; set; } = new(); // Thêm danh sách ngữ pháp

        // Danh sách hiển thị lên bảng dữ liệu chính
        public List<QuestionAdminModel> Questions { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public IndexModel(
            QuestionClientService questionService,
            LessonClientService lessonClientService,
            LevelClientService levelClientService,
            GrammarClientService grammarService) // Inject service ngữ pháp vào constructor
        {
            _questionService = questionService;
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
            _grammarService = grammarService;
        }

        public async Task OnGetAsync()
        {
            // Bước 1: Luôn nạp danh sách Cấp độ (N5, N4...)
            Levels = await _levelClientService.GetLevelsAsync() ?? new();

            // Bước 2: Tải các Bài học tương ứng nếu đã chọn Cấp độ
            if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                Lessons = await _lessonClientService.GetLessonsByLevelAsync(SelectedLevelId.Value) ?? new();
            }

            // Bước 3: Đổ dữ liệu linh hoạt dựa theo Bài học và Ngữ pháp
            if (SelectedLessonId.HasValue && SelectedLessonId.Value > 0)
            {
                // Tải danh mục cấu trúc ngữ pháp thuộc bài học này để đổ vào Dropdown bước 3
                var allGrammars = await _grammarService.GetAllForAdminAsync() ?? new List<GrammarAdminModel>();
                Grammars = allGrammars.Where(g => g.LessonId == SelectedLessonId.Value).ToList();

                // Kéo trực tiếp toàn bộ câu hỏi của Bài học này từ API Backend về
                var allQuestionsInLesson = await _questionService.GetQuestionsByLessonForAdminAsync(SelectedLessonId.Value) ?? new List<QuestionAdminModel>();

                // Nếu Admin chọn lọc theo Ngữ pháp cụ thể ở bước 3
                if (SelectedGrammarId.HasValue && SelectedGrammarId.Value > 0)
                {
                    Questions = allQuestionsInLesson.Where(q => q.GrammarId == SelectedGrammarId.Value).ToList();
                }
                else
                {
                    // Nếu chưa lọc ngữ pháp -> Mặc định hiện thị TẤT CẢ câu hỏi thuộc bài học
                    Questions = allQuestionsInLesson;
                }
            }
            else
            {
                // Chưa chọn bài học -> Để bảng trống
                Questions = new List<QuestionAdminModel>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _questionService.DeleteQuestionAsync(id);
            Message = success ? "Xóa câu hỏi thành công!" : "Xóa câu hỏi thất bại.";

            // Giữ nguyên cấu hình bộ lọc 3 tầng sau khi thực hiện xóa
            return RedirectToPage(new
            {
                SelectedLevelId = SelectedLevelId,
                SelectedLessonId = SelectedLessonId,
                SelectedGrammarId = SelectedGrammarId
            });
        }
    }
}