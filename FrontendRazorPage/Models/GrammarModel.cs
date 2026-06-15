namespace FrontendRazorPage.Models
{
    public class GrammarModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Structure { get; set; } = null!;
        public string Explanation { get; set; } = null!;
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}
