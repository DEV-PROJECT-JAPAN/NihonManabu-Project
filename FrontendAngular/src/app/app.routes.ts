import { Routes } from '@angular/router';
import { Questions } from './Features/Grammar/questions/questions';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Levels } from './Features/Grammar/levels/levels';
import { LessonComponent } from './Features/Grammar/lessons/lessons';
import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';
import { LessonsComponent as VocabLessions } from './Features/Vocabulary/lessons/lessons';
// Thêm dòng này lên nhóm import ở đầu file
import { VocabularyComponent } from './Features/Vocabulary/vocabulary/vocabulary';

export const routes: Routes = [
    // 1. Mặc định vào thẳng trang chọn Level
    { path: 'grammar', component: Levels },

    // 2. Bấm vào Level -> Ra danh sách Lesson của Level đó
    { path: 'grammar/level/:levelId', component: LessonComponent },

    // 3. Bấm vào Lesson -> Ra danh sách cấu trúc Ngữ pháp
    { path: 'grammar/level/:levelId/lesson/:lessonId', component: GrammarList },

    // 4. Bấm vào Ngữ pháp -> Ra trang khay làm bài tập thực hành
    { path: 'grammar/level/:levelId/lesson/:lessonId/practice/:grammarId', component: Questions },

    // Điều hướng mặc định nếu gõ sai url
    { path: 'grammar/**', redirectTo: 'grammar' },
    // ==========================================
    //  ROUTES CHO TỪ VỰNG (VOCABULARY)
    // ==========================================
    // 1. Mặc định vào trang chọn Level Từ vựng
    { path: 'vocabulary/levels', component: VocabLevels },

    // 2. Bấm vào Level -> Ra danh sách Lesson Từ vựng
    { path: 'vocabulary/lessons/:id', component: VocabLessions },
    // 3. Bấm vào Lesson -> Ra danh sách Từ vựng chi tiết để học
    { path: 'vocabulary/list/:levelId/:lessonId', component: VocabularyComponent },
];
