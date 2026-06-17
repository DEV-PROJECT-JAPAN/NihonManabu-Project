import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './BaseService'; 

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
  private readonly adminApiUrl = `${this._apiBaseUrl}/admin`; 

  // Sử dụng inject(HttpClient) thay vì khai báo private trong constructor 
  // để tránh hoàn toàn việc ghi đè (shadowing) biến http từ BaseService nếu có.
  private readonly http = inject(HttpClient);

  constructor() { 
    super(); 
  }

  // Lấy dữ liệu Dashboard
  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.adminApiUrl}/dashboard`);
  }

  // Lấy danh sách toàn bộ người dùng
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.adminApiUrl}/users`);
  }

  // Thay đổi quyền của User 
  changeUserRole(payload: ChangeRoleDto): Observable<any> {
    return this.http.put(`${this.adminApiUrl}/change-role`, payload);
  }
}