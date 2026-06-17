import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';


@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterModule,RouterOutlet], // Nhớ import RouterModule để xài router-outlet
    templateUrl: './user-layout.html',
    styles: [`
        .app-layout-wrapper {
            display: flex;
            width: 100vw;
            height: 100vh;
            overflow: hidden;
            background-color: #0F172A;
            color: #F1F5F9;
        }

        .app-sidebar {
            width: 240px;
            height: 100%;
            background-color: #0B0F19;
            border-right: 1px solid #1E293B;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            flex-shrink: 0;
        }

        .sidebar-brand {
            padding: 24px;
            display: flex;
            align-items: center;
            gap: 10px;
            border-bottom: 1px solid rgba(255,255,255,.03);
        }

        .brand-name {
            font-size: 16px;
            font-weight: 800;
            color: #fff;
            letter-spacing: .5px;
        }

        .sidebar-menu {
            display: flex;
            flex-direction: column;
            gap: 4px;
            padding: 16px 12px;
            flex-grow: 1;
        }

        .menu-item {
            display: flex;
            align-items: center;
            gap: 14px;
            padding: 12px 16px;
            color: #94A3B8;
            font-size: 14px;
            font-weight: 600;
            border-radius: 10px;
            text-decoration: none;
            transition: all .2s ease;
        }

        .menu-item i {
            width: 18px;
            text-align: center;
        }

        .menu-item:hover {
            color: #fff;
            background: rgba(255,255,255,.03);
        }

        .menu-item.active {
            background: #3B82F6;
            color: #fff;
        }

        .premium-link {
            color: #F59E0B !important;
        }

        .premium-link:hover {
            background: rgba(245,158,11,.1) !important;
        }

        .sidebar-footer {
            padding: 12px;
            border-top: 1px solid rgba(255,255,255,.03);
        }

        .app-main-content {
            flex-grow: 1;
            height: 100%;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .app-top-navbar {
            height: 64px;
            background: #0F172A;
            border-bottom: 1px solid #1E293B;
            flex-shrink: 0;
        }

        .nav-icon-btn {
            background: transparent;
            border: none;
            color: #94A3B8;
            font-size: 18px;
            cursor: pointer;
            padding: 6px 10px;
        }

        .nav-icon-btn:hover {
            color: #fff;
        }

        .login-btn {
            border-color: #334155;
            color: #CBD5E1;
            font-weight: 600;
            border-radius: 8px;
        }

        .login-btn:hover {
            background: #3B82F6;
            border-color: #3B82F6;
        }

        .content-body {
            flex-grow: 1;
            overflow-y: auto;
            padding: 10px 0;
        }

        .content-body::-webkit-scrollbar {
            width: 6px;
        }

        .content-body::-webkit-scrollbar-thumb {
            background: #1E293B;
            border-radius: 10px;
        }

        .content-body::-webkit-scrollbar-thumb:hover {
            background: #334155;
        }
    `]
})

export class UserLayoutComponent {

    constructor(private _router: Router) { }

    goToVocabulary(): void {
        this._router.navigate(['/vocabulary/levels']);
    }

    goToGrammar(): void {
        this._router.navigate(['/grammar/levels']);
    }

    /**
     * 🌟 HÀM KIỂM TRA ACTIVE THÔNG MINH
     * Chỉ cần URL hiện tại chứa từ khóa phân hệ (Vd: 'vocabulary' hoặc 'grammar')
     * là menu đó sẽ luôn giữ trạng thái active sáng đèn.
     */
    checkActive(moduleName: string): boolean {
        return this._router.url.includes(moduleName);
    }
}