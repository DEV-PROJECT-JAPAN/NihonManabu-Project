import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { VocabularyClientService } from '../../../../Core/Services/vocabulary-client-service';
import { VocabularyAdminModel } from '../../../../Models/AdminModel/vocabulary-admin-model';

@Component({
    selector: 'app-admin-vocabulary-edit',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    templateUrl: './edit.html',
    styleUrls: ['./edit.css']
})
export class VocabularyEditComponent implements OnInit {
    private _vocabClientService = inject(VocabularyClientService);
    private _router = inject(Router);
    private _route = inject(ActivatedRoute);
    private _fb = inject(FormBuilder);

    // Biến lưu trữ lại bộ lọc cũ để quay về đúng bài (Tương đương SupportsGet = true)
    selectedLevelId = signal<number>(0);
    selectedLessonId = signal<number>(0);

    // Form chứa dữ liệu sửa
    vocabForm: FormGroup = this._fb.group({
        id: [0], // Phải giữ lại ID ẩn để gửi lệnh PUT
        lessonId: [0], // Giữ lại Bài học gốc
        kanji: [''],
        hiragana: ['', Validators.required],
        meaning: ['', Validators.required],
        romaji: ['', Validators.required],
        exampleSentence: ['']
    });

    // Quản lý trạng thái giao diện
    errorMessage = signal<string>('');
    isSubmitting = signal<boolean>(false);
    isLoading = signal<boolean>(true); // Hiệu ứng loading lúc nạp dữ liệu cũ từ API

    // 1. TỰ ĐỘNG ĐỔ DỮ LIỆU CŨ LÊN FORM KHI VỪA VÀO TRANG (Tương đương OnGetAsync)
    ngOnInit(): void {
        // A. "Bắt" bộ lọc trên URL (?selectedLevelId=...&selectedLessonId=...)
        this._route.queryParams.subscribe(params => {
            if (params['selectedLevelId']) this.selectedLevelId.set(Number(params['selectedLevelId']));
            if (params['selectedLessonId']) this.selectedLessonId.set(Number(params['selectedLessonId']));
        });

        // B. "Bắt" ID từ vựng đang cần sửa (Ví dụ: edit/15 -> lấy số 15)
        const idParam = this._route.snapshot.paramMap.get('id');
        const vocabId = idParam ? Number(idParam) : 0;

        if (vocabId === 0) {
            this._router.navigate(['/admin/vocabulary']);
            return;
        }

        // C. Gọi API Backend lấy chi tiết
        this._vocabClientService.getVocabularyByIdAsync(vocabId).subscribe({
            next: (data) => {
                if (data) {
                    // Điền dữ liệu bốc được vào Form
                    this.vocabForm.patchValue({
                        id: data.id,
                        lessonId: data.lessonId,
                        kanji: data.kanji,
                        hiragana: data.hiragana,
                        meaning: data.meaning,
                        romaji: data.romaji,
                        exampleSentence: data.exampleSentence
                    });
                    this.isLoading.set(false); // Tắt cờ loading, hiện form ra
                } else {
                    this.errorMessage.set('Không tìm thấy từ vựng này trong hệ thống.');
                    this.isLoading.set(false);
                }
            },
            error: (err) => {
                console.error('Lỗi API lấy chi tiết từ vựng:', err);
                this.errorMessage.set('Lỗi kết nối đến máy chủ khi tải dữ liệu.');
                this.isLoading.set(false);
            }
        });
    }

    // 2. XỬ LÝ LƯU DỮ LIỆU KHI BẤM NÚT CẬP NHẬT (Tương đương OnPostAsync)
    onSubmit(): void {
        if (this.vocabForm.invalid) {
            this.vocabForm.markAllAsTouched();
            this.errorMessage.set('Vui lòng kiểm tra lại các trường dữ liệu bắt buộc màu đỏ.');
            return;
        }

        this.isSubmitting.set(true);
        this.errorMessage.set('');

        // Dùng kỹ thuật 'as' để ép kiểu lỏng lẻo, bỏ qua việc ép buộc khai báo ngày tháng y như file Create
        const inputVocab = this.vocabForm.value as VocabularyAdminModel;

        // Gọi API Update đè dữ liệu mới lên bản ghi cũ
        this._vocabClientService.updateVocabularyAsync(inputVocab.id, inputVocab).subscribe({
            next: (success) => {
                if (success) {
                    // Quay về trang danh sách và dắt theo bộ lọc + TempData
                    this._router.navigate(['/admin/vocabulary'], {
                        queryParams: {
                            selectedLevelId: this.selectedLevelId(),
                            selectedLessonId: this.selectedLessonId()
                        },
                        state: { successMsg: '✅ Cập nhật từ vựng thành công!' }
                    });
                } else {
                    this.errorMessage.set('Đã xảy ra lỗi trong quá trình cập nhật từ vựng.');
                    this.isSubmitting.set(false);
                }
            },
            error: (err) => {
                console.error('Lỗi API Update:', err);
                this.errorMessage.set('Lỗi hệ thống khi lưu dữ liệu.');
                this.isSubmitting.set(false);
            }
        });
    }
}