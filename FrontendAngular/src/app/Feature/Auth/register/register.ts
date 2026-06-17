import { Component, inject,ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../Core/Services/auth-service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly cdr = inject(ChangeDetectorRef);

  registerForm: FormGroup;
  isOtpSent = false;
  isLoading = false;
  message = '';

  constructor() {
    this.registerForm = this.fb.group({
      // Đã thêm fullName và confirmPassword để khớp với HTML và DTO
      fullName: ['', Validators.required], 
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      confirmPassword: ['', Validators.required],
      otp: ['', Validators.required]
    }, { validators: this.passwordMatchValidator }); // Thêm validator kiểm tra mật khẩu
  }

  // Validator kiểm tra mật khẩu trùng khớp
  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value 
      ? null : { mismatch: true };
  }

  onSendOtp() {
    const emailControl = this.registerForm.get('email');
    if (emailControl?.invalid) {
      this.message = 'Vui lòng nhập email hợp lệ.';
      return;
    }

    this.isLoading = true;
    this.authService.sendOtp(emailControl?.value).subscribe({
      next: (res: any) => { // Thêm biến res để nhận JSON từ server
        this.isLoading = false;
        this.isOtpSent = true; // Kích hoạt hiển thị ô OTP
        this.message = res.message || 'Mã OTP đã được gửi!';
        console.log("OTP sent, isOtpSent set to true");
        this.cdr.detectChanges(); 
      console.log("Đã gọi detectChanges()");
      },
      error: (err) => {
        console.error("Lỗi gửi OTP:", err);
        this.isLoading = false;
        // Kiểm tra xem lỗi có trả về message không
        this.message = err.error?.message || 'Không thể gửi OTP.';
        this.isOtpSent = false;
      }
    });
  }

  onRegister() {
    // Kiểm tra trạng thái form
    if (this.registerForm.invalid) {
      this.message = 'Vui lòng kiểm tra lại thông tin (đảm bảo mật khẩu trùng khớp).';
      return;
    }

    this.isLoading = true;
    console.log("Sending data:", this.registerForm.value); // Debug dữ liệu gửi đi

    this.authService.verifyAndRegister(this.registerForm.value).subscribe({
      next: (res) => {
        this.message = 'Đăng ký thành công! Đang chuyển hướng...';
        this.isLoading = false;
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        console.error("Lỗi đăng ký:", err);
        // Sử dụng err.error.Message (viết hoa chữ M) theo DTO/Controller bạn đã sửa
        this.message = 'Đăng ký thất bại: ' + (err.error?.Message || err.error?.message || 'Mã OTP không đúng.');
        this.isLoading = false;
      }
    });
  }
}