using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Services;
var builder = WebApplication.CreateBuilder(args);


// 1. KÍCH HOẠT HẠ TẦNG HTTPCLIENT FACTORY (DÒNG CHÍ MẠNG BỊ THIẾU TẠI ĐÂY)
builder.Services.AddHttpClient();

// 2. Cấu hình HTTP Client đặt tên riêng để gọi sang Backend API
//builder.Services.AddHttpClient("BackendAPI", client =>
//{
//    // Đội trưởng nhớ check chuẩn số cổng Port của Backend nhé
//    client.BaseAddress = new Uri("https://localhost:7193/");
//});

// 3. Đăng ký Client Service của phân hệ Từ vựng
builder.Services.AddScoped<VocabularyClientService>();

builder.Services.AddScoped<GrammarClientService>();
builder.Services.AddScoped<QuestionClientService>();
builder.Services.AddScoped<LevelClientService>();


builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Thay cổng Port 7193 bằng Port thực tế của dự án Backend API của bạn
//builder.Services.AddHttpClient("BackendAPI", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7193/");
//});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
