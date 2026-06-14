namespace FrontendRazorPage.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int CurrentStreak { get; set; }
        public int TotalExp { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }

    public class ChangeRoleRequest
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}