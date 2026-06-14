using System.Text.Json.Serialization;

namespace FrontendRazorPage.Models
{
    public class Dashboard
    {
        [JsonPropertyName("totalUsers")]
        public int TotalUsers { get; set; }

    }
}
