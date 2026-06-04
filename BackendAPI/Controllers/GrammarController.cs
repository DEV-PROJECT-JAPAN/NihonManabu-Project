using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrammarController : ControllerBase
    {
        private readonly IGrammarService _grammarService;

        // Tiêm cái Service xử lý dữ liệu Ngữ pháp lồng Câu hỏi vào đây
        public GrammarController(IGrammarService grammarService)
        {
            _grammarService = grammarService;
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
    }
}