using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FrontendRazorPage.Pages.Features.Admin
{
    [Authorize(Roles = "Admin")] // Chỉ những người có Role Admin mới được vào
    public class AdminDashboardModel : PageModel
    {
        private readonly DashboardService _dashboardService;

        public AdminDashboardModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public Dashboard Dashboard { get; set; } = new();


        public string Message { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var result = await _dashboardService.GetDashboardAsync();

                if (result == null)
                {
                    Message = "Không tải được dữ liệu dashboard.";
                    return;
                }

                Dashboard = result;
            }
            catch (Exception ex)
            {
                Message = "Lỗi khi tải dashboard: " + ex.Message;
            }
        }
    }
}