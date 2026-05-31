using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class GrammarService : IGrammarService
    {
        private readonly JapaneseDbContext _context;

        public GrammarService(JapaneseDbContext context)
        {
            _context = context;
        }

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
                    Content = q.Content ?? string.Empty,
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

       
    }
}