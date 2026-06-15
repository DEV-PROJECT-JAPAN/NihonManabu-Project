using FrontendRazorPage.Core.Services;
using FrontendRazorPage.Services;
var builder = WebApplication.CreateBuilder(args);


// 1. KÍCH HOẠT HẠ TẦNG HTTPCLIENT FACTORY (DÒNG CHÍ MẠNG BỊ THIẾU TẠI ĐÂY)
builder.Services.AddHttpClient();

// 3. Đăng ký Client Service của phân hệ Từ vựng
builder.Services.AddScoped<VocabularyClientService>();
builder.Services.AddScoped<GrammarClientService>();
builder.Services.AddScoped<QuestionClientService>();
builder.Services.AddScoped<LevelClientService>();
builder.Services.AddHttpClient<LessonClientService>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
