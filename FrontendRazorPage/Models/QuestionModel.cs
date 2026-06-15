namespace FrontendRazorPage.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public int GrammarId { get; set; }
        public string Content { get; set; } = null!;
        public int QuestionType { get; set; } = 0;
        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();
    }
}
