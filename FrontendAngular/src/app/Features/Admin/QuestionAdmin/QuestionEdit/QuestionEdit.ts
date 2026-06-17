import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';

// SERVICES
import { QuestionClientService } from '../../../../Core/Services/question-client-service';
import { GrammarClientService } from '../../../../Core/Services/grammar-client-service';

// MODELS
import { GrammarAdminModel } from '../../../../Models/AdminModel/grammar-admin-model';

@Component({
  selector: 'app-question-edit',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  providers: [QuestionClientService, GrammarClientService],
  templateUrl: './QuestionEdit.html',
  styleUrls: ['./QuestionEdit.css']
})
export class QuestionEdit implements OnInit {
  questionForm!: FormGroup;
  questionId!: number;

  // Giữ bộ lọc 3 tầng cũ để khi Hủy/Lưu thì quay về không bị mất dấu
  selectedLevelId: number = 0;
  selectedLessonId: number = 0;
  selectedGrammarId: number = 0;

  grammars: GrammarAdminModel[] = [];
  errorMessage: string = '';

  constructor(
    private _fb: FormBuilder,
    private _questionService: QuestionClientService,
    private _grammarService: GrammarClientService,
    private _route: ActivatedRoute,
    private _router: Router,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // 1. Lấy ID câu hỏi từ tham số Router URL (:id) và bộ lọc từ Query Params
    this.questionId = +this._route.snapshot.params['id'] || 0;

    const queryParams = this._route.snapshot.queryParams;
    this.selectedLevelId = +queryParams['levelId'] || 0;
    this.selectedLessonId = +queryParams['lessonId'] || 0;
    this.selectedGrammarId = +queryParams['grammarId'] || 0;

    // 2. Khởi tạo cấu trúc Reactive Form trống ban đầu
    this.questionForm = this._fb.group({
      id: [this.questionId],
      grammarId: ['', Validators.required],
      content: ['', Validators.required],
      questionType: [1, Validators.required],
      answers: this._fb.array([])
    });

    // 3. Tải danh mục cấu trúc ngữ pháp để đổ vào Dropdown Bước 2 trước
    if (this.selectedLessonId > 0) {
      this._grammarService.getAllForAdminAsync().subscribe(grmRes => {
        const allGrammars = grmRes ?? [];
        this.grammars = allGrammars.filter(g => (g.lessonId || (g as any).LessonId) == this.selectedLessonId);
        this._cdr.markForCheck();
      });
    }

    // 4. Gọi API lôi dữ liệu câu hỏi cũ kèm mảng đáp án từ Database lên form
    if (this.questionId > 0) {
      this._questionService.getQuestionByIdAsync(this.questionId).subscribe((res: any) => {
        if (!res) {
          this.goBackToList();
          return;
        }

        // Map ngược dữ liệu vào Form (Bao quát cả trường hợp API trả về chữ Hoa/Thường)
        this.questionForm.patchValue({
          grammarId: res.grammarId || res.GrammarId || '',
          content: res.content || res.Content || '',
          questionType: res.questionType !== undefined ? res.questionType : (res.QuestionType ?? 1)
        });

        // Bốc mảng đáp án con ra và dựng dòng FormArray động
        const rawAnswers = res.answers || res.Answers || [];
        if (rawAnswers.length > 0) {
          // Sắp xếp lại phương án theo DisplayOrder tăng dần giống hệt C# Linq `.OrderBy`
          rawAnswers.sort((a: any, b: any) => {
            const orderA = a.displayOrder || a.DisplayOrder || '0';
            const orderB = b.displayOrder || b.DisplayOrder || '0';
            return orderA.localeCompare(orderB);
          });

          rawAnswers.forEach((ans: any) => {
            this.answersFormArray.push(this._fb.group({
              id: [ans.id || ans.Id || 0], // Bảo tồn Id đáp án cũ để C# check cập nhật/thêm mới
              text: [ans.text || ans.Text || '', Validators.required],
              isCorrect: [ans.isCorrect === true || ans.IsCorrect === true],
              displayOrder: [ans.displayOrder || ans.DisplayOrder || '']
            }));
          });
        }

        this.handleQuestionTypeChange(); // Đồng bộ hóa giao diện khóa gạt theo loại câu hỏi cũ
        this._cdr.markForCheck();
      });
    }
  }

  // Quyền trợ giúp lấy khay FormArray ra ngoài HTML lặp
  get answersFormArray(): FormArray {
    return this.questionForm.get('answers') as FormArray;
  }

  /**
   * Ấn nút "➕ Thêm Dòng" đáp án mới tinh (mặc định id = 0)
   */
  addAnswerRow(): void {
    const isCorrectValue = this.questionForm.get('questionType')?.value == 2;

    const row = this._fb.group({
      id: [0], // 0 biểu thị cho dòng đáp án mới ấn nút "+" sinh thêm
      text: ['', Validators.required],
      isCorrect: [isCorrectValue],
      displayOrder: ['']
    });

    this.answersFormArray.push(row);
    this.reIndexAnswers();
  }

  /**
   * Xóa một dòng đáp án khỏi danh sách kèm ràng buộc số lượng tối thiểu
   */
  removeAnswerRow(index: number): void {
    if (this.answersFormArray.length <= 1) {
      alert("Phải có tối thiểu 1 dòng đáp án.");
      return;
    }
    this.answersFormArray.removeAt(index);
    this.reIndexAnswers();
  }

  /**
   * Thay đổi Dropdown loại câu hỏi -> Cấu hình đồng bộ lại trạng thái
   */
  handleQuestionTypeChange(): void {
    this.reIndexAnswers();
  }

  /**
   * Quản lý cơ chế bật tắt độc quyền Single Choice cho Trắc nghiệm (Mã loại 1)
   */
  handleSwitchToggle(triggeredIndex: number): void {
    if (this.questionForm.get('questionType')?.value == 1) {
      const currentControl = this.answersFormArray.at(triggeredIndex).get('isCorrect');

      if (currentControl?.value === true) {
        this.answersFormArray.controls.forEach((control, idx) => {
          if (idx !== triggeredIndex) {
            control.get('isCorrect')?.setValue(false);
          }
        });
      }
    }
  }

  /**
   * Đồng bộ chỉ mục hiển thị (DisplayOrder) tự động từ trên xuống dưới
   */
  private reIndexAnswers(): void {
    const qType = this.questionForm.get('questionType')?.value;

    this.answersFormArray.controls.forEach((control, idx) => {
      control.get('displayOrder')?.setValue((idx + 1).toString());

      if (qType == 2) {
        control.get('isCorrect')?.setValue(true);
      }
    });
    this._cdr.markForCheck();
  }

  /**
   * Hàm hỗ trợ quay xe lùi lại trang List chính, giữ nguyên vẹn 3 tầng bộ lọc
   */
  goBackToList(): void {
    this._router.navigate(['/admin/question/index'], {
      queryParams: {
        SelectedLevelId: this.selectedLevelId,
        SelectedLessonId: this.selectedLessonId,
        SelectedGrammarId: this.selectedGrammarId
      }
    });
  }

  /**
   * Bấm nút "LƯU THAY ĐỔI": Phát lệnh gửi gói tin JSON PUT lên máy chủ .NET API
   */
  onSubmit(): void {
    if (this.questionForm.invalid) return;

    this.errorMessage = '';
    const formData = this.questionForm.value;

    // KIỂM TRA ĐIỀU KIỆN TẦNG CLIENT: Dạng trắc nghiệm bắt buộc phải gạt bật 1 đáp án đúng
    if (formData.questionType == 1) {
      const hasCorrect = formData.answers.some((a: any) => a.isCorrect === true);
      if (!hasCorrect) {
        alert("Vui lòng gạt chọn duy nhất 1 đáp án thành [Đúng]!");
        return;
      }
    }

    // Ép kiểu chuẩn số nguyên
    formData.grammarId = +formData.grammarId;
    formData.questionType = +formData.questionType;

    // Gửi cấu trúc gói dữ liệu hoàn thiện lên API Service để thực hiện UPDATE
    this._questionService.updateQuestionAsync(this.questionId, formData).subscribe({
      next: (success) => {
        if (success) {
          this.goBackToList();
        } else {
          this.errorMessage = 'API Backend từ chối cập nhật dữ liệu. Hãy kiểm tra lại cấu trúc DB.';
          this._cdr.markForCheck();
        }
      },
      error: () => {
        this.errorMessage = 'Lỗi kết nối máy chủ đường truyền Backend.';
        this._cdr.markForCheck();
      }
    });
  }
}