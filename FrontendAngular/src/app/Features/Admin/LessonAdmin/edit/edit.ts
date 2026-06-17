import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { LessonAdminModel } from '../../../../Models/AdminModel/lesson-admin-model';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';

import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';
import { LevelModel } from '../../../../Models/level-model';
@Component({
  selector: 'app-admin-lesson-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './edit.html',
  styleUrls: ['./edit.css']
})
export class LessonEditComponent implements OnInit {
  private _lessonClientService = inject(LessonClientService);
  private _levelClientService = inject(LevelClientService);
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);
  private _fb = inject(FormBuilder);

  // Danh sách Level cho thẻ Select
  levels = signal<LevelModel[]>([]);

  // Biến lưu trữ lại bộ lọc cũ (Tương đương SelectedLevelId bên C#)
  previousSelectedLevelId = signal<number>(0);

  // Form chứa dữ liệu sửa
  lessonForm: FormGroup = this._fb.group({
    id: [0], // Bắt buộc phải có ID ẩn để gửi lệnh PUT
    levelId: ['', Validators.required],
    title: ['', [Validators.required, Validators.maxLength(255)]],
    order: [1, [Validators.required, Validators.min(1)]]
  });

  errorMessage = signal<string>('');
  isSubmitting = signal<boolean>(false);
  isLoading = signal<boolean>(true); // Hiệu ứng loading lúc nạp dữ liệu cũ

  ngOnInit(): void {
    // 1. "Bắt" cái tham số bộ lọc trên URL (?selectedLevelId=...)
    const filterParam = this._route.snapshot.queryParamMap.get('selectedLevelId');
    if (filterParam) {
      this.previousSelectedLevelId.set(Number(filterParam));
    }

    // 2. "Bắt" cái ID bài học đang cần sửa (edit/5 -> lấy số 5)
    const idParam = this._route.snapshot.paramMap.get('id');
    const lessonId = idParam ? Number(idParam) : 0;

    if (lessonId === 0) {
      this._router.navigate(['/admin/lesson']);
      return;
    }

    // 3. Tải danh sách Level
    this._levelClientService.getLevelsAsync().subscribe({
      next: (data) => this.levels.set(data),
      error: (err) => console.error('Lỗi tải danh sách Cấp độ:', err)
    });

    // 4. Tải chi tiết bài học cũ
    this._lessonClientService.getLessonByIdForAdminAsync(lessonId).subscribe({
      next: (data) => {
        if (data) {
          // Đổ dữ liệu vào form (Tương đương LessonInput = data)
          this.lessonForm.patchValue({
            id: data.id,
            levelId: data.levelId,
            title: data.title,
            order: data.order
          });
          this.isLoading.set(false); // Tắt loading
        } else {
          this.errorMessage.set('Không tìm thấy bài học cần chỉnh sửa.');
          this.isLoading.set(false);
        }
      },
      error: (err) => {
        console.error('Lỗi API lấy chi tiết:', err);
        this.errorMessage.set('Lỗi kết nối đến máy chủ.');
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.lessonForm.invalid) {
      this.lessonForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    // Ép kiểu dữ liệu an toàn
    const inputLesson: LessonAdminModel = this.lessonForm.value as LessonAdminModel;

    this._lessonClientService.updateLessonAsync(inputLesson.id, inputLesson).subscribe({
      next: (success) => {
        if (success) {
          // Thành công -> Quay về trang Index và MANG THEO bộ lọc cũ
          this._router.navigate(['/admin/lesson'], {
            // Tương đương return RedirectToPage("./Index", new { SelectedLevelId = ... })
            queryParams: { selectedLevelId: this.previousSelectedLevelId() }
          });
        } else {
          this.errorMessage.set('Có lỗi xảy ra trong quá trình cập nhật dữ liệu.');
          this.isSubmitting.set(false);
        }
      },
      error: (err) => {
        console.error('Lỗi API update:', err);
        this.errorMessage.set('Lỗi kết nối đến máy chủ.');
        this.isSubmitting.set(false);
      }
    });
  }
}