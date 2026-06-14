namespace BackendAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GenerateVipPaymentQrAsync(int userId);
        Task<bool> CheckIsVipAsync(int userId);
    }
}
