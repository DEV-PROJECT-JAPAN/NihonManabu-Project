using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;
namespace FrontendRazorPage.Models.AdminModel
{
    public class VocabularyAdminModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
     
        public string ?Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
    
        public string ? ExampleSentence { get; set; }
        public string? AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
