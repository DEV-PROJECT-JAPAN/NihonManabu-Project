import { Component, OnInit } from '@angular/core';
import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
import { PracticeClientService } from '../../../Core/Services/practice-client-service';
import { PracticeGachavocabulary } from '../practice-gachavocabulary/practice-gachavocabulary';
import { PracticeMenu } from '../practice-menu/practice-menu';
import { VocabularyModel } from '../../../Models/vocabulary-model';
import { PracticeUserfolder } from '../practice-userfolder/practice-userfolder';

@Component({
  selector: 'app-practice-main',
  imports: [PracticeMenu, PracticeGachavocabulary, PracticeUserfolder],
  templateUrl: './practice-main.html',
  styleUrl: './practice-main.css',
})
export class PracticeMain implements OnInit { // Nhớ implements OnInit
  // Trạng thái hiện tại của màn hình
  currentView: 'menu' | 'system' | 'folders' | 'folder-gacha' = 'menu';
  pageSubtitle: string = 'Chọn phương thức học của bạn';
  
  // Dữ liệu dùng chung
  activeSystemVocabList: VocabularyModel[] = []; // Dùng chung cho cả màn Gacha System và Folder
  activeFolderList: PracticeUserFolderModel[] = [];
  activeTitle: string = '';
  isLoadingGacha: boolean = false;
  
  // BIẾN MỚI: Lưu ID của folder đang được chọn để chơi
  currentFolderId: number = 0; 

  constructor(private practiceclientService: PracticeClientService) {}

  ngOnInit() {
    // Tải sẵn data Hệ Thống ngay khi vào trang, để mở ra là có luôn
    this.practiceclientService.getSystemVocab().subscribe(data => {
      this.activeSystemVocabList = data; // Cất sẵn vào kho, đợi user bấm
    });
    this.practiceclientService.getUserFolders().subscribe(data => {
      this.activeFolderList = data; // Cất sẵn vào kho, đợi user bấm
    });
  }

  // Xử lý khi Con (MainMenu) báo người dùng chọn Hệ Thống
  openSystemVocab() {
    this.currentView = 'system';
    this.pageSubtitle = '✨ Gacha Random Words ✨';
    this.activeTitle = '✨ LẮC TỪ VỰNG NGẪU NHIÊN TỪ HỆ THỐNG ✨';
  }

  // Xử lý khi Con (MainMenu) báo người dùng chọn Thư mục
  openFoldersView() {
    this.currentView = 'folders';
    this.pageSubtitle = '📂 Quản lý từ vựng cá nhân';
  }

  // ==========================================
  // LOGIC MỚI: XỬ LÝ FOLDER & GACHA
  // ==========================================

  // Khi click vào 1 Folder cụ thể
  handleOpenFolder(folderInfo: { id: number, name: string }) {
    this.currentFolderId = folderInfo.id; // Nhớ ID folder đang chơi
    folderInfo.name = this.activeFolderList.find(f => f.id === folderInfo.id)?.name || 'Thư Mục';
    this.activeTitle = `✨ THƯ MỤC: ${folderInfo.name.toUpperCase()} ✨`;
    this.currentView = 'folder-gacha';
    this.pageSubtitle = '✨ Luyện Tập Gacha ✨';
  }

  // Xử lý Xóa Folder
  handleDeleteFolder(folderId: number) {
    // Chỗ này gọi API Delete Folder
    this.practiceclientService.deleteFolder(folderId).subscribe(() => {
      // Sau khi xóa thành công, refresh lại danh sách folder để folder vừa xóa biến mất
      this.practiceclientService.getUserFolders().subscribe(data => {
        this.activeFolderList = data;
      });
    });
  }

  // Xử lý Upload Form Data
  handleUploadFolder(formData: FormData) {
    // Chỗ này gọi API Upload Folder
    this.practiceclientService.uploadNewFolder(formData).subscribe(() => {
      // Sau khi upload thành công, refresh lại danh sách folder để có folder mới hiện lên
      this.practiceclientService.getUserFolders().subscribe(data => {
        this.activeFolderList = data;
      });
    });
  }

  // ==========================================
  // QUAY LẠI MENU
  // ==========================================
  goToMenu() {
    this.currentView = 'menu';
    this.pageSubtitle = 'Chọn phương thức học của bạn';
    
    // Refresh lại data cho nóng (như anh đã viết)
    this.practiceclientService.getSystemVocab().subscribe(data => {
      this.activeSystemVocabList = data; 
    }); 
    this.practiceclientService.getUserFolders().subscribe(data => {
      this.activeFolderList = data; 
    });
  }
}


