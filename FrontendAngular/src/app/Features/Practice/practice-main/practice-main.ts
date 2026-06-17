// import { Component } from '@angular/core';
// import { PracticeUserFolderModel } from '../../../Models/practice-uservocabularyfolder-model';
// import { PracticeClientService } from '../../../Core/Services/practice-client-service';
// import { PracticeGachavocabulary } from '../practice-gachavocabulary/practice-gachavocabulary';
// import { PracticeMenu } from '../practice-menu/practice-menu';
// import { VocabularyModel } from '../../../Models/vocabulary-model';

// @Component({
//   selector: 'app-practice-main',
//   imports: [PracticeMenu, PracticeGachavocabulary],
//   templateUrl: './practice-main.html',
//   styleUrl: './practice-main.css',
// })
// export class PracticeMain {
// // Trạng thái hiện tại của màn hình
//   currentView: 'menu' | 'system' | 'folders' | 'folder-gacha' = 'menu';
//   pageSubtitle: string = 'Chọn phương thức học của bạn';
  
//   // Dữ liệu dùng chung
//   activeVocabList: VocabularyModel[] = [];
//   activeTitle: string = '';
//   isLoadingGacha: boolean = false;

//   constructor(private practiceclientService: PracticeClientService) {}

//   // Xử lý khi Con (MainMenu) báo người dùng chọn Hệ Thống
//   openSystemVocab() {
//       this.currentView = 'system';
//       this.pageSubtitle = '✨ Gacha Random Words ✨';
//       this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
    
//   }

//   // Xử lý khi Con (MainMenu) báo người dùng chọn Thư mục
//   openFoldersView() {
//     this.currentView = 'folders';
//     this.pageSubtitle = '📂 Quản lý từ vựng cá nhân';
//     // Logic tải danh sách thư mục...
//   }

//   // Quay lại menu chính
//   goToMenu() {
//     this.currentView = 'menu';
//     this.pageSubtitle = 'Chọn phương thức học của bạn';
//     this.activeVocabList = []; // Xóa data cũ
//   }
// }


import { Component, OnInit } from '@angular/core';
import { firstValueFrom, lastValueFrom } from 'rxjs'; // Dùng để chuyển Observable thành Promise
import { PracticeUserFolderModel } from '../../../Models/practice-uservocabularyfolder-model';
import { PracticeClientService } from '../../../Core/Services/practice-client-service';
import { PracticeGachavocabulary } from '../practice-gachavocabulary/practice-gachavocabulary';
import { PracticeMenu } from '../practice-menu/practice-menu';
import { VocabularyModel } from '../../../Models/vocabulary-model';

@Component({
  selector: 'app-practice-main',
  imports: [PracticeMenu, PracticeGachavocabulary],
  templateUrl: './practice-main.html',
  styleUrl: './practice-main.css',
})
export class PracticeMain implements OnInit {
  currentView: 'menu' | 'system' | 'folders' | 'folder-gacha' = 'menu';
  pageSubtitle: string = 'Chọn phương thức học của bạn';
  
  activeVocabList: VocabularyModel[] = [];
  activeTitle: string = '';
  isLoadingGacha: boolean = false;

  constructor(private practiceclientService: PracticeClientService) {}

  // 1. TẢI NGẦM NGAY KHI VÀO TRANG MENU
  ngOnInit() {
    this.practiceclientService.getSystemVocab().subscribe(data => {
      this.activeVocabList = data; // Cất sẵn vào kho, đợi user bấm
    });
  }

  // 2. MỞ RA LÀ CÓ NGAY LIỀN, KHÔNG CẦN GỌI API NỮA
  openSystemVocab() {
      this.currentView = 'system';
      this.pageSubtitle = '✨ Gacha Random Words ✨';
      this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
  }

  // 3. HÀM QUAY GACHA KHI BẤM DRAW AGAIN
  async rollNewCards() {
    this.isLoadingGacha = true; // Bật màn hình Loading

    try {
      // Ép chờ đúng 2 giây (2000ms)
      const timerPromise = new Promise(resolve => setTimeout(resolve, 2000));
      // Gọi API xuống Backend lấy 5 từ mới
      const apiPromise = firstValueFrom(this.practiceclientService.getSystemVocab());

      // Promise.all bắt buộc chờ cả API và đồng hồ 2s đếm xong
      const [newData] = await Promise.all([apiPromise, timerPromise]);

      // Nạp đạn mới
      this.activeVocabList = newData;
    } catch (error) {
      console.error('Lỗi khi roll data:', error);
    } finally {
      this.isLoadingGacha = false; // Tắt Loading
    }
  }

  openFoldersView() {
    this.currentView = 'folders';
    this.pageSubtitle = '📂 Quản lý từ vựng cá nhân';
  }

  goToMenu() {
    this.currentView = 'menu';
    this.pageSubtitle = 'Chọn phương thức học của bạn';
    this.activeVocabList = []; 
    // Ghi chú: Nếu quay lại menu mà muốn tải trước mẻ mới, 
    // anh có thể gọi lại hàm getSystemVocab ở đây.
  }
}