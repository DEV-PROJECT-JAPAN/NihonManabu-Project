import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
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
  selector: 'app-grammar-list-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  providers: [GrammarClientService, LessonClientService, LevelClientService],
  templateUrl: './grammar-list-admin.html',
  styleUrls: ['./grammar-list-admin.css']
})
export class index implements OnInit {
  // Trạng thái lưu bộ lọc
  selectedLevelId: number | null = null;
  selectedLessonId: number | null = null;

  // Khay dữ liệu đổ vào giao diện
  levels: LevelModel[] = [];
  lessons: LessonModel[] = [];
  grammars: GrammarAdminModel[] = [];

  message: string = '';

  constructor(
    private _levelService: LevelClientService,
    private _lessonService: LessonClientService,
    private _grammarService: GrammarClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Luôn nạp danh sách Cấp độ (N5, N4...) lúc khởi chạy trang
    this._levelService.getLevelsAsync().subscribe(res => {
      this.levels = res ?? [];
      this._cdr.markForCheck();
    });
  }

  /**
   * Bước 1: Khi Admin thay đổi Cấp độ (Dropdown 1)
   */
  onLevelChange(): void {
    this.selectedLessonId = null; // Reset bộ lọc bài học con
    this.lessons = [];
    this.grammars = [];

    if (this.selectedLevelId && this.selectedLevelId > 0) {
      this._lessonService.getLessonsByLevelAsync(this.selectedLevelId).subscribe(res => {
        this.lessons = res ?? [];
        this._cdr.markForCheck();
      });
    }
  }

  /**
   * Bước 2: Khi Admin chọn một Bài học cụ thể (Dropdown 2)
   */
  onLessonChange(): void {
    this.loadGrammarList();
  }

  /**
   * Hàm gọi API nạp danh sách và lọc dữ liệu ngữ pháp theo mã bài học
   */
  private loadGrammarList(): void {
    if (this.selectedLessonId && this.selectedLessonId > 0) {
      this._grammarService.getAllForAdminAsync().subscribe(res => {
        const allGrammars = res ?? [];

        // Thực hiện lọc chính xác theo LessonId giống hệt C# Linq (.Where)
        this.grammars = allGrammars.filter(g => (g.lessonId || (g as any).LessonId) === this.selectedLessonId);
        this._cdr.markForCheck();
      });
    } else {
      this.grammars = [];
    }
  }

  /**
   * Hành động xóa mẫu ngữ pháp vĩnh viễn
   */
  onDelete(id: number): void {
    if (confirm('Khôi có chắc chắn muốn xóa mẫu ngữ pháp này vĩnh viễn khỏi Database?')) {
      this._grammarService.deleteAsync(id).subscribe(success => {
        if (success) {
          this.message = 'Xóa mẫu ngữ pháp thành công!';
          this.loadGrammarList(); // Tải lại bảng dữ liệu tại chỗ, giữ nguyên bộ lọc
        } else {
          this.message = 'Xóa mẫu ngữ pháp thất bại.';
        }
        this._cdr.markForCheck();
      });
    }
  }
}