using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Practice
{
    /// <summary>
    /// ViewModel for vocabulary items displayed in the Daily Draw
    /// </summary>
    public class DailyVocabularyItemDto
    {
        public int Id { get; set; }
        public string Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Meaning { get; set; }
    }

    /// <summary>
    /// IndexModel - Handles Daily Vocabulary Draw (Gacha/Lootbox style)
    /// Fetches 5 random vocabulary words and displays them as face-down cards
    /// that users can flip to reveal the content.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly VocabularyClientService _vocabularyService;
        private readonly Random _random = new();

        /// <summary>
        /// Public property binding: Collection of 5 random vocabulary items
        /// Accessible from the Razor view via @Model.DailyVocabulary
        /// </summary>
        public List<DailyVocabularyItemDto> DailyVocabulary { get; set; } = new();

        public IndexModel(VocabularyClientService vocabularyService)
        {
            _vocabularyService = vocabularyService;
        }

        /// <summary>
        /// GET handler - Fetches 5 random vocabulary words on page load
        /// Gets all available vocabulary and randomly selects 5 items
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                // For demonstration, we'll fetch vocabulary from the first lesson
                // In production, you might want to fetch from all lessons or a specific category
                var lessons = await _vocabularyService.GetLessonsAsync(1);

                if (lessons.Any())
                {
                    var firstLesson = lessons.First();
                    var allVocab = await _vocabularyService.GetCardsAsync(firstLesson.Id);

                    if (allVocab.Any())
                    {
                        // Randomly select 5 items from the available vocabulary
                        // If less than 5 items exist, return all available items
                        DailyVocabulary = allVocab
                            .OrderBy(x => _random.Next())
                            .Take(5)
                            .Select(v => new DailyVocabularyItemDto
                            {
                                Id = v.Id,
                                Kanji = v.Kanji,
                                Hiragana = v.Hiragana,
                                Meaning = v.Meaning
                            })
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and provide empty list as fallback
                Console.WriteLine($"Error fetching daily vocabulary: {ex.Message}");
                DailyVocabulary = new List<DailyVocabularyItemDto>();
            }
        }
    }
}
