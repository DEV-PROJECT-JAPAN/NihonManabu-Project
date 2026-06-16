import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';

@Component({
  selector: 'app-admin-level-index',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './index.html',
  styleUrls: ['./index.css']
})
export class LevelIndexComponent implements OnInit {
  // Tiêm service (Tương đương Constructor Injection bên C#)
  private _levelClientService = inject(LevelClientService);

  // Khai báo biến (Dùng Signal để giao diện tự cập nhật mượt mà)
  levels = signal<LevelAdminModel[]>([]);
  message = signal<string>('');

  // Tương đương OnGetAsync() bên C#
  ngOnInit(): void {
    this.loadLevels();
  }

  // Hàm tải danh sách
  loadLevels(): void {
    this._levelClientService.getLevelsForAdminAsync().subscribe({
      next: (data) => this.levels.set(data),
      error: (err) => console.error('Lỗi khi tải danh sách Level:', err)
    });
  }

  // Tương đương OnPostDeleteAsync() bên C#
  deleteLevel(id: number): void {
    // Thêm hàm confirm cho an toàn trước khi xóa
    if (confirm('Bạn có chắc chắn muốn xóa cấp độ này không?')) {
      this._levelClientService.deleteLevelAsync(id).subscribe({
        next: (success) => {
          if (success) {
            this.message.set('Xóa cấp độ thành công!');
            // Gọi lại hàm loadLevels để cập nhật bảng, KHÔNG cần reload lại trang
            this.loadLevels();
          } else {
            this.message.set('Xóa thất bại.');
          }

          // Tự động ẩn thông báo sau 3 giây (Option thêm cho đẹp)
          setTimeout(() => this.message.set(''), 3000);
        },
        error: (err) => {
          this.message.set('Lỗi hệ thống khi xóa.');
          console.error(err);
        }
      });
    }
  }
}