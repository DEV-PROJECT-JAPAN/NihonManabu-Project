import { Component, OnInit } from '@angular/core';
import { AdminService, DashboardData } from '../../../../Core/Services/admin-service';
import { Observable } from 'rxjs';
import { User } from '../../../../Core/Services/admin-service';
import { ChangeRoleDto } from '../../../../Core/Services/admin-service';
import { CommonModule } from "@angular/common";
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';  

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,                    
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.html', 
  styleUrls: ['./admin-dashboard.css']  
})
export class AdminDashboardComponent implements OnInit {
  dashboardData: DashboardData | null = null;
  users: User[] = [];
  isLoading = true;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
     forkJoin({
    dashboard: this.adminService.getDashboard(),
    users: this.adminService.getAllUsers()
  }).subscribe({
    next: (results) => {
      console.log('Dữ liệu đã nhận đủ:', results);
      this.dashboardData = results.dashboard;
      this.users = results.users;
      this.isLoading = false; // 🔥 Chỉ tắt khi CẢ HAI đã xong
    },
    error: (err) => {
      console.error('Lỗi khi tải dữ liệu:', err);
      this.isLoading = false; // 🔥 Tắt cả khi có lỗi để tránh treo
    }
  });
  }

  onChangeRole(user: User, newRole: string) {
    if (!confirm(`Bạn có chắc muốn đổi quyền của ${user.username} thành ${newRole}?`)) return;

    this.adminService.changeUserRole({ userId: user.id, role: newRole }).subscribe({
      next: () => {
        alert('Cập nhật quyền thành công!');
        user.role = newRole; // Cập nhật giao diện tạm thời
      },
      error: (err) => alert('Lỗi: ' + err.message)
    });
  }

  // Thêm vào class AdminDashboardComponent
// onAddUser() {
//   // Logic mở Modal Thêm mới
//   console.log('Mở modal thêm user...');
// }

// onEditUser(user: User) {
//   // Logic mở Modal Sửa, truyền dữ liệu user vào form
//   console.log('Sửa user:', user);
// }

}