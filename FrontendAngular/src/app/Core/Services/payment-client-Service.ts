import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './BaseService'; // Hoặc lấy _apiBaseUrl từ BaseService của anh

@Injectable({
  providedIn: 'root'
})
export class PaymentClientService extends BaseService {
  private readonly _apiBase = `${this._apiBaseUrl}/Payment`;

  constructor(private _httpClient: HttpClient) {
    super();
  }

  // Hàm tự động lấy Token và tạo Header chứa thẻ ưu tiên (Bearer)
  private getAuthHeaders(): HttpHeaders {
    // LƯU Ý: Anh kiểm tra xem lúc Login, anh lưu Token vào localStorage dưới tên gì nhé. 
    // Em đang giả sử là 'JWToken', nếu anh đặt là 'token' thì sửa lại.
    const token = localStorage.getItem('JWToken'); 
    
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  generateVipQr(): Observable<{ success: boolean, qrUrl: string, message?: string }> {
    // Nhét headers vào param thứ 3 của hàm post
    return this._httpClient.post<any>(`${this._apiBase}/generate-vip-qr`, null, { 
      headers: this.getAuthHeaders() 
    });
  }

  checkVipStatus(): Observable<{ isVip: boolean }> {
    // Nhét headers vào param thứ 2 của hàm get
    return this._httpClient.get<{ isVip: boolean }>(`${this._apiBase}/check-status`, { 
      headers: this.getAuthHeaders() 
    });
  }
}