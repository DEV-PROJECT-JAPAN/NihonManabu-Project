import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GrammarModel } from '../../Models/grammar-model';
import { QuestionModel } from '../../Models/question-model';




@Injectable({
    providedIn: 'root'
})
export class GrammarClientService {
    private readonly _apiBase = 'https://localhost:7104/api/grammar';

    constructor(private _http: HttpClient) { }

    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<GrammarModel>($"{_apiBase}/{grammarId}")
     */
    public getGrammarByIdAsync(grammarId: number): Observable<GrammarModel> {
        if (grammarId <= 0) return of({} as GrammarModel);
        return this._http.get<GrammarModel>(`${this._apiBase}/${grammarId}`).pipe(
            catchError(() => of({} as GrammarModel))
        );
    }

    /**
     * Bắn lệnh GET tương ứng: await _http.GetFromJsonAsync<List<GrammarModel>>($"{_apiBase}/lesson/{lessonId}")
     */
    public getGrammarByLessonAsync(lessonId: number): Observable<GrammarModel[]> {
        if (lessonId <= 0) return of([]);
        return this._http.get<GrammarModel[]>(`${this._apiBase}/lesson/${lessonId}`).pipe(
            catchError(() => of([]))
        );
    }

    /**
     * Bắn URL dạng tham số query: /api/grammar/questions?grammarId=1&questionType=1
     */
    public getQuestionsByGrammarAsync(grammarId: number, questionType: number = 0): Observable<QuestionModel[]> {
        if (grammarId <= 0) return of([]);
        const url = `${this._apiBase}/questions?grammarId=${grammarId}&questionType=${questionType}`;
        return this._http.get<QuestionModel[]>(url).pipe(
            catchError(() => of([]))
        );
    }
}