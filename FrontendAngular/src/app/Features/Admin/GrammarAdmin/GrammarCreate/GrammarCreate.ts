import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

// SERVICES
import { GrammarClientService } from '../../../../Core/Services/grammar-client-service';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';
import { LevelClientService } from '../../../../Core/Services/level-client-service';

// MODELS
import { LevelModel } from '../../../../Models/level-model';
import { LessonModel } from '../../../../Models/lesson-model';
import { GrammarAdminModel } from '../../../../Models/AdminModel/grammar-admin-model';

@Component({
  selector: 'app-grammar-create-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  providers: [GrammarClientService, LessonClientService, LevelClientService],
  templateUrl: './GrammarCreate.html',
  styleUrls: ['./GrammarCreate.css']
})
export class GrammarCreate implements OnInit {
  selectedLevelId: number | null = null;

  // Khởi tạo Model rỗng chuẩn bị hứng data form giống bên C# `new()`
  grammar: GrammarAdminModel = {
    id: 0,
    lessonId: 0,
    structure: '',
    explanation: '',
    createdAt: new Date(),
    updatedAt: new Date()
  };

  levels: LevelModel[] = [];
  lessons: LessonModel[] = [];
  errorMessage: string = '';

  constructor(
    private _levelService: LevelClientService,
    private _lessonService: LessonClientService,
    private _grammarService: GrammarClientService,
    private _route: ActivatedRoute,
    private _router: Router,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // 1. Đồng bộ nạp danh mục Cấp độ hệ thống
    this._levelService.getLevelsAsync().subscribe(levelsRes => {
      this.levels = levelsRes ?? [];

      // 2. Nhận tham số lessonId từ trang danh sách bắn sang (nếu có)
      const queryLessonId = this._route.snapshot.queryParams['lessonId'];
      if (queryLessonId && +queryLessonId > 0) {
        this.grammar.lessonId = +queryLessonId;

        // Tự động suy đoán ngược lại Level và tải danh sách bài học tương ứng
        this.inferLevelFromLesson(this.grammar.lessonId);
      }
      this._cdr.markForCheck();
    });
  }

  /**
   * Thay đổi Dropdown cấp độ chủ động tải danh sách bài học tương ứng
   */
  onLevelChange(): void {
    this.lessons = [];
    this.grammar.lessonId = 0; // Reset bài học đã chọn

    if (this.selectedLevelId && +this.selectedLevelId > 0) {
      this._lessonService.getLessonsByLevelAsync(+this.selectedLevelId).subscribe(res => {
        this.lessons = res ?? [];
        this._cdr.markForCheck();
      });
    }
  }

  /**
   * Submit đẩy gói tin dữ liệu Post lên Web API .NET
   */
  onSubmit(): void {
    this.errorMessage = '';

    // Ép kiểu đảm bảo dữ liệu gửi lên là số nguyên chuẩn chỉ
    this.grammar.lessonId = +this.grammar.lessonId;

    this._grammarService.createAsync(this.grammar as any).subscribe({
      next: (success) => {
        if (success) {
          // Lưu thành công, đá người dùng về trang list chính
          this._router.navigate(['/admin/grammar']);
        } else {
          this.errorMessage = 'Lỗi khi thêm mới dữ liệu vào hệ thống Web API.';
          this._cdr.markForCheck();
        }
      },
      error: () => {
        this.errorMessage = 'Đường truyền mạng trục trặc, không thể kết nối đến máy chủ .NET.';
        this._cdr.markForCheck();
      }
    });
  }

  /**
   * Tìm kiếm ngược danh mục bài học phục vụ trải nghiệm người dùng
   */
  private inferLevelFromLesson(lessonId: number): void {
    // Có thể gọi API nạp trực tiếp toàn bộ bài học để tìm LevelId của Lesson đó
    // Ở đây ta tạm thời lấy danh sách bài học của Level đầu tiên hoặc đợi người dùng map thủ công
    // Nếu Backend có DTO tốt, nó sẽ tự trả về LevelId kèm theo.
  }
}