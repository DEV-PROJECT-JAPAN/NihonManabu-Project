import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
import { VocabularyModel } from '../../../Models/vocabulary-model';


@Component({
  selector: 'app-practice-gachavocabulary',
  imports: [],
  templateUrl: './practice-gachavocabulary.html',
  styleUrl: './practice-gachavocabulary.css',
})
export class PracticeGachavocabulary {
  // Dữ liệu Cha truyền xuống
  @Input() vocabList: VocabularyModel[] | PracticeUserFolderModel[] = [];
  @Input() title: string = '✨ LẮC TỪ VỰNG NGẪU NHIÊN ✨';
  
  // Nút quay lại báo cho Cha
  @Output() onBack = new EventEmitter<void>();
  
  // Biến lưu trạng thái lật thẻ (lưu ID của các thẻ đang lật)
  flippedCards: Set<number> = new Set<number>();

  toggleCard(id: number) {
    if (!this.flippedCards.has(id)) {
      this.flippedCards.add(id);
    }
  }

  resetCards() {
    this.flippedCards.clear();
  }

  shuffleCards() {
    // 1. Tạo bản sao của mảng hiện tại
    const shuffled = [...this.vocabList];
    
    // 2. Trộn ngẫu nhiên
    shuffled.sort(() => Math.random() - 0.5);
    
    // 3. Ép kiểu bằng 'as any' khi gán ngược lại để TypeScript không báo lỗi
    this.vocabList = shuffled as any;
    
    this.resetCards();
  }

  playAudio(event: Event, textToSpeak: string) {
    event.stopPropagation(); // Tránh bị lật thẻ khi bấm nút loa
    if ('speechSynthesis' in window) {
      window.speechSynthesis.cancel();
      const utterance = new SpeechSynthesisUtterance(textToSpeak);
      utterance.lang = 'ja-JP';
      utterance.rate = 0.85;
      window.speechSynthesis.speak(utterance);
    }
  }
}


// import { Component, EventEmitter, Input, Output } from '@angular/core';
// import { PracticeUserFolderModel } from '../../../Models/practice-userfolder-model';
// import { VocabularyModel } from '../../../Models/vocabulary-model';

// @Component({
//   selector: 'app-practice-gachavocabulary',
//   imports: [],
//   templateUrl: './practice-gachavocabulary.html',
//   styleUrl: './practice-gachavocabulary.css',
// })
// export class PracticeGachavocabulary {
//   @Input() vocabList: VocabularyModel[] = [];
//   @Input() title: string = '✨ LẮC TỪ VỰNG NGẪU NHIÊN ✨';
  
//   // Nhận trạng thái Loading từ Cha
//   @Input() isLoading: boolean = false;
  
//   @Output() onBack = new EventEmitter<void>();
  
//   // Báo cho Cha biết User muốn quay thẻ mới
//   @Output() onDrawAgain = new EventEmitter<void>();
  
//   flippedCards: Set<number> = new Set<number>();

//   toggleCard(id: number) {
//     if (!this.flippedCards.has(id)) {
//       this.flippedCards.add(id);
//     }
//   }

//   resetCards() {
//     this.flippedCards.clear();
//   }

//   // Đổi logic: Không tự xáo bài nữa, úp bài và gọi API
//   triggerDrawAgain() {
//     this.resetCards();
//     this.onDrawAgain.emit(); // Bắn tín hiệu lên Cha
//   }

//   playAudio(event: Event, textToSpeak: string) {
//     event.stopPropagation(); 
//     if ('speechSynthesis' in window) {
//       window.speechSynthesis.cancel();
//       const utterance = new SpeechSynthesisUtterance(textToSpeak);
//       utterance.lang = 'ja-JP';
//       utterance.rate = 0.85;
//       window.speechSynthesis.speak(utterance);
//     }
//   }
// }