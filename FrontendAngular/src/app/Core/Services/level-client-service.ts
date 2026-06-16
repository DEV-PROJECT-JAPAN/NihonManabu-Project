import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { LevelModel } from '../../Models/level-model';
import { catchError } from 'rxjs/operators';
import { BaseService } from './BaseService';

@Injectable({
  providedIn: 'root'
})
export class LevelClientService extends BaseService {
  // Dùng port 7104 cho đồng bộ với các service khác của bạn
  private readonly _apiBase = `${this._apiBaseUrl}/levels`;

  constructor(private _http: HttpClient) {
    super();
  }

  /**
   * Bắn lệnh GET tương ứng: await _httpClient.GetFromJsonAsync<List<LevelModel>>($"{_baseUrl}")
   * Lấy danh sách Level cơ bản cho người dùng hiển thị
   */
  public getLevelsAsync(): Observable<LevelModel[]> {
    return this._http.get<LevelModel[]>(this._apiBase).pipe(
      catchError((error) => {
        console.error('Lỗi khi tải danh sách Cấp độ:', error);
        return of([]); // Ép kiểu về mảng rỗng nếu có lỗi để giao diện không bị sập
      })
    );
  }
}