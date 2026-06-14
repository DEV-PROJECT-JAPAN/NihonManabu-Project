using System.Net.Http.Json;
using FrontendRazorPage.Models;
using Microsoft.AspNetCore.Http;

namespace FrontendRazorPage.Core.Services
{
    public class UService : BaseApiServices
    {
        public UService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<UserDto>?> GetAllUsersAsync()
        {
            SetAuthHeader();

            try
            {
                var response = await _httpClient.GetAsync("api/admin/users");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, string newRole)
        {
            SetAuthHeader();

            try
            {
                var request = new ChangeRoleRequest { UserId = userId, Role = newRole };
                var response = await _httpClient.PutAsJsonAsync("api/admin/change-role", request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing role: {ex.Message}");
                return false;
            }
        }
    }
}