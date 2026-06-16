import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { LevelModel } from '../../Models/level-model';
import { LessonModel } from '../../Models/lesson-model';
import { BaseService } from './BaseService';
import { VocabularyModel } from '../../Models/vocabulary-model';

import { VocabularyAdminModel } from '../../Models/AdminModel/vocabulary-admin-model';
@Injectable({
    providedIn: 'root'
})
export class VocabularyClientService extends BaseService {
    private readonly _apiBase = `${this._apiBaseUrl}/vocabulary`;

    constructor(private _http: HttpClient) {
        super();
    }



    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}")
     */
    public getCardsAsync(lessonId: number): Observable<VocabularyModel[]> {
        return this._http.get<VocabularyModel[]>(`${this._apiBase}/cards?lessonId=${lessonId}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi tải thẻ từ vựng của bài học ${lessonId}:`, error);
                return of([]); // Trả về mảng rỗng nếu lỗi để tránh sập app
            })
        );
    }
    /**
     * Bắn lệnh POST tương ứng: await _http.PostAsJsonAsync($"{_apiBase}/update-LearningProgressByUser", input)
     */
    public updateProgressAsync(input: any): Observable<boolean> {
        // Bạn có thể đổi 'any' thành interface UpdateLearningProgresByUserModel nếu có
        return this._http.post(`${this._apiBase}/update-LearningProgressByUser`, input).pipe(
            map(() => true), // Trả về true nếu IsSuccessStatusCode
            catchError((error) => {
                console.error('Lỗi khi cập nhật tiến độ học:', error);
                return of(false); // Tránh sập ứng dụng Frontend khi Backend nghẽn mạch
            })
        );
    }
    // ==================== VÙNG DÀNH CHO ADMIN ====================

    /**
     * 1. Lấy danh sách từ vựng theo bài học cho Admin (thực thể gốc có ngày tháng)
     * URL tương ứng: api/vocabulary/admin/by-lesson/...
     */
    public getVocabulariesByLessonForAdminAsync(lessonId: number): Observable<VocabularyAdminModel[]> {
        return this._http.get<VocabularyAdminModel[]>(`${this._apiBase}/admin/by-lesson/${lessonId}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy danh sách từ vựng (Admin) cho bài học ${lessonId}:`, error);
                return of([]);
            })
        );
    }

    /**
     * 2. ADMIN tạo mới từ vựng
     * URL tương ứng: POST api/vocabulary/admin
     */
    public createVocabularyAsync(model: VocabularyAdminModel): Observable<boolean> {
        return this._http.post(`${this._apiBase}/admin`, model).pipe(
            map(() => true),
            catchError((error) => {
                console.error('Lỗi khi Admin tạo từ vựng:', error);
                return of(false);
            })
        );
    }

    /**
     * 3. ADMIN xóa từ vựng ra khỏi bài học
     * URL tương ứng: DELETE api/vocabulary/admin/...
     */
    public deleteVocabularyAsync(id: number): Observable<boolean> {
        return this._http.delete(`${this._apiBase}/admin/${id}`).pipe(
            map(() => true),
            catchError((error) => {
                console.error(`Lỗi khi xóa từ vựng ${id}:`, error);
                return of(false);
            })
        );
    }

    /**
     * 4. Hàm lấy chi tiết 1 từ vựng theo ID (Đổ dữ liệu cũ lên form Edit)
     * URL tương ứng: GET api/vocabulary/admin/...
     */
    public getVocabularyByIdAsync(id: number): Observable<VocabularyAdminModel | null> {
        return this._http.get<VocabularyAdminModel>(`${this._apiBase}/admin/${id}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy chi tiết từ vựng ${id}:`, error);
                return of(null); // Trả về null nếu xảy ra lỗi giống khối catch bên C#
            })
        );
    }

    /**
     * 5. ADMIN cập nhật từ vựng
     * URL tương ứng: PUT api/vocabulary/admin/...
     */
    public updateVocabularyAsync(id: number, model: VocabularyAdminModel): Observable<boolean> {
        return this._http.put(`${this._apiBase}/admin/${id}`, model).pipe(
            map(() => true),
            catchError((error) => {
                console.error(`Lỗi khi cập nhật từ vựng ${id}:`, error);
                return of(false);
            })
        );
    }
    /**
       * Gọi API Backend để xuất file PDF danh sách từ vựng
       * Trả về định dạng Blob (File nhị phân)
       */
    public downloadPdfAsync(lessonId: number): Observable<Blob> {
        return this._http.get(`${this._apiBase}/export-pdf/${lessonId}`, {
            // CỰC KỲ QUAN TRỌNG: Phải báo cho Angular biết đây là File, không phải dữ liệu JSON thông thường
            responseType: 'blob'
        });
    }
}