namespace FrontendRazorPage.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public string DisplayOrder { get; set; } // Đã đồng bộ kiểu với Backend của Đội trưởng
    }
}
