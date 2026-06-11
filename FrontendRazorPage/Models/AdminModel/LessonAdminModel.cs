namespace FrontendRazorPage.Models.AdminModel
{
    public class LessonAdminModel
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
