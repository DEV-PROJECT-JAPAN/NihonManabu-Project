using BackendAPI.DTOs; // Gọi sang thư mục DTOs của Backend để sử dụng các class như LevelDTO, LessonDTO, CardDTO
using BackendAPI.Interfaces;
using BackendAPI.Models; // Gọi sang thư mục Models của Backend để sử dụng các class như Level, Lesson, Card
using BackendAPI.Services; // Gọi sang thư mục Services của Backend vừa tạo
<<<<<<< Updated upstream
=======
using Microsoft.AspNetCore.Authorization;
>>>>>>> Stashed changes
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
        //lấy danh sách từ vựng của user đã học
<<<<<<< Updated upstream
        [HttpGet("practice-system")]
=======
        //chạy ổn
        [HttpGet("practice-system")]
        //[Authorize]
>>>>>>> Stashed changes
        public async Task<IActionResult> GetVocabularySystemAsync()
        {
            // 1. Lấy ID người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();

            var vocabularies = await _practiceService.GetVocabularySystemAsync(userId);

            return Ok(vocabularies);
        }
        //chức năng ôn tập, lấy từ vựng của người dùng
        [HttpGet("practice-user")]
        public async Task<IActionResult> PracticeUser([FromQuery] int FolderId)
        {
            //lấy id người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();

            //id user lấy từ JWT
            //int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = await _practiceService.GetVocabularyUserAsync(FolderId, userId);
            return Ok(data);
        }
        //danh sách các folder của người dùng
<<<<<<< Updated upstream
=======
        //chạy ổn
>>>>>>> Stashed changes
        [HttpGet("user-folders")]
        public async Task<IActionResult> UserFolders()
        {
            //lấy id người dùng từ trạm trung chuyển (Bây giờ ra 1, sau này ra ID thật)
            int userId = _userContext.GetCurrentUserId();
            //id user lấy từ JWT
            //int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = await _practiceService.GetUserFoldersAsync(userId);
            return Ok(data);
        }
        // Tải file excel lên để nạp từ vựng vào flashcard list của user
<<<<<<< Updated upstream
        [HttpPost("upload-folder-excel")]
=======
        //chạy ổn
        [HttpPost("upload-folder-excel")]
        //[Authorize]
>>>>>>> Stashed changes
        public async Task<IActionResult> UploadFolderExcel([FromForm] UploadFolderRequestDTO request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { success = false, message = "File trống hoặc không nhận được file!" });

            if (!request.File.FileName.EndsWith(".xlsx"))
                return BadRequest(new { success = false, message = "Chỉ chấp nhận file Excel (.xlsx)!" });

            int userId = _userContext.GetCurrentUserId();

            // Lấy dữ liệu từ biến request truyền vào Service
            var result = await _practiceService.UploadFolderExcelAsync(userId, request.FolderName, request.Description, request.File);

            if (result.isSuccess)
            {
                return Ok("Folder uploaded successfully.");
            }
            return BadRequest(new { success = false, message = result.errorMessage });
        }
        // Xóa folder của người dùng
<<<<<<< Updated upstream
=======
        //chạy ổn
>>>>>>> Stashed changes
        [HttpDelete("delete-folder/{folderId}")]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {
            int userId = _userContext.GetCurrentUserId();
            var result = await _practiceService.DeleteFolderAsync(folderId, userId);

            if (result) return Ok(new { success = true, message = "Đã xóa thư mục thành công." });

            return BadRequest(new { success = false, message = "Lỗi không thể xóa thư mục!" });
        }
    }
}