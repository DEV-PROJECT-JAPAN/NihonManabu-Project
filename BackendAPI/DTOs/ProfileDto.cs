namespace BackendAPI.DTOs
{
    public class ProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalExp { get; set; }
        public int CurrentStreak { get; set; }
        public string Role { get; set; } = "User";
        public DateTime? LastActiveDate { get; set; }
    }
}
