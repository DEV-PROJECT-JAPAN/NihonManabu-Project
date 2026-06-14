
    using System.Net.Http.Json;
    using FrontendRazorPage.Models;
    using Microsoft.AspNetCore.Http;    

namespace FrontendRazorPage.Core.Services
    {
        public class DashboardService : BaseApiServices
    {
      
        public DashboardService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<Dashboard?> GetDashboardAsync()
        {
            
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWToken"];

            if (string.IsNullOrEmpty(token)) return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<Dashboard>("api/admin/dashboard");
        }
    }
    }

