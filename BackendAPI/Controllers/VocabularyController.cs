using Microsoft.AspNetCore.Mvc;
using BackendAPI.Services; // Gọi sang thư mục Services của Backend vừa tạo
using BackendAPI.Models; // Gọi sang thư mục Models của Backend để sử dụng các class như Level, Lesson, Card
using BackendAPI.DTOs; // Gọi sang thư mục DTOs của Backend để sử dụng các class như LevelDTO, LessonDTO, CardDTO
using BackendAPI.Interfaces;
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

        #region ================= ADMIN: QUẢN LÝ CẤP ĐỘ (CRUD) =================

        // 1. CREATE: Thêm mới cấp độ
        // POST: api/vocabulary/levels
        [HttpPost("levels")]
        public async Task<IActionResult> CreateLevel([FromBody] LevelDTO input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Name))
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            // Gọi Service để lưu vào DB (Cần bổ sung hàm này bên IVocabularyService)
            bool isSuccess = await _backendService.CreateLevelAsync(input);

            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Không thể tạo cấp độ mới." });
        }

        // 2. READ (Chi tiết): Lấy 1 cấp độ theo ID để đổ lên form Sửa
        // GET: api/vocabulary/levels/{id}
        [HttpGet("levels/{id}")]
        public async Task<IActionResult> GetLevelById(int id)
        {
            var data = await _backendService.GetLevelByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy cấp độ có ID = {id}" });
            }
            return Ok(data);
        }

        // 3. UPDATE: Cập nhật thông tin cấp độ
        // PUT: api/vocabulary/levels/{id}
        [HttpPut("levels/{id}")]
        public async Task<IActionResult> UpdateLevel(int id, [FromBody] LevelDTO input)
        {
            if (id != input.Id)
            {
                return BadRequest(new { success = false, message = "ID trên URL và ID trong dữ liệu không khớp." });
            }

            bool isSuccess = await _backendService.UpdateLevelAsync(input);
            if (isSuccess) return Ok(new { success = true });

            return BadRequest(new { success = false, message = "Lỗi cập nhật dữ liệu." });
        }

        // 4. DELETE: Xóa cấp độ
        // DELETE: api/vocabulary/levels/{id}
        [HttpDelete("levels/{id}")]
        public async Task<IActionResult> DeleteLevel(int id)
        {
            bool isSuccess = await _backendService.DeleteLevelAsync(id);
            if (isSuccess) return Ok(new { success = true });

            return BadRequest(new { success = false, message = "Lỗi xóa dữ liệu (Có thể đang dính khóa ngoại với Bài học)." });
        }

        #endregion
        #region ================= ADMIN: QUẢN LÝ BÀI HỌC (CRUD) =================

        [HttpPost("lessons")]
        public async Task<IActionResult> CreateLesson([FromBody] LessonDTO input)
        {
            bool isSuccess = await _backendService.CreateLessonAsync(input);
            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Không thể tạo bài học." });
        }

        [HttpGet("lessons/{id}")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var data = await _backendService.GetLessonByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPut("lessons/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonDTO input)
        {
            if (id != input.Id) return BadRequest();
            bool isSuccess = await _backendService.UpdateLessonAsync(input);
            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Lỗi cập nhật." });
        }

        [HttpDelete("lessons/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            bool isSuccess = await _backendService.DeleteLessonAsync(id);
            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Lỗi xóa dữ liệu (Dính khóa ngoại Từ vựng)." });
        }

        #endregion
        #region ================= ADMIN: QUẢN LÝ TỪ VỰNG =================

        [HttpGet("all-lessons")]
        public async Task<IActionResult> GetAllLessons()
        {
            return Ok(await _backendService.GetAllLessonsAsync());
        }

        [HttpPost("vocabularies")]
        public async Task<IActionResult> CreateVocab([FromBody] VocabularyDTO input)
        {
            if (await _backendService.CreateVocabularyAsync(input)) return Ok(new { success = true });
            return BadRequest(new { success = false });
        }

        [HttpGet("vocabularies/{id}")]
        public async Task<IActionResult> GetVocabById(int id)
        {
            var data = await _backendService.GetVocabularyByIdAsync(id);
            return data != null ? Ok(data) : NotFound();
        }

        [HttpPut("vocabularies/{id}")]
        public async Task<IActionResult> UpdateVocab(int id, [FromBody] VocabularyDTO input)
        {
            if (id != input.Id) return BadRequest();
            if (await _backendService.UpdateVocabularyAsync(input)) return Ok(new { success = true });
            return BadRequest(new { success = false });
        }

        [HttpDelete("vocabularies/{id}")]
        public async Task<IActionResult> DeleteVocab(int id)
        {
            if (await _backendService.DeleteVocabularyAsync(id)) return Ok(new { success = true });
            return BadRequest(new { success = false });
        }

        #endregion
        [HttpPost("import/{lessonId}")]
        public async Task<IActionResult> ImportVocabularies(int lessonId, IFormFile file)
        {
            bool isSuccess = await _backendService.ImportVocabulariesAsync(lessonId, file);
            if (isSuccess) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Import thất bại. Vui lòng kiểm tra lại định dạng file." });
        }
    }
}