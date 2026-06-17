import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { LevelClientService } from '../../../../Core/Services/level-client-service';
import { LevelAdminModel } from '../../../../Models/AdminModel/level-admin-model';

@Component({
  selector: 'app-admin-level-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './edit.html', // Trỏ tới file HTML của bạn
  styleUrls: ['./edit.css']
})
export class LevelEditComponent implements OnInit {
  private _levelClientService = inject(LevelClientService);
  private _router = inject(Router);
  private _route = inject(ActivatedRoute); // Dịch vụ mới: Dùng để đọc URL
  private _fb = inject(FormBuilder);

  // Form chứa dữ liệu sửa (giống hệt bên Create nhưng có thêm ID)
  levelForm: FormGroup = this._fb.group({
    id: [0], // Phải giữ lại ID để biết đang sửa thằng nào
    name: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['']
  });

  errorMessage = signal<string>('');
  isSubmitting = signal<boolean>(false);
  isLoading = signal<boolean>(true); // Cờ tạo hiệu ứng loading lúc mới vào trang

  // Tương đương OnGetAsync(int id) bên C#
  ngOnInit(): void {
    // 1. "Bắt" cái số ID trên đường link (VD: /admin/level/edit/5 -> lấy số 5)
    const idParam = this._route.snapshot.paramMap.get('id');
    const levelId = idParam ? Number(idParam) : 0;

    if (levelId === 0) {
      // Nếu không có ID, đá về trang danh sách (Tương đương RedirectToPage)
      this._router.navigate(['/admin/level']);
      return;
    }

    // 2. Gọi API lấy dữ liệu cũ
    this._levelClientService.getLevelByIdForAdminAsync(levelId).subscribe({
      next: (data) => {
        if (data) {
          // Đổ dữ liệu cũ vào Form (Tương đương InputLevel = level)
          this.levelForm.patchValue({
            id: data.id,
            name: data.name,
            description: data.description
          });
          this.isLoading.set(false); // Tắt hiệu ứng loading
        } else {
          this._router.navigate(['/admin/level']);
        }
      },
      error: (err) => {
        console.error('Lỗi tải dữ liệu:', err);
        this.errorMessage.set('Không thể tải dữ liệu cấp độ.');
        this.isLoading.set(false);
      }
    });
  }

  // Tương đương OnPostAsync() bên C#
  onSubmit(): void {
    if (this.levelForm.invalid) {
      this.levelForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const updatedLevel = this.levelForm.value;

    // Gọi API cập nhật
    this._levelClientService.updateLevelAsync(updatedLevel.id, updatedLevel).subscribe({
      next: (success) => {
        if (success) {
          // Thành công -> Quay về danh sách
          this._router.navigate(['/admin/level']);
        } else {
          this.errorMessage.set('Có lỗi xảy ra khi cập nhật dữ liệu.');
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