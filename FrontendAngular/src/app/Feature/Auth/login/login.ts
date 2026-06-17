import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup;
  isLoading = false;
  message = '';

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onLogin() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.message = ''; // Reset thông báo cũ nếu có

      // 1. Gọi API Login để lấy và lưu Token
      this.authService.login(this.loginForm.value).subscribe({
        next: (res) => {
          console.log('Đăng nhập thành công! Đang tiến hành nạp dữ liệu hồ sơ...');

          // 🚀 2. GỌI NGAY API Profile để cập nhật Signal currentUser hệ thống
          this.authService.getProfile().subscribe({
            next: (userProfile) => {
              console.log('Đã đồng bộ thông tin user vào Signal:', userProfile);
              this.isLoading = false;
              
              // 3. Signal đã có dữ liệu -> Chuyển hướng an toàn về trang chủ, navbar sẽ đổi trạng thái ngay lập tức
              this.router.navigate(['/']);
            },
            error: (profileErr) => {
              console.error('Lỗi khi tải profile sau đăng nhập:', profileErr);
              this.isLoading = false;
              
              // Nếu không lấy được profile do lỗi mạng/hệ thống, vẫn cho về trang chủ để xử lý tiếp
              this.router.navigate(['/']);
            }
          });
        },
        error: (err) => {
          console.error('Lỗi đăng nhập:', err);
          this.isLoading = false;
          this.message = 'Email hoặc mật khẩu không chính xác!';
        }
      });
    }
  }
}