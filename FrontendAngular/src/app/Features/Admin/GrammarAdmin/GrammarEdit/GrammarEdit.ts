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
  selector: 'app-grammar-edit-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  providers: [GrammarClientService, LessonClientService, LevelClientService],
  templateUrl: './GrammarEdit.html',
  styleUrls: ['./GrammarEdit.css']
})
export class GrammarEdit implements OnInit {
  id!: number;
  selectedLevelId: number | null = null;

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
    // 1. Đọc ID bản ghi cần sửa từ tham số URL Router
    this.id = +this._route.snapshot.params['id'];

    // 2. Luôn nạp danh mục Cấp độ hệ thống trước
    this._levelService.getLevelsAsync().subscribe(levelsRes => {
      this.levels = levelsRes ?? [];

      // 3. Gọi API nạp dữ liệu thực thể gốc cần sửa
      this._grammarService.getByIdForAdminAsync(this.id).subscribe((grammarRes: any) => {
        if (!grammarRes) {
          this._router.navigate(['/admin/grammar/index']);
          return;
        }

        // Gán dữ liệu map bao quát cả kiểu chữ Hoa/Thường của API đổ về
        this.grammar = {
          id: grammarRes.id || grammarRes.Id,
          lessonId: grammarRes.lessonId || grammarRes.LessonId,
          structure: grammarRes.structure || grammarRes.Structure,
          explanation: grammarRes.explanation || grammarRes.Explanation,
          createdAt: grammarRes.createdAt || grammarRes.CreatedAt,
          updatedAt: grammarRes.updatedAt || grammarRes.UpdatedAt
        };

        // 4. Dò tìm ngược bài học xem thuộc Level nào để render Dropdown số 2 đồng bộ
        if (this.grammar.lessonId > 0) {
          // Bạn thay bằng hàm GetLessonById thực tế trong hệ thống của bạn nhé
          this._lessonService.getLessonsByLevelAsync(1).subscribe(() => {
            // Tạm thời nạp danh sách bài học tương ứng dựa theo cơ chế đồng bộ
            // Để tối ưu trải nghiệm, ta tự tạo hàm dò tìm dựa vào lessonId
            this.loadLessonsByCurrentGrammar(this.grammar.lessonId);
          });
        } else {
          this._cdr.markForCheck();
        }
      });
    });
  }

  /**
   * Tải khay bài học con khi đổi Dropdown Cấp độ hệ thống
   */
  onLevelChange(): void {
    this.lessons = [];
    this.grammar.lessonId = 0; // Reset lựa chọn bài học con

    if (this.selectedLevelId && +this.selectedLevelId > 0) {
      this._lessonService.getLessonsByLevelAsync(+this.selectedLevelId).subscribe(res => {
        this.lessons = res ?? [];
        this._cdr.markForCheck();
      });
    }
  }

  /**
   * Gửi gói tin Put cập nhật sửa đổi lên .NET API
   */
  onSubmit(): void {
    this.errorMessage = '';
    this.grammar.lessonId = +this.grammar.lessonId; // Ép kiểu số nguyên chắc chắn

    this._grammarService.updateAsync(this.id, this.grammar as any).subscribe({
      next: (success) => {
        if (success) {
          this._router.navigate(['/admin/grammar/index']);
        } else {
          this.errorMessage = 'Lỗi trong quá trình cập nhật dữ liệu vào hệ thống Web API .NET.';
          this._cdr.markForCheck();
        }
      },
      error: () => {
        this.errorMessage = 'Lỗi kết nối máy chủ đường truyền Backend.';
        this._cdr.markForCheck();
      }
    });
  }

  /**
   * Hàm hỗ trợ nạp mảng bài học theo bài học hiện tại để khay select box không bị trống
   */
  private loadLessonsByCurrentGrammar(lessonId: number): void {
    // Để xử lý tối ưu, nạp danh sách bài học từ database hoặc cấp độ phán đoán
    // Giả định nạp danh mục bài học của level mặc định hoặc gọi API lấy chi tiết lesson
    // Ví dụ tạm thời ép nạp để tránh select box rỗng:
    this._lessonService.getLessonsByLevelAsync(1).subscribe(res => {
      this.lessons = res ?? [];
      // Tự lọc tìm để gán ngược selectedLevelId lên giao diện
      this.selectedLevelId = 1; // Gán cứng level tương ứng hoặc xử lý linh hoạt theo DTO của bạn
      this._cdr.markForCheck();
    });
  }
}