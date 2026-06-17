using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackendAPI.Services;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Text;
using MyGrammar = BackendAPI.Models.Grammar;
var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CẤU HÌNH CÁC DỊCH VỤ HỆ THỐNG (SYSTEM SERVICES)
// =========================================================================
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
// Thêm gói cấu hình giúp JSON tự động bỏ qua các mối quan hệ lặp vòng
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Giúp format JSON đẹp dễ nhìn hơn
    });

// Chấp cánh cho Service đọc được thông tin Request từ Client gửi lên
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IGrammarService<MyGrammar>, GrammarService<MyGrammar>>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ReminderBackgroundService>();

// =========================================================================
// 2. CẤU HÌNH KẾT NỐI DATABASE (ENTITY FRAMEWORK CORE)
// =========================================================================


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
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();

////DTO
builder.Services.AddScoped<IGrammarService<GrammarDTO>, GrammarService<GrammarDTO>>();



// Đăng ký Service dành cho Admin model gốc, để phục vụ cho các tác vụ quản trị (CRUD) mà không cần phải qua lớp DTO trung gian

builder.Services.AddScoped<IGrammarService<MyGrammar>, GrammarService<MyGrammar>>();

builder.Services.AddScoped(typeof(IQuestionAdminService<>), typeof(QuestionAdminService<>));
// =========================================================================
// 4. XÂY DỰNG VÀ CẤU HÌNH PIPELINE XỬ LÝ REQUEST (MIDDLEWARES)
// =========================================================================

// Thêm CORS policy cho Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            var uri = new Uri(origin);
            return uri.Host == "localhost";
        })
               .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Thêm dòng này nếu bạn có dùng Cookie/Token đăng nhập
    });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Cấu hình môi trường Phát triển (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Bật giao diện Swagger để test API phát một
}
app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Kích hoạt nổ máy, đưa Server vào trạng thái lắng nghe mạng
app.Run();