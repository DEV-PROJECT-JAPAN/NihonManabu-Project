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
  selector: 'app-question-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  providers: [QuestionClientService, GrammarClientService],
  templateUrl: './QuestionCreate.html',
  styleUrls: ['./QuestionCreate.css']
})
export class QuestionCreate implements OnInit {
  questionForm!: FormGroup;

  // Trạng thái giữ bộ lọc từ URL trang List đẩy sang
  selectedLevelId: number = 0;
  selectedLessonId: number = 0;
  selectedGrammarId: number = 0; // Hứng GrammarId phục vụ chọn sẵn dropdown

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
    // 1. Nhận trọn vẹn tham số lọc 3 tầng từ Query URL trang List bắn sang
    const queryParams = this._route.snapshot.queryParams;
    this.selectedLevelId = +queryParams['levelId'] || 0;
    this.selectedLessonId = +queryParams['lessonId'] || 0;
    this.selectedGrammarId = +queryParams['grammarId'] || 0;

    // 2. Khởi tạo cấu trúc Reactive Form quản lý dữ liệu lồng nhau
    this.questionForm = this._fb.group({
      id: [0],
      // Điền thẳng selectedGrammarId vào đây làm giá trị mặc định ban đầu
      grammarId: [this.selectedGrammarId || '', Validators.required],
      content: ['', Validators.required],
      questionType: [1, Validators.required],
      answers: this._fb.array([]) // Tạo khay chứa danh sách động
    });

    // 3. Khởi tạo mặc định sẵn 4 hàng đáp án trống giống C# backend lúc OnGet
    for (let i = 0; i < 4; i++) {
      this.addAnswerRow();
    }

    // 4. Gọi API nạp danh mục cấu trúc ngữ pháp thuộc bài học hiện tại để lọc dropdown
    if (this.selectedLessonId > 0) {
      this._grammarService.getAllForAdminAsync().subscribe(res => {
        const allGrammars = res ?? [];
        this.grammars = allGrammars.filter(g => (g.lessonId || (g as any).LessonId) == this.selectedLessonId);

        // Cập nhật lại trạng thái kiểm tra hợp lệ sau khi dropdown đã có data ngậm vào
        this.questionForm.get('grammarId')?.updateValueAndValidity();
        this._cdr.markForCheck();
      });
    }
  }

  // Getter tiện ích giúp ngoài HTML lặp được danh sách hàng của FormArray
  get answersFormArray(): FormArray {
    return this.questionForm.get('answers') as FormArray;
  }

  /**
   * Chèn (Push) một dòng đáp án rỗng mới vào khay chứa FormArray
   */
  addAnswerRow(): void {
    const isCorrectValue = this.questionForm.get('questionType')?.value == 2; // Nếu loại 2 tự động gán true

    const row = this._fb.group({
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
   * Thay đổi Dropdown loại câu hỏi -> Đồng bộ trạng thái khóa gạt công tắc
   */
  handleQuestionTypeChange(): void {
    this.reIndexAnswers();
  }

  /**
   * Xử lý cơ chế độc quyền Single Choice (Chỉ có 1 đáp án chuẩn) khi bấm gạt công tắc
   */
  handleSwitchToggle(triggeredIndex: number): void {
    // Chỉ áp dụng logic chọn 1 đáp án cho hình thức Trắc nghiệm (Mã loại 1)
    if (this.questionForm.get('questionType')?.value == 1) {
      const currentControl = this.answersFormArray.at(triggeredIndex).get('isCorrect');

      // Nếu Admin gạt sang bật (True) -> Ép tất cả các hàng còn lại về trạng thái tắt (False)
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
   * Hàm đồng bộ chỉ mục thứ tự (DisplayOrder) và cấu hình tự động cho dạng Sắp Xếp Câu
   */
  private reIndexAnswers(): void {
    const qType = this.questionForm.get('questionType')?.value;

    this.answersFormArray.controls.forEach((control, idx) => {
      // Đánh lại số thứ tự hiển thị
      control.get('displayOrder')?.setValue((idx + 1).toString());

      // Nếu đang chọn dạng Sắp xếp câu (Mã số 2) -> Ép tất cả đáp án là Đúng
      if (qType == 2) {
        control.get('isCorrect')?.setValue(true);
      }
    });
    this._cdr.markForCheck();
  }

  /**
   * Bấm submit đẩy trọn vẹn Object JSON cấu trúc phức tạp lên API .NET Core
   */
  onSubmit(): void {
    if (this.questionForm.invalid) return;

    this.errorMessage = '';
    const formData = this.questionForm.value;

    // VALIDATION TẦNG CLIENT: Kiểm tra hình thức trắc nghiệm đã gạt chọn đáp án đúng chưa
    if (formData.questionType == 1) {
      const hasCorrect = formData.answers.some((a: any) => a.isCorrect === true);
      if (!hasCorrect) {
        alert("Vui lòng gạt chọn duy nhất 1 đáp án thành [Đúng]!");
        return;
      }
    }

    // Ép kiểu chuẩn số nguyên cho ID ngữ pháp và loại câu hỏi
    formData.grammarId = +formData.grammarId;
    formData.questionType = +formData.questionType;

    // Bắn AJAX Post gói dữ liệu JSON sang Backend C# xử lý lưu Database
    this._questionService.createQuestionAsync(formData).subscribe({
      next: (success) => {
        if (success) {
          // Lưu thành công, điều hướng quay lại trang list chính kèm bộ lọc vẹn toàn 3 tầng cũ
          this._router.navigate(['/admin/question'], {
            queryParams: {
              SelectedLevelId: this.selectedLevelId,
              SelectedLessonId: this.selectedLessonId,
              SelectedGrammarId: this.selectedGrammarId
            }
          });
        } else {
          this.errorMessage = 'API Backend .NET từ chối lưu dữ liệu. Hãy kiểm tra lại các trường ràng buộc trên DB.';
          this._cdr.markForCheck();
        }
      },
      error: () => {
        this.errorMessage = 'Lỗi kết nối máy chủ Web API. Đường truyền mạng trục trặc.';
        this._cdr.markForCheck();
      }
    });
  }
}