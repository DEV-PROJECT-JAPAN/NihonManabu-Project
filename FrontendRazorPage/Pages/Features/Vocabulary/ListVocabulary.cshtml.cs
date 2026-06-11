using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FrontendRazorPage.Pages.Features.Vocabulary
{
    public class ListVocabularyModel : PageModel
    {
        private readonly VocabularyClientService _service;

        public ListVocabularyModel(VocabularyClientService service) => _service = service;

        [BindProperty(SupportsGet = true)]
        public int LessonId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int LevelId { get; set; }

        // Biến hiển thị tên bài học ra giao diện
        public string LessonTitle { get; set; } = "Danh sách từ vựng";

        public List<VocabularyModel> Cards { get; set; } = new();

        public async Task OnGetAsync(int lessonId, int levelId)
        {
            LessonId = lessonId;
            LevelId = levelId;

            // Gọi Service lấy danh sách từ vựng theo ID bài học
            Cards = await _service.GetCardsAsync(lessonId);
        }

        // Hàm này cực kỳ quan trọng cho giao diện 3D: Nơi JS gửi data xuống để chấm điểm (thuộc/chưa thuộc)
        public async Task<JsonResult> OnPostUpdateProgressAsync([FromBody] UpdateLearningProgresByUserModel input)
        {
            if (input == null || input.VocabularyId <= 0)
                return new JsonResult(new { success = false });

            bool success = await _service.UpdateProgressAsync(input);
            return new JsonResult(new { success = success });
        }
        public async Task<IActionResult> OnGetExportPdfAsync(int lessonId, int levelId)
        {
            string levelName = $"Level_{levelId}";
            string lessonTitle = $"Bai_{lessonId}";
            //var levels = await _service.GetLevelsAsync();
            //var currentLevel = levels.FirstOrDefault(l => l.Id == levelId);
            //if (currentLevel != null)
            //{
            //    levelName = currentLevel.Name;
            //}

            //// 3. Lấy Tên Bài Học (Sử dụng hàm GetLessonByIdAsync xịn xò của bạn)
            //var currentLesson = await _service.GetLessonByIdAsync(lessonId);
            //if (currentLesson != null)
            //{
            //    lessonTitle = currentLesson.Title;
            //}
            // 1. Lấy danh sách từ vựng của bài học này
            var vocabularies = await _service.GetCardsAsync(lessonId);

            // 2. Dùng code C# "vẽ" ra file PDF mới tinh (không liên quan gì đến HTML/CSS của web)
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Tiêu đề file PDF
                    page.Header().Text($"Danh Sách Từ Vựng")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    // Bảng từ vựng
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Cột Từ vựng
                            columns.RelativeColumn(2); // Cột Nghĩa
                            columns.RelativeColumn(3); // Cột Ví dụ
                        });

                        // Header của bảng
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Từ vựng").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ý nghĩa").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ví dụ").Bold();
                        });

                        // Đổ dữ liệu
                        foreach (var item in vocabularies)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                 .Text($"{item.Kanji}\n({item.Hiragana})");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                 .Text(item.Meaning);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                 .Text(item.ExampleSentence).Italic();
                        }
                    });

                    // Đánh số trang ở góc dưới
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            // 3. Đóng gói thành file và ép trình duyệt tải về
            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"TuVung_Bai_{levelName}_{lessonTitle}.pdf");
        }
    }
}