namespace BackendAPI.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(JapaneseDbContext context)
        {
            // Đảm bảo Database đã được tạo
            context.Database.EnsureCreated();

            // KIỂM TRA: Nếu bảng Users đã có dữ liệu thì không seed nữa để tránh lỗi trùng lặp
            if (context.Users.Any())
            {
                return;
            }

            // ==========================================
            // 1. TẠO MÓNG (KHÔNG PHỤ THUỘC AI): USERS & LEVELS
            // ==========================================
            var users = new User[]
            {
                new User { UserName = "hoang_dev", Email = "hoang@gmail.com", PasswordHash = "hashed_pw_1", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, CurrentStreak = 15, LastActiveDate = DateTime.Now, Role = "Admin", TotalExp = 4500, IsEmailConfirmed = true, LastTokenSentAt = DateTime.Now, TokenExpiresAt = DateTime.Now.AddDays(1), VerificationToken = "token_xabc123" },
                new User { UserName = "alpha_pgr", Email = "alpha@gmail.com", PasswordHash = "hashed_pw_2", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, CurrentStreak = 7, LastActiveDate = DateTime.Now, Role = "User", TotalExp = 1200, IsEmailConfirmed = true, LastTokenSentAt = DateTime.Now, TokenExpiresAt = DateTime.Now.AddDays(1), VerificationToken = "token_ydef456" }
            };
            context.Users.AddRange(users);

            var levels = new Level[]
            {
                new Level { Name = "JLPT N5", Description = "Sơ cấp 1", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Level { Name = "JLPT N4", Description = "Sơ cấp 2", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.Levels.AddRange(levels);

            // Lưu để lấy ID thật của Users và Levels
            context.SaveChanges();

            // ==========================================
            // 2. TẠO LESSONS (Phụ thuộc vào Levels)
            // ==========================================
            var lessons = new Lesson[]
            {
                new Lesson { LevelId = levels[0].Id, Title = "Bài 1: Chào hỏi", Order = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Lesson { LevelId = levels[1].Id, Title = "Bài 26: Thể điều kiện", Order = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.Lessons.AddRange(lessons);
            context.SaveChanges(); // Lưu để lấy ID thật của Lessons

            // ==========================================
            // 3. TẠO VOCABULARIES (Phụ thuộc vào Lessons)
            // ==========================================
            var vocabularies = new Vocabulary[]
            {
                new Vocabulary { LessonId = lessons[0].Id, Kanji = "妻", Hiragana = "つま", Meaning = "Vợ", Romaji = "tsuma", AudioUrl = "/audio/sys/tsuma.mp3", ExampleSentence = "彼女は私の妻です。 (Cô ấy là vợ tôi.)", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Vocabulary { LessonId = lessons[0].Id, Kanji = "夫", Hiragana = "おっと", Meaning = "Chồng", Romaji = "otto", AudioUrl = "/audio/sys/otto.mp3", ExampleSentence = "彼は私の夫です。 (Anh ấy là chồng tôi.)", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Vocabulary { LessonId = lessons[1].Id, Kanji = "戦闘", Hiragana = "せんとう", Meaning = "Trận chiến", Romaji = "sentou", AudioUrl = "/audio/sys/sentou.mp3", ExampleSentence = "戦闘が始まった。 (Trận chiến đã bắt đầu.)", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.Vocabularies.AddRange(vocabularies);
            context.SaveChanges(); // Lưu để lấy ID thật của Vocabularies

            // ==========================================
            // 4. TẠO LEARNING PROGRESSES (Phụ thuộc Users & Vocabularies)
            // ==========================================
            var progresses = new LearningProgress[]
            {
                new LearningProgress { UserId = users[0].Id, VocabularyId = vocabularies[0].Id, IsMasstered = true, ReviewCount = 5, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new LearningProgress { UserId = users[0].Id, VocabularyId = vocabularies[1].Id, IsMasstered = true, ReviewCount = 4, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new LearningProgress { UserId = users[1].Id, VocabularyId = vocabularies[2].Id, IsMasstered = false, ReviewCount = 1, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.LearningProgresses.AddRange(progresses);

            // ==========================================
            // 5. TẠO USER FLASHCARD LISTS (Phụ thuộc Users)
            // ==========================================
            var flashcardLists = new UserFlashcardList[]
            {
                new UserFlashcardList { UserId = users[0].Id, Name = "JLPT N4 - Tuần 1", Description = "Danh sách từ vựng luyện thi cấp tốc", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new UserFlashcardList { UserId = users[1].Id, Name = "Từ vựng Giao tiếp", Description = "Từ vựng dùng trong sinh hoạt hàng ngày", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.UserFlashcardLists.AddRange(flashcardLists);
            context.SaveChanges(); // Lưu để lấy ListId

            // ==========================================
            // 6. TẠO USER VOCABULARIES (Phụ thuộc UserFlashcardLists)
            // ==========================================
            var userVocabs = new UserVocabulary[]
            {
                new UserVocabulary { Kanji = "努力", Hiragana = "どりょく", Meaning = "Nỗ lực", Romaji = "doryoku", AudioUrl = "/audio/usr/doryoku.mp3", ExampleSentence = "努力は必ず報われる。 (Nỗ lực chắc chắn sẽ được đền đáp.)", IsMastered = false, ReviewCount = 2, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new UserVocabulary { Kanji = "成功", Hiragana = "せいこう", Meaning = "Thành công", Romaji = "seikou", AudioUrl = "/audio/usr/seikou.mp3", ExampleSentence = "プロジェクトが成功した。 (Dự án đã thành công.)", IsMastered = true, ReviewCount = 6, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new UserVocabulary { Kanji = "構造体", Hiragana = "こうぞうたい", Meaning = "Cấu trúc", Romaji = "kouzoutai", AudioUrl = "/audio/usr/kouzou.mp3", ExampleSentence = "構造体を破壊する。 (Phá hủy cấu trúc.)", IsMastered = false, ReviewCount = 0, LastReviewed = DateTime.Now, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            context.UserVocabularies.AddRange(userVocabs);

            // Lưu nhát chót
            context.SaveChanges();
            // ==========================================
            // 7. TẠO BẢNG CẦU NỐI FOLDER VOCABULARIES (Link 2 bảng lại)
            // ==========================================
            var folderVocabularies = new FolderVocabulary[]
            {
                // Dùng ID thật lấy từ Object thay vì đoán số 1, 2, 3
                new FolderVocabulary { ListId = flashcardLists[0].Id, VocabularyId = userVocabs[0].Id },
                new FolderVocabulary { ListId = flashcardLists[0].Id, VocabularyId = userVocabs[1].Id },
                new FolderVocabulary { ListId = flashcardLists[1].Id, VocabularyId = userVocabs[2].Id }
            };
            context.FolderVocabularies.AddRange(folderVocabularies);

            // Lưu nhát chót cho Bảng cầu nối
            context.SaveChanges();
        }
    }
}
