using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Models.Data
{
    public class JapaneseDbContext : DbContext
    {
        public JapaneseDbContext(DbContextOptions<JapaneseDbContext> options) : base(options)
        {
        }

        public DbSet<Level> Levels { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<Grammar> Grammars { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserFlashcardList> UserFlashcardLists { get; set; }
        public DbSet<UserVocabulary> UserVocabularies { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<FolderVocabulary> FolderVocabularies { get; set; }
        public DbSet<UserSubscription> Subscriptions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Chuyển các cột thành chỉ mục cấu trúc cây phục vụ tra cứu từ điển nhanh
            modelBuilder.Entity<Vocabulary>().HasIndex(v => v.Kanji);
            modelBuilder.Entity<Vocabulary>().HasIndex(v => v.Hiragana);

            // 2. ÉP KIỂU TIỀN TỆ CHO AMOUNT (Triệt tiêu Warning 30000 hoàn toàn)
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Amount)
                      .HasColumnType("decimal(18,2)");
            });

            // 3. ĐỊNH NGHĨA CHUẨN MỐI QUAN HỆ LEARNINGPROGRESS (Diệt tận gốc UserId1)
            modelBuilder.Entity<LearningProgress>(entity =>
            {
                entity.HasKey(lp => lp.Id);

                // Khóa ngoại liên kết chuẩn chỉ từ LearningProgresses sang Users
                entity.HasOne(lp => lp.User)
                      .WithMany(u => u.LearningProgresses) // Đưa đích danh biến Collection trong User vào đây!
                      .HasForeignKey(lp => lp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Khóa ngoại liên kết chuẩn chỉ từ LearningProgresses sang Vocabularies
                entity.HasOne(lp => lp.Vocabulary)
                      .WithMany() // Để trống nếu trong class Vocabulary không có Collection Progresses
                      .HasForeignKey(lp => lp.VocabularyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 4. Các mối quan hệ xóa dây chuyền cũ giữ nguyên
            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Vocabularies)
                .WithOne(v => v.Lesson)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}