
    using System.Net.Http.Json;
    using FrontendRazorPage.Models;


    namespace FrontendRazorPage.Core.Services
    {
        public class DashboardService
        {
            private readonly HttpClient _httpClient;

            public DashboardService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<Dashboard?> GetDashboardAsync()
            {
                return await _httpClient.GetFromJsonAsync<Dashboard>("api/admin/dashboard");
            }
        }
    }

