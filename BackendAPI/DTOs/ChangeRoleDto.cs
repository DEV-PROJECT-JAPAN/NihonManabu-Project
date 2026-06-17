namespace BackendAPI.DTOs
{
    public class ChangeRoleDto
    {
        public int UserId { get; set; }
        public string Role { get; set; } = null!;
    }
}
