import { Component, inject, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';

@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterModule, RouterOutlet],
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

        /* 👤 STYLE MINI DROPDOWN HỒ SƠ TÀI KHOẢN GÓC PHẢI */
        .profile-dropdown-wrapper {
            position: relative;
            display: inline-block;
        }

        .avatar-toggle-btn {
            background: transparent;
            border: none;
            color: #94A3B8;
            font-size: 22px;
            cursor: pointer;
            display: flex;
            align-items: center;
            padding: 4px 8px;
            border-radius: 20px;
            transition: all 0.2s ease;
        }

        .avatar-toggle-btn:hover {
            color: #FFFFFF;
            background-color: rgba(255, 255, 255, 0.05);
        }

        .avatar-toggle-btn .arrow-icon {
            font-size: 11px;
            transition: transform 0.2s ease;
        }

        .avatar-toggle-btn .arrow-icon.rotated {
            transform: rotate(180deg);
        }

        .profile-mini-dropdown {
            position: absolute;
            top: calc(100% + 8px);
            right: 0;
            width: 180px;
            background-color: #0B0F19;
            border: 1px solid #1E293B;
            border-radius: 10px;
            box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.5);
            padding: 8px 0;
            z-index: 1000;
            animation: fadeInDropdown 0.15s ease-out;
        }

        @keyframes fadeInDropdown {
            from { opacity: 0; transform: translateY(-5px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .dropdown-header {
            padding: 6px 16px;
            font-size: 11px;
        }

        .user-role-badge {
            background: rgba(59, 130, 246, 0.2);
            color: #60A5FA;
            padding: 2px 8px;
            border-radius: 12px;
            font-weight: 700;
        }

        .dropdown-divider {
            border-color: #1E293B;
            margin: 6px 0;
        }

        .dropdown-item {
            display: flex;
            align-items: center;
            width: 100%;
            padding: 10px 16px;
            color: #94A3B8;
            font-size: 13px;
            font-weight: 600;
            border: none;
            background: transparent;
            text-align: left;
            text-decoration: none !important;
            cursor: pointer;
            transition: all 0.15s ease;
        }

        .dropdown-item:hover {
            color: #FFFFFF;
            background-color: rgba(255, 255, 255, 0.03);
        }

        .dropdown-item.logout-action {
            color: #EF4444;
        }

        .dropdown-item.logout-action:hover {
            background-color: rgba(239, 68, 68, 0.08);
        }
    `]
})
export class UserLayoutComponent {
    // Sử dụng inject (Angular 14+) thay cho Constructor để giữ code sạch và hiện đại
    public authService = inject(AuthService); 
    private _router = inject(Router);

    isDropdownOpen = false;

navigateToProfile(): void {
    this.isDropdownOpen = false; // Đóng hộp dropdown lại trước
    
    // Điều hướng dựa vào Router hệ thống
    this._router.navigate(['/auth/profile']); 
}
    goToVocabulary(): void {
        this._router.navigate(['/vocabulary/levels']);
    }

    goToGrammar(): void {
        this._router.navigate(['/grammar/levels']);
    }

    toggleDropdown(event: Event): void {
        event.stopPropagation(); // Ngăn sự kiện click bọt nước làm đóng menu lập tức
        this.isDropdownOpen = !this.isDropdownOpen;
    }

    onLogout(): void {
        this.authService.logout();
        this.isDropdownOpen = false;
        this._router.navigate(['/']); // Điều hướng về trang chủ sau khi thoát
    }

    // Lắng nghe sự kiện click trên toàn bộ document để đóng Dropdown khi bấm ra ngoài
    @HostListener('document:click')
    closeDropdown(): void {
        this.isDropdownOpen = false;
    }

    checkActive(moduleName: string): boolean {
        return this._router.url.includes(moduleName);
    }
}