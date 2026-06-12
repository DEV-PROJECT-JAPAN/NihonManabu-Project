using BackendAPI.DTOs; // Gọi sang thư mục DTOs của Backend để sử dụng các class như LevelDTO, LessonDTO, CardDTO
using BackendAPI.Interfaces;
using BackendAPI.Models; // Gọi sang thư mục Models của Backend để sử dụng các class như Level, Lesson, Card
using BackendAPI.Services; // Gọi sang thư mục Services của Backend vừa tạo
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PracticeController : ControllerBase
    {
        private readonly IVocabularyService _backendService;
        private readonly IUserService _userContext;
        private readonly IPracticeService _practiceService;

        // Tiêm thẳng Service của Backend vào đây
        public PracticeController(IVocabularyService backendService, IUserService userContext, IPracticeService practiceService)
        {
            _backendService = backendService;
            _userContext = userContext;
            _practiceService = practiceService;
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
        //chức năng ôn tập, lấy từ vựng của hệ thống
        [HttpGet("practice-system")]
        public async Task<IActionResult> PracticeSystem([FromQuery] int LessonId, [FromQuery] int UserId)
        {
            //lấy id người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();

            //id user lấy từ JWT
            //int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = await _practiceService.GetVocabularySystemAsync(LessonId, UserId);
            return Ok(data);
        }
        //chức năng ôn tập, lấy từ vựng của người dùng
        [HttpGet("practice-user")]
        public async Task<IActionResult> PracticeUser([FromQuery] int FolderId, [FromQuery] int UserId)
        {
            //lấy id người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();

            //id user lấy từ JWT
            //int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = await _practiceService.GetVocabularyUserAsync(FolderId, UserId);
            return Ok(data);
        }
        //danh sách các folder của người dùng
        [HttpGet("user-folders")]
        public async Task<IActionResult> UserFolders([FromQuery] int UserId)
        {
            //lấy id người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();
            //id user lấy từ JWT
            //int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = await _practiceService.GetUserFoldersAsync(UserId);
            return Ok(data);
        }
    }
}