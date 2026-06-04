using BackendAPI.Interfaces;
using BackendAPI.Models.Data;
using BackendAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CẤU HÌNH CÁC DỊCH VỤ HỆ THỐNG (SYSTEM SERVICES)
// =========================================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Chấp cánh cho Service đọc được thông tin Request từ Client gửi lên
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<IGrammarService, GrammarService>();

// =========================================================================
// 4. XÂY DỰNG VÀ CẤU HÌNH PIPELINE XỬ LÝ REQUEST (MIDDLEWARES)
// =========================================================================

var app = builder.Build();

// Cấu hình môi trường Phát triển (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Bật giao diện Swagger để test API phát một
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Kích hoạt nổ máy, đưa Server vào trạng thái lắng nghe mạng
app.Run();