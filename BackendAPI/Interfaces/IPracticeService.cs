using BackendAPI.DTOs;

namespace BackendAPI.Interfaces
{
    public interface IPracticeService
    {
        Task<List<PracticeDTO>> GetAllPracticesAsync(int IdLesson);
    }
}
