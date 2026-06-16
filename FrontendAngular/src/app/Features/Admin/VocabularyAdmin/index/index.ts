import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule, Params } from '@angular/router';

import { VocabularyClientService } from '../../../../Core/Services/vocabulary-client-service';
import { VocabularyAdminModel } from '../../../../Models/AdminModel/vocabulary-admin-model';
import { LessonAdminModel } from '../../../../Models/AdminModel/lesson-admin-model';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';

import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';
import { LevelModel } from '../../../../Models/level-model';
@Component({
  selector: 'app-admin-vocabulary-index',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './index.html',
  styleUrls: ['./index.css']
})
export class VocabularyIndexComponent implements OnInit {
  private _vocabClientService = inject(VocabularyClientService);
  private _levelClientService = inject(LevelClientService);
  private _lessonClientService = inject(LessonClientService);
  private _route = inject(ActivatedRoute);

  // Dữ liệu cho các bảng và dropdown
  vocabularies = signal<VocabularyAdminModel[]>([]);
  levels = signal<LevelModel[]>([]);
  lessons = signal<LessonAdminModel[]>([]);

  // Lưu trữ trạng thái bộ lọc
  selectedLevelId = signal<number>(0);
  selectedLessonId = signal<number>(0);

  message = signal<string>('');

  ngOnInit(): void {
    // 1. Hứng "TempData" từ các trang Thêm/Sửa trả về
    if (history.state.successMsg) {
      this.message.set(history.state.successMsg);
      setTimeout(() => this.message.set(''), 3000);
    }

    // 2. Luôn nạp danh sách Level khi mở trang
    this.loadLevels();

    // 3. Đọc URL xem có đang lưu bộ lọc cũ (từ trang Edit/Create quay về) không
    this._route.queryParams.subscribe((params: Params) => {
      const levelId = Number(params['selectedLevelId']);
      const lessonId = Number(params['selectedLessonId']);

      if (levelId) {
        this.selectedLevelId.set(levelId);
        this.loadLessons(levelId); // Nạp danh sách Lesson tương ứng
      }

      if (lessonId) {
        this.selectedLessonId.set(lessonId);
        this.loadVocabularies(lessonId); // Nạp danh sách Từ vựng
      }
    });
  }

  // --- CÁC HÀM GỌI API ---

  loadLevels(): void {
    this._levelClientService.getLevelsAsync().subscribe({
      next: (data) => this.levels.set(data),
      error: (err) => console.error('Lỗi tải danh sách Cấp độ:', err)
    });
  }

  loadLessons(levelId: number): void {
    if (levelId > 0) {
      this._lessonClientService.getLessonsByLevelForAdminAsync(levelId).subscribe({
        next: (data) => this.lessons.set(data),
        error: (err) => console.error(`Lỗi tải Lesson cho Level ${levelId}:`, err)
      });
    } else {
      this.lessons.set([]); // Rỗng nếu chưa chọn Level
    }
  }

  loadVocabularies(lessonId: number): void {
    if (lessonId > 0) {
      this._vocabClientService.getVocabulariesByLessonForAdminAsync(lessonId).subscribe({
        next: (data) => this.vocabularies.set(data),
        error: (err) => console.error(`Lỗi tải Từ vựng cho Lesson ${lessonId}:`, err)
      });
    } else {
      this.vocabularies.set([]); // Tránh quá tải dữ liệu nếu chưa chọn bài học
    }
  }

  // --- CÁC HÀM XỬ LÝ SỰ KIỆN TỪ GIAO DIỆN (HTML) ---

  // Chạy khi Admin đổi Level ở Dropdown 1
  onLevelChange(): void {
    const currentLevel = this.selectedLevelId();

    // 💡 Logic quan trọng: Đổi Level thì phải reset ô Lesson và xóa bảng Từ vựng cũ đi
    this.selectedLessonId.set(0);
    this.vocabularies.set([]);

    this.loadLessons(currentLevel);
  }

  // Chạy khi Admin đổi Lesson ở Dropdown 2
  onLessonChange(): void {
    const currentLesson = this.selectedLessonId();
    this.loadVocabularies(currentLesson);
  }

  // --- XỬ LÝ XÓA ---

  deleteVocabulary(id: number): void {
    if (confirm('Bạn có chắc chắn muốn xóa từ vựng này không?')) {
      this._vocabClientService.deleteVocabularyAsync(id).subscribe({
        next: (success) => {
          if (success) {
            this.message.set('Xóa từ vựng thành công!');
            // Nạp lại danh sách từ vựng hiện tại mà không cần load lại trang
            this.loadVocabularies(this.selectedLessonId());
          } else {
            this.message.set('Xóa từ vựng thất bại.');
          }
          setTimeout(() => this.message.set(''), 3000);
        },
        error: (err) => {
          console.error('Lỗi hệ thống khi xóa từ vựng:', err);
          this.message.set('Lỗi hệ thống khi kết nối đến máy chủ.');
        }
      });
    }
  }
}