using BackendAPI.DTOs;
using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackendAPI.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Text;
using MyGrammar = BackendAPI.Models.Grammar;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CẤU HÌNH CÁC DỊCH VỤ HỆ THỐNG (SYSTEM SERVICES)
// =========================================================================
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Thêm cấu hình giúp JSON tự động bỏ qua các mối quan hệ lặp vòng
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Format JSON đẹp dễ nhìn hơn
    });

// =========================================================================
// 2. CẤU HÌNH KẾT NỐI DATABASE (ENTITY FRAMEWORK CORE)
// =========================================================================
builder.Services.AddDbContext<JapaneseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // Chia nhỏ câu lệnh truy vấn (SplitQuery) giúp triệt hạ hoàn toàn Warning 20504
        sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));

// =========================================================================
// 3. ĐĂNG KÝ CÁC DỊCH VỤ NGHIỆP VỤ (BUSINESS SERVICES - DEPENDENCY INJECTION)
// =========================================================================
// Core Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Feature Services
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();

// Grammar Services (DTO & Admin Model)
builder.Services.AddScoped<IGrammarService<GrammarDTO>, GrammarService<GrammarDTO>>();
builder.Services.AddScoped<IGrammarService<MyGrammar>, GrammarService<MyGrammar>>();
builder.Services.AddScoped(typeof(IQuestionAdminService<>), typeof(QuestionAdminService<>));

// Payment Services
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentWebhookService, PaymentWebhookService>();

// Background Services
builder.Services.AddScoped<ReminderBackgroundService>();
builder.Services.AddScoped<VipExpirationBackgroundService>();

// =========================================================================
// 4. BẢO MẬT: CORS, AUTHENTICATION & AUTHORIZATION
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
        .AllowCredentials(); // Hỗ trợ Cookie/Token đăng nhập
    });
});

// Cấu hình JWT Authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

// =========================================================================
// 5. XÂY DỰNG VÀ CẤU HÌNH PIPELINE XỬ LÝ REQUEST (MIDDLEWARES)
// =========================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Bật giao diện Swagger để test API
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();