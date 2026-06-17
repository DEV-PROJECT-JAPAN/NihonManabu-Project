import { Component, OnInit } from '@angular/core';
import { AdminService, DashboardData } from '../../../../Core/Services/admin-service';
import { Observable } from 'rxjs';
import { User } from '../../../../Core/Services/admin-service';
import { ChangeRoleDto } from '../../../../Core/Services/admin-service';
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,                    
  imports: [CommonModule],
  templateUrl: './admin-dashboard.html', 
  styleUrls: ['./admin-dashboard.css']  
})
export class AdminDashboardComponent implements OnInit {
  dashboardData: DashboardData | null = null;
  isLoading = true;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.adminService.getDashboard().subscribe({
      next: (data) => {
        console.log('Dữ liệu API trả về thực tế:', data);
        this.dashboardData = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi tải dữ liệu dashboard', err);
        this.isLoading = false;
      }
    });
  }
}