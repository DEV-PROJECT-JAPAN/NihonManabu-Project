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