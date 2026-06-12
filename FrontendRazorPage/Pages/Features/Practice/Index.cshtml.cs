using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.DTOs;

namespace FrontendRazorPage.Pages.Features.Practice
{

    /// <summary>
    /// IndexModel - Handles Daily Vocabulary Draw (Gacha/Lootbox style)
    /// Fetches 5 random vocabulary words and displays them as face-down cards
    /// that users can flip to reveal the content.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly PracticeClientService _PracticeService;
        private readonly Random _random = new();

        public List<PracticeModel> PracticeVocabularySystem { get; set; } = new();

        public List<UserFlashcardList> Userfolders { get; set; } = new();

        /// <summary>
        /// Public property binding: Collection of 5 random vocabulary items
        /// Accessible from the Razor view via @Model.DailyVocabulary
        /// </summary>
        public List<PracticeModel> PracticeVocabularyUser { get; set; } = new();

        public IndexModel(PracticeClientService PracticeService)
        {
            _PracticeService = PracticeService;
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
                var lessons = await _PracticeService.GetUserVocabularySystemPracticeAsync(1);

                if (lessons.Any())
                {
                    var firstLesson = lessons.First();
                    var allVocab = await _PracticeService.GetUserVocabularySystemPracticeAsync(firstLesson.Id);

                    if (allVocab.Any())
                    {
                        // Randomly select 5 items from the available vocabulary
                        // If less than 5 items exist, return all available items
                        PracticeVocabularyUser = allVocab
                            .OrderBy(x => _random.Next())
                            .Take(5)
                            .Select(v => new PracticeModel
                            {
                                Id = v.Id,
                                IdLesson = v.IdLesson,
                                Kanji = v.Kanji,
                                Hiragana = v.Hiragana,
                                Romaji = v.Romaji,
                                Meaning = v.Meaning
                            })
                            .ToList();
                        Console.WriteLine($"Fetched {PracticeVocabularyUser.Count} vocabulary items for practice.");
                    }
                }
                if (PracticeVocabularyUser.Any())
                {
                    Console.WriteLine("No vocabulary items found for practice.");
                }
            }
            catch (Exception ex)
            {
                // Log error and provide empty list as fallback
                Console.WriteLine($"Error fetching daily vocabulary: {ex.Message}");
                PracticeVocabularyUser = new List<PracticeModel>();
            }
        }
    }
}
