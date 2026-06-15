namespace BackendAPI.DTOs
{
    public class GrammarDTO
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Structure { get; set; } = null!;
        public string Explanation { get; set; } = null!;

        // Hốt kèm danh sách câu hỏi liên kết với mẫu ngữ pháp này
        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
    }
}
