using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrammarController : ControllerBase
    {
        private readonly IGrammarService<GrammarDTO> _grammarService;
        private readonly IGrammarService<Grammar> _grammarAdminService;

        // Tiêm cái Service xử lý dữ liệu Ngữ pháp lồng Câu hỏi vào đây
        public GrammarController(IGrammarService<GrammarDTO> grammarService, IGrammarService<Grammar> grammarAdminService)
        {
            _grammarService = grammarService;
            _grammarAdminService = grammarAdminService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID ngữ pháp không hợp lệ rồi trai đẹp!");
            }
            var result = await _grammarService.GetGrammarByIdAsync(id);
            return Ok(result);
        }
        /// <summary>

        // Đường dẫn gọi API: GET /api/grammar/lesson/1
        [HttpGet("lesson/{lessonId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GrammarDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByLesson(int lessonId)
        {
            if (lessonId <= 0)
            {
                return BadRequest("ID bài học không hợp lệ rồi trai đẹp!");
            }

            var result = await _grammarService.GetGrammarByLessonAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// 🌐 API 2: LẤY NGÂN HÀNG CÂU HỎI THEO MẪU NGỮ PHÁP VÀ LOẠI BÀI TẬP
        /// Endpoint: GET /api/Grammar/questions?grammarId=1&questionType=Quiz
        /// </summary>
        /// <param name="grammarId">ID của mẫu ngữ pháp</param>
        /// <param name="questionType">"Quiz" (Trắc nghiệm), "Arrange" (Sắp xếp), hoặc "All" (Tổng hợp)</param>
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions([FromQuery] int grammarId, [FromQuery] int questionType = 0)
        {
            if (grammarId <= 0) return BadRequest("ID Ngữ pháp không hợp lệ!");

            var result = await _grammarService.GetQuestionsByGrammarAsync(grammarId, questionType);
            return Ok(result);
        }




        // ==========================================
        // 🔐 ĐẦU QUẢN TRỊ - ADMIN ENDPOINTS (Dùng bảng gốc)
        // ==========================================

        [HttpGet("admin-list")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            // Trả thẳng danh sách thực thể gốc kèm thực thể liên kết Lesson
            return Ok(await _grammarAdminService.GetAllGrammarsAsync());
        }

        [HttpGet("admin/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            // Gọi hàm Generics truyền kiểu bảng gốc Grammar cho Admin thấy đủ ngày giờ hệ thống
            var result = await _grammarAdminService.GetGrammarByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Grammar grammar)
        {
            var result = await _grammarAdminService.CreateAsync(grammar);
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Grammar grammar)
        {
            var success = await _grammarAdminService.UpdateAsync(id, grammar);
            return success ? Ok(new { message = "Cập nhật dữ liệu hệ thống thành công!" }) : NotFound();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _grammarAdminService.DeleteAsync(id);
            return success ? Ok(new { message = "Đã xóa bản ghi vĩnh viễn khỏi Database!" }) : NotFound();
        }
    }
}