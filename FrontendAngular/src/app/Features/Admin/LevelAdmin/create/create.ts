import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';
import { LevelClientService } from '../../../../Core/Services/level-client-service';
@Component({
  selector: 'app-admin-level-create',
  standalone: true,
  // Cực kỳ quan trọng: Phải import ReactiveFormsModule để dùng Form
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './create.html',
  styleUrls: ['./create.css']
})
export class LevelCreateComponent {
  // Tiêm các dịch vụ (Dependency Injection)
  private _levelClientService = inject(LevelClientService);
  private _router = inject(Router);
  private _fb = inject(FormBuilder);

  // 1. Tương đương [BindProperty] InputLevel: Khai báo Form và các luật Validate
  levelForm: FormGroup = this._fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]], // Bắt buộc nhập, tối đa 50 ký tự
    description: [''] // Không bắt buộc
  });

  // 2. Tương đương [TempData] Message: Dùng Signal để quản lý trạng thái hiển thị
  errorMessage = signal<string>('');
  isSubmitting = signal<boolean>(false); // Cờ chặn người dùng bấm nút thêm nhiều lần

  // 3. Tương đương OnPostAsync()
  onSubmit(): void {
    // Tương đương kiểm tra (!ModelState.IsValid)
    if (this.levelForm.invalid) {
      // Đánh dấu tất cả các ô input là "đã chạm vào" để hiện dòng chữ báo lỗi màu đỏ trên HTML
      this.levelForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    // Lấy dữ liệu từ form
    const inputLevel = this.levelForm.value;

    this._levelClientService.createLevelAsync(inputLevel).subscribe({
      next: (success) => {
        if (success) {
          // Tương đương: return RedirectToPage("./Index");
          // Angular sẽ tự động nhảy về trang danh sách mà không cần tải lại web
          this._router.navigate(['/admin/level']);
        } else {
          // Tương đương: ModelState.AddModelError(...)
          this.errorMessage.set('Có lỗi xảy ra khi tạo mới dữ liệu.');
          this.isSubmitting.set(false);
        }
      },
      error: (err) => {
        console.error('Lỗi API:', err);
        this.errorMessage.set('Lỗi hệ thống khi kết nối tới máy chủ.');
        this.isSubmitting.set(false);
      }
    });
  }
}