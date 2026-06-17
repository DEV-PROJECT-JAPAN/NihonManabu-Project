import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

// SERVICES
import { QuestionClientService } from '../../../../Core/Services/question-client-service';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';
import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { GrammarClientService } from '../../../../Core/Services/grammar-client-service';

// MODELS
import { LevelModel } from '../../../../Models/level-model';
import { LessonModel } from '../../../../Models/lesson-model';
import { GrammarAdminModel } from '../../../../Models/AdminModel/grammar-admin-model';
import { QuestionAdminModel } from '../../../../Models/AdminModel/question-admin-model';

@Component({
  selector: 'app-question-list-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  providers: [QuestionClientService, LessonClientService, LevelClientService, GrammarClientService],
  templateUrl: './QuestionList.html',
  styleUrls: ['./QuestionList.css']
})
export class QuestionList implements OnInit {
  // Biến lưu trạng thái bộ lọc 3 tầng lồng nhau
  selectedLevelId: number | null = null;
  selectedLessonId: number | null = null;
  selectedGrammarId: number | null = null;

  // Khay nạp dữ liệu danh mục
  levels: LevelModel[] = [];
  lessons: LessonModel[] = [];
  grammars: GrammarAdminModel[] = [];

  // Toàn bộ câu hỏi gốc của bài học phục vụ bộ lọc offline tầng 3
  private _allQuestionsInLesson: QuestionAdminModel[] = [];
  questions: QuestionAdminModel[] = [];

  message: string = '';

  constructor(
    private _questionService: QuestionClientService,
    private _lessonService: LessonClientService,
    private _levelService: LevelClientService,
    private _grammarService: GrammarClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Luôn nạp danh sách Cấp độ (N5, N4...) khi vừa đặt chân vào trang
    this._levelService.getLevelsAsync().subscribe(res => {
      this.levels = res ?? [];
      this._cdr.markForCheck();
    });
  }

  /**
   * TẦNG 1: Khi thay đổi Cấp độ hệ thống
   */
  onLevelChange(): void {
    this.selectedLessonId = null; // Reset sạch các tầng dưới
    this.selectedGrammarId = null;
    this.lessons = [];
    this.grammars = [];
    this.questions = [];
    this._allQuestionsInLesson = [];
    this._cdr.markForCheck();

    if (this.selectedLevelId && +this.selectedLevelId > 0) {
      this._lessonService.getLessonsByLevelAsync(+this.selectedLevelId).subscribe(res => {
        this.lessons = res ?? [];
        this._cdr.markForCheck();
      });
    }
  }

  /**
   * TẦNG 2: Khi chọn một Bài học cụ thể
   */
  onLessonChange(): void {
    this.selectedGrammarId = null; // Triệt tiêu liên kết lọc cũ tức thì
    this.grammars = [];
    this.questions = [];
    this._allQuestionsInLesson = [];
    this._cdr.markForCheck();

    if (this.selectedLessonId && +this.selectedLessonId > 0) {
      const searchLessonId = +this.selectedLessonId;

      // 1. Tải danh mục cấu trúc ngữ pháp thuộc bài học này để đổ vào Dropdown bước 3
      this._grammarService.getAllForAdminAsync().subscribe(grmRes => {
        const allGrammars = grmRes ?? [];
        this.grammars = allGrammars.filter(g => {
          const gLessonId = g.lessonId || (g as any).LessonId;
          return Number(gLessonId) === searchLessonId;
        });
        this._cdr.markForCheck();
      });

      // 2. Kéo trực tiếp toàn bộ câu hỏi của Bài học này từ API Backend về qua Observable
      this.loadQuestionsFromBackend(searchLessonId);
    }
  }

  /**
   * TẦNG 3: Lọc offline theo cấu trúc ngữ pháp cụ thể (Tùy chọn)
   */
  onGrammarChange(): void {
    if (this.selectedGrammarId && this.selectedGrammarId !== null && +this.selectedGrammarId > 0) {
      const searchGrammarId = +this.selectedGrammarId;
      // Thực hiện so sánh an toàn dạng Number tránh lệch kiểu dữ liệu
      this.questions = this._allQuestionsInLesson.filter(q => {
        const qGrammarId = q.grammarId || (q as any).GrammarId;
        return Number(qGrammarId) === searchGrammarId;
      });
    } else {
      // Nếu chọn hiển thị tất cả ngữ pháp trong bài
      this.questions = [...this._allQuestionsInLesson];
    }
    this._cdr.markForCheck();
  }

  /**
   * Hàm gọi API nạp câu hỏi từ Backend (.NET Web API)
   */
  private loadQuestionsFromBackend(lessonId: number): void {
    this._questionService.getQuestionsByLessonForAdminAsync(lessonId).subscribe(res => {
      this._allQuestionsInLesson = res ?? [];

      // Đồng bộ nạp lại bộ lọc tầng 3 nếu đang chọn dở
      if (this.selectedGrammarId && +this.selectedGrammarId > 0) {
        this.onGrammarChange();
      } else {
        this.questions = [...this._allQuestionsInLesson];
      }
      this._cdr.markForCheck();
    });
  }

  /**
   * Hành động xóa câu hỏi vĩnh viễn
   */
  onDelete(id: number): void {
    if (confirm('Khôi có chắc chắn muốn xóa câu hỏi này không?')) {
      this._questionService.deleteQuestionAsync(id).subscribe(success => {
        if (success) {
          this.message = 'Xóa câu hỏi thành công!';
          if (this.selectedLessonId) {
            this.loadQuestionsFromBackend(+this.selectedLessonId);
          }
        } else {
          this.message = 'Xóa câu hỏi thất bại.';
        }
        this._cdr.markForCheck();
      });
    }
  }

  /**
   * Hàm hỗ trợ kiểm tra đáp án đúng an toàn cho cả chữ Hoa/Thường
   */
  isAnswerCorrect(ans: any): boolean {
    if (!ans) return false;
    return ans.isCorrect === true || ans.IsCorrect === true;
  }

  /**
   * Hàm hỗ trợ lấy thể thức câu hỏi an toàn cho cả chữ Hoa/Thường
   */
  getQuestionType(q: any): number {
    if (!q) return 1;
    return q.questionType !== undefined ? q.questionType : (q.QuestionType ?? 1);
  }
}