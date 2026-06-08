import { Routes } from '@angular/router';
import { Questions } from './Features/Grammar/questions/questions';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Levels } from './Features/Grammar/levels/levels';
import { LessonComponent } from './Features/Grammar/lessons/lessons';

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
    { path: 'grammar/**', redirectTo: 'grammar' }
];
