using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrontendRazorPage.Models.AdminModel;

namespace FrontendRazorPage.Pages.Features.Admin.Lessons
{
    public class EditModel : PageModel
    {
        private readonly LessonClientService _lessonClientService;
        private readonly LevelClientService _levelClientService;

        public EditModel(LessonClientService lessonClientService, LevelClientService levelClientService)
        {
            _lessonClientService = lessonClientService;
            _levelClientService = levelClientService;
        }

        // Đối tượng chứa dữ liệu bài học để bọc vào Form Sửa
        [BindProperty]
        public LessonAdminModel LessonInput { get; set; } = new();

        // Danh sách cấp độ hiển thị cho ô Dropdown Select
        public List<LevelModel> Levels { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? SelectedLevelId { get; set; }
        [TempData]
        public string Message { get; set; } = string.Empty;

        // Hàm chạy khi Admin vừa nhấn nút "Sửa" từ trang Index nhảy qua
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // 1. Lấy danh sách cấp độ để nạp vào thẻ <select>
            Levels = await _levelClientService.GetLevelsAsync();

            // 2. Gọi API Backend lấy chi tiết bài học hiện tại theo ID
            // Lưu ý: Đảm bảo LessonClientService của bạn đã viết hàm GetLessonByIdAsync
            LessonInput = await _lessonClientService.GetLessonByIdForAdminAsync(id);

            if (LessonInput == null)
            {
                Message = "Không tìm thấy bài học cần chỉnh sửa.";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        // Hàm xử lý khi Admin sửa xong và bấm nút "Cập Nhật"
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Levels = await _levelClientService.GetLevelsAsync();
                return Page();
            }

            // Gọi Service để gửi lệnh PUT cập nhật xuống Backend API
            bool isSuccess = await _lessonClientService.UpdateLessonAsync(LessonInput.Id, LessonInput);

            if (isSuccess)
            {
                Message = "Cập nhật bài học thành công!";
                return RedirectToPage("./Index", new { SelectedLevelId = SelectedLevelId });
            }

            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình cập nhật dữ liệu.");
            Levels = await _levelClientService.GetLevelsAsync();
            return Page();
        }
    }
}