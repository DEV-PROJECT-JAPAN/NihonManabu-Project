import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
    selector: 'app-admin-layout',
    standalone: true,
    imports: [CommonModule, RouterModule], // Nhớ import RouterModule để xài router-outlet
    templateUrl: './admin-layout.html'

})

export class AdminLayoutComponent {

    constructor(private _router: Router) { }

    goToQuestion(): void {
        this._router.navigate(['admin/question/index']);
    }

    goToGrammar(): void {
        this._router.navigate(['/admin/grammar/index']);
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
