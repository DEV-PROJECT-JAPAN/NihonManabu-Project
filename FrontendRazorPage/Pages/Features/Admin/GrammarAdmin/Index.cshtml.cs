using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Models;
using FrontendRazorPage.Services;
using FrontendRazorPage.Core.Services; // Thêm namespace chứa VocabularyClientService
using FrontendRazorPage.Models.AdminModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Features.Admin.GrammarAdmin
{
    public class IndexModel : PageModel
    {
        private readonly GrammarClientService _grammarService;
        private readonly VocabularyClientService _vocabularyService; // Khai báo service lọc

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        // Các thuộc tính lưu giữ trạng thái bộ lọc trên UI
        public int? SelectedLevelId { get; set; }
        public int? SelectedLessonId { get; set; }

        // Danh sách đổ vào các ô Select Box lọc
        public List<LevelModel> Levels { get; set; } = new();
        public List<LessonModel> Lessons { get; set; } = new();

        public List<GrammarAdminModel> Grammars { get; set; } = new();

        // Inject bổ sung VocabularyClientService vào Constructor
        public IndexModel(GrammarClientService grammarService, VocabularyClientService vocabularyService)
        {
            _grammarService = grammarService;
            _vocabularyService = vocabularyService;
        }

        public async Task OnGetAsync(int p = 1, int? levelId = null, int? lessonId = null)
        {
            CurrentPage = p;
            SelectedLevelId = levelId;
            SelectedLessonId = lessonId;

            

            // 2. Lấy toàn bộ danh sách ngữ pháp gốc từ API về
            var allGrammars = await _grammarService.GetAllForAdminAsync() ?? new List<GrammarAdminModel>();

            // 3. Thực hiện lọc dữ liệu dựa trên bài học được chọn
            // (Nếu chọn Level nhưng chưa chọn Bài, ta lọc theo danh sách ID bài học thuộc level đó)
            if (SelectedLessonId.HasValue && SelectedLessonId.Value > 0)
            {
                allGrammars = allGrammars.Where(g => g.LessonId == SelectedLessonId.Value).ToList();
            }
            else if (SelectedLevelId.HasValue && SelectedLevelId.Value > 0)
            {
                var lessonIdsInLevel = Lessons.Select(l => l.Id).ToList();
                allGrammars = allGrammars.Where(g => lessonIdsInLevel.Contains(g.LessonId)).ToList();
            }

            // 4. Tính toán tổng số trang dựa trên danh sách dữ liệu SAU KHI LỌC
            int totalRecords = allGrammars.Count;
            TotalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            // 5. Cắt phân đoạn mảng hiển thị trang hiện tại
            Grammars = allGrammars
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _grammarService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}