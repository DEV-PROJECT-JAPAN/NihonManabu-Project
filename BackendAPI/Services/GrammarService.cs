using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using BackendAPI.DTOs.AdminDTO;

namespace BackendAPI.Services
{
    public class GrammarService<T> : IGrammarService<T> where T : class
    {
        private readonly JapaneseDbContext _context;

        public GrammarService(JapaneseDbContext context)
        {
            _context = context;
        }

        //public async Task<GrammarDTO> GetGrammarByIdAsync(int grammarId)
        //{
        //    return await _context.Grammars.Where(g => g.Id == grammarId)
        //        .Select(g => new GrammarDTO
        //        {
        //            Id = g.Id,
        //            LessonId = g.LessonId,
        //            Structure = g.Structure ?? string.Empty,
        //            Explanation = g.Explanation ?? string.Empty
        //        })
        //        .FirstOrDefaultAsync() ?? new GrammarDTO();
        //}


        public async Task<List<GrammarDTO>> GetGrammarByLessonAsync(int lessonId)
        {
            if (lessonId <= 0) return new List<GrammarDTO>();

            return await _context.Grammars
                .Where(g => g.LessonId == lessonId)
                .Select(g => new GrammarDTO
                {
                    Id = g.Id,
                    LessonId = g.LessonId,
                    Structure = g.Structure ?? string.Empty,
                    Explanation = g.Explanation ?? string.Empty
                    // Khách hàng không đòi hỏi Questions nên ở đây không thèm nạp, tiết kiệm tài nguyên!
                })
                .ToListAsync();
        }

        /// <summary>
        /// 🎯 HÀM 2: Lấy ngân hàng câu hỏi + đáp án theo Mẫu ngữ pháp và Chức năng lựa chọn
        /// </summary>
        /// <param name="grammarId">ID của mẫu ngữ pháp đang học</param>
        /// <param name="questionType">"Quiz" (Trắc nghiệm), "Arrange" (Sắp xếp), hoặc "All" (Tổng hợp)</param>
        public async Task<List<QuestionDTO>> GetQuestionsByGrammarAsync(int grammarId, int questionType)
        {
            if (grammarId <= 0) return new List<QuestionDTO>();

            var query = _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.GrammarId == grammarId);

            // Phân loại bộ lọc theo đúng số nguyên Đội trưởng quy định
            if (questionType == 1 || questionType == 2)
            {
                query = query.Where(q => q.QuestionType == questionType);
            }

                return await query
                    .Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        GrammarId = q.GrammarId ?? 0,
                        Content = q.Content ?? string.Empty, // câu hỏi 
                        QuestionType = q.QuestionType, // Bây giờ nó trả về int (1 hoặc 2)

                        Answers = q.Answers != null
                            ? q.Answers.Select(a => new AnswerDTO
                            {
                                Id = a.Id,
                                QuestionId = a.QuestionId,
                                Text = a.Text ?? string.Empty,
                                IsCorrect = a.IsCorrect,
                                DisplayOrder = a.DisplayOrder ?? string.Empty
                            }).ToList()
                            : new List<AnswerDTO>()
                    })
                    .ToListAsync();
        }


        public async Task<GrammarAdminDTO?> GetByIdAsync(int id)
        {
            return await _context.Grammars
                .Include(g => g.Lesson)
                .Select(g => new GrammarAdminDTO
                {
                    Id = g.Id,
                    LessonId = g.LessonId,
                    Structure = g.Structure,
                    Explanation = g.Explanation,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
                })
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Grammar> CreateAsync(Grammar grammar)
        {
            // 1. Tự động gán thời gian tạo/cập nhật trực tiếp trên Entity gốc
            grammar.CreatedAt = DateTime.Now; // Hoặc DateTime.UtcNow tùy dự án của bạn
            grammar.UpdatedAt = DateTime.Now;

            // 2. Vì không dùng DTO nữa, bạn chỉ việc add thẳng đối tượng grammar nhận từ Controller vào DB Context
            _context.Grammars.Add(grammar);

            // 3. Lưu thay đổi xuống Database, sau dòng này thuộc tính grammar.Id sẽ tự động có giá trị do DB sinh ra
            await _context.SaveChangesAsync();

            // 4. Trả về chính đối tượng grammar đã có đầy đủ Id và ngày giờ
            return grammar;
        }

        //public async Task<GrammarAdminDTO> CreateAsync(GrammarAdminDTO grammar)
        //{
        //    // Tự động gán thời gian tạo/cập nhật tại Server 
        //    // Sử dụng DateTime.UtcNow là chuẩn tốt nhất cho API tách rời
        //    grammar.CreatedAt = DateTime.UtcNow;
        //    grammar.UpdatedAt = DateTime.UtcNow;

        //    // Map từ DTO sang Entity gốc (Sửa tên từ "GrammarD" thành "Grammar" chuẩn theo Model của bạn)
        //    var entity = new Grammar
        //    {
        //        LessonId = grammar.LessonId,
        //        Structure = grammar.Structure,
        //        Explanation = grammar.Explanation,
        //        CreatedAt = grammar.CreatedAt,
        //        UpdatedAt = grammar.UpdatedAt
        //    };

        //    _context.Grammars.Add(entity);
        //    await _context.SaveChangesAsync(); // Sau dòng này, entity sẽ tự động có Id từ DB sinh ra

        //    // Trả về DTO kèm Id mới và thời gian chính xác
        //    return new GrammarAdminDTO
        //    {
        //        Id = entity.Id,
        //        LessonId = entity.LessonId,
        //        Structure = entity.Structure,
        //        Explanation = entity.Explanation,
        //        CreatedAt = entity.CreatedAt,
        //        UpdatedAt = entity.UpdatedAt
        //    };
        //}


        public async Task<bool> UpdateAsync(int id, Grammar grammar)
        {
            var existing = await _context.Grammars.FindAsync(id);
            if (existing == null) return false;

            // Đè trực tiếp dữ liệu Admin sửa vào bảng gốc
            existing.LessonId = grammar.LessonId;
            existing.Structure = grammar.Structure;
            existing.Explanation = grammar.Explanation;
            existing.UpdatedAt = DateTime.Now; // Ghi nhận thời gian sửa

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var grammar = await _context.Grammars.FindAsync(id);
            if (grammar == null) return false;

            _context.Grammars.Remove(grammar);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T?> GetGrammarByIdAsync(int grammarId)
        {
            // 1. Luôn luôn bốc bảng gốc lên trước
            var g = await _context.Grammars.Include(g => g.Lesson).FirstOrDefaultAsync(x => x.Id == grammarId);
            if (g == null) return null;

            // 2. Phân loại DTO dựa trên yêu cầu của Controller

            // Nếu phía Admin gọi và đòi GrammarAdminDto
            if (typeof(T) == typeof(Grammar))
            {
                var adminDto = new Grammar
                {
                    Id = g.Id,
                    LessonId = g.LessonId,
                    Structure = g.Structure,
                    Explanation = g.Explanation,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
                };
                return adminDto as T; // Ép kiểu về T để trả về
            }

            if (typeof(T) == typeof(GrammarDTO))
            {
                var userDto = new GrammarDTO
                {
                    Id = g.Id,
                    LessonId = g.LessonId,
                    Structure = g.Structure,
                    Explanation = g.Explanation
                };
                return userDto as T;
            }

            return null;
        }

        public async Task<IEnumerable<Grammar>> GetAllGrammarsAsync()
        {
            return await _context.Grammars
             .AsNoTracking()
             .Select(g => new Grammar
             {
                 Id = g.Id,
                 LessonId = g.LessonId,
                 Structure = g.Structure,
                 Explanation = g.Explanation,
                 CreatedAt = g.CreatedAt,
                 UpdatedAt = g.UpdatedAt
             })
             .ToListAsync();
        }
    }
}