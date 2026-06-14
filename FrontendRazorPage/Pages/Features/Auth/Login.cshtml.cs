    using FrontendRazorPage.Core.Services;
    using FrontendRazorPage.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Security.Claims;

namespace FrontendRazorPage.Pages.Features.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new LoginViewModel();

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Index");
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Dữ liệu nhập không hợp lệ.";
                return Page();
            }

            var result = await _authService.LoginAsync(Input);

            if (result == null)
            {
                Message = "Không nhận được phản hồi từ máy chủ.";
                return Page();
            }

            Message = result.Message;

            if (result.Success)
            {
                // Cookie JWT
                Response.Cookies.Append(
                    "JWToken",
                    result.Token ?? string.Empty,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                    });

                
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, Input.Email),
            new Claim(ClaimTypes.Role, result.Role ?? "User")
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                Console.WriteLine("ROLE FROM SERVER: " + result.Role);
                if (string.Equals(result.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToPage("/Features/Admin/AdminDashboard");
                }   

                    return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}