using BackendAPI.DTOs;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/levels")]
    public class LevelController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        // ==================== 1 & 2. READ (ALL & DETAILS) ====================

        [HttpGet]
        public async Task<IActionResult> GetAllForUser()
        {
            var userData = await _levelService.GetAllLevelsAsync<LevelDTO>();
            return Ok(userData);
        }

        // ==================== 2. API LẤY DANH SÁCH CHO ADMIN ====================
        // Đường dẫn: GET /api/levels/admin
        [HttpGet("admin")]
        // [Authorize(Roles = "Admin")] // Bạn có thể mở comment này nếu có làm phân quyền JWT
        public async Task<IActionResult> GetAllForAdmin()
        {
            var adminData = await _levelService.GetAllLevelsAsync<Level>();
            return Ok(adminData);
        }

        // Đường dẫn: GET /api/levels/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdForUser(int id)
        {
            var userData = await _levelService.GetLevelByIdAsync<LevelDTO>(id);
            if (userData == null) return NotFound("Không tìm thấy Level.");

            return Ok(userData);
        }

        // ==================== 2. API LẤY CHI TIẾT CHO ADMIN ====================
        // Đường dẫn: GET /api/levels/admin/5
        [HttpGet("admin/{id:int}")]
        // [Authorize(Roles = "Admin")] // Mở ra nếu bạn cấu hình phân quyền JWT
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var adminData = await _levelService.GetLevelByIdAsync<Level>(id);
            if (adminData == null) return NotFound("Không tìm thấy Level.");

            return Ok(adminData);
        }

        /// ==================== 3. CREATE (CHỈ ADMIN) ====================
        // Đường dẫn: POST /api/levels/admin
        [HttpPost("admin")]
        // [Authorize(Roles = "Admin")] // Mở ra nếu bạn sử dụng JWT để chặn User từ vòng gửi xe
        public async Task<IActionResult> Create([FromBody] Level levelData)
        {
            // Admin tạo thì truyền thẳng nguyên Model Level vào Service để lưu đủ thông tin
            var result = await _levelService.CreateLevelAsync(levelData);

            // Trả về kèm đường dẫn link tới hàm GetByIdForAdmin đã tạo ở bước trước
            return CreatedAtAction("GetByIdForAdmin", new { id = result.Id }, result);
        }

        // ==================== 4. UPDATE (CHỈ ADMIN) ====================
        // Đường dẫn: PUT /api/levels/admin/5
        [HttpPut("admin/{id:int}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Level levelData)
        {
            // Truyền trực tiếp Model Level vào xử lý
            var isUpdated = await _levelService.UpdateLevelAsync(id, levelData);

            if (!isUpdated)
                return NotFound("Không tìm thấy Level để cập nhật.");

            return NoContent(); // Trả về mã 204 (Cập nhật thành công, không trả về dữ liệu dư thừa)
        }

        // ==================== 5. DELETE (CHỈ ADMIN) ====================
        // Đường dẫn: DELETE /api/levels/admin/5
        [HttpDelete("admin/{id:int}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _levelService.DeleteLevelAsync(id);

            if (!isDeleted)
                return NotFound("Không tìm thấy Level để xóa.");

            return NoContent(); // Trả về mã 204 Xóa thành công
        }
    }
}