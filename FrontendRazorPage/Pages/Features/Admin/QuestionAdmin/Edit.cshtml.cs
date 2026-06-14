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
    public class EditModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly QuestionClientService _questionService;

        public EditModel(GrammarClientService grammarService, QuestionClientService questionService)
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

        [BindProperty(SupportsGet = true)]
        public int QuestionId { get; set; }

        public List<GrammarAdminModel> Grammars { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, int? levelId, int? lessonId)
        {
            if (levelId.HasValue) SelectedLevelId = levelId.Value;
            if (lessonId.HasValue) SelectedLessonId = lessonId.Value;

            QuestionId = id;

            // Tải danh sách cấu trúc ngữ pháp theo bài học
            var allGrammars = await _grammarService.GetAllForAdminAsync() ?? new();
            Grammars = allGrammars.FindAll(g => g.LessonId == SelectedLessonId);

            // Tải dữ liệu câu hỏi hiện tại theo ID trùng khớp điều hướng
            var existing = await _questionService.GetQuestionByIdAsync(id);
            if (existing == null)
                return RedirectToPage("./Index", new { SelectedLevelId, SelectedLessonId });

            Question = existing;
            return Page();
        }

        /// <summary>
        /// ĐÃ ĐỒNG BỘ: Tiếp nhận JSON sạch, đóng gói cả 2 cột mốc thời gian hệ thống và cập nhật xuống Database
        /// </summary>
        public async Task<IActionResult> OnPostUpdateQuestionAsync([FromBody] QuestionAdminModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Content))
                return BadRequest(new { message = "Nội dung câu hỏi không được để trống." });

            if (model.Id <= 0)
                return BadRequest(new { message = "ID câu hỏi không hợp lệ." });

            try
            {
                DateTime currentUtc = DateTime.UtcNow;

                // Lấy dữ liệu thực tế đang lưu trong DB để bảo tồn mốc thời gian khởi tạo (CreatedAt)
                var existingQuestion = await _questionService.GetQuestionByIdAsync(model.Id);
                if (existingQuestion == null)
                    return BadRequest(new { message = "Không tìm thấy câu hỏi gốc trên hệ thống." });

                // Đóng gói dữ liệu thời gian cho Câu hỏi
                model.CreatedAt = existingQuestion.CreatedAt;
                model.UpdatedAt = currentUtc;

                // Đồng bộ và đóng gói dữ liệu thời gian cho mảng Đáp án
                if (model.Answers != null)
                {
                    foreach (var ans in model.Answers)
                    {
                        ans.OrderId = 0; // Đưa về 0 đồng nhất với Create
                        ans.UpdatedAt = currentUtc;

                        // Tìm xem đáp án này đã tồn tại ở bản ghi cũ chưa
                        var oldAnswer = existingQuestion.Answers?.Find(a => a.Id == ans.Id);

                        if (oldAnswer != null && ans.Id > 0)
                        {
                            // Nếu là đáp án cũ đang cập nhật -> giữ nguyên ngày tạo cũ
                            ans.CreatedAt = oldAnswer.CreatedAt;
                        }
                        else
                        {
                            // Nếu là dòng đáp án mới được ấn nút "+" thêm vào -> gán ngày tạo mới
                            ans.CreatedAt = currentUtc;
                        }
                    }
                }

                // Gửi cấu trúc dữ liệu hoàn thiện xuống API Service
                bool isUpdated = await _questionService.UpdateQuestionAsync(model.Id, model);
                if (isUpdated)
                    return new JsonResult(new { success = true, message = "Cập nhật thành công!" });

                return BadRequest(new { message = "API Backend từ chối cập nhật. Hãy kiểm tra lại dữ liệu." });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = $"[Lỗi xử lý C#]: {innerMessage}" });
            }
        }
    }
}
