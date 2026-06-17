using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace BackendAPI.Services
{
    public class VocabularyService : IVocabularyService
    {
        private readonly JapaneseDbContext _context;
        public VocabularyService(JapaneseDbContext context) => _context = context;
        // ==================== KHU VỰC DÀNH CHO USER  =======
        public async Task<List<LevelDTO>> GetAllLevelsAsync()
        {
            return await _context.Levels.Select(l => new LevelDTO
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description
            }).ToListAsync();
        }

        public async Task<List<LessonDTO>> GetLessonsByLevelAsync(int levelId)
        {
            return await _context.Lessons
                .Where(l => l.LevelId == levelId)
                .OrderBy(l => l.Order)
                .Select(l => new LessonDTO//gasn vào đt 
                {
                    Id = l.Id,
                    LevelId = l.LevelId,
                    Title = l.Title,
                    Order = l.Order
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<VocabularyDTO>> GetVocabulariesByLessonAsync(int lessonId)
        {
            return await _context.Vocabularies
                 .Where(v => v.LessonId == lessonId)
                 .Select(v => new VocabularyDTO
                 {
                     Id = v.Id,
                     LessonId = v.LessonId,
                     Kanji = v.Kanji,
                     Hiragana = v.Hiragana,
                     Meaning = v.Meaning,
                     Romaji = v.Romaji,
                     ExampleSentence = v.ExampleSentence
                 })
                 .ToListAsync();
        }



        public async Task<bool> UpdateProgressAsync(int userId, UpdateLearningProgresByUserDTO input)
        {
            var progress = await _context.LearningProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.VocabularyId == input.VocabularyId);

            if (progress == null)
            {
                _context.LearningProgresses.Add(new LearningProgress
                {
                    UserId = userId,
                    VocabularyId = input.VocabularyId,
                    IsMasstered = input.IsMastered,
                    LastReviewed = DateTime.Now,
                    ReviewCount = 1 // Lần đầu tiên tương tác thì khởi tạo bằng 1
                });
            }
            else
            {
                progress.IsMasstered = input.IsMastered;
                progress.LastReviewed = DateTime.Now;
                progress.ReviewCount += 1; // Các lần sau bấm lại thì tự động cộng dồn lên 1 đơn vị
                _context.LearningProgresses.Update(progress);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        // ==================== KHU VỰC XỬ LÝ DÀNH CHO ADMIN ====================


        public async Task<IEnumerable<Vocabulary>> GetVocabulariesByLessonForAdminAsync(int lessonId)
        {
            return await _context.Vocabularies
                .AsNoTracking() // Tối ưu hiệu năng đọc dữ liệu danh sách
                .Where(v => v.LessonId == lessonId)
                .ToListAsync();
        }

        /// <summary>
        /// 2. Lấy chi tiết 1 từ vựng theo ID để đổ lên Form Sửa (Edit)
        /// </summary>
        public async Task<Vocabulary?> GetVocabByIdAsync(int id)
        {
            return await _context.Vocabularies
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
        }


        /// <summary>
        /// 3. Hàm tạo mới Từ vựng (Tự động bốc TextToSpeak sạch để ép sinh link âm thanh Google TTS)
        /// </summary>
        public async Task<Vocabulary> CreateVocabularyAsync(Vocabulary vocabData)
        {
            if (vocabData == null) throw new ArgumentNullException(nameof(vocabData));
            vocabData.Kanji = vocabData.Kanji ?? "";
            vocabData.ExampleSentence = vocabData.ExampleSentence ?? "";
            // Đóng dấu thời gian hệ thống chuẩn quốc tế UTC
            vocabData.CreatedAt = DateTime.Now;
            vocabData.UpdatedAt = DateTime.Now;

            // Đẩy vào context chuẩn bị lưu (Trường AudioUrl sẽ tự nhận null từ Frontend gửi qua)
            _context.Vocabularies.Add(vocabData);

            // Chốt hạ lưu xuống cơ sở dữ liệu SQL Server
            await _context.SaveChangesAsync();
            return vocabData;
        }
        /// <summary>
        /// 4. Hàm cập nhật dữ liệu sửa đổi của Admin (Giữ nguyên ngày tạo, tự động đẩy ngày giờ cập nhật)
        /// </summary>
        public async Task<bool> UpdateVocabularyAsync(int id, Vocabulary vocabData)
        {
            var existing = await _context.Vocabularies.FindAsync(id);
            if (existing == null || vocabData == null) return false;
            if (!string.IsNullOrWhiteSpace(vocabData.Kanji))
                existing.Kanji = vocabData.Kanji;

            // Tiến hành map đè dữ liệu mới từ Form sang bản ghi cũ trong DB
            existing.Kanji = vocabData.Kanji;
            existing.Hiragana = vocabData.Hiragana;
            existing.Meaning = vocabData.Meaning;
            existing.Romaji = vocabData.Romaji;
            existing.ExampleSentence = vocabData.ExampleSentence;
            existing.LessonId = vocabData.LessonId;

            // Admin giấu trường này đi nên giá trị truyền qua lại luôn là null an toàn
            existing.AudioUrl = vocabData.AudioUrl;

            // Cập nhật lại mốc thời gian chỉnh sửa mới nhất, tuyệt đối giữ nguyên CreatedAt gốc
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 5. Hàm xóa vĩnh viễn từ vựng ra khỏi bài học
        /// </summary>
        public async Task<bool> DeleteVocabularyAsync(int id)
        {
            var vocab = await _context.Vocabularies.FindAsync(id);
            if (vocab == null) return false;

            _context.Vocabularies.Remove(vocab);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<byte[]> ExportVocabularyPdfAsync(int lessonId)
        {
            // 1. Dùng _context để chọc thẳng vào Database lấy từ vựng theo LessonId
            var vocabularies = await _context.Vocabularies // (Nhớ sửa 'Vocabularies' thành tên DbSet thật trong file DbContext của bạn)
                                             .Where(v => v.LessonId == lessonId)
                                             .ToListAsync();

            if (vocabularies == null || !vocabularies.Any())
            {
                return Array.Empty<byte>(); // Trả về mảng rỗng nếu không có dữ liệu
            }

            // 2. Dùng QuestPDF vẽ giao diện file PDF
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Tiêu đề
                    page.Header().Text("Danh Sách Từ Vựng")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    // Bảng dữ liệu
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Cột Từ vựng
                            columns.RelativeColumn(2); // Cột Ý nghĩa
                            columns.RelativeColumn(3); // Cột Ví dụ
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Từ vựng").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ý nghĩa").Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Ví dụ").Bold();
                        });

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

                    // Số trang
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            // 3. Xuất ra mảng bytes để Controller mang đi trả về cho Angular
            return document.GeneratePdf();
        }
    }
}