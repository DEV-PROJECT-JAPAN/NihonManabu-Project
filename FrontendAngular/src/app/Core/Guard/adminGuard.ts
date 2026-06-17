import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth-service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Nếu là Admin thì cho phép đi tiếp
  if (authService.isAdmin()) {
    return true;
  }

  // Nếu không phải Admin, chuyển hướng về trang báo lỗi hoặc trang chủ
  alert('Bạn không có quyền truy cập vào khu vực này!');
  router.navigate(['/login']);
  return false;
};