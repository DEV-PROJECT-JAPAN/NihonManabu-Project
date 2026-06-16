import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('FrontendAngular');
  constructor(private _router: Router) { }
  goToVocabulary(): void {
    this._router.navigate(['/vocabulary/levels']);
  }
  // Hàm kiểm tra URL để tự động bật sáng Menu
  checkActive(url: string): boolean {
    // Nếu URL hiện tại của trình duyệt có chứa đoạn đường dẫn này thì trả về true (sáng đèn)
    return this._router.url.includes(url);
  }
}
