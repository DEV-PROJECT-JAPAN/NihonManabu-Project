namespace BackendAPI.DTOs
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public int GrammarId { get; set; }
        public string Content { get; set; } = null!;
        public int QuestionType { get; set; } = 0;

        // Trong mỗi câu hỏi lại có danh sách các đáp án lựa chọn
        public List<AnswerDTO> Answers { get; set; } = new List<AnswerDTO>();
    }
}
