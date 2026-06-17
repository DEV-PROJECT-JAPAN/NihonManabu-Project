import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { LoginViewModel, LoginResponse } from '../../Models/login-model';

import { RegisterRequest, RegisterResponse } from '../../Models/register-model';
import { BaseService } from './BaseService'; // Đảm bảo đường dẫn này đúng

@Injectable({ providedIn: 'root' })
export class AuthService extends BaseService { // ◄ 1. Thêm 'extends'
  private readonly http = inject(HttpClient);

  // 1. ĐĂNG NHẬP
  login(loginData: LoginViewModel): Observable<LoginResponse> {
    // ◄ 2. Giờ đây có thể dùng this._apiBaseUrl
    return this.http.post<LoginResponse>(`${this._apiBaseUrl}/Auth/login`, loginData);
  }

  // 2. GỬI OTP
 // AuthService.ts
sendOtp(email: string): Observable<any> {
  return this.http.post(`${this._apiBaseUrl}/Auth/send-otp`, { Email: email });
}

  // 3. XÁC THỰC OTP & HOÀN TẤT ĐĂNG KÝ
 verifyAndRegister(data: RegisterRequest): Observable<RegisterResponse> {
  // Tách otp ra khỏi body, chỉ gửi các trường còn lại vào body
  const { otp, ...registerData } = data;

  // Sử dụng Template Literal để chèn OTP vào đường dẫn URL
  return this.http.post<RegisterResponse>(
    `${this._apiBaseUrl}/Auth/verify-and-register?otp=${otp}`, 
    registerData 
  );
}
}
