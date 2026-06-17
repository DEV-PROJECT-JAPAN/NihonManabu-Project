// import { Component, OnInit } from '@angular/core';
// import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
// import { PracticeClientService } from '../../../Core/Services/practice-client-service';
// import { PracticeGachavocabulary } from '../practice-gachavocabulary/practice-gachavocabulary';
// import { PracticeMenu } from '../practice-menu/practice-menu';
// import { VocabularyModel } from '../../../Models/vocabulary-model';
// import { PracticeUserfolder } from '../practice-userfolder/practice-userfolder';

// @Component({
//   selector: 'app-practice-main',
//   imports: [PracticeMenu, PracticeGachavocabulary, PracticeUserfolder],
//   templateUrl: './practice-main.html',
//   styleUrl: './practice-main.css',
// })
// export class PracticeMain implements OnInit { // Nhớ implements OnInit
//   // Trạng thái hiện tại của màn hình
//   currentView: 'menu' | 'system' | 'folders' | 'folder-gacha' = 'menu';
//   pageSubtitle: string = 'Chọn phương thức học của bạn';
  
//   // Dữ liệu dùng chung
//   activeSystemVocabList: VocabularyModel[] = []; // Dùng chung cho cả màn Gacha System và Folder
//   activeFolderList: PracticeUserFolderModel[] = [];
//   activeTitle: string = '';
//   isLoadingGacha: boolean = false;
  
//   // BIẾN MỚI: Lưu ID của folder đang được chọn để chơi
//   currentFolderId: number = 0; 

//   constructor(private practiceclientService: PracticeClientService) {}

//   ngOnInit() {
//     // Tải sẵn data Hệ Thống ngay khi vào trang, để mở ra là có luôn
//     this.practiceclientService.getSystemVocab().subscribe(data => {
//       this.activeSystemVocabList = data; // Cất sẵn vào kho, đợi user bấm
//     });
//     this.practiceclientService.getUserFolders().subscribe(data => {
//       this.activeFolderList = data; // Cất sẵn vào kho, đợi user bấm
//     });
//   }

//   // Xử lý khi Con (MainMenu) báo người dùng chọn Hệ Thống
//   openSystemVocab() {
//     this.currentView = 'system';
//     this.pageSubtitle = '✨ Gacha Random Words ✨';
//     this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
//   }

//   openFolderVocab() {
//     this.currentView = 'folders';
//     this.pageSubtitle = '✨ Gacha Random Words ✨';
//     this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
//   }

//   // Xử lý khi Con (MainMenu) báo người dùng chọn Thư mục
//   openFoldersView() {
//     this.currentView = 'folders';
//     this.pageSubtitle = '📂 Quản lý từ vựng cá nhân';
//   }

//   // ==========================================
//   // LOGIC MỚI: XỬ LÝ FOLDER & GACHA
//   // ==========================================

//   // Khi click vào 1 Folder cụ thể
//   handleOpenFolder(folderInfo: { id: number, name: string }) {
//     this.currentFolderId = folderInfo.id; // Nhớ ID folder đang chơi
//     folderInfo.name = this.activeFolderList.find(f => f.id === folderInfo.id)?.name || 'Thư Mục';
//     this.activeTitle = `✨ THƯ MỤC: ${folderInfo.name.toUpperCase()} ✨`;
//     this.currentView = 'folder-gacha';
//     this.pageSubtitle = '✨ Luyện Tập Gacha ✨';
//   }

//   // Xử lý Xóa Folder
//   handleDeleteFolder(folderId: number) {
//     // Chỗ này gọi API Delete Folder
//     this.practiceclientService.deleteFolder(folderId).subscribe(() => {
//       // Sau khi xóa thành công, refresh lại danh sách folder để folder vừa xóa biến mất
//       this.practiceclientService.getUserFolders().subscribe(data => {
//         this.activeFolderList = data;
//       });
//     });
//   }

//   // Xử lý Upload Form Data
//   handleUploadFolder(formData: FormData) {
//     // Chỗ này gọi API Upload Folder
//     this.practiceclientService.uploadNewFolder(formData).subscribe(() => {
//       // Sau khi upload thành công, refresh lại danh sách folder để có folder mới hiện lên
//       this.practiceclientService.getUserFolders().subscribe(data => {
//         this.activeFolderList = data;
//       });
//     });
//   }

//   // ==========================================
//   // QUAY LẠI MENU
//   // ==========================================
//   goToMenu() {
//     this.currentView = 'menu';
//     this.pageSubtitle = 'Chọn phương thức học của bạn';
    
//     // Refresh lại data cho nóng (như anh đã viết)
//     this.practiceclientService.getSystemVocab().subscribe(data => {
//       this.activeSystemVocabList = data; 
//     }); 
//     this.practiceclientService.getUserFolders().subscribe(data => {
//       this.activeFolderList = data; 
//     });
//   }
// }


import { Component, OnInit } from '@angular/core';
import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
import { PracticeClientService } from '../../../Core/Services/practice-client-service';
import { PracticeGachavocabulary } from '../practice-gachavocabulary/practice-gachavocabulary';
import { PracticeMenu } from '../practice-menu/practice-menu';
import { VocabularyModel } from '../../../Models/vocabulary-model';
import { PracticeUserfolder } from '../practice-userfolder/practice-userfolder';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-practice-main',
  imports: [PracticeMenu, PracticeGachavocabulary, PracticeUserfolder],
  templateUrl: './practice-main.html',
  styleUrl: './practice-main.css',
})
export class PracticeMain implements OnInit { 
  currentView: 'menu' | 'system' | 'folders' | 'folder-gacha' = 'menu';
  pageSubtitle: string = 'Chọn phương thức học của bạn';
  
  activeSystemVocabList: VocabularyModel[] = []; // Kho tải ngầm cho Hệ thống
  activeFolderVocabList: PracticeUserFolderModel[] = []; // Kho tải ngầm cho Thư mục đang chơi
  activeFolderList: PracticeUserFolderModel[] = []; 
  
  // BIẾN MỚI: Biến trung gian chỉ dùng để bưng bê từ vựng đổ vào Component Gacha
  // currentSystemGachaList: VocabularyModel[] = []; 
  // currentFolderGachaList: PracticeGachavocabulary[] = [];
  
  activeTitle: string = '';
  currentFolderId: number = 0; 

  constructor(private practiceclientService: PracticeClientService) {}

  ngOnInit() {
    this.practiceclientService.getSystemVocab().subscribe(data => {
      this.activeSystemVocabList = data; 
    });
    this.practiceclientService.getUserFolders().subscribe(data => {
      this.activeFolderList = data; 
    });
  }

  // MỞ HỆ THỐNG
  openSystemVocab() {
    this.currentView = 'system';
    this.pageSubtitle = '✨ Gacha Random Words ✨';
    this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
    
    // Gán cục data tải ngầm vào mâm Gacha
    this.practiceclientService.getSystemVocab().subscribe({
      next: (data) => {
        this.activeSystemVocabList = data; // Đổ list từ vựng mới lấy được lên mâm Gacha
      },
      error: (err) => {
        console.error('Lỗi khi lấy từ vựng thư mục:', err);
      }
    })
  }

  // MỞ KỆ THƯ MỤC
  openFoldersView() {
    this.currentView = 'folders';
    this.pageSubtitle = '📂 Quản lý từ vựng cá nhân';
  }

  // ==========================================
  // LOGIC CLICK VÀO 1 THƯ MỤC ĐỂ CHƠI
  // ==========================================
  // handleOpenFolder(folderInfo: { id: number, name: string }) {
  //   this.currentFolderId = folderInfo.id; 
    

  //   // GỌI API LẤY TỪ VỰNG CỦA FOLDER NÀY TỪ BACKEND
  //   this.practiceclientService.getFolderVocab(this.currentFolderId).subscribe({
  //     next: (data) => {
  //       this.activeFolderVocabList = data; // Đổ list từ vựng mới lấy được lên mâm Gacha
  //       folderInfo.name = this.activeFolderList.find(f => f.id === folderInfo.id)?.name || 'Thư Mục';
  //   this.activeTitle = `✨ THƯ MỤC: ${folderInfo.name.toUpperCase()} ✨`;
  //   this.currentView = 'folder-gacha';
  //   this.pageSubtitle = '✨ Luyện Tập Gacha ✨';
  //   console.log(data);
  //     },
  //     error: (err) => {
  //       console.error('Lỗi khi lấy từ vựng thư mục:', err);
  //     }
  //   });
  // }
  async handleOpenFolder(folderInfo: { id: number, name: string }) {
  // Ghi nhớ thông tin Folder
  this.currentFolderId = folderInfo.id; 
  folderInfo.name = this.activeFolderList.find(f => f.id === folderInfo.id)?.name || 'Thư Mục';
  this.activeTitle = `✨ THƯ MỤC: ${folderInfo.name.toUpperCase()} ✨`;

  try {
    // 1. CHỜ ĐỢI: Ép hệ thống đứng đây đợi cho đến khi Backend trả về list từ vựng
    const dataFromBackend = await firstValueFrom(this.practiceclientService.getFolderVocab(this.currentFolderId));
    
    // 2. NẠP ĐẠN: Có data rồi thì đổ ngay vào mâm Gacha
    this.activeFolderVocabList = dataFromBackend;

    // 3. CHUYỂN CẢNH: Mọi thứ đã sẵn sàng, bây giờ mới lật màn hình sang Gacha
    this.currentView = 'folder-gacha';
    this.pageSubtitle = '✨ Luyện Tập Gacha ✨';

  } catch (error) {
    console.error('Lỗi khi lấy từ vựng thư mục từ Backend:', error);
    alert('Lỗi tải dữ liệu. Hãy kiểm tra xem Thư mục này đã có từ vựng bên trong chưa nhé!');
  }
}

  // Xử lý Xóa Folder
  handleDeleteFolder(folderId: number) {
    this.practiceclientService.deleteFolder(folderId).subscribe(() => {
      this.practiceclientService.getUserFolders().subscribe(data => {
        this.activeFolderList = data;
      });
    });
  }

  // Xử lý Upload Folder Data
  handleUploadFolder(formData: FormData) {
    this.practiceclientService.uploadNewFolder(formData).subscribe(() => {
      this.practiceclientService.getUserFolders().subscribe(data => {
        this.activeFolderList = data;
      });
    });
  }

  // QUAY LẠI MENU
  goToMenu() {
    this.currentView = 'menu';
    this.pageSubtitle = 'Chọn phương thức học của bạn';
    
    this.practiceclientService.getSystemVocab().subscribe(data => {
      this.activeSystemVocabList = data; 
    }); 
    this.practiceclientService.getUserFolders().subscribe(data => {
      this.activeFolderList = data; 
    });
  }
}