import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth-service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class ProfileComponent implements OnInit {
  // 1. Chuyển authService thành public để tầng HTML template đọc được trực tiếp từ Signal
  public authService = inject(AuthService);
  private router = inject(Router);

  error = '';

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }

    // 2. Kiểm tra xem Signal chung đã có dữ liệu chưa. Nếu chưa có (như khi F5 trực tiếp trang profile), tiến hành nạp lại.
    if (!this.authService.currentUser()) {
      this.authService.getProfile().subscribe({
        next: (data) => {
          console.log('Đã nạp dữ liệu Profile thành công vào Signal hệ thống:', data);
        },
        error: (err) => {
          console.error('Lỗi khi tải thông tin Profile:', err);
          this.error = 'Không tải được profile. Vui lòng đăng nhập lại.';
        }
      });
    }
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}