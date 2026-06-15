namespace FrontendRazorPage.Core.Services
{
    public class BaseClientService
    {
        protected readonly HttpClient _httpClient;

        protected BaseClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Gom cái URL gốc về một nơi duy nhất ở đây!
            _httpClient.BaseAddress = new Uri("https://localhost:7104/");
        }
    }
}
