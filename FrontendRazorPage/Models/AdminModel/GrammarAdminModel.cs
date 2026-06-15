namespace FrontendRazorPage.Models.AdminModel
{
    public class GrammarAdminModel
    {

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Dữ liệu cốt lõi phục vụ Form nhập liệu
        public int LessonId { get; set; }
        public string Structure { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;

    }

}
