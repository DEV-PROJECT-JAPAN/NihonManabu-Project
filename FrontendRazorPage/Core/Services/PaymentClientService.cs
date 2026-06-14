using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontendRazorPage.Services
{
    public class PaymentClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "https://localhost:7104/api/Payment";

        public PaymentClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateVipQrAsync(string token)
        {
            try
            {
                // Gắn JWT vào Header gửi đi
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"{_apiBase}/generate-vip-qr", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<QrResponseResult>();
                    return result?.Success == true ? result.QrUrl : null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private class QrResponseResult
        {
            public bool Success { get; set; }
            public string QrUrl { get; set; }
        }
    }
}