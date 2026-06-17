import { Component, signal, inject, OnInit } from '@angular/core'; // 💡 Thêm inject và OnInit
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { AuthService } from './Core/Services/auth-service'; // 💡 Đường dẫn tới AuthService của bạn

@Component({
  selector: 'app-root',
  standalone: true, // Đảm bảo thuộc tính standalone hoạt động đồng bộ
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit { // 💡 Thực thi OnInit để chạy code khi app vừa bật
  protected readonly title = signal('FrontendAngular');
  
  // Sử dụng inject() thay vì constructor để đồng bộ với cách viết hiện đại của bạn
  private _router = inject(Router);
  private _authService = inject(AuthService); 

  ngOnInit(): void {
    // 💡 TỰ ĐỘNG PHỤC HỒI PROFILE KHI F5:
    // Nếu kiểm tra thấy trong localStorage có Token (đã đăng nhập trước đó)
    if (this._authService.isLoggedIn()) {
      this._authService.getProfile().subscribe({
        next: (profile) => {
          console.log('Phục hồi dữ liệu Profile thành công:', profile);
        },
        error: (err) => {
          console.error('Token hết hạn hoặc lỗi, tự động đăng xuất:', err);
          this._authService.logout(); // Token rác/hết hạn thì xóa đi để bắt đăng nhập lại
        }
      });
    }
  }

  goToVocabulary(): void {
    this._router.navigate(['/vocabulary/levels']);
  }

  // Hàm kiểm tra URL để tự động bật sáng Menu
  checkActive(url: string): boolean {
    return this._router.url.includes(url);
  }
}