using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;

namespace FrontendRazorPage.Pages.Features.Admin
{
    [Authorize(Roles = "Admin")]
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
                    // Token có thể hết hạn hoặc không có quyền
                    Message = "Phiên đăng nhập hết hạn hoặc bạn không có quyền truy cập.";
                    return;
                }

                Dashboard = result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                await Task.Delay(2000);
                Response.Redirect("/Features/Auth/Login");
            }
            catch (Exception ex)
            {
                Message = $"Lỗi khi tải dashboard: {ex.Message}";
            }
        }


    }
}
