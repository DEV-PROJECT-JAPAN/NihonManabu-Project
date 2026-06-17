import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';

// Nhớ kiểm tra lại đường dẫn import cho đúng thư mục của bạn nhé
import { LessonClientService } from '../../../Core/Services/lesson-client-service';
import { LessonModel } from '../../../Models/lesson-model';

@Component({
  selector: 'app-vocabulary-lessons',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './lessons.html',
  styleUrls: ['./lessons.css']
})
export class LessonsComponent implements OnInit {

  // 1. Tương đương với: public int LevelId { get; set; }
  public levelId: number = 0;

  // 2. Tương đương với: public List<LessonModel> Lessons { get; set; } = new();
  public lessons: LessonModel[] = [];

  constructor(
    private _lessonService: LessonClientService,
    private _route: ActivatedRoute,
    private _cdr: ChangeDetectorRef
  ) { }
  // 4. Tương đương với: public async Task OnGetAsync(int levelId)
  ngOnInit(): void {
    // Bắt tham số 'id' mà trang Level vừa ném sang qua thanh URL
    const paramId = this._route.snapshot.paramMap.get('id');

    if (paramId) {
      this.levelId = Number(paramId); // Ép kiểu chuỗi về số
      this.fetchLessons(); // Chạy hàm gọi API
    }
  }

  // Tách riêng hàm gọi API cho gọn gàng và dễ bảo trì
  private fetchLessons(): void {
    this._lessonService.getLessonsByLevelAsync(this.levelId).subscribe({
      next: (data) => {
        // Nhận được dữ liệu thì gán vào biến lessons để HTML tự động vẽ ra
        this.lessons = data;
        this._cdr.markForCheck();
      },
      error: (err) => {
        console.error('Lỗi khi tải danh sách bài học:', err);
      }
    });
  }
}