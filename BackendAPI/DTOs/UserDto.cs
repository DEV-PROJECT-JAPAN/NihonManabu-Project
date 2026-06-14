namespace BackendAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        public int TotalExp { get; set; }
        public int CurrentStreak { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
