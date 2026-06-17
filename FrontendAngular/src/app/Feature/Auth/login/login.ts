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
      this.message = ''; 

      const loginPayload = this.loginForm.value;

      // Ép kiểu 'res: any' để TypeScript không báo đỏ thuộc tính
      this.authService.login(loginPayload).subscribe({
        next: (res: any) => { 
          console.log('Đăng nhập thành công, hệ thống tự động xử lý token...');

          // Trì hoãn nhẹ 100ms để đảm bảo bộ nhớ localStorage đã ổn định hoàn toàn
          setTimeout(() => {
            this.isLoading = false;

            // Tiến hành kiểm tra quyền Admin và điều hướng trang công việc
            if (this.authService.isAdmin()) {
              this.router.navigate(['/admin/dashboard']); 
            } else {
              this.router.navigate(['/grammar/levels']);
            }

            // Đồng bộ dữ liệu Profile chạy ngầm
            this.authService.getProfile().subscribe({
              next: (userProfile) => {
                console.log('Đã cập nhật xong dữ liệu Signal ngầm:', userProfile);
              },
              error: (profileErr) => {
                console.error('Lỗi khi tải profile chạy ngầm:', profileErr);
              }
            });
          }, 100); 
        },
        error: (err) => {
          console.error('Lỗi đăng nhập:', err);
          this.isLoading = false;
          this.message = 'Email hoặc mật khẩu không chính xác!';
          alert('Đăng nhập thất bại! Vui lòng kiểm tra lại tài khoản, mật khẩu.');
        }
      });
    }
  }
}