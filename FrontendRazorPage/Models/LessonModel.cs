namespace FrontendRazorPage.Models
{
    public class LessonModel
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
