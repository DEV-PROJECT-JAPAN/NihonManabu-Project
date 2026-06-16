import { Routes } from '@angular/router';

// LAYOUTS
import { UserLayoutComponent } from './Shared/UserLayout/user-layout';
import { AdminLayoutComponent } from './Shared/AdminLayout/admin-layout';

// USER COMPONENTS (Học viên)
import { levelsGrammar } from './Features/Grammar/levelsGrammar/levelsGrammar'; // Component Levels Ngữ pháp gốc của bạn
import { lessonsGrammar } from './Features/Grammar/lessonsGrammar/lessonsGrammar';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Questions } from './Features/Grammar/questions/questions';

import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';
import { LessonsComponent, LessonsComponent as VocabLessions } from './Features/Vocabulary/lessons/lessons';

// Thêm dòng này lên nhóm import ở đầu file
import { VocabularyComponent } from './Features/Vocabulary/vocabulary/vocabulary';
import { GrammarListAdmin } from './Features/Admin/GrammarAdmin/GrammarListAdmin/GrammarListAdmin';
import { GrammarCreate } from './Features/Admin/GrammarAdmin/GrammarCreate/GrammarCreate';
import { GrammarEdit } from './Features/Admin/GrammarAdmin/GrammarEdit/GrammarEdit';
import { QuestionList } from './Features/Admin/QuestionAdmin/QuestionList/QuestionList';
import { QuestionCreate } from './Features/Admin/QuestionAdmin/QuestionCreate/QuestionCreate';
import { QuestionEdit } from './Features/Admin/QuestionAdmin/QuestionEdit/QuestionEdit';

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
            {
                path: 'vocabulary',
                children: [
                    { path: 'levels', component: VocabLevels },                           // /vocabulary/levels
                    { path: 'lessons/:id', component: LessonsComponent },                 // /vocabulary/lessons/:id
                    { path: 'list/:levelId/:lessonId', component: VocabularyComponent }   // /vocabulary/list/:levelId/:lessonId
                ]
            },

            // ⛩️ ROUTES NGỮ PHÁP (GRAMMAR) ĐỂ XUỐNG DƯỚI
            {
                path: 'grammar',
                children: [
                    { path: 'levels', component: levelsGrammar },
                    { path: 'level/:levelId', component: lessonsGrammar },
                    { path: 'level/:levelId/lesson/:lessonId', component: GrammarList },
                    { path: 'level/:levelId/lesson/:lessonId/grammar/:grammarId', component: Questions }
                ]
            }
        ]
    },

    // =========================================================================
    // 🧭 PHÂN HỆ 2: TẤT CẢ TRANG QUẢN TRỊ (ĂN THEO ADMIN LAYOUT CYBERPUNK)
    // =========================================================================
    {
        path: 'admin',
        component: AdminLayoutComponent, // ◄ Lớp ngoài cùng: Bọc giao diện Admin tổng
        children: [
            // Vào /admin mặc định đá sang /admin/grammar
            { path: '', redirectTo: 'grammar', pathMatch: 'full' },

            // =========================================================
            // TẦNG 1: PHÂN HỆ GRAMMAR ADMIN (/admin/grammar)
            // =========================================================
            {
                path: 'grammar',
                children: [
                    // TẦNG 2: Các trang con bên trong Grammar Admin
                    { path: 'index', component: GrammarListAdmin },                 // URL: /admin/grammar (Trang danh sách)
                    { path: 'create', component: GrammarCreate },
                    { path: 'edit/:id', component: GrammarEdit },
                ]
            },

            // =========================================================
            // TẦNG 1: PHÂN HỆ LESSON ADMIN (/admin/lesson)
            // =========================================================
            {
                path: 'question',
                children: [
                    // TẦNG 2: Các trang con bên trong Lesson Admin
                    { path: 'index', component: QuestionList },
                    { path: 'create', component: QuestionCreate },
                    { path: 'edit/:id', component: QuestionEdit }
                ]
            },

            // =========================================================
            // TẦNG 1: PHÂN HỆ VOCABULARY ADMIN (/admin/vocabulary)
            // =========================================================
            {
                path: 'vocabulary',
                children: [
                    // URL: /admin/vocabulary/edit/5
                ]
            }
        ]
    },


    // =========================================================================
    // 🚨 CẢNH SÁT GIAO THÔNG (Xử lý URL bậy bạ)
    // =========================================================================
    { path: '**', redirectTo: 'grammar/levels' }
];
