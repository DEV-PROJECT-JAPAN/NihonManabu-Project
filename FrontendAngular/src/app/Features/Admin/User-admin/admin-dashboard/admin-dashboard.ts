import { Component, OnInit } from '@angular/core';
import { AdminService, DashboardData } from '../Core/Services/admin-service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  dashboardData: DashboardData | null = null;
  isLoading = true;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.adminService.getDashboard().subscribe({
      next: (data) => {
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