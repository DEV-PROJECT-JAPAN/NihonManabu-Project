using BackendAPI.DTOs;

namespace BackendAPI.Interfaces
{
    public interface IPaymentWebhookService
    {
        Task<bool> ProcessWebhookAsync(BankWebhookDTO data);
    }
}
