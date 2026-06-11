namespace BackendAPI.DTOs // Hoặc đặt theo Namespace dự án Razor của bạn, ví dụ: RazorFrontend.Models
{
    public class VocabularyDTO

    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string? Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
        public string? ExampleSentence { get; set; }
        public string? AudioUrl { get; set; }
        // dùng để dịch vụ đọc khi mà có cả Kanji và Hiragana thì ưu tiên đọc Hiragana, nếu không có thì đọc Kanji
        public string ? TextToSpeak => !string.IsNullOrEmpty(Hiragana) ? Hiragana : Kanji;
    }
}