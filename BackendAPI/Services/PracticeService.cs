using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class PracticeService : IPracticeService
    {
        private readonly JapaneseDbContext _context;

        public PracticeService(JapaneseDbContext context)
        {
            _context = context;
        }
        //danh sách lesson mà user đã học
        public async Task<List<VocabularyDTO>> GetVocabularySystemAsync(int userId)
        {
            // Dùng ToListAsync() chuẩn của EF Core, không cần Task.Run
            return await _context.LearningProgresses
                .Where(lp => lp.UserId == userId)
                // Dùng thuộc tính điều hướng (Navigation Property) để sang thẳng bảng Vocabulary
                .Select(lp => new VocabularyDTO
                {
                    Id = lp.Vocabulary.Id,
                    LessonId = lp.Vocabulary.LessonId,
                    Kanji = lp.Vocabulary.Kanji,
                    Hiragana = lp.Vocabulary.Hiragana,
                    Romaji = lp.Vocabulary.Romaji,
                    Meaning = lp.Vocabulary.Meaning,
                    // Ánh xạ đúng tên cột của anh
                    ExampleSentence = lp.Vocabulary.ExampleSentence
                })
                .ToListAsync();
        }
        //danh sách folder mà user đã tạo
        public async Task<List<UserFlashcardListDTO>> GetUserFoldersAsync(int userId)
        {
            // Không dùng Task.Run, dùng thẳng ToListAsync() của EF Core
            return await _context.UserFlashcardLists
                .Where(p => p.UserId == userId)
                .Select(p => new UserFlashcardListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();
        }
        // ôn tập : lấy tất cả từ vựng mà user đã thêm vào flashcard list của họ
        //public async Task<List<PracticeDTO>> GetVocabularyUserAsync(int FolderId, int UserId)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var listId = _context.UserFlashcardLists.Where(p => p.Id == FolderId && p.UserId == UserId).Select(p => p.Id);

        //        var practicesFolderVocabulary = _context.FolderVocabularies.Where(p => listId.Contains(p.ListId)).ToList();

        //        var practicesUserVocabulary = _context.UserVocabularies.Where(p => practicesFolderVocabulary.Select(fv => fv.VocabularyId).Contains(p.Id)).ToList();
        //        return practicesUserVocabulary.Select(p => new PracticeDTO
        //        {
        //            Id = p.Id,
        //            Kanji = p.Kanji,
        //            Hiragana = p.Hiragana,
        //            Romaji = p.Romaji,
        //            Meaning = p.Meaning
        //        }).ToList();
        //    });
        //}
        // Ôn tập Gacha: Lấy 5 từ vựng ngẫu nhiên qua BẢNG CẦU NỐI
        public async Task<List<PracticeDTO>> GetVocabularyUserAsync(int FolderId, int UserId)
        {
            // Đi thẳng vào bảng cầu nối FolderVocabularies
            return await _context.FolderVocabularies
                .Include(fv => fv.UserVocabulary) // Nối sang lấy mặt chữ
                .Include(fv => fv.UserFlashcardList) // Nối sang để check quyền sở hữu
                .Where(fv => fv.ListId == FolderId && fv.UserFlashcardList.UserId == UserId)
                .OrderBy(fv => Guid.NewGuid()) // Trộn bài ngẫu nhiên (Gacha)
                .Take(5) // Bốc đúng 5 lá
                .Select(fv => new PracticeDTO
                {
                    Id = fv.UserVocabulary.Id,
                    Kanji = fv.UserVocabulary.Kanji,
                    Hiragana = fv.UserVocabulary.Hiragana,
                    Romaji = fv.UserVocabulary.Romaji,
                    Meaning = fv.UserVocabulary.Meaning
                }).ToListAsync();
        }
        //đọc file excel để nạp từ vựng vào flashcard list của user
        //public async Task<bool> UploadFolderExcelAsync(int userId, string folderName, string description, IFormFile file)
        //{
        //    try
        //    {
        //        // 1. Lưu Folder mới vào DB trước để lấy cái ListId
        //        var newFolder = new UserFlashcardList
        //        {
        //            UserId = userId,
        //            Name = folderName,
        //            Description = description
        //        };
        //        _context.UserFlashcardLists.Add(newFolder);
        //        await _context.SaveChangesAsync(); // C# tự động nhét ID mới vào biến newFolder.Id

        //        // 2. Mở file Excel ra đọc
        //        using var stream = new MemoryStream();
        //        await file.CopyToAsync(stream);
        //        using var workbook = new XLWorkbook(stream);

        //        var worksheet = workbook.Worksheet(1); // Mở Sheet đầu tiên

        //        // 1. Quét vùng có dữ liệu
        //        var usedRange = worksheet.RangeUsed();

        //        // 2. MẶC ÁO GIÁP: Kiểm tra xem file có trống không
        //        if (usedRange == null)
        //        {
        //            Console.WriteLine("Lỗi: File Excel trắng tinh hoặc sai định dạng!");
        //            return false; // Dừng cuộc chơi luôn, không lưu vào DB nữa
        //        }

        //        // 3. Nếu có dữ liệu thì mới đếm hàng và bỏ qua dòng Tiêu đề
        //        var rows = usedRange.RowsUsed().Skip(1);
        //        var vocabList = new List<UserVocabulary>();

        //        // 3. Quét từng dòng, map vào DB
        //        foreach (var row in rows)
        //        {
        //            // Lấy dữ liệu từng ô (Cell), hàm GetString() giúp chống lỗi Null
        //            var kanji = row.Cell(1).GetValue<string>();
        //            var hiragana = row.Cell(2).GetValue<string>();
        //            var meaning = row.Cell(3).GetValue<string>();
        //            var Romaji = row.Cell(4).GetValue<string>(); // Nếu có cột Romaji

        //            // Nếu cả 3 cột đều trống thì bỏ qua dòng này
        //            if (string.IsNullOrWhiteSpace(kanji) && string.IsNullOrWhiteSpace(hiragana)) continue;

        //            vocabList.Add(new UserVocabulary
        //            {
        //                Kanji = kanji,
        //                Hiragana = hiragana,
        //                Meaning = meaning,
        //                Romaji = Romaji

        //                // Thêm Romaji hoặc Example nếu Excel của anh có cột thứ 4, 5
        //            });
        //        }

        //        // 4. Lưu một cục nguyên đống từ vựng vào DB
        //        _context.UserVocabularies.AddRange(vocabList);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi đọc Excel: {ex.Message}");
        //        return false;
        //    }
        //}
        // 1. Đổi kiểu trả về ở đây (Nhớ sửa cả trong IPracticeService.cs nữa nhé)
        public async Task<(bool isSuccess, string errorMessage)> UploadFolderExcelAsync(int userId, string folderName, string description, IFormFile file)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. TẠO FOLDER (Bổ sung CreatedAt, UpdatedAt)
                var newFolder = new UserFlashcardList
                {
                    UserId = userId,
                    Name = folderName,
                    Description = description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.UserFlashcardLists.Add(newFolder);
                await _context.SaveChangesAsync();

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var usedRange = worksheet.RangeUsed();
                if (usedRange == null)
                {
                    // 2. Trả về false kèm lý do
                    return (false, "File Excel trắng tinh không có dữ liệu!");
                }

                var rows = usedRange.RowsUsed().Skip(1);
                var folderVocabLinks = new List<FolderVocabulary>();

                foreach (var row in rows)
                {
                    // 2. ĐỌC EXCEL AN TOÀN (Dùng GetString() thay vì GetValue<string>())
                    var kanji = row.Cell(1).GetString()?.Trim();
                    var hiragana = row.Cell(2).GetString()?.Trim();
                    var meaning = row.Cell(3).GetString()?.Trim();
                    var romaji = row.Cell(4).GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(kanji) && string.IsNullOrWhiteSpace(hiragana)) continue;

                    var existingVocab = await _context.UserVocabularies
                        .FirstOrDefaultAsync(v => v.Kanji == kanji && v.Hiragana == hiragana);

                    int vocabIdToLink;

                    if (existingVocab != null)
                    {
                        vocabIdToLink = existingVocab.Id;
                    }
                    else
                    {
                        // 3. TẠO TỪ VỰNG MỚI (Bổ sung đầy đủ các cột bắt buộc)
                        var newVocab = new UserVocabulary
                        {
                            Kanji = kanji,
                            Hiragana = hiragana,
                            Meaning = meaning,
                            Romaji = romaji,
                            IsMastered = false,
                            ReviewCount = 0,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.UserVocabularies.Add(newVocab);
                        await _context.SaveChangesAsync();

                        vocabIdToLink = newVocab.Id;
                    }

                    // 4. TẠO CẦU NỐI (Bổ sung CreatedAt, UpdatedAt)
                    folderVocabLinks.Add(new FolderVocabulary
                    {
                        ListId = newFolder.Id,
                        VocabularyId = vocabIdToLink,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }

                if (folderVocabLinks.Any())
                {
                    var uniqueLinks = folderVocabLinks
                        .GroupBy(x => x.VocabularyId)
                        .Select(g => g.First())
                        .ToList();

                    await _context.FolderVocabularies.AddRangeAsync(uniqueLinks);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // 3. Xử lý xong mượt mà thì báo true
                return (true, "Thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // 4. BẮT GỌN LỖI: Lấy lỗi sâu nhất của EF Core để báo về Swagger

                string errorDetail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return (false, $"Lỗi Database/Excel: {errorDetail}");
            }
        }
        //xóa folder của user, khi xóa folder thì sẽ tự động xóa các bản ghi liên quan trong bảng cầu nối (FolderVocabulary) nhờ vào thiết lập cascade delete trong EF Core
        public async Task<bool> DeleteFolderAsync(int folderId, int userId)
        {
            try
            {
                // 1. Tìm Folder đúng của User đó
                var folder = await _context.UserFlashcardLists
                    .FirstOrDefaultAsync(f => f.Id == folderId && f.UserId == userId);

                if (folder == null) return false;

                // 2. Chỉ cần xóa Folder. EF Core và SQL Server sẽ lo phần còn lại (dọn bảng cầu nối)
                _context.UserFlashcardLists.Remove(folder);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xóa thư mục: {ex.Message}");
                return false;
            }
        }
    }
}
