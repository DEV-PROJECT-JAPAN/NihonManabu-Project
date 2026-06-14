using BackendAPI.DTOs;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/lessons")] // Route chính cho bài học
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        // ==================== CHO USER ====================

        // Lấy bài học theo Cấp độ (Dùng cho trang hiển thị của người học)
        // Đường dẫn: GET /api/lessons/level/5
        [HttpGet("level/{levelId:int}")]
        public async Task<IActionResult> GetByLevel(int levelId)
        {
            var userData = await _lessonService.GetLessonsByLevelAsync<LessonDTO>(levelId);
            return Ok(userData);
        }

        [HttpGet("admin/by-level/{levelId:int}")]
        public async Task<IActionResult> GetByLevelForAdmin(int levelId)
        {
            // Gọi hàm Generic dưới Service với kiểu <Lesson> gốc để lôi đủ ngày tháng ra
            var adminData = await _lessonService.GetLessonsByLevelAsync<Lesson>(levelId);
            return Ok(adminData);
        }

        [HttpGet("{id:int}")] // Đường dẫn sẽ là: GET api/lessons/{id} (Ví dụ: api/lessons/5)
        public async Task<IActionResult> GetByIdForUser(int id)
        {
            // Gọi hàm Generic dưới Service với kiểu Nhận là LessonDTO để lấy dữ liệu sạch
            var userData = await _lessonService.GetLessonByIdAsync<LessonDTO>(id);

            if (userData == null)
            {
                return NotFound(new { message = "Không tìm thấy bài học này hoặc bài học đã bị xóa." });
            }

            // Trả về dữ liệu kiểu LessonDTO cho người dùng học bài
            return Ok(userData);
        }
        // ==================== CHO ADMIN ====================

        // Đường dẫn: GET /api/lessons/admin
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var adminData = await _lessonService.GetAllLessonsAsync<Lesson>();
            return Ok(adminData);
        }

        // Đường dẫn: GET /api/lessons/admin/5
        [HttpGet("admin/{id:int}")]
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var adminData = await _lessonService.GetLessonByIdAsync<Lesson>(id);
            if (adminData == null) return NotFound("Không tìm thấy bài học.");
            return Ok(adminData);
        }

        // Đường dẫn: POST /api/lessons/admin
        //[HttpPost("admin")]
        //public async Task<IActionResult> Create([FromBody] Lesson lessonData)
        //{
        //    var result = await _lessonService.CreateLessonAsync(lessonData);
        //    return CreatedAtAction(nameof(GetByIdForAdmin), new { id = result.Id }, result);
        //}
        [HttpPost("admin")]
        public async Task<IActionResult> Create([FromBody] LessonDTO lessonDto) // 🟢 1. Hứng bằng DTO sạch
        {
            if (lessonDto == null) return BadRequest("Dữ liệu trống.");

            // 🟢 2. Tự tạo thực thể Lesson mới, không mang theo Id = 0 bậy bạ từ ngoài vào
            var lessonData = new Lesson
            {
                LevelId = lessonDto.LevelId,
                Title = lessonDto.Title,
                Order = lessonDto.Order,

                // Tiện tay tự gán ngày giờ hệ thống tại đây luôn, Admin không cần nhập
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 🟢 3. Truyền thực thể sạch này xuống Service để lưu
            var result = await _lessonService.CreateLessonAsync(lessonData);
            var responseDto = new LessonDTO
            {
                Id = result.Id,
                LevelId = result.LevelId,
                Title = result.Title,
                Order = result.Order
            };
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = result.Id }, result);
        }

        // Đường dẫn: PUT /api/lessons/admin/5
        [HttpPut("admin/{id:int}")]
        //public async Task<IActionResult> Update(int id, [FromBody] Lesson lessonData)
        //{
        //    var isUpdated = await _lessonService.UpdateLessonAsync(id, lessonData);
        //    if (!isUpdated) return NotFound("Không tìm thấy bài học để cập nhật.");
        //    return NoContent();
        //}
        public async Task<IActionResult> Update(int id, [FromBody] LessonDTO lessonDto) // 🟢 1. Đổi sang LessonDTO để hứng dữ liệu thô sạch sẽ
        {
            if (lessonDto == null) return BadRequest("Dữ liệu không hợp lệ.");

            // 🟢 2. Tự tay đóng gói dữ liệu từ DTO sang một thực thể Lesson tạm thời để truyền xuống Service của bạn
            var lessonData = new Lesson
            {
                Id = id, // Gán ID từ URL vào thực thể
                LevelId = lessonDto.LevelId,
                Title = lessonDto.Title,
                Order = lessonDto.Order
            };

            // 🟢 3. Gọi lại đúng cái hàm Service hiện tại của bạn mà không cần sửa đổi gì bên dưới
            var isUpdated = await _lessonService.UpdateLessonAsync(id, lessonData);

            if (!isUpdated) return NotFound("Không tìm thấy bài học để cập nhật.");
            return NoContent();
        }
        // Đường dẫn: DELETE /api/lessons/admin/5
        [HttpDelete("admin/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _lessonService.DeleteLessonAsync(id);
            if (!isDeleted) return NotFound("Không tìm thấy bài học để xóa.");
            return NoContent();
        }
    }
}