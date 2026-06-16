import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LevelModel } from '../../Models/level-model';
import { LessonModel } from '../../Models/lesson-model';
import { BaseService } from './BaseService';

@Injectable({
    providedIn: 'root'
})
export class VocabularyClientService extends BaseService {
    private readonly _apiBase = `${this._apiBaseUrl}/vocabulary`;

    constructor(private _http: HttpClient) {
        super();
    }

    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<List<LevelModel>>($"{_apiBase}/levels")
     */
    public getLevelsAsync(): Observable<LevelModel[]> {
        return this._http.get<LevelModel[]>(`${this._apiBase}/levels`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<List<LessonModel>>($"{_apiBase}/lessons?levelId={levelId}")
     * Đã sửa chuẩn Query String để triệt hạ lỗi 400 Bad Request
     */
    public getLessonsAsync(levelId: number): Observable<LessonModel[]> {
        if (levelId <= 0) return of([]);
        return this._http.get<LessonModel[]>(`${this._apiBase}/lessons?levelId=${levelId}`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<List<VocabularyModel>>($"{_apiBase}/cards?lessonId={lessonId}")
     */
    public getCardsAsync(lessonId: number): Observable<any[]> {
        if (lessonId <= 0) return of([]);
        return this._http.get<any[]>(`${this._apiBase}/cards?lessonId=${lessonId}`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * Bắn lệnh POST tương ứng: await _http.PostAsJsonAsync($"{_apiBase}/update-LearningProgressByUser", input)
     */
    public updateProgressAsync(input: any): Observable<boolean> {
        return this._http.post<boolean>(`${this._apiBase}/update-LearningProgressByUser`, input).pipe(
            catchError(() => of(false))
        );
    }
}