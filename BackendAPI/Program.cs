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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGrammarService, GrammarService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ReminderBackgroundService>();

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


app.Run();
