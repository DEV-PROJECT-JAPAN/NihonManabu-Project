namespace FrontendRazorPage.Models
{
    public class PracticeModel
    {
        public int Id { get; set; }
        public int IdLesson { get; set; }
        public string Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; } = string.Empty;
    }
}
