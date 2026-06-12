using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using BackendAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// 1. Đăng ký HttpContextAccessor để Service đọc được thông tin Request mạng
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();
builder.Services.AddScoped<IUserService, MockUserService>();
builder.Services.AddScoped<IGrammarService, GrammarService>();

builder.Services.AddDbContext<JapaneseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ==========================================
// KÍCH HOẠT SEED DATA KHI APP CHẠY LÊN
// ==========================================
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        // Lấy DbContext từ DI Container
//        var context = services.GetRequiredService<BackendAPI.Models.Data.JapaneseDbContext>();

//        // Gọi hàm Initialize của anh em mình vừa tạo
//        BackendAPI.Models.Data.DbInitializer.Initialize(context);
//    }
//    catch (Exception ex)
//    {
//        // Ghi log ra console nếu có lỗi trong quá trình nhét data
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Có lỗi xảy ra trong quá trình Seed Database.");
//    }
//}

app.Run();
