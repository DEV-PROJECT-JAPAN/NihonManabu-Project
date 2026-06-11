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

        [BindProperty(SupportsGet = true)]
        public int SelectedLessonId { get; set; }

        // Đối tượng hứng dữ liệu từ Form gõ tay
        [BindProperty]
        public VocabularyAdminModel VocabularyInput { get; set; } = new();

        // Đối tượng hứng File Import từ giao diện
        [BindProperty]
        public IFormFile? FileUpload { get; set; }

        [TempData]
        public string Message { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Tự động gán LessonId từ bộ lọc URL vào đối tượng nhập liệu
            VocabularyInput.LessonId = SelectedLessonId;
        }

        /// <summary>
        /// XỬ LÝ 1: Admin gõ tay thủ công bằng Form
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng điền đầy đủ các thông tin bắt buộc.";
                return Page();
            }
            var success = await _vocabClientService.CreateVocabularyAsync(VocabularyInput);
            if (success)
            {
                Message = $"Thêm từ vựng '{VocabularyInput.Kanji}' thành công!";
                return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
            }

            ErrorMessage = "Đã xảy ra lỗi trong quá trình thêm từ vựng.";
            return Page();
        }

        /// <summary>
        /// XỬ LÝ 2: Admin Import file dữ liệu hàng loạt (CSV hoặc TXT phân tách bằng dấu phẩy)
        /// </summary>
        public async Task<IActionResult> OnPostImportAsync()
        {
            if (FileUpload == null || FileUpload.Length == 0)
            {
                ErrorMessage = "Vui lòng chọn một file dữ liệu hợp lệ (.csv hoặc .txt) để import.";
                return Page();
            }

            var extension = Path.GetExtension(FileUpload.FileName).ToLower();
            if (extension != ".csv" && extension != ".txt")
            {
                ErrorMessage = "Hệ thống hiện tại chỉ hỗ trợ import từ file định dạng .csv hoặc .txt";
                return Page();
            }

            int successCount = 0;
            int failCount = 0;

            try
            {
                // 🟢 ĐỒNG BỘ: Sử dụng Encoding.UTF8 để triệt tiêu ký tự ẩn BOM chống lỗi tiếng Nhật
                using var reader = new StreamReader(FileUpload.OpenReadStream(), System.Text.Encoding.UTF8);
                bool isHeader = true;

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (isHeader) { isHeader = false; continue; }

                    // Cấu trúc cột file CSV mới: Kanji, Hiragana, Meaning, Romaji, ExampleSentence, TextToSpeak
                    var parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        var model = new VocabularyAdminModel
                        {
                            LessonId = SelectedLessonId,
                            Kanji = string.IsNullOrEmpty(parts[0].Trim()) ? null : parts[0].Trim(),
                            Hiragana = parts[1].Trim(),
                            Meaning = parts[2].Trim(),
                            Romaji = parts[3].Trim(),
                            // Xử lý an toàn nếu câu ví dụ hoặc text chỉnh âm để trống
                            ExampleSentence = parts.Length > 4 ? parts[4].Trim() : "",    };

                        var isSaved = await _vocabClientService.CreateVocabularyAsync(model);
                        if (isSaved) successCount++; else failCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }

                Message = $"Import hoàn tất! Thêm thành công {successCount} từ vựng. Thất bại {failCount} từ.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống khi đọc file: {ex.Message}";
            }

            return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId, SelectedLessonId = SelectedLessonId });
        }
    }
}