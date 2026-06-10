using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models.AdminModel;
using FrontendRazorPage.Services;

namespace FrontendRazorPage.Pages.Features.Admin.QuestionAdmin
{
    public class IndexModel : PageModel
    {
        private readonly QuestionClientService _questionService;
        private readonly GrammarClientService _grammarService; // 🌟 Gọi Service Ngữ pháp có sẵn của bạn

        public IndexModel(QuestionClientService questionService, GrammarClientService grammarService)
        {
            _questionService = questionService;
            _grammarService = grammarService;
        }

        [BindProperty]
        public QuestionAdminModel QuestionForm { get; set; } = new QuestionAdminModel();

        // Danh sách lưu trữ bộ mẫu ngữ pháp để hiển thị lên Form động
        public List<GrammarAdminModel> Grammars { get; set; } = new List<GrammarAdminModel>();

        public async Task OnGetAsync()
        {
            // Tải danh sách ngữ pháp khi vừa mở trang quản trị
            Grammars = await _grammarService.GetAllForAdminAsync() ?? new List<GrammarAdminModel>();
        }
    }
}