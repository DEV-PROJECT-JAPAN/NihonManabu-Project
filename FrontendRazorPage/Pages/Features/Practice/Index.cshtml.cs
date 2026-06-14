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

        public List<VocabularyModel> PracticeVocabularySystem { get; set; } = new();

        public List<UserFlashcardList> Userfolders { get; set; } = new();

        public string DebugError { get; set; } = "";

        /// <summary>
        /// Public property binding: Collection of 5 random vocabulary items
        /// Accessible from the Razor view via @Model.DailyVocabulary
        /// </summary>
        public List<PracticeModel> PracticeVocabularyUser { get; set; } = new();

        public IndexModel(PracticeClientService PracticeService)
        {
            _PracticeService = PracticeService;
        }


        // Hàm này sinh ra chỉ để đón cái ID từ JS gửi lên và trả về JSON
        public async Task<JsonResult> OnGetLoadFolderVocabAsync(int folderId)
        {
            try
            {
                // Gọi Service lấy dữ liệu
                var words = await _PracticeService.GetFolderVocabAsync(folderId);

                // Trả về JSON với biến words viết thường
                return new JsonResult(words);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi load từ vựng: {ex.Message}");
                return new JsonResult(new List<PracticeModel>());
            }
        }
        // Hàm này sẽ được gọi khi người dùng vào trang, nó sẽ lấy tất cả từ vựng của system và chọn ngẫu nhiên 5 từ theo lesson để hiển thị
        public async Task OnGetAsync()
        {
            try
            {
                Userfolders = await _PracticeService.GetUserFoldersAsync();
                Console.WriteLine($"Fetched {Userfolders.Count} user folders for dropdown.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user folders: {ex.Message}");
                Userfolders = new List<UserFlashcardList>();
            }

            try
            {
                var allVocab = await _PracticeService.GetVocabularySystemPracticeAsync();

                if (allVocab.Any())
                {
                        // Randomly select 5 items from the available vocabulary
                        // If less than 5 items exist, return all available items
                        PracticeVocabularySystem = allVocab
                            .OrderBy(x => _random.Next())
                            .Take(5)
                            .Select(v => new VocabularyModel
                            {
                                Id = v.Id,
                                LessonId = v.LessonId,
                                Kanji = v.Kanji,
                                Hiragana = v.Hiragana,
                                Romaji = v.Romaji,
                                Meaning = v.Meaning,
                                ExampleSentence = v.ExampleSentence,
                                TextToSpeak = v.TextToSpeak
                            })
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error and provide empty list as fallback
                DebugError += $"Error fetching daily vocabulary: {ex.Message}";
                PracticeVocabularySystem = new List<VocabularyModel>();
            }
        }
        // Tên tham số FolderName và VocabularyFile phải khớp với thuộc tính name="..." trong thẻ <input> HTML của anh
        public async Task<IActionResult> OnPostUploadDataAsync(string FolderName, string Description, IFormFile VocabularyFile)
        {
            if (VocabularyFile != null && !string.IsNullOrWhiteSpace(FolderName))
            {
                bool isSuccess = await _PracticeService.UploadExcelToApiAsync(FolderName, Description, VocabularyFile);

                if (isSuccess)
                {
                    // Tải xong thì F5 lại trang để cập nhật cái Folder mới tinh lên giao diện
                    return RedirectToPage();
                }
                else
                {
                    // Chỗ này anh có thể nhét lỗi vào biến DebugError như bữa trước
                    Console.WriteLine("Upload thất bại!");
                }
            }

            return Page(); // Lỗi thì ở lại trang cũ
        }
        // Hàm này đón lệnh từ nút Xóa trên giao diện
        public async Task<JsonResult> OnGetDeleteFolderAsync(int folderId)
        {
            bool isSuccess = await _PracticeService.DeleteFolderAsync(folderId);
            return new JsonResult(new { success = isSuccess });
        }
    }
}
