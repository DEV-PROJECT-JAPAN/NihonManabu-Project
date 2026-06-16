import { Routes } from '@angular/router';
import { Questions } from './Features/Grammar/questions/questions';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Levels } from './Features/Grammar/levels/levels';
import { LessonComponent } from './Features/Grammar/lessons/lessons';
import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';


export const routes: Routes = [
    // Mặc định chạy vào web nếu trống thì đá qua grammar
    // { path: '', redirectTo: 'grammar', pathMatch: 'full' },

    // ==========================================
    // ⛩️ ROUTES NGỮ PHÁP (GRAMMAR)
    // ==========================================
    {
        path: 'grammar',
        children: [
            { path: 'levels', component: Levels }, // /grammar -> Trang chọn Level (N5, N4...)
            { path: 'level/:levelId', component: LessonComponent }, // /grammar/level/:levelId -> Chọn bài học
            { path: 'level/:levelId/lesson/:lessonId', component: GrammarList }, // /grammar/level/.../lesson/... -> List cấu trúc
            { path: 'level/:levelId/lesson/:lessonId/practice/:grammarId', component: Questions } // Làm bài tập
        ]
    },

    // ==========================================
    // 📑 ROUTES TỪ VỰNG (VOCABULARY)
    // ==========================================
    {
        path: 'vocabulary',
        children: [
            { path: 'levels', component: VocabLevels }, // /vocabulary/levels -> Chọn Level từ vựng
            // Sau này bạn thêm bài học từ vựng thì cứ nhét vào đây:
            // { path: 'level/:levelId', component: VocabLessonComponent }
        ]
    },

    // ==========================================
    // 🚨 CẢNH SÁT GIAO THÔNG (Xử lý URL bậy bạ)
    // ==========================================
    { path: '**', redirectTo: 'grammar' }
];