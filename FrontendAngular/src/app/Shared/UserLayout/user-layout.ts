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
        this._router.navigate(['/Grammar/levels']);
    }
    // Hàm kiểm tra URL để tự động bật sáng Menu
    checkActive(url: string): boolean {
        // Nếu URL hiện tại của trình duyệt có chứa đoạn đường dẫn này thì trả về true (sáng đèn)
        return this._router.url.includes(url);
    }
}
