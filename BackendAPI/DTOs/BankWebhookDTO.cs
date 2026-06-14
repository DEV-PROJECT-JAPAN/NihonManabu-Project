namespace BackendAPI.DTOs
{
    public class BankWebhookDTO
    {
        public long Id { get; set; }
        public string Gateway { get; set; } // Tên ngân hàng (MBBank, Vietcombank...)
        public decimal Amount { get; set; } // Số tiền nhận được (Ví dụ: 50000)
        public string Content { get; set; } // Nội dung chuyển khoản (Ví dụ: "ALPHAVIP 1024")
        public DateTime TransferDate { get; set; }
        public string ReferenceCode { get; set; } // Mã đặc định của ngân hàng
    }
}
