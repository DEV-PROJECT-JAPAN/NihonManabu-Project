using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using FrontendRazorPage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.QuestionAdmin
{
    public class CreateModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly QuestionClientService _questionService;

        public CreateModel(GrammarClientService grammarService, QuestionClientService questionService)
        {
            _grammarService = grammarService;
            _questionService = questionService;
        }

        [BindProperty]
        public QuestionAdminModel Question { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedLessonId { get; set; }

        public List<GrammarAdminModel> Grammars { get; set; } = new();

        public async Task OnGetAsync(int? levelId, int? lessonId)
        {
            if (levelId.HasValue) SelectedLevelId = levelId.Value;
            if (lessonId.HasValue) SelectedLessonId = lessonId.Value;

            var allGrammars = await _grammarService.GetAllForAdminAsync() ?? new();
            Grammars = allGrammars.FindAll(g => g.LessonId == SelectedLessonId);

            if (Question.Answers.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Question.Answers.Add(new AnswerAdminModel { Text = "", IsCorrect = false });
                }
            }
        }

        /// <summary>
        /// Tiếp nhận luồng JSON từ client, tự nạp trường hệ thống và chuyển giao dữ liệu xuống database
        /// </summary>
        public async Task<IActionResult> OnPostSaveQuestionAsync([FromBody] QuestionAdminModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Content))
            {
                return BadRequest(new { message = "Nội dung câu hỏi không được để trống." });
            }

            try
            {
                DateTime currentUtc = DateTime.UtcNow;
                model.CreatedAt = currentUtc;
                model.UpdatedAt = currentUtc;

                if (model.Answers != null)
                {
                    foreach (var ans in model.Answers)
                    {
                       // Cắt đứt hoàn toàn lặp tham chiếu điều hướng
                        ans.CreatedAt = currentUtc;
                        ans.UpdatedAt = currentUtc;
                    }
                }

                bool isSaved = await _questionService.CreateQuestionAsync(model);

                if (isSaved)
                {
                    return new JsonResult(new { success = true });
                }

                return BadRequest(new { message = "API Backend từ chối lưu dữ liệu. Hãy kiểm tra lại các trường ràng buộc trên DB." });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = $"[Lỗi xử lý C#]: {innerMessage}" });
            }
        }
    }
}