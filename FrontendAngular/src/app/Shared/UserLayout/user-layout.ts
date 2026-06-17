import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';


@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterModule], // Nhớ import RouterModule để xài router-outlet
    templateUrl: './user-layout.html'
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
