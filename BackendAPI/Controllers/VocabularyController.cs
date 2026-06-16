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

        // ==================== KHU VỰC DÀNH CHO USER LEARNING ====================
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
        // ===================================================================
        // ==================== KHU VỰC XỬ LÝ DÀNH CHO ADMIN ====================
        // ===================================================================

        
        [HttpGet("admin/by-lesson/{lessonId:int}")]
        public async Task<IActionResult> GetVocabulariesByLessonForAdmin(int lessonId)
        {
            // Gọi xuống Service bốc nguyên bản danh sách từ vựng (chứa đủ CreatedAt, UpdatedAt, Romaji, ExampleSentence, TextToSpeak)
            var data = await _backendService.GetVocabulariesByLessonForAdminAsync(lessonId);
            return Ok(data);
        }

        /// <summary>
        /// 2. API lấy chi tiết 1 từ vựng theo ID (Phục vụ đổ dữ liệu lên Form Sửa)
        /// Đường dẫn: GET api/vocabulary/admin/5
        /// </summary>
        [HttpGet("admin/{id:int}")]
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var vocab = await _backendService.GetVocabByIdAsync(id); // Bạn nhớ viết hàm GetVocabByIdAsync dưới Service nhé
            if (vocab == null) return NotFound("Không tìm thấy từ vựng này.");
            return Ok(vocab);
        }

        /// <summary>
        /// 3. API Thêm mới Từ vựng (Hỗ trợ cả Gõ tay lẫn Import File từ Frontend)
        /// Đường dẫn: POST api/vocabulary/admin
        /// </summary>
        [HttpPost("admin")]
        public async Task<IActionResult> CreateForAdmin([FromBody] Vocabulary vocabData)
        {
            if (!ModelState.IsValid)
            {
                // Trả về lỗi 400 kèm theo danh sách các ô nhập liệu bị sai/thiếu cụ thể
                return BadRequest(ModelState);
            }


            vocabData.AudioUrl = null;

            var result = await _backendService.CreateVocabularyAsync(vocabData);
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = result.Id }, result);
        }
        /// <summary>
        /// 4. API Cập nhật Từ vựng
        /// Đường dẫn: PUT api/vocabulary/admin/5
        /// </summary>
        [HttpPut("admin/{id:int}")]
        public async Task<IActionResult> UpdateForAdmin(int id, [FromBody] Vocabulary vocabData)
        {
            if (vocabData == null) return BadRequest("Dữ liệu không hợp lệ.");

            // Gọi xuống hàm Update dưới tầng Service của bạn
            bool isUpdated = await _backendService.UpdateVocabularyAsync(id, vocabData);

            if (!isUpdated) return NotFound("Không tìm thấy từ vựng để cập nhật.");
            return NoContent(); // Trả về 204 chuẩn RESTful khi update thành công
        }

        /// <summary>
        /// 5. API Xóa từ vựng ra khỏi bài học
        /// Đường dẫn: DELETE api/vocabulary/admin/5
        /// </summary>
        [HttpDelete("admin/{id:int}")]
        public async Task<IActionResult> DeleteForAdmin(int id)
        {
            // Gọi xuống hàm Delete dưới tầng Service của bạn
            bool isDeleted = await _backendService.DeleteVocabularyAsync(id);

            if (!isDeleted) return NotFound("Không tìm thấy từ vựng để xóa.");
            return Ok(new { message = "Xóa từ vựng thành công!" });
        }
        [HttpGet("export-pdf/{lessonId}")]
        public async Task<IActionResult> ExportPdf(int lessonId)
        {
            // Controller chỉ gọi xuống Service giải quyết hộ
            byte[] pdfBytes = await _backendService.ExportVocabularyPdfAsync(lessonId);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("Bài học này chưa có từ vựng để xuất file PDF.");
            }

            // Trả file về cho bên phía Angular đón nhận
            return File(pdfBytes, "application/pdf", $"DanhSachTuVung_Bai{lessonId}.pdf");
        }

    }
}