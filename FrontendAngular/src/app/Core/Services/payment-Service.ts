import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './BaseService'; // Hoặc lấy _apiBaseUrl từ BaseService của anh

@Injectable({
  providedIn: 'root'
})
export class PaymentClientService extends BaseService {
  private readonly _apiBase = `${this._apiBaseUrl}/Practice`;

  constructor(private _httpClient: HttpClient) {
    super();
  }

  generateVipQr(): Observable<{ success: boolean, qrUrl: string, message?: string }> {
    return this._httpClient.post<any>(`${this._apiBase}/generate-vip-qr`, null);
  }

  checkVipStatus(): Observable<{ isVip: boolean }> {
    return this._httpClient.get<{ isVip: boolean }>(`${this._apiBase}/check-status`);
  }
}