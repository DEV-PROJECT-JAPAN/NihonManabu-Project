using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendRazorPage.Pages.Vocabulary
{
    public class LessonsModel : PageModel
    {
        private readonly LessonClientService _lessonService;
        public LessonsModel(LessonClientService lessonService) => _lessonService = lessonService;

        public int LevelId { get; set; }

        public List<LessonModel> Lessons { get; set; } = new();

        public async Task OnGetAsync(int levelId)
        {
            LevelId = levelId;
            Lessons = await _lessonService.GetLessonsByLevelAsync(levelId);
        }
    }
}