import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
import { PracticeClientService } from '../../../Core/Services/practice-client-service';
import { PracticeMenu } from '../practice-menu/practice-menu';
import { VocabularyModel } from '../../../Models/vocabulary-model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-practice-userfolder',
  imports: [FormsModule],
  templateUrl: './practice-userfolder.html',
  styleUrl: './practice-userfolder.css',
})
export class PracticeUserfolder {
  // Nhận danh sách thư mục từ C# (thông qua Cha)
  @Input() folders: PracticeUserFolderModel[] = [];

  // Các tín hiệu gửi lên Cha
  @Output() onBack = new EventEmitter<void>();
  @Output() onSelectFolder = new EventEmitter<{ id: number, name: string }>();
  @Output() onDeleteFolder = new EventEmitter<number>();
  @Output() onUploadNewFolder = new EventEmitter<FormData>();

  // Biến hứng dữ liệu cho Form Modal
  newFolderName: string = '';
  newFolderDesc: string = '';
  selectedFile: File | null = null;
  isModalOpen: boolean = false;

  // Xử lý khi user chọn file Excel/CSV
  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  openModal() { this.isModalOpen = true; }
  closeModal() { this.isModalOpen = false; }

  // Xử lý Gửi Form tạo thư mục mới

  submitUpload() {
    if (!this.newFolderName || !this.selectedFile) {
      alert('Vui lòng nhập tên thư mục và chọn file!');
      return;
    }

    const formData = new FormData();
    formData.append('FolderName', this.newFolderName);
    formData.append('Description', this.newFolderDesc);
    
    // SỬA Ở ĐÂY: Đổi 'VocabularyFile' thành 'File' để khớp 100% với DTO của C#
    formData.append('File', this.selectedFile);

    this.onUploadNewFolder.emit(formData);

    this.newFolderName = '';
    this.newFolderDesc = '';
    this.selectedFile = null;
    this.closeModal();
  }

  // Xử lý Xóa thư mục
  triggerDelete(event: Event, folderId: number, folderName: string) {
    event.stopPropagation(); // CỰC KỲ QUAN TRỌNG: Ngăn không cho click xuyên xuống thẻ Folder
    if (confirm(`CẢNH BÁO: Bạn có chắc muốn xóa thư mục [${folderName}]?`)) {
      this.onDeleteFolder.emit(folderId);
    }
  }
}
