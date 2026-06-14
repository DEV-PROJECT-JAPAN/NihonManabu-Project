namespace BackendAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GenerateVipPaymentQrAsync(int userId);
    }
}
