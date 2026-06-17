import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { firstValueFrom } from 'rxjs'; // Dùng để await các request API trong vòng lặp

import { VocabularyClientService } from '../../../../Core/Services/vocabulary-client-service';
import { VocabularyAdminModel } from '../../../../Models/AdminModel/vocabulary-admin-model';

@Component({
  selector: 'app-admin-vocabulary-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './create.html',
  styleUrls: ['./create.css']
})
export class VocabularyCreateComponent implements OnInit {
  private _vocabClientService = inject(VocabularyClientService);
  private _router = inject(Router);
  private _route = inject(ActivatedRoute);
  private _fb = inject(FormBuilder);

  // Hứng bộ lọc từ URL
  selectedLevelId = signal<number>(0);
  selectedLessonId = signal<number>(0);

  // Form gõ tay
  vocabForm: FormGroup = this._fb.group({
    kanji: [''], // Kanji có thể rỗng (ví dụ từ mượn Katakana)
    hiragana: ['', Validators.required],
    meaning: ['', Validators.required],
    romaji: ['', Validators.required],
    exampleSentence: ['']
  });

  // Quản lý file import
  selectedFile: File | null = null;

  // Quản lý trạng thái
  errorMessage = signal<string>('');
  isSubmitting = signal<boolean>(false);
  isImporting = signal<boolean>(false); // Cờ riêng cho nút Import

  ngOnInit(): void {
    // Tương đương [BindProperty(SupportsGet = true)]
    this._route.queryParams.subscribe(params => {
      if (params['selectedLevelId']) this.selectedLevelId.set(Number(params['selectedLevelId']));
      if (params['selectedLessonId']) this.selectedLessonId.set(Number(params['selectedLessonId']));
    });
  }

  // ==========================================
  // CHỨC NĂNG 1: THÊM TỪ VỰNG THỦ CÔNG
  // ==========================================
  onSubmit(): void {
    if (this.vocabForm.invalid) {
      this.vocabForm.markAllAsTouched();
      this.errorMessage.set('Vui lòng kiểm tra lại các trường dữ liệu bắt buộc màu đỏ.');
      return;
    }

    if (this.selectedLessonId() === 0) {
      this.errorMessage.set('Lỗi: Không xác định được bài học hiện tại.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const inputVocab = this.vocabForm.value as VocabularyAdminModel;
    inputVocab.lessonId = this.selectedLessonId(); // Gán LessonId ngầm

    this._vocabClientService.createVocabularyAsync(inputVocab).subscribe({
      next: (success) => {
        if (success) {
          // Bẻ lái về Index, mang theo thông báo và bộ lọc cũ
          this._router.navigate(['/admin/vocabulary'], {
            queryParams: {
              selectedLevelId: this.selectedLevelId(),
              selectedLessonId: this.selectedLessonId()
            },
            state: { successMsg: '🎉 Thêm từ vựng thành công!' }
          });
        } else {
          this.errorMessage.set('Đã xảy ra lỗi trong quá trình thêm từ vựng.');
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

  // ==========================================
  // CHỨC NĂNG 2: IMPORT TỪ FILE CSV/TXT
  // ==========================================

  // Hàm này gắn vào sự kiện (change) của thẻ <input type="file">
  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      if (file.name.endsWith('.csv') || file.name.endsWith('.txt')) {
        this.selectedFile = file;
        this.errorMessage.set(''); // Xóa lỗi nếu chọn file đúng
      } else {
        this.errorMessage.set('Chỉ chấp nhận file định dạng .csv hoặc .txt!');
        this.selectedFile = null;
        event.target.value = ''; // Reset ô chọn file
      }
    }
  }

  async onImport(): Promise<void> {
    if (!this.selectedFile) {
      this.errorMessage.set('Vui lòng chọn một file CSV/TXT hợp lệ!');
      return;
    }



    this.isImporting.set(true);
    this.errorMessage.set('');

    // Dùng FileReader để đọc nội dung file ngay trên trình duyệt
    const reader = new FileReader();

    reader.onload = async (e) => {
      try {
        const text = e.target?.result as string;
        const lines = text.split('\n'); // Tách văn bản thành từng dòng

        let successCount = 0;

        // Vòng lặp bỏ qua dòng đầu tiên (i = 1) - Tương đương dòng headerLine
        for (let i = 1; i < lines.length; i++) {
          const line = lines[i];
          if (!line || line.trim() === '') continue;

          // Tách các cột bằng dấu phẩy hoặc khoảng trắng Tab
          const values = line.split(/,|\t/);
          if (values.length < 4) continue;

          // Map dữ liệu
          const vocab = {
            id: 0,
            lessonId: this.selectedLessonId(),
            kanji: values[0]?.trim() || '',
            hiragana: values[1]?.trim() || '',
            meaning: values[2]?.trim() || '',
            romaji: values[3]?.trim() || '',
            exampleSentence: values.length > 4 ? values[4]?.trim() : '',

          } as VocabularyAdminModel;

          // Gọi API và chờ kết quả (giống await trong C#)
          await firstValueFrom(this._vocabClientService.createVocabularyAsync(vocab));
          successCount++;
        }

        // Chạy xong vòng lặp thì chuyển trang
        this._router.navigate(['/admin/vocabulary'], {
          queryParams: {
            selectedLevelId: this.selectedLevelId(),
            selectedLessonId: this.selectedLessonId()
          },
          state: { successMsg: `🎉 Import thành công ${successCount} từ vựng!` }
        });

      } catch (error: any) {
        console.error('Lỗi khi đọc file hoặc gọi API:', error);
        this.errorMessage.set('Lỗi khi xử lý file: ' + (error.message || 'Lỗi hệ thống'));
        this.isImporting.set(false);
      }
    };

    reader.onerror = () => {
      this.errorMessage.set('Không thể đọc được nội dung file.');
      this.isImporting.set(false);
    };

    // Lệnh kích hoạt quá trình đọc file dưới dạng văn bản
    reader.readAsText(this.selectedFile);
  }
}