using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// HTTP + Services
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7104/");
});

builder.Services.AddHttpClient<DashboardService>();

// AUTH
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Features/Auth/Login";
        options.AccessDeniedPath = "/Features/Auth/AccessDenied";
    });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

// PIPELINE (QUAN TRỌNG)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // PHẢI TRƯỚC Authorization
app.UseAuthorization();

app.MapRazorPages();

app.Run();