import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs'; // ◄ Dùng Observable và of từ RxJS
import { catchError, map } from 'rxjs/operators'; // ◄ Các toán tử bắt lỗi và biến đổi dữ liệu
import { BaseService } from './BaseService';
import { QuestionModel } from '../../Models/question-model';
import { QuestionAdminModel } from '../../Models/AdminModel/question-admin-model';

@Injectable({
    providedIn: 'root'
})
export class QuestionClientService extends BaseService {

    // 🌟 Định nghĩa chuẩn Endpoint gốc cho controller QuestionAdmins
    private readonly _apiBase = `${this._apiBaseUrl}/QuestionAdmin`;

    constructor(private _httpClient: HttpClient) {
        super(); // Gọi constructor của lớp cha BaseService
    }

    /**
     * 🔐 1. [ADMIN] Gọi API lấy danh sách câu hỏi theo bài học dạng đầy đủ Model gốc
     */
    public getQuestionsByLessonForAdminAsync(lessonId: number): Observable<QuestionAdminModel[]> {
        if (lessonId <= 0) return of([]);

        return this._httpClient.get<QuestionAdminModel[]>(`${this._apiBase}/admin/lesson/${lessonId}`).pipe(
            catchError(() => of([])) // Nếu lỗi, trả về mảng rỗng
        );
    }

    /**
     * 🌐 2. [USER] Gọi API lấy danh sách câu hỏi theo bài học dạng rút gọn
     */
    public getQuestionsByLessonForUserAsync(lessonId: number): Observable<QuestionModel[]> {
        if (lessonId <= 0) return of([]);

        return this._httpClient.get<QuestionModel[]>(`${this._apiBase}/user/lesson/${lessonId}`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * 🔍 3. [CHUNG] Gọi API lấy chi tiết một câu hỏi theo ID hệ thống
     */
    public getQuestionByIdAsync(id: number): Observable<QuestionAdminModel | null> {
        return this._httpClient.get<QuestionAdminModel>(`${this._apiBase}/${id}`).pipe(
            catchError(() => of(null)) // Nếu lỗi, trả về null để tránh sập app
        );
    }

    /**
     * ➕ 4. [ADMIN] Gửi Request thêm mới toàn bộ cụm dữ liệu câu hỏi lồng kèm đáp án
     */
    public createQuestionAsync(question: QuestionAdminModel): Observable<boolean> {
        return this._httpClient.post(`${this._apiBase}`, question, { observe: 'response' }).pipe(
            map(response => response.ok), // Nếu status code dạng 2xx (Thành công) -> trả về true
            catchError(() => of(false))   // Nếu lỗi HTTP -> trả về false
        );
    }

    /**
     * ✏️ 5. [ADMIN] Gửi Request cập nhật sửa đổi, đồng bộ mảng câu trả lời Answers
     */
    public updateQuestionAsync(id: number, question: QuestionAdminModel): Observable<boolean> {
        return this._httpClient.put(`${this._apiBase}/${id}`, question, { observe: 'response' }).pipe(
            map(response => response.ok),
            catchError(() => of(false))
        );
    }

    /**
     * ❌ 6. [ADMIN] Gửi lệnh xóa câu hỏi vĩnh viễn khỏi hệ thống database
     */
    public deleteQuestionAsync(id: number): Observable<boolean> {
        return this._httpClient.delete(`${this._apiBase}/${id}`, { observe: 'response' }).pipe(
            map(response => response.ok),
            catchError(() => of(false))
        );
    }

    /**
     * 🌾 7. Bắn URL dạng tham số query: /questions?grammarId=1&questionType=1
     */
    public getQuestionsByGrammarAsync(grammarId: number, questionType: number = 0): Observable<QuestionModel[]> {
        if (grammarId <= 0) return of([]);

        const url = `${this._apiBase}/questions?grammarId=${grammarId}&questionType=${questionType}`;
        return this._httpClient.get<QuestionModel[]>(url).pipe(
            catchError(() => of([]))
        );
    }
}