using BackendAPI.DTOs;

namespace BackendAPI.Interfaces
{
    public interface IGrammarService
    {
        Task<List<GrammarDTO>> GetGrammarByLessonAsync(int lessonId);
        Task<List<QuestionDTO>> GetQuestionsByGrammarAsync(int grammarId, int questionType);
    }
}
