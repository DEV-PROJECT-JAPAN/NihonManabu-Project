import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';//cung cấp công cụ điều hướng giữa các trang

import { LevelClientService } from '../../../Core/Services/level-client-service';

import { LevelModel } from '../../../Models/level-model'; // Hoặc đường dẫn chứa class LevelModel

@Component({
  selector: 'app-vocabulary-levels',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './levels.html',
  styleUrls: ['./levels.css']
})
export class LevelsComponent implements OnInit {

  // 1. "State" của Component (Tương đương: public List<LevelModel> Levels { get; set; } = new(); )
  public levels: LevelModel[] = [];//biến chứa danh sach
  constructor(//tiêm các dịch vụ vào component để sử dụng
    private _levelClientService: LevelClientService,
    private _cdr: ChangeDetectorRef,
    private _router: Router
  ) { }

  // 3. ngOnInit chạy ngay khi trang vừa mở (Tương đương: public async Task OnGetAsync() )
  ngOnInit(): void {
    this.loadLevels();
  }

  // Hàm xử lý việc gọi API
  private loadLevels(): void {
    // Đăng ký (subscribe) để lắng nghe dữ liệu từ hàm getLevelsAsync() bên file Service
    this._levelClientService.getLevelsAsync().subscribe({
      next: (data: LevelModel[]) => {
        // Khi API trả dữ liệu về thành công, gán nó vào biến levels của màn hình
        this.levels = data;
        this._cdr.markForCheck();
      },
      error: (error) => {
        console.error('Lỗi hệ thống khi tải cấp độ:', error);
      }
    });
  }
  public selectLevel(id: number): void {
    console.log('ID cấp độ nhận được là:', id); // ◄ Thêm dòng này để kiểm tra ở tab Console (F12)

    if (!id) {
      console.error('Lỗi: ID bị undefined hoặc bằng 0, không thể chuyển trang!');
      return;
    }

    this._router.navigate(['/vocabulary/lessons', id]);
  }

}