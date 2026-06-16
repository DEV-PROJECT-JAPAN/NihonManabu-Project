import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { LessonClientService } from '../../../Core/Services/lesson-client-service';
import { VocabularyClientService } from '../../../Core/Services/vocabulary-client-service';
import { VocabularyModel } from '../../../Models/vocabulary-model';
import { LevelClientService } from '../../../Core/Services/level-client-service';

// import { UpdateLearningProgresByUserModel } from '../../../Models/update-learning-progress.model';

@Component({
  selector: 'app-vocabulary',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './vocabulary.html',
  styleUrls: ['./vocabulary.css']
})
export class VocabularyComponent implements OnInit {

  // Các biến State (Giống các property BindProperty bên C#)
  public lessonId: number = 0;
  public levelId: number = 0;
  public lessonTitle: string = "Danh sách từ vựng";
  public cards: VocabularyModel[] = [];
  public originalCards: VocabularyModel[] = []; // Giữ bản gốc để Reset
  public displayCards: VocabularyModel[] = [];  // Mảng đang hiển thị (có thể bị xáo trộn)

  // Trạng thái (State)
  public currentIndex: number = 0;
  public isAutoPlay: boolean = false;
  public isFlipped: boolean = false;
  public starredState: { [key: number]: boolean } = {};
  get currentCard(): VocabularyModel | undefined {
    return this.displayCards[this.currentIndex];
  }
  constructor(
    private _vocabService: VocabularyClientService,
    private _lessonService: LessonClientService,
    private _levelClientService: LevelClientService,
    private _route: ActivatedRoute,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Đọc parameters từ thanh URL (ví dụ: /vocabulary/list/:levelId/:lessonId)
    const paramLevelId = this._route.snapshot.paramMap.get('levelId');
    const paramLessonId = this._route.snapshot.paramMap.get('lessonId');

    if (paramLessonId && paramLevelId) {
      this.levelId = Number(paramLevelId);
      this.lessonId = Number(paramLessonId);

      this.fetchCards();
    }
  }

  // 1. Hàm gọi API lấy danh sách từ vựng
  private fetchCards(): void {
    this._vocabService.getCardsAsync(this.lessonId).subscribe({
      next: (data: VocabularyModel[]) => {
        this.cards = data;
        this._cdr.markForCheck(); // Ép giao diện vẽ lại
        setTimeout(() => {
          if (this.isAutoPlay && this.currentCard?.textToSpeak) {
            this.speakText(this.currentCard.textToSpeak);
          }
        }, 500);
      },
      error: (err) => console.error('Lỗi khi tải từ vựng:', err)
    });
  }

  // 2. Hàm xử lý gửi tiến độ học (Tương đương OnPostUpdateProgressAsync)
  // Giao diện 3D (JS/HTML) sẽ gọi hàm này khi người dùng lật thẻ hoặc đánh dấu thuộc
  public updateProgress(vocabularyId: number, isPassed: boolean): void {
    if (vocabularyId <= 0) return;

    const input = {
      vocabularyId: vocabularyId,
      isPassed: isPassed
      // Thêm các trường khác nếu UpdateLearningProgresByUserModel yêu cầu
    };

    this._vocabService.updateProgressAsync(input).subscribe({
      next: (success: boolean) => {
        if (success) {
          console.log(`Đã lưu tiến độ cho từ vựng ${vocabularyId}`);
        }
      }
    });
  }

  // 3. Hàm kích hoạt tải file PDF
  public exportPdf(): void {
    if (this.lessonId <= 0) return;

    // Gọi hàm download file từ Service (Bạn xem hướng dẫn thêm hàm này ở phần dưới)
    this._vocabService.downloadPdfAsync(this.lessonId).subscribe({
      next: (blob: Blob) => {
        // Tạo một đường link ảo để ép trình duyệt tải file Blob về máy
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `DanhSachTuVung_Bai${this.lessonId}.pdf`; // Tên file khi tải về
        document.body.appendChild(a);
        a.click();

        // Dọn dẹp bộ nhớ
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Lỗi khi xuất PDF:', err)
    });
  }

  // ================= CÁC HÀM ĐIỀU KHIỂN UI =================

  public toggleFlip(): void {
    this.isFlipped = !this.isFlipped;
  }

  public nextCard(): void {
    if (this.currentIndex < this.displayCards.length - 1) {
      this.currentIndex++;
      this.isFlipped = false; // Trả về mặt trước
      if (this.isAutoPlay && this.currentCard?.textToSpeak) {
        this.speakText(this.currentCard.textToSpeak);
      }
    }
  }

  public prevCard(): void {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.isFlipped = false;
      if (this.isAutoPlay && this.currentCard?.textToSpeak) {
        this.speakText(this.currentCard.textToSpeak);
      }
    }
  }

  public toggleAutoPlay(): void {
    this.isAutoPlay = !this.isAutoPlay;
    if (this.isAutoPlay && this.currentCard?.textToSpeak) {
      this.speakText(this.currentCard.textToSpeak);
    } else {
      if ('speechSynthesis' in window) window.speechSynthesis.cancel();
    }
  }

  public shuffleCards(): void {
    this.displayCards.sort(() => Math.random() - 0.5);
    this.currentIndex = 0;
    this.isFlipped = false;
  }

  public resetOrder(): void {
    this.displayCards = [...this.originalCards];
    this.currentIndex = 0;
    this.isFlipped = false;
  }

  public toggleBookmark(): void {
    if (!this.currentCard) return;
    this.starredState[this.currentCard.id] = !this.starredState[this.currentCard.id];
  }

  public toggleMastered(type: 'correct' | 'wrong'): void {
    if (!this.currentCard) return;

    const isMaster = type === 'correct';
    const input = {
      vocabularyId: this.currentCard.id,
      isPassed: isMaster
    };

    // Gọi Service cập nhật tiến độ
    this._vocabService.updateProgressAsync(input).subscribe({
      next: (success) => {
        if (success) console.log('Đã lưu tiến độ!');
      }
    });

    this.nextCard(); // Tự động nhảy sang từ tiếp theo
  }

  public speakText(text?: string, event?: Event): void {
    if (event) event.stopPropagation(); // Ngăn sự kiện lật thẻ khi bấm vào nút Loa
    if (!text || !('speechSynthesis' in window)) return;

    window.speechSynthesis.cancel();
    const msg = new SpeechSynthesisUtterance(text);
    msg.lang = 'ja-JP';
    msg.rate = 0.9;
    window.speechSynthesis.speak(msg);
  }



}