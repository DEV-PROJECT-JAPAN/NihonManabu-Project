import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './BaseService'; // Thay đổi đường dẫn cho đúng với dự án của bạn

// Định nghĩa các Interface khớp với DTO và Model của Backend
export interface User {
  id: number;
  username?: string; 
  email?: string;
  role: string;
}

export interface DashboardData {
  totalUsers: number;
}

export interface ChangeRoleDto {
  userId: number;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService extends BaseService {
  // Kết hợp _apiBaseUrl từ BaseService để tạo endpoint cụ thể cho Admin
  private readonly adminApiUrl = `${this._apiBaseUrl}/admin`; 

  // Sử dụng 'public override http: HttpClient' nếu BaseService cũng inject HttpClient,
  // Hoặc giữ nguyên nếu BaseService không inject nó.
  constructor(private http: HttpClient) { 
    super(); // Bắt buộc phải gọi super() khi kế thừa trong TypeScript
  }

  // Lấy dữ liệu Dashboard
  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.adminApiUrl}/dashboard`);
  }

  // Lấy danh sách toàn bộ người dùng
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.adminApiUrl}/users`);
  }

  // Thay đổi quyền của User (khớp với [HttpPut("change-role")] và ChangeRoleDto)
  changeUserRole(payload: ChangeRoleDto): Observable<any> {
    return this.http.put(`${this.adminApiUrl}/change-role`, payload);
  }
}