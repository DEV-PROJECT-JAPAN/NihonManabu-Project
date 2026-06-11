using BackendAPI.DTOs;
using BackendAPI.Models;
using System.Collections.Generic;
namespace BackendAPI.Interfaces
{
    public interface IGrammarService<T> where T : class
    {
        Task<List<GrammarDTO>> GetGrammarByLessonAsync(int lessonId);
        Task<List<QuestionDTO>> GetQuestionsByGrammarAsync(int grammarId, int questionType);

        Task<T?> GetGrammarByIdAsync(int grammarId);

        Task<IEnumerable<Grammar>> GetAllGrammarsAsync();
        Task<Grammar> CreateAsync(Grammar grammar);
        Task<bool> UpdateAsync(int id, Grammar grammar);
        Task<bool> DeleteAsync(int id);
    }
}
