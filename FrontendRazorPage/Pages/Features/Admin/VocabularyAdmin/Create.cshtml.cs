using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models.AdminModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

namespace FrontendRazorPage.Pages.Features.Admin.Vocabularies
{
    public class CreateModel : PageModel
    {
        private readonly VocabularyClientService _vocabClientService;

        public CreateModel(VocabularyClientService vocabClientService)
        {
            _vocabClientService = vocabClientService;
        }

        // Hứng bộ lọc từ URL để sau khi thêm xong thì quay lại đúng bài học cũ
        [BindProperty(SupportsGet = true)]
        public int SelectedLevelId { get; set; }

        [BindProperty(SupportsGet = true, Name = "lessonId")]
        public int SelectedLessonId { get; set; }

        // Đối tượng hứng dữ liệu từ Form gõ tay
        [BindProperty]
        public VocabularyAdminModel VocabularyInput { get; set; } = new();

        // Đối tượng hứng File Import từ giao diện
        [BindProperty]
        public IFormFile? FileUpload { get; set; }

        [TempData]
        public string Message { get; set; } = string.Empty;

       
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            VocabularyInput = new VocabularyAdminModel
            {
                LessonId = SelectedLessonId
            };
        }

      
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng kiểm tra lại các trường dữ liệu bắt buộc màu đỏ.";
                return Page(); // Trả lại trang để hiện các dòng lỗi asp-validation-for
            }

            VocabularyInput.LessonId = SelectedLessonId;
           

            var success = await _vocabClientService.CreateVocabularyAsync(VocabularyInput);
            if (success)
            {
              Message= $"Thêm từ vựng  thành công!";
                return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
            }

            ErrorMessage = "Đã xảy ra lỗi trong quá trình thêm từ vựng.";
            return Page();
        }

      
        public async Task<IActionResult> OnPostImportAsync(int lessonId)
        {
            if (lessonId == 0) lessonId = SelectedLessonId;
          
            // 1. Kiểm tra file có tồn tại và đúng định dạng không
            if (FileUpload == null || FileUpload.Length == 0)
            {
                ErrorMessage = "Vui lòng chọn một file CSV hợp lệ!";
                return Page();
            }

            if (!FileUpload.FileName.EndsWith(".csv") && !FileUpload.FileName.EndsWith(".txt"))
            
                {
                ErrorMessage = "Chỉ chấp nhận file định dạng .csv hoặc .txt!";
                return Page();
            }

            try
            {
                using (var reader = new StreamReader(FileUpload.OpenReadStream()))//đọc 
                {
                    // Bỏ qua dòng tiêu đề nếu file CSV của bạn có tiêu đề (ví dụ: Kanji,Hiragana...)
                    var headerLine = await reader.ReadLineAsync();

                    while (!reader.EndOfStream)// Vòng lặp này sẽ chạy liên tục cho đến khi chạm đáy file.
                    {
                        var line = await reader.ReadLineAsync();//đọc 1 dòng văn bản
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var values = line.Split(new char[] { ',', '\t' });//thành mảng chứa các từ 
                        // Kiểm tra đủ số lượng cột cần thiết
                        if (values.Length < 4) continue;

                        // 2. Map dữ liệu từ CSV vào model
                        var vocab = new VocabularyAdminModel
                        {
                            LessonId = SelectedLessonId,
                            Kanji = values[0].Trim(),
                            Hiragana = values[1].Trim(),
                            Meaning = values[2].Trim(),
                            Romaji = values[3].Trim(),
                            ExampleSentence = values.Length > 4 ? values[4].Trim() : string.Empty
                        };

                        // 3. Gọi Service để lưu vào DB
                        // 3. Gọi Service để lưu vào DB
                        await _vocabClientService.CreateVocabularyAsync(vocab);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi khi đọc file: " + ex.Message;
                return Page();
            }
            Message = "Import file từ vựng thành công!";
            // 4. Thành công thì quay lại trang danh sách
            return RedirectToPage("./Index", new { SelectedLevelId, SelectedLessonId });
        }
    }
}