import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LoginViewModel, LoginResponse } from '../../Models/login-model';
import { ProfileModel } from '../../Models/profile-model';
import { RegisterRequest, RegisterResponse } from '../../Models/register-model';
import { BaseService } from './BaseService'; 

@Injectable({ providedIn: 'root' })
export class AuthService extends BaseService { 
  private readonly http = inject(HttpClient);
  private readonly TOKEN_KEY = 'auth_token';
  currentUser = signal<ProfileModel | null>(null);

  // 1. ĐĂNG NHẬP
  login(loginData: LoginViewModel): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this._apiBaseUrl}/Auth/login`, loginData)
      .pipe(tap(res => {
        if (res.token) localStorage.setItem(this.TOKEN_KEY, res.token);
      }));
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // ==========================================
  // THÀNH PHẦN THÊM MỚI: NHẬN DIỆN VÀ PHÂN QUYỀN ROLE
  // ==========================================
  
  // Hàm giải mã Token để lấy chuỗi Role thực tế từ Claim của Backend .NET
  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const tokenParts = token.split('.');
      if (tokenParts.length < 2) return null;

      // 🛠️ ĐÃ SỬA: Lấy chính xác phần tử index 1 và ép kiểu chuỗi (string)
      const payloadBase64 = tokenParts[1] as string; 
      
      // Giải mã an toàn bằng cách xử lý triệt để các ký tự đặc biệt / UTF-8
      const base64 = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      const decodedPayload = JSON.parse(jsonPayload);
      
      // Trả về đúng claim schema từ hình ảnh thực tế của bạn
      return decodedPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 
             decodedPayload['role'] || 
             null;
    } catch (error) {
      console.error('Không thể giải mã Token để lấy Role do lỗi ký tự:', error);
      return null;
    }
  }

  // Kiểm tra nhanh xem tài khoản hiện tại có phải Admin hay không
  isAdmin(): boolean {
    return this.getUserRole() === 'Admin';
  }

  // ==========================================

  getProfile(): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(`${this._apiBaseUrl}/Auth/profile`).pipe(
      tap(profile => {
        this.currentUser.set(profile); 
      })
    );
  }

  loadProfile(): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(`${this._apiBaseUrl}/Auth/profile`).pipe(
      tap(profile => {
        this.currentUser.set(profile); 
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUser.set(null);
  }

  // 2. GỬI OTP
  sendOtp(email: string): Observable<any> {
    return this.http.post(`${this._apiBaseUrl}/Auth/send-otp`, { Email: email });
  }

  // 3. XÁC THỰC OTP & HOÀN TẤT ĐĂNG KÝ
  verifyAndRegister(data: RegisterRequest): Observable<RegisterResponse> {
    const { otp, ...registerData } = data;

    return this.http.post<RegisterResponse>(
      `${this._apiBaseUrl}/Auth/verify-and-register?otp=${otp}`, 
      registerData 
    );  
  }
}