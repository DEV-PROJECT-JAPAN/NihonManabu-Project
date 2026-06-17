import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // 🟢 BẮT BUỘC: Để dùng tính năng ngModel cho bộ lọc Dropdown
import { ActivatedRoute, RouterModule, Params } from '@angular/router';
import { LessonAdminModel } from '../../../../Models/AdminModel/lesson-admin-model';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';

import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';
import { LevelModel } from '../../../../Models/level-model';
@Component({
  selector: 'app-admin-lesson-index',
  standalone: true,
  // 💡 Nhớ thêm FormsModule vào imports
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './index.html',
  styleUrls: ['./index.css']
})
export class LessonIndexComponent implements OnInit {
  // Tiêm 2 Service (Tương đương Constructor bên C#)
  private _lessonClientService = inject(LessonClientService);
  private _levelClientService = inject(LevelClientService);
  private _route = inject(ActivatedRoute);
  // Khai báo các biến State (Dùng Signal)
  lessons = signal<LessonAdminModel[]>([]);
  levels = signal<LevelModel[]>([]);

  // Tương đương [BindProperty(SupportsGet = true)] SelectedLevelId
  // Đặt mặc định là 0 (Tương ứng với mục "Tất cả bài học")
  selectedLevelId = signal<number>(0);

  message = signal<string>('');
  ngOnInit(): void {
    this.loadLevels();

    // 🟢 2. Cú pháp chuẩn có ngoặc tròn: (params: any) => 
    this._route.queryParams.subscribe((params: any) => {
      if (params['selectedLevelId']) {
        this.selectedLevelId.set(Number(params['selectedLevelId']));
      }
      this.loadLessons();
    });
  }

  // 1. Luôn nạp danh sách Level lên bộ lọc Dropdown
  loadLevels(): void {
    this._levelClientService.getLevelsAsync().subscribe({
      next: (data) => this.levels.set(data),
      error: (err) => console.error('Lỗi khi tải danh sách bộ lọc Level:', err)
    });
  }

  // 2. Hàm xử lý logic Lọc và Tải danh sách Bài học
  loadLessons(): void {
    const levelId = this.selectedLevelId(); // Lấy giá trị đang được chọn trong Dropdown

    if (levelId && levelId > 0) {
      // 🟢 Gọi hàm lọc của Admin
      this._lessonClientService.getLessonsByLevelForAdminAsync(levelId).subscribe({
        next: (data) => this.lessons.set(data),
        error: (err) => console.error(`Lỗi khi lọc bài học theo Level ${levelId}:`, err)
      });
    } else {
      // Nếu chọn "Tất cả" -> Lấy toàn bộ như cũ
      this._lessonClientService.getLessonsForAdminAsync().subscribe({
        next: (data) => this.lessons.set(data),
        error: (err) => console.error('Lỗi tải toàn bộ danh sách Bài học:', err)
      });
    }
  }

  // Hàm được gọi khi người dùng bấm chọn một Level khác trên Dropdown (Giao diện HTML)
  onFilterChange(): void {
    this.loadLessons();
  }

  // Tương đương OnPostDeleteAsync()
  deleteLesson(id: number): void {
    if (confirm('Bạn có chắc chắn muốn xóa bài học này không?')) {
      this._lessonClientService.deleteLessonAsync(id).subscribe({
        next: (success) => {
          if (success) {
            this.message.set('Xóa bài học thành công!');
            this.loadLessons(); // 🟢 Cập nhật lại danh sách mà không cần RedirectToPage
          } else {
            this.message.set('Xóa bài học thất bại.');
          }

          // Tự động dọn dẹp câu thông báo sau 3 giây
          setTimeout(() => this.message.set(''), 3000);
        },
        error: (err) => {
          console.error('Lỗi API khi xóa:', err);
          this.message.set('Lỗi hệ thống khi xóa bài học.');
        }
      });
    }
  }
}