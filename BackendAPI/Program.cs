using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using BackendAPI.DTOs;
using BackendAPI.Services;
using DocumentFormat.OpenXml.EMMA;
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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGrammarService<Grammar>, GrammarService<Grammar>>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ReminderBackgroundService>();

builder.Services.AddDbContext<JapaneseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // ⚡ THẦN CHÚ: Ép hệ thống dùng cơ chế Chia nhỏ câu lệnh truy vấn (SplitQuery)
        // Giúp triệt hạ hoàn toàn Warning 20504, tăng tốc bốc câu hỏi và đáp án!
        sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));

// 💡 LƯU Ý CHO NHÓM: Đã xóa bỏ đoạn AddDbContext thứ hai bị thừa ở đây để tránh đè cấu hình!

// =========================================================================
// 3. ĐĂNG KÝ CÁC DỊCH VỤ NGHIỆP VỤ (BUSINESS SERVICES - DEPENDENCY INJECTION)
// =========================================================================

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();


////DTO
builder.Services.AddScoped<IGrammarService<GrammarDTO>, GrammarService<GrammarDTO>>();



// Đăng ký Service dành cho Admin model gốc, để phục vụ cho các tác vụ quản trị (CRUD) mà không cần phải qua lớp DTO trung gian
builder.Services.AddScoped<IGrammarService<Grammar>, GrammarService<Grammar>>();
builder.Services.AddScoped(typeof(IQuestionAdminService<>), typeof(QuestionAdminService<>));
// =========================================================================
// 4. XÂY DỰNG VÀ CẤU HÌNH PIPELINE XỬ LÝ REQUEST (MIDDLEWARES)
// =========================================================================

// Thêm CORS policy cho Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Cấp phép cho Angular
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Thêm dòng này nếu bạn có dùng Cookie/Token đăng nhập
    });
});


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
// Kích hoạt nổ máy, đưa Server vào trạng thái lắng nghe mạng
app.Run();