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
builder.Services.AddScoped<UService>();
builder.Services.AddHttpClient<UService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7104/"); // URL của Backend API
});
builder.Services.AddScoped<DashboardService>();
builder.Services.AddHttpClient<DashboardService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7104/"); // URL của Backend API
});

// AUTH
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Features/Auth/Login";
        options.AccessDeniedPath = "/Features/Auth/AccessDenied";
        options.Cookie.Name = "NihonManabu_AuthCookie";
        options.ClaimsIssuer = "YourAuthIssuer";
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
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