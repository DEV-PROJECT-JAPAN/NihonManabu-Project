import { Routes } from '@angular/router';

import { UserLayoutComponent } from './Shared/UserLayout/user-layout';
import { AdminLayoutComponent } from './Shared/AdminLayout/admin-layout';

// USER COMPONENTS (Học viên)
import { levelsGrammar } from './Features/Grammar/levelsGrammar/levelsGrammar'; 
import { lessonsGrammar } from './Features/Grammar/lessonsGrammar/lessonsGrammar';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Questions } from './Features/Grammar/questions/questions';

//Auth
import { Login } from './Feature/Auth/login/login';
import { RegisterComponent } from './Feature/Auth/register/register';
import { ProfileComponent } from './Feature/Auth/profile/profile';

import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';
import { LessonsComponent, LessonsComponent as VocabLessions } from './Features/Vocabulary/lessons/lessons';

import { PracticeMain } from './Features/Practice/practice-main/practice-main';

import { VocabularyComponent } from './Features/Vocabulary/vocabulary/vocabulary';
import { GrammarListAdmin } from './Features/Admin/GrammarAdmin/GrammarListAdmin/GrammarListAdmin';

export const routes: Routes = [
    // 0. Vừa vào web không gõ gì -> Tự động đá sang trang levels của học viên
    { path: '', redirectTo: 'grammar/levels', pathMatch: 'full' },

    // =========================================================================
    // 🧭 PHÂN HỆ 1: TẤT CẢ TRANG CỦA NGƯỜI DÙNG (ĂN THEO USER LAYOUT)
    // =========================================================================
    {
        path: '',
        component: UserLayoutComponent, // ◄ Bọc toàn bộ giao diện học viên
        children: [
            // ✨ Đã chuyển ra đây: Cùng cấp với vocabulary, grammar, practice
            { path: 'auth/profile', component: ProfileComponent }, // URL chuẩn: /auth/profile

            {
                path: 'vocabulary',
                children: [
                    // 🎯 Đã dọn dẹp profile lỗi ở vị trí này
                    { path: 'levels', component: VocabLevels },                          // /vocabulary/levels
                    { path: 'lessons/:id', component: LessonsComponent },                 // /vocabulary/lessons/:id
                    { path: 'list/:levelId/:lessonId', component: VocabularyComponent }   // /vocabulary/list/:levelId/:lessonId
                ]
            },

            // ⛩️ ROUTES NGỮ PHÁP (GRAMMAR)
            {
                path: 'grammar',
                children: [
                    { path: 'levels', component: levelsGrammar },
                    { path: 'level/:levelId', component: lessonsGrammar },
                    { path: 'level/:levelId/lesson/:lessonId', component: GrammarList },
                    { path: 'level/:levelId/lesson/:lessonId/grammar/:grammarId', component: Questions }
                ]
            },

            // 🚀 ROUTE MỚI CHO PRACTICE (GHI NHỚ)
            {
                path: 'practice', 
                component: PracticeMain
            }
        ]
    },
    
    // =========================================================================
    // 🔐 AUTH ĐỘC LẬP (KHÔNG DÙNG SIDEBAR/NAVBAR CỦA USER)
    // =========================================================================
    {
        path: 'auth',
        children: [
            { path: 'login', component: Login },
            { path: 'register', component: RegisterComponent },
        ]
    },

    // =========================================================================
    // 🧭 PHÂN HỆ 2: TẤT CẢ TRANG QUẢN TRỊ (ĂN THEO ADMIN LAYOUT CYBERPUNK)
    // =========================================================================
    {
        path: 'admin',
        component: AdminLayoutComponent, 
        children: [
            { path: '', redirectTo: 'grammar', pathMatch: 'full' },
            {
                path: 'grammarAdmin',
                children: [
                    { path: 'grammarIndex', component: GrammarListAdmin },
                ]
            },
            {
                path: 'lesson',
                children: []
            },
            {
                path: 'vocabulary',
                children: []
            }
        ]
    },

    // =========================================================================
    // 🚨 CẢNH SÁT GIAO THÔNG (Xử lý URL sai cấu trúc)
    // =========================================================================
    { path: '**', redirectTo: 'grammar/levels' }   
];