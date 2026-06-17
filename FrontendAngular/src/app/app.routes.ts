import { Routes } from '@angular/router';

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
// USER - AUTH & PROFILE
// ==========================
import { Login } from './Feature/Auth/login/login';
import { RegisterComponent } from './Feature/Auth/register/register';
import { ProfileComponent } from './Feature/Auth/profile/profile';

// ==========================
// USER - PRACTICE & PAYMENT
// ==========================
import { PracticeMain } from './Features/Practice/practice-main/practice-main';
import { PaymentComponent } from './Features/payment/payment';

// ==========================
// ADMIN - DASHBOARD & GUARDS
// ==========================
import { AdminDashboardComponent } from './Features/Admin/User-admin/admin-dashboard/admin-dashboard'; // ◄ Thêm Import Dashboard
import { adminGuard } from './Core/Guard/adminGuard'; // ◄ Thêm Import Guard (Chỉnh lại path nếu file guard nằm ở thư mục khác)

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
    // {
    //     path: '',
    //     redirectTo: 'grammar/levels',
    //     pathMatch: 'full'
    // },

    // =========================================
    // USER AREA
    // =========================================
    {
        path: '',
        component: UserLayoutComponent,
        children: [
            // ✨ Profile cá nhân của người dùng nằm trong User Layout
            {
                path: 'auth/profile',
                component: ProfileComponent
            },

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
            },

            // ---------- Practice ----------
            {
                path: 'practice', 
                component: PracticeMain
            },

            // ---------- Upgrade Account (Payment) ----------
            {
                path: 'upgradeACC',
                component: PaymentComponent
            }
        ]
    },

    // =========================================
    // AUTH AREA (ĐỘC LẬP - KHÔNG ĂN THEO LAYOUT CHUNG)
    // =========================================
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

    // =========================================
    // ADMIN AREA
    // =========================================
    {
        path: 'admin',
        component: AdminLayoutComponent,
        canActivate: [adminGuard], // ◄ Gắn Guard ở đây để bảo vệ TOÀN BỘ các trang admin con
        children: [
            // Khi vào đường dẫn /admin, hệ thống sẽ tự chuyển hướng vào trang dashboard quản trị mới
            {
                path: '',
                redirectTo: 'dashboard', 
                pathMatch: 'full'
            },

            // ---------- Admin Dashboard ----------
            {
                path: 'dashboard',
                component: AdminDashboardComponent // ◄ Thêm Route Dashboard quản trị (`/admin/dashboard`)
            },

            // ---------- Admin Grammar ----------
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

            // ---------- Admin Question ----------
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

            // ---------- Admin Level ----------
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

            // ---------- Admin Lesson ----------
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

            // ---------- Admin Vocabulary ----------
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
    // CẢNH SÁT GIAO THÔNG (NOT FOUND REDIRECT)
    // =========================================
    {
        path: '**',
        redirectTo: 'grammar/levels'
    }
];