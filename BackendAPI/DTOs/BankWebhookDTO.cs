namespace BackendAPI.DTOs
{
    public class BankWebhookDTO
    {
        public string Gateway { get; set; } // Tên ngân hàng (VD: MBBank)
        public decimal Amount { get; set; } // Số tiền nhận được
        public string Content { get; set; } // Nội dung chuyển khoản (Chứa chữ ALPHAVIP)
        public string ReferenceCode { get; set; } // Mã giao dịch của ngân hàng
    }
}