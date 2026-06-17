import { Component } from '@angular/core';
import { EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-practice-menu',
  imports: [],
  templateUrl: './practice-menu.html',
  styleUrl: './practice-menu.css',
})
export class PracticeMenu {
  // gửi sự kiện khi người dùng chọn hệ thống hoặc thư mục
  @Output() onSelectSystem = new EventEmitter<void>();
  @Output() onSelectFolders = new EventEmitter<void>();
}
