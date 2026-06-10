using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionAdminController : ControllerBase
    {
        private readonly IQuestionAdminService<QuestionDTO> _questionService;     // Đầu mapping DTO cho User
        private readonly IQuestionAdminService<Question> _questionAdminService; // Đầu giữ Model cho Admin

        public QuestionAdminController(
            IQuestionAdminService<QuestionDTO> questionService,
            IQuestionAdminService<Question> questionAdminService)
        {
            _questionService = questionService;
            _questionAdminService = questionAdminService;
        }

        // 🌐 GET: api/QuestionAdmin/user/lesson/5 -> Trả về danh sách DTO rút gọn
        [HttpGet("user/lesson/{lessonId}")]
        public async Task<IActionResult> GetByLessonForUser(int lessonId)
        {
            if (lessonId <= 0) return BadRequest("ID bài học không hợp lệ!");
            var result = await _questionService.GetQuestionsByLessonAsync(lessonId);
            return Ok(result);
        }

        // 🔐 GET: api/QuestionAdmin/admin/lesson/5 -> Trả về danh sách Model đầy đủ thông số hệ thống
        [HttpGet("admin/lesson/{lessonId}")]
        public async Task<IActionResult> GetByLessonForAdmin(int lessonId)
        {
            if (lessonId <= 0) return BadRequest("ID bài học không hợp lệ!");
            var result = await _questionAdminService.GetQuestionsByLessonAsync(lessonId);
            return Ok(result);
        }

        // 🔍 GET: api/QuestionAdmin/5 -> Chi tiết 1 câu hỏi (Dùng Model gốc)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _questionAdminService.GetQuestionByIdAsync(id);
            return result != null ? Ok(result) : NotFound(new { message = "Không tìm thấy câu hỏi!" });
        }

        // ➕ POST: api/QuestionAdmin -> Tạo mới cụm dữ liệu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Question question)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _questionAdminService.CreateQuestionAsync(question);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ✏️ PUT: api/QuestionAdmin/5 -> Cập nhật đồng bộ cụm dữ liệu
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question question)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _questionAdminService.UpdateQuestionAsync(id, question);
            return success ? Ok(new { message = "Cập nhật ngân hàng câu hỏi thành công!" }) : NotFound();
        }

        // ❌ DELETE: api/QuestionAdmin/5 -> Xóa vĩnh viễn
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _questionAdminService.DeleteQuestionAsync(id);
            return success ? Ok(new { message = "Đã xóa bản ghi thành công!" }) : NotFound();
        }
    }
}