using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace FrontendRazorPage.Models
{
    public class VocabularyModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string? Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
        public string ExampleSentence { get; set; }
        [ValidateNever]
        public string ? TextToSpeak { get; set; }
    }
}
