import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
//service
import { BaseService } from './BaseService';
//Model
import { GrammarModel } from '../../Models/grammar-model';
import { GrammarAdminModel } from '../../Models/AdminModel/grammar-admin-model'; // Đảm bảo bạn đã tạo model này


@Injectable({
    providedIn: 'root'
})
export class GrammarClientService extends BaseService {
    // api/grammar
    private readonly _apiBase = `${this._apiBaseUrl}/grammar`;

    constructor(private _http: HttpClient) {
        super(); // Bắt buộc khi kế thừa BaseService
    }

    /**
     * Lấy chi tiết ngữ pháp theo ID cho User
     * Tương ứng C#: GetGrammarByIdAsync
     */
    public getGrammarByIdAsync(grammarId: number): Observable<GrammarModel> {
        if (grammarId <= 0) return of({} as GrammarModel);

        return this._http.get<GrammarModel>(`${this._apiBase}/${grammarId}`).pipe(
            catchError(() => of({} as GrammarModel)) // Trả về object trống nếu lỗi mạng/backend sập
        );
    }

    /**
     * Lấy danh sách ngữ pháp theo bài học (Lesson)
     * Tương ứng C#: GetGrammarByLessonAsync
     */
    public getGrammarByLessonAsync(lessonId: number): Observable<GrammarModel[]> {
        if (lessonId <= 0) return of([]);

        return this._http.get<GrammarModel[]>(`${this._apiBase}/lesson/${lessonId}`).pipe(
            catchError(() => of([])) // Trả về list trống nếu lỗi mạng
        );
    }

    /// //////////////////////////// ADMIN //////////////////////////////

    /**
     * 1. Lấy danh sách tất cả ngữ pháp cho Admin
     * Tương ứng C#: GetAllForAdminAsync
     */
    public getAllForAdminAsync(): Observable<GrammarAdminModel[]> {
        return this._http.get<GrammarAdminModel[]>(`${this._apiBase}/admin-list`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * 2. Lấy chi tiết ngữ pháp theo ID cho Admin
     * Tương ứng C#: GetByIdForAdminAsync
     */
    public getByIdForAdminAsync(id: number): Observable<GrammarAdminModel | null> {
        return this._http.get<GrammarAdminModel>(`${this._apiBase}/admin/${id}`).pipe(
            catchError(() => of(null)) // Trả về null nếu xảy ra lỗi giống C#
        );
    }


    /**
     * 3. Thêm mới mẫu ngữ pháp
     * Tương ứng C#: CreateAsync
     */
    public createAsync(grammar: GrammarAdminModel): Observable<boolean> {
        return this._http.post(`${this._apiBase}`, grammar).pipe(
            map(() => true),             // Nếu thành công (2xx), ép kết quả về true
            catchError(() => of(false))  // Nếu lỗi mạng/backend sập, trả về false
        );
    }

    /**
     * 4. Cập nhật mẫu ngữ pháp
     * Tương ứng C#: UpdateAsync
     */
    public updateAsync(id: number, grammar: GrammarAdminModel): Observable<boolean> {
        return this._http.put(`${this._apiBase}/${id}`, grammar).pipe(
            map(() => true),
            catchError(() => of(false))
        );
    }

    /**
     * 5. Xóa mẫu ngữ pháp
     * Tương ứng C#: DeleteAsync
     */
    public deleteAsync(id: number): Observable<boolean> {
        return this._http.delete(`${this._apiBase}/${id}`).pipe(
            map(() => true),
            catchError(() => of(false))
        );
    }
}