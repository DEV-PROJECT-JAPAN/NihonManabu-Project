import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { LevelModel } from '../../Models/level-model';
import { BaseService } from './BaseService';
import { catchError, map } from 'rxjs/operators';
import { LevelAdminModel } from '../../Models/AdminModel/level-admin-model';

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



  /**
     * 1. Lấy danh sách cho ADMIN (Trả về LevelAdminModel)
     */
  public getLevelsForAdminAsync(): Observable<any[]> { // Hãy đổi 'any' thành 'LevelAdminModel' nếu bạn đã tạo interface
    return this._http.get<any[]>(`${this._apiBase}/admin`).pipe(
      catchError((error) => {
        console.error('Lỗi khi tải danh sách Cấp độ cho Admin:', error);
        return of([]);
      })
    );
  }

  /**
   * 2. Lấy chi tiết cho ADMIN
   */
  public getLevelByIdForAdminAsync(id: number): Observable<any | null> { // Hãy đổi 'any' thành 'LevelAdminModel'
    return this._http.get<any>(`${this._apiBase}/admin/${id}`).pipe(
      catchError((error) => {
        console.error(`Lỗi khi tải chi tiết Cấp độ ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * 3. ADMIN tạo mới (Trả về true nếu thành công, false nếu lỗi)
   */
  public createLevelAsync(model: any): Observable<boolean> { // Hãy đổi 'any' thành 'LevelAdminModel'
    return this._http.post(`${this._apiBase}/admin`, model).pipe(
      map(() => true), // Nếu chạy lọt qua dòng này tức là HTTP 200 OK -> Thành công
      catchError((error) => {
        console.error('Lỗi khi tạo Cấp độ:', error);
        return of(false); // Bắt lỗi và trả về false giống IsSuccessStatusCode
      })
    );
  }

  /**
   * 4. ADMIN cập nhật (Trả về true nếu thành công, false nếu lỗi)
   */
  public updateLevelAsync(id: number, model: any): Observable<boolean> { // Hãy đổi 'any' thành 'LevelAdminModel'
    return this._http.put(`${this._apiBase}/admin/${id}`, model).pipe(
      map(() => true),
      catchError((error) => {
        console.error(`Lỗi khi cập nhật Cấp độ ${id}:`, error);
        return of(false);
      })
    );
  }

  /**
   * 5. ADMIN xóa (Trả về true nếu thành công, false nếu lỗi)
   */
  public deleteLevelAsync(id: number): Observable<boolean> {
    return this._http.delete(`${this._apiBase}/admin/${id}`).pipe(
      map(() => true),
      catchError((error) => {
        console.error(`Lỗi khi xóa Cấp độ ${id}:`, error);
        return of(false);
      })
    );
  }
}