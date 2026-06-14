using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendRazorPage.Pages.Features.Auth
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Xóa cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. Xóa JWT Token trong Cookie
            Response.Cookies.Delete("JWToken");

            return RedirectToPage("/Index");
        }
    }
}