using BackendAPI.DTOs;
using BackendAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace BackendAPI.Controllers
{
    [Authorize(Roles = "Admin")] 
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // =========================
        // DASHBOARD
        // =========================
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var totalUsers = await _adminService.GetTotalUsers();
                return Ok(new { totalUsers = totalUsers }); // Sửa chữ t thường ở đây luôn nhé!
            }
            catch (Exception ex)
            {
                // In lỗi chi tiết ra màn hình Console của Backend để dễ Debug
                Console.WriteLine($"[Dashboard Error]: {ex.Message}");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // =========================
        // GET ALL USERS
        // =========================
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetAllUsers();

            return Ok(users);
        }

        // =========================
        // CHANGE ROLE
        // =========================
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto request)
        {
            var result = await _adminService.ChangeUserRole(request.UserId, request.Role);

            if (!result)
                return NotFound("User not found");

            return Ok(new { message = "Updated successfully" });
        }
    }
}