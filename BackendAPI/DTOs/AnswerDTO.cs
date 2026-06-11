namespace BackendAPI.DTOs
{
    public class AnswerDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public string DisplayOrder { get; set; } = null!;

    }
}
