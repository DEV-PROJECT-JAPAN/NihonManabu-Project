import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { LessonAdminModel } from '../../../../Models/AdminModel/lesson-admin-model';
import { LessonClientService } from '../../../../Core/Services/lesson-client-service';

import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';
import { LevelModel } from '../../../../Models/level-model';
@Component({
  selector: 'app-admin-lesson-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './create.html',
  styleUrls: ['./create.css']
})
export class LessonCreateComponent implements OnInit {
  // Tiêm các Service cần thiết
  private _lessonClientService = inject(LessonClientService);
  private _levelClientService = inject(LevelClientService);
  private _router = inject(Router);
  private _fb = inject(FormBuilder);

  // 1. Tương đương public List<LevelModel> Levels: Chứa dữ liệu đổ vào thẻ <select>
  levels = signal<LevelModel[]>([]);

  // 2. Tương đương [BindProperty] LessonInput: Khuôn mẫu của Form
  lessonForm: FormGroup = this._fb.group({
    levelId: ['', Validators.required], // Bắt buộc phải chọn 1 cấp độ
    title: ['', [Validators.required, Validators.maxLength(255)]], // Tên bài học
    order: [1, [Validators.required, Validators.min(1)]] // Thứ tự (mặc định là 1)
  });

  // Quản lý thông báo và trạng thái nút bấm
  errorMessage = signal<string>('');
  isSubmitting = signal<boolean>(false);

  // 3. Tương đương OnGetAsync()
  ngOnInit(): void {
    // Gọi API nạp danh sách Level (N1 -> N5) ngay khi vừa mở trang
    this._levelClientService.getLevelsAsync().subscribe({
      next: (data) => this.levels.set(data),
      error: (err) => {
        console.error('Lỗi tải danh sách Cấp độ:', err);
        this.errorMessage.set('Không thể tải danh sách cấp độ để chọn.');
      }
    });
  }

  // 4. Tương đương OnPostAsync()
  onSubmit(): void {
    // Tương đương !ModelState.IsValid
    if (this.lessonForm.invalid) {
      this.lessonForm.markAllAsTouched();
      return;
      // 💡 Điểm hay của Angular: Form lỗi thì đứng im báo đỏ tại chỗ, 
      // không cần phải gọi lại API lấy Levels như dòng `Levels = await...` bên C#!
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const inputLesson = this.lessonForm.value;

    this._lessonClientService.createLessonAsync(inputLesson).subscribe({
      next: (success) => {
        if (success) {
          // Thành công -> Bẻ lái về trang Index
          this._router.navigate(['/admin/lesson']);
        } else {
          this.errorMessage.set('Có lỗi xảy ra trong quá trình lưu dữ liệu vào hệ thống.');
          this.isSubmitting.set(false);
        }
      },
      error: (err) => {
        console.error('Lỗi API:', err);
        this.errorMessage.set('Lỗi kết nối đến máy chủ.');
        this.isSubmitting.set(false);
      }
    });
  }
}