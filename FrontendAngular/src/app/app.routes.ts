import { Routes } from '@angular/router';

// LAYOUTS
import { UserLayoutComponent } from './Shared/UserLayout/user-layout';
import { AdminLayoutComponent } from './Shared/AdminLayout/admin-layout';

// USER COMPONENTS (Học viên)
import { Levels } from './Features/Grammar/levels/levels'; // Component Levels Ngữ pháp gốc của bạn
import { LessonComponent } from './Features/Grammar/lessons/lessons';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Questions } from './Features/Grammar/questions/questions';
import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';

// ADMIN COMPONENTS (Bạn tự import các file admin thực tế vào đây nhé)
// import { GrammarListAdmin } from './Features/Admin/GrammarAdmin/grammar-list-admin';
// import { QuestionListAdmin } from './Features/Admin/QuestionAdmin/question-list-admin';

export const routes: Routes = [
    // 0. Vừa vào web không gõ gì -> Tự động đá sang trang levels của học viên
    { path: '', redirectTo: 'grammar/levels', pathMatch: 'full' },

    // =========================================================================
    // 🧭 PHÂN HỆ 1: TẤT CẢ TRANG CỦA NGƯỜI DÙNG (ĂN THEO USER LAYOUT)
    // =========================================================================
    {
        path: '',
        component: UserLayoutComponent, // ◄ Bọc toàn bộ giao diện học viên cũ của bạn
        children: [
            // ⛩️ ROUTES NGỮ PHÁP (GRAMMAR)
            {
                path: 'grammar',
                children: [
                    { path: 'levels', component: Levels },                                         // /grammar/levels
                    { path: 'level/:levelId', component: LessonComponent },                        // /grammar/level/:levelId
                    { path: 'level/:levelId/lesson/:lessonId', component: GrammarList },           // /grammar/level/.../lesson/...
                    { path: 'level/:levelId/lesson/:lessonId/grammar/:grammarId', component: Questions } // /grammar/level/.../grammar/:grammarId
                ]
            },

            // 📑 ROUTES TỪ VỰNG (VOCABULARY)
            {
                path: 'vocabulary',
                children: [
                    { path: 'levels', component: VocabLevels }, // /vocabulary/levels
                    // { path: 'level/:levelId', component: VocabLessonComponent } // Hàng chờ sau này của bạn
                ]
            }
        ]
    },

    // =========================================================================
    // 🧭 PHÂN HỆ 2: TẤT CẢ TRANG QUẢN TRỊ (ĂN THEO ADMIN LAYOUT CYBERPUNK)
    // =========================================================================
    {
        path: 'admin',
        component: AdminLayoutComponent, // ◄ Bọc giao diện tối tân Admin vừa làm xong
        children: [
            // Vào /admin mà để trống -> Tự động nhảy sang trang quản lý ngữ pháp
            { path: '', redirectTo: 'grammar', pathMatch: 'full' },

            // Các trang tính năng của Admin (Khi nào làm bạn mở ra và điền Component vào nhé)
            // { path: 'grammar', component: GrammarListAdmin }, // /admin/grammar
            // { path: 'question', component: QuestionListAdmin }, // /admin/question

            // Trang Dashboard mẫu (Nếu thích làm)
            // { path: 'dashboard', component: AdminDashboardComponent }
        ]
    },

    // =========================================================================
    // 🚨 CẢNH SÁT GIAO THÔNG (Xử lý URL bậy bạ)
    // =========================================================================
    { path: '**', redirectTo: 'grammar/levels' }
];