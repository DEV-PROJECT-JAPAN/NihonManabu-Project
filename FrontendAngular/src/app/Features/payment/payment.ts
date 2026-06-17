import { Component, OnInit, OnDestroy } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { PaymentClientService } from '../../Core/Services/payment-Service';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.html',
  styleUrls: ['./payment.css'] // Chứa nguyên bộ CSS cũ của anh
})
export class PaymentComponent implements OnInit, OnDestroy {
  // Biến quản lý Role hiện tại của người dùng
  currentRole: 'User' | 'VIP' | 'Admin' = 'User'; 
  userName: string = 'Thành viên';

  qrUrl: string = '';
  isLoading: boolean = false;
  isQrVisible: boolean = false;
  
  private checkInterval: any;
  private timeoutTimer: any;

  constructor(private paymentService: PaymentClientService) {}

  ngOnInit() {
    // THỰC TẾ: Lấy Role và Name từ Token JWT trong localStorage hoặc AuthService
    const token = localStorage.getItem('JWToken'); 
    if (token) {
      // Decode JWT token để lấy role (Giả lập logic)
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.currentRole = payload.role || 'User';
      this.userName = payload.unique_name || 'Thành viên';
    }
  }

  ngOnDestroy() {
    // Dọn dẹp bộ đếm khi người dùng rời khỏi trang để tránh rò rỉ bộ nhớ
    this.clearTimers();
  }

  async requestVipQr() {
    this.isLoading = true;
    
    try {
      const response = await firstValueFrom(this.paymentService.generateVipQr());
      
      if (response.success) {
        this.qrUrl = response.qrUrl;
        this.isQrVisible = true;
        this.startCheckingVipStatus();
      } else {
        alert(response.message || 'Lỗi tạo mã QR');
      }
    } catch (error) {
      alert('Lỗi kết nối đến máy chủ thanh toán.');
      console.error(error);
    } finally {
      this.isLoading = false;
    }
  }

  startCheckingVipStatus() {
    // Hỏi thăm server mỗi 3 giây
    this.checkInterval = setInterval(async () => {
      try {
        const response = await firstValueFrom(this.paymentService.checkVipStatus());
        if (response.isVip) {
          this.clearTimers();
          alert('🎉 Ting ting! Đã nhận được thanh toán. Chào mừng VIP Member!');
          
          // Cập nhật lại UI và lưu Role mới vào LocalStorage nếu cần
          this.currentRole = 'VIP';
          // window.location.reload(); // Không cần reload trang trong Angular, UI tự update
        }
      } catch (error) {
        console.log('Đang chờ thanh toán...');
      }
    }, 3000);

    // Hủy bỏ việc hỏi thăm sau 10 phút (600,000 ms)
    this.timeoutTimer = setTimeout(() => {
      this.clearTimers();
      console.log('Đã hết thời gian chờ thanh toán (10 phút).');
    }, 600000);
  }

  private clearTimers() {
    if (this.checkInterval) clearInterval(this.checkInterval);
    if (this.timeoutTimer) clearTimeout(this.timeoutTimer);
  }
}