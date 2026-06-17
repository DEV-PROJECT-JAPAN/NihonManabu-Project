import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { BaseService } from './BaseService';
import { LessonModel } from '../../Models/lesson-model';
import { LessonAdminModel } from '../../Models/AdminModel/lesson-admin-model';

@Injectable({
    providedIn: 'root'
})
export class LessonClientService extends BaseService {
    // Dùng chung port với Backend .NET của bạn (giống bên LevelClientService)
    private readonly _apiBase = `${this._apiBaseUrl}/lessons`;

    constructor(private _http: HttpClient) {
        super();
    }

    // ==================== VÙNG DÀNH CHO USER ====================

    /**
     * 1. Gọi API lấy bài học cho User dựa theo LevelId
     */
    public getLessonsByLevelAsync(levelId: number): Observable<LessonModel[]> {
        return this._http.get<LessonModel[]>(`${this._apiBase}/level/${levelId}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy danh sách bài học cho Level ${levelId}:`, error);
                return of([]); // Trả về mảng rỗng nếu lỗi
            })
        );
    }

    public getLessonsAsync(levelId: number): Observable<LessonModel[]> {
        if (levelId <= 0) return of([]);
        return this._http.get<LessonModel[]>(`${this._apiBase}/lessons?levelId=${levelId}`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * Thêm hàm này vào Frontend Client Service cho User gọi
     */
    public getLessonByIdAsync(id: number): Observable<LessonModel | null> {
        return this._http.get<LessonModel>(`${this._apiBase}/admin/${id}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy chi tiết bài học ${id}:`, error);
                return of(null); // Trả về null nếu lỗi, giống khối try-catch bên C#
            })
        );
    }

    // ==================== VÙNG DÀNH CHO ADMIN ====================

    /**
     * Lấy danh sách bài học theo Level cho Admin (trả về thực thể gốc có ngày tháng)
     */
    public getLessonsByLevelForAdminAsync(levelId: number): Observable<LessonAdminModel[]> {
        return this._http.get<LessonAdminModel[]>(`${this._apiBase}/admin/by-level/${levelId}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy bài học (Admin) cho Level ${levelId}:`, error);
                return of([]);
            })
        );
    }

    /**
     * 2. Lấy toàn bộ danh sách bài học cho ADMIN
     */
    public getLessonsForAdminAsync(): Observable<LessonAdminModel[]> {
        return this._http.get<LessonAdminModel[]>(`${this._apiBase}/admin`).pipe(
            catchError((error) => {
                console.error('Lỗi khi lấy toàn bộ danh sách bài học cho Admin:', error);
                return of([]);
            })
        );
    }

    /**
     * 3. Lấy chi tiết bài học cho ADMIN
     */
    public getLessonByIdForAdminAsync(id: number): Observable<LessonAdminModel | null> {
        return this._http.get<LessonAdminModel>(`${this._apiBase}/admin/${id}`).pipe(
            catchError((error) => {
                console.error(`Lỗi khi lấy chi tiết bài học (Admin) ${id}:`, error);
                return of(null);
            })
        );
    }

    /**
     * 4. ADMIN tạo mới
     */
    public createLessonAsync(model: LessonAdminModel): Observable<boolean> {
        return this._http.post(`${this._apiBase}/admin`, model).pipe(
            map(() => true), // Thành công trả về true
            catchError((error) => {
                console.error('Lỗi khi tạo mới bài học:', error);
                return of(false); // Lỗi trả về false
            })
        );
    }

    /**
     * 5. ADMIN cập nhật
     */
    public updateLessonAsync(id: number, model: LessonAdminModel): Observable<boolean> {
        return this._http.put(`${this._apiBase}/admin/${id}`, model).pipe(
            map(() => true),
            catchError((error) => {
                console.error(`Lỗi khi cập nhật bài học ${id}:`, error);
                return of(false);
            })
        );
    }

    /**
     * 6. ADMIN xóa
     */
    public deleteLessonAsync(id: number): Observable<boolean> {
        return this._http.delete(`${this._apiBase}/admin/${id}`).pipe(
            map(() => true),
            catchError((error) => {
                console.error(`Lỗi khi xóa bài học ${id}:`, error);
                return of(false);
            })
        );
    }
}