// FrontendRazorPage/Core/Services/BaseApiService.cs
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace FrontendRazorPage.Core.Services
{
    public abstract class BaseApiServices
    {
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiServices(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        protected void SetAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected void ClearAuthHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}