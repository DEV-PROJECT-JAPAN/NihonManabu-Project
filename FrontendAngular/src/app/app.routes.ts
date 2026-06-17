import { Routes } from '@angular/router';

<<<<<<< HEAD
// // LAYOUTS
// import { UserLayoutComponent } from './Shared/UserLayout/user-layout';
// import { AdminLayoutComponent } from './Shared/AdminLayout/admin-layout';

// // USER COMPONENTS (Học viên)
// import { levelsGrammar } from './Features/Grammar/levelsGrammar/levelsGrammar'; // Component Levels Ngữ pháp gốc của bạn
// import { lessonsGrammar } from './Features/Grammar/lessonsGrammar/lessonsGrammar';
// import { GrammarList } from './Features/Grammar/grammar/grammar';
// import { Questions } from './Features/Grammar/questions/questions';

// import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';
// import { LessonsComponent, LessonsComponent as VocabLessions } from './Features/Vocabulary/lessons/lessons';


// import { VocabularyComponent } from './Features/Vocabulary/vocabulary/vocabulary';
// import { GrammarListAdmin } from './Features/Admin/GrammarAdmin/GrammarListAdmin/GrammarListAdmin';
// import { GrammarCreate } from './Features/Admin/GrammarAdmin/GrammarCreate/GrammarCreate';
// import { GrammarEdit } from './Features/Admin/GrammarAdmin/GrammarEdit/GrammarEdit';
// import { QuestionList } from './Features/Admin/QuestionAdmin/QuestionList/QuestionList';
// import { QuestionCreate } from './Features/Admin/QuestionAdmin/QuestionCreate/QuestionCreate';
// import { QuestionEdit } from './Features/Admin/QuestionAdmin/QuestionEdit/QuestionEdit';
// import { LevelIndexComponent } from './Features/Admin/LevelAdmin/index';

// import { LevelEditComponent } from './Features/Admin/LevelAdmin/edit/edit';
// import { LevelCreateComponent } from './Features/Admin/LevelAdmin/create/create';
// import { LessonIndexComponent } from './Features/Admin/LessonAdmin/index';
// import { LessonCreateComponent } from './Features/Admin/LessonAdmin/create/create';
// import { LessonEditComponent } from './Features/Admin/LessonAdmin/edit/edit';
// import { VocabularyEditComponent } from './Features/Admin/VocabularyAdmin/edit/edit';
// import { VocabularyIndexComponent } from './Features/Admin/VocabularyAdmin/index';
// import { VocabularyCreateComponent } from './Features/Admin/VocabularyAdmin/create/create';

// export const routes: Routes = [
//     // 0. Vừa vào web không gõ gì -> Tự động đá sang trang levels của học viên
//     { path: '', redirectTo: 'grammar/levels', pathMatch: 'full' },

//     // =========================================================================
//     // 🧭 PHÂN HỆ 1: TẤT CẢ TRANG CỦA NGƯỜI DÙNG (ĂN THEO USER LAYOUT)
//     // =========================================================================
//     {
//         path: '',
//         component: UserLayoutComponent, // ◄ Bọc toàn bộ giao diện học viên cũ của bạn
//         children: [
//             {
//                 path: 'vocabulary',
//                 children: [
//                     { path: 'levels', component: VocabLevels },                           // /vocabulary/levels
//                     { path: 'lessons/:id', component: LessonsComponent },                 // /vocabulary/lessons/:id
//                     { path: 'list/:levelId/:lessonId', component: VocabularyComponent }   // /vocabulary/list/:levelId/:lessonId
//                 ]
//             },

//             // ⛩️ ROUTES NGỮ PHÁP (GRAMMAR) ĐỂ XUỐNG DƯỚI
//             {
//                 path: 'grammar',
//                 children: [
//                     { path: 'levels', component: levelsGrammar },
//                     { path: 'level/:levelId', component: lessonsGrammar },
//                     { path: 'level/:levelId/lesson/:lessonId', component: GrammarList },
//                     { path: 'level/:levelId/lesson/:lessonId/grammar/:grammarId', component: Questions }
//                 ]
//             }
//         ]
//     },

//     // =========================================================================
//     // 🧭 PHÂN HỆ 2: TẤT CẢ TRANG QUẢN TRỊ (ĂN THEO ADMIN LAYOUT CYBERPUNK)
//     // =========================================================================
//     {
//         path: 'admin',
//         component: AdminLayoutComponent, // ◄ Lớp ngoài cùng: Bọc giao diện Admin tổng
//         children: [
//             // Vào /admin mặc định đá sang /admin/grammar
//             { path: '', redirectTo: 'grammar', pathMatch: 'full' },

//             // =========================================================
//             // TẦNG 1: PHÂN HỆ GRAMMAR ADMIN (/admin/grammar)
//             // =========================================================
//             {
//                 path: 'grammar',
//                 children: [
//                     // TẦNG 2: Các trang con bên trong Grammar Admin
//                     { path: 'index', component: GrammarListAdmin },                 // URL: /admin/grammar (Trang danh sách)
//                     { path: 'create', component: GrammarCreate },
//                     { path: 'edit/:id', component: GrammarEdit },
//                 ]
//             },
//             // =========================================================
//             // TẦNG 1: PHÂN HỆ LEVEL ADMIN (/admin/level)
//             // =========================================================
//             {
//                 path: 'level',
//                 children: [
//                     // TẦNG 2: Các trang con bên trong Level Admin

//                     // URL: /admin/level (Trang danh sách mặc định)
//                     { path: '', component: LevelIndexComponent },

//                     // URL: /admin/level/create (Trang thêm mới)
//                     { path: 'create', component: LevelCreateComponent },

//                     // URL: /admin/level/edit/5 (Trang sửa)
//                     { path: 'edit/:id', component: LevelEditComponent }
//                 ]
//             },

//             {
//                 path: 'question',
//                 children: [
//                     // TẦNG 2: Các trang con bên trong Lesson Admin
//                     { path: 'index', component: QuestionList },
//                     { path: 'create', component: QuestionCreate },
//                     { path: 'edit/:id', component: QuestionEdit }



//                 ]
//             },
//             {
//                 path: 'lesson',
//                 children: [

//                     { path: '', component: LessonIndexComponent },

//                     // URL: /admin/lesson/create (Trang thêm mới)
//                     { path: 'create', component: LessonCreateComponent },

//                     // URL: /admin/lesson/edit/5 (Trang sửa)
//                     { path: 'edit/:id', component: LessonEditComponent }
//                 ]
//             },


//             // =========================================================
//             // TẦNG 1: PHÂN HỆ VOCABULARY ADMIN (/admin/vocabulary)
//             // =========================================================
//             {
//                 path: 'vocabulary',
//                 children: [
//                     // URL: /admin/vocabulary (Trang quản lý danh sách từ vựng)
//                     { path: '', component: VocabularyIndexComponent },

//                     // URL: /admin/vocabulary/create (Trang thêm từ vựng / import file)
//                     { path: 'create', component: VocabularyCreateComponent },

//                     // URL: /admin/vocabulary/edit/5 (Trang sửa từ vựng)
//                     { path: 'edit/:id', component: VocabularyEditComponent }
//                 ]
//             }
//         ]
//     },


//     // =========================================================================
//     // 🚨 CẢNH SÁT GIAO THÔNG (Xử lý URL bậy bạ)
//     // =========================================================================
//     { path: '**', redirectTo: 'grammar/levels' }
// ];
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

import { PracticeMain } from './Features/Practice/practice-main/practice-main';

// Thêm dòng này lên nhóm import ở đầu file
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
            },

            // 🚀 ROUTE MỚI CHO PRACTICE (GHI NHỚ) - Tạm thời để ở đây, sau này có thể tách ra module riêng nếu cần
            {
                path: 'practice', 
                component: PracticeMain

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
                path: 'grammarAdmin',
                children: [
                    // TẦNG 2: Các trang con bên trong Grammar Admin
                    { path: 'grammarIndex', component: GrammarListAdmin },                 // URL: /admin/grammar (Trang danh sách)
                    // URL: /admin/grammar/edit/5 (Trang sửa)
                ]
            },

            // =========================================================
            // TẦNG 1: PHÂN HỆ LESSON ADMIN (/admin/lesson)
            // =========================================================
            {
                path: 'lesson',
                children: [
                    // TẦNG 2: Các trang con bên trong Lesson Admin
                    // URL: /admin/lesson/edit/5 (Trang sửa)
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
=======
// ==========================
// LAYOUTS
// ==========================
import { UserLayoutComponent } from './Shared/UserLayout/user-layout';
import { AdminLayoutComponent } from './Shared/AdminLayout/admin-layout';

// ==========================
// USER - GRAMMAR
// ==========================
import { levelsGrammar } from './Features/Grammar/levelsGrammar/levelsGrammar';
import { lessonsGrammar } from './Features/Grammar/lessonsGrammar/lessonsGrammar';
import { GrammarList } from './Features/Grammar/grammar/grammar';
import { Questions } from './Features/Grammar/questions/questions';

// ==========================
// USER - VOCABULARY
// ==========================
import { LevelsComponent as VocabLevels } from './Features/Vocabulary/levels/levels';
import { LessonsComponent } from './Features/Vocabulary/lessons/lessons';
import { VocabularyComponent } from './Features/Vocabulary/vocabulary/vocabulary';

// ==========================
// USER - AUTH
// ==========================
import { Login} from './Feature/Auth/login/login';
import { RegisterComponent} from './Feature/Auth/register/register';
// ==========================
// ADMIN - GRAMMAR
// ==========================
import { GrammarListAdmin } from './Features/Admin/GrammarAdmin/GrammarListAdmin/GrammarListAdmin';
import { GrammarCreate } from './Features/Admin/GrammarAdmin/GrammarCreate/GrammarCreate';
import { GrammarEdit } from './Features/Admin/GrammarAdmin/GrammarEdit/GrammarEdit';

// ==========================
// ADMIN - QUESTION
// ==========================
import { QuestionList } from './Features/Admin/QuestionAdmin/QuestionList/QuestionList';
import { QuestionCreate } from './Features/Admin/QuestionAdmin/QuestionCreate/QuestionCreate';
import { QuestionEdit } from './Features/Admin/QuestionAdmin/QuestionEdit/QuestionEdit';

// ==========================
// ADMIN - LEVEL
// ==========================
import { LevelIndexComponent } from './Features/Admin/LevelAdmin/index';
import { LevelCreateComponent } from './Features/Admin/LevelAdmin/create/create';
import { LevelEditComponent } from './Features/Admin/LevelAdmin/edit/edit';

// ==========================
// ADMIN - LESSON
// ==========================
import { LessonIndexComponent } from './Features/Admin/LessonAdmin/index';
import { LessonCreateComponent } from './Features/Admin/LessonAdmin/create/create';
import { LessonEditComponent } from './Features/Admin/LessonAdmin/edit/edit';

// ==========================
// ADMIN - VOCABULARY
// ==========================
import { VocabularyIndexComponent } from './Features/Admin/VocabularyAdmin/index';
import { VocabularyCreateComponent } from './Features/Admin/VocabularyAdmin/create/create';
import { VocabularyEditComponent } from './Features/Admin/VocabularyAdmin/edit/edit';

export const routes: Routes = [

    // =========================================
    // ROOT
    // =========================================
    {
        path: '',
        redirectTo: 'grammar/levels',
        pathMatch: 'full'
    },

    // =========================================
    // USER AREA
    // =========================================
    {
        path: '',
        component: UserLayoutComponent,
        children: [

            // ---------- Vocabulary ----------
            {
                path: 'vocabulary',
                children: [
                    {
                        path: '',
                        redirectTo: 'levels',
                        pathMatch: 'full'
                    },
                    {
                        path: 'levels',
                        component: VocabLevels
                    },
                    {
                        path: 'lessons/:id',
                        component: LessonsComponent
                    },
                    {
                        path: 'list/:levelId/:lessonId',
                        component: VocabularyComponent
                    }
                ]
            },
            // ---------- Auth ----------
            {
               path: 'auth',
                children: [
                    {
                        path: 'login',
                        component: Login
                    },
                    {
                        path: 'register',
                        component: RegisterComponent
                    }
                ]
            },
            // ---------- Grammar ----------
            {
                path: 'grammar',
                children: [
                    {
                        path: '',
                        redirectTo: 'levels',
                        pathMatch: 'full'
                    },
                    {
                        path: 'levels',
                        component: levelsGrammar
                    },
                    {
                        path: 'level/:levelId',
                        component: lessonsGrammar
                    },
                    {
                        path: 'level/:levelId/lesson/:lessonId',
                        component: GrammarList
                    },
                    {
                        path: 'level/:levelId/lesson/:lessonId/grammar/:grammarId',
                        component: Questions
                    }
                ]
            }
        ]
    },

    // =========================================
    // ADMIN AREA
    // =========================================
    {
        path: 'admin',
        component: AdminLayoutComponent,
        children: [

            {
                path: '',
                redirectTo: 'grammar',
                pathMatch: 'full'
            },

            // ---------- Grammar ----------
            {
                path: 'grammar',
                children: [
                    {
                        path: '',
                        component: GrammarListAdmin
                    },
                    {
                        path: 'create',
                        component: GrammarCreate
                    },
                    {
                        path: 'edit/:id',
                        component: GrammarEdit
                    }
                ]
            },

            // ---------- Question ----------
            {
                path: 'question',
                children: [
                    {
                        path: '',
                        component: QuestionList
                    },
                    {
                        path: 'create',
                        component: QuestionCreate
                    },
                    {
                        path: 'edit/:id',
                        component: QuestionEdit
                    }
                ]
            },

            // ---------- Level ----------
            {
                path: 'level',
                children: [
                    {
                        path: '',
                        component: LevelIndexComponent
                    },
                    {
                        path: 'create',
                        component: LevelCreateComponent
                    },
                    {
                        path: 'edit/:id',
                        component: LevelEditComponent
                    }
                ]
            },

            // ---------- Lesson ----------
            {
                path: 'lesson',
                children: [
                    {
                        path: '',
                        component: LessonIndexComponent
                    },
                    {
                        path: 'create',
                        component: LessonCreateComponent
                    },
                    {
                        path: 'edit/:id',
                        component: LessonEditComponent
                    }
                ]
            },

            // ---------- Vocabulary ----------
            {
                path: 'vocabulary',
                children: [
                    {
                        path: '',
                        component: VocabularyIndexComponent
                    },
                    {
                        path: 'create',
                        component: VocabularyCreateComponent
                    },
                    {
                        path: 'edit/:id',
                        component: VocabularyEditComponent
                    }
                ]
            }
        ]
    },

    // =========================================
    // NOT FOUND
    // =========================================
    {
        path: '**',
        redirectTo: 'grammar/levels'
    }
];
>>>>>>> feature/login
