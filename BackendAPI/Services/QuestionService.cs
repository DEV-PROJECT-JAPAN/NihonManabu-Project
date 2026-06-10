using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class QuestionAdminService<T> : IQuestionAdminService<T> where T : class
    {
        private readonly JapaneseDbContext _context;

        public QuestionAdminService(JapaneseDbContext context)
        {
            _context = context;
        }

        // 🔍 [HÀM CHUNG] Lấy danh sách câu hỏi theo Bài học, tự động trả về Model hoặc DTO
        public async Task<IEnumerable<T>> GetQuestionsByLessonAsync(int lessonId)
        {
            if (lessonId <= 0) return new List<T>();

            // Trường hợp 1: Admin gọi và yêu cầu thực thể gốc (Model)
            if (typeof(T) == typeof(Question))
            {
                var list = await _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.Grammar.LessonId == lessonId)
                    .AsNoTracking()
                    .ToListAsync();

                return list as IEnumerable<T>;
            }

            // Trường hợp 2: Học viên gọi và yêu cầu bản thu gọn (DTO)
            if (typeof(T) == typeof(QuestionDTO))
            {
                var listDto = await _context.Questions
                    .Where(q => q.Grammar.LessonId == lessonId)
                    .AsNoTracking()
                    .Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        GrammarId = q.GrammarId ?? 0,
                        Content = q.Content ?? string.Empty,
                        QuestionType = q.QuestionType,
                        Answers = q.Answers.Select(a => new AnswerDTO
                        {
                            Id = a.Id,
                            QuestionId = a.QuestionId,
                            Text = a.Text ?? string.Empty,
                            IsCorrect = a.IsCorrect,
                            DisplayOrder = a.DisplayOrder ?? string.Empty
                        }).ToList()
                    }).ToListAsync();

                return listDto as IEnumerable<T>;
            }

            return new List<T>();
        }

        // 🔍 [HÀM CHUNG] Lấy chi tiết câu hỏi theo ID
        public async Task<T?> GetQuestionByIdAsync(int id)
        {
            var q = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(x => x.Id == id);
            if (q == null) return null;

            if (typeof(T) == typeof(Question))
            {
                return q as T;
            }

            if (typeof(T) == typeof(QuestionDTO))
            {
                var dto = new QuestionDTO
                {
                    Id = q.Id,
                    GrammarId = q.GrammarId ?? 0,
                    Content = q.Content ?? string.Empty,
                    QuestionType = q.QuestionType,
                    Answers = q.Answers.Select(a => new AnswerDTO
                    {
                        Id = a.Id,
                        QuestionId = a.QuestionId,
                        Text = a.Text ?? string.Empty,
                        IsCorrect = a.IsCorrect,
                        DisplayOrder = a.DisplayOrder ?? string.Empty
                    }).ToList()
                };
                return dto as T;
            }

            return null;
        }

        // ➕ [ADMIN] Thêm mới câu hỏi lồng kèm tập hợp đáp án
        public async Task<Question> CreateQuestionAsync(Question question)
        {
            question.CreatedAt = DateTime.Now;
            question.UpdatedAt = DateTime.Now;

            if (question.Answers != null)
            {
                foreach (var answer in question.Answers)
                {
                    answer.CreatedAt = DateTime.Now;
                    answer.UpdatedAt = DateTime.Now;
                }
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        // ✏️ [ADMIN] Cập nhật và đồng bộ mảng đáp án (Thêm mới/Sửa/Xóa phần tử thừa)
        public async Task<bool> UpdateQuestionAsync(int id, Question question)
        {
            var existingQuestion = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
            if (existingQuestion == null) return false;

            existingQuestion.GrammarId = question.GrammarId;
            existingQuestion.Content = question.Content;
            existingQuestion.QuestionType = question.QuestionType;
            existingQuestion.UpdatedAt = DateTime.Now;

            if (question.Answers != null)
            {
                // Lọc bỏ đáp án cũ không còn nằm trong danh sách chỉnh sửa gửi lên
                var incomingAnswerIds = question.Answers.Select(a => a.Id).ToList();
                var answersToDelete = existingQuestion.Answers.Where(a => !incomingAnswerIds.Contains(a.Id)).ToList();

                foreach (var oldAnswer in answersToDelete)
                {
                    _context.Answers.Remove(oldAnswer);
                }

                // Cập nhật phần tử hiện có hoặc tạo mới phần tử chưa có ID
                foreach (var incomingAnswer in question.Answers)
                {
                    var existingAnswer = existingQuestion.Answers.FirstOrDefault(a => a.Id == incomingAnswer.Id && a.Id != 0);

                    if (existingAnswer != null)
                    {
                        existingAnswer.Text = incomingAnswer.Text;
                        existingAnswer.IsCorrect = incomingAnswer.IsCorrect;
                        existingAnswer.DisplayOrder = incomingAnswer.DisplayOrder;
                        existingAnswer.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        incomingAnswer.CreatedAt = DateTime.Now;
                        incomingAnswer.UpdatedAt = DateTime.Now;
                        existingQuestion.Answers.Add(incomingAnswer);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ❌ [ADMIN] Xóa câu hỏi (Database tự cascade delete Answers đi kèm)
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}