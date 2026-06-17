import { Injectable } from '@angular/core';
import { BaseService } from './BaseService';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PracticeUserFolderModel } from '../../Models/practice-userfolder-model';
import { VocabularyModel } from '../../Models/vocabulary-model';

@Injectable({
  providedIn: 'root',
})
export class PracticeClientService extends BaseService{
  private readonly _apiBase = `${this._apiBaseUrl}/Practice`;

  constructor(private readonly _httpClient: HttpClient) {
    super();
  }

  // Các phương thức gọi API liên quan đến Practice sẽ được định nghĩa ở đây
    getSystemVocab(): Observable<VocabularyModel[]> {
    return this._httpClient.get<VocabularyModel[]>(`${this._apiBase}/practice-system`);
  }

  // Lấy danh sách thư mục cá nhân
  getUserFolders(): Observable<PracticeUserFolderModel[]> {
    return this._httpClient.get<PracticeUserFolderModel[]>(`${this._apiBase}/user-folders`);
  }

  // Lấy từ vựng trong 1 thư mục cụ thể
  getFolderVocab(FolderId: number): Observable<PracticeUserFolderModel[]> {
    return this._httpClient.get<PracticeUserFolderModel[]>(`${this._apiBase}/practice-user/${FolderId}`);
  }

  // Upload thư mục mới (có file Excel/CSV)
  uploadNewFolder(formData: FormData): Observable<any> {
    return this._httpClient.post(`${this._apiBase}/upload-folder-excel`, formData);
  }

  // Xóa thư mục
  deleteFolder(folderId: number): Observable<any> {
    return this._httpClient.delete(`${this._apiBase}/delete-folder/${folderId}`);
  }
}
