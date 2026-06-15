import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

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
  public levels: LevelModel[] = [];

  // 2. Tiêm Service (Tương đương: public IndexModel(LevelClientService levelClientService) )
  constructor(
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
    this._router.navigate(['/vocabulary/lessons', id]);
  }

}