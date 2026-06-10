using Microsoft.AspNetCore.Mvc;
using BackendAPI.Services; // Gọi sang thư mục Services của Backend vừa tạo
using BackendAPI.Models; // Gọi sang thư mục Models của Backend để sử dụng các class như Level, Lesson, Card
using BackendAPI.Interfaces;
using BackendAPI.DTOs;
namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VocabularyController : ControllerBase
    {
        private readonly IVocabularyService _backendService;
        private readonly IUserService _userContext;

        // Tiêm thẳng Service của Backend vào đây
        public VocabularyController(IVocabularyService backendService, IUserService userContext)
        {
            _backendService = backendService;
            _userContext = userContext;
        }

        // TẦNG 1: GET api/vocabulary/levels
        [HttpGet("levels")]
        public async Task<IActionResult> GetLevels()
        {
            var data = await _backendService.GetAllLevelsAsync();
            return Ok(data);
        }

        // TẦNG 2: GET api/vocabulary/lessons?levelId=1
        [HttpGet("lessons")]
        public async Task<IActionResult> GetLessons([FromQuery] int levelId)
        {
            var data = await _backendService.GetLessonsByLevelAsync(levelId);
            return Ok(data);
        }

        // TẦNG 3: GET api/vocabulary/cards?lessonId=1
        [HttpGet("cards")]
        public async Task<IActionResult> GetCards([FromQuery] int lessonId)
        {
            var data = await _backendService.GetVocabulariesByLessonAsync(lessonId);
            return Ok(data);
        }

        [HttpPost("update-LearningProgressByUser")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateLearningProgresByUserDTO input)
        {
            // 1. Lấy ID người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();

            // 2. Đẩy nhiệm vụ xử lý Database xuống cho tầng Service gánh vác
            bool isSuccess = await _backendService.UpdateProgressAsync(userId, input);

            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Lỗi lưu tiến độ" });
        }



    }
}