import { Component, OnInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { VocabularyClientService } from '../../Core/Services/vocabulary-client-service';
import { forkJoin, map, of } from 'rxjs';
import { VocabularyModel } from '../../Models/vocabulary-model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-kanji-notebook',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './kanji.html',
  styleUrls: ['./kanji.css']
})
export class Kanji implements OnInit {
  // === KHAI BÁO CÁC BIẾN STATE (ĐÃ SỬA LỖI does not exist) ===
  vocabularyList: VocabularyModel[] = [];
  filteredKanjiList: VocabularyModel[] = [];
  categoryList: any[] = [];

  private readonly kanjiLessons = [209, 210, 211, 212, 213];
  private readonly lessonNames: { [key: number]: string } = {
    209: 'Thiên nhiên',
    210: 'Con người',
    211: 'Số đếm',
    212: 'Vị trí & Kích thước',
    213: 'Học đường'
  };

  activeCategory: string = 'all';
  selectedWord: VocabularyModel | null = null;

  showStrokeNumbers: boolean = false;
  statusLine: string = 'Đang đồng bộ dữ liệu...';

  readonly guideSvg: string = `
    <svg viewBox="0 0 109 109" xmlns="http://www.w3.org/2000/svg">
      <rect x="1" y="1" width="107" height="107" fill="none" stroke="#e2d6bf" stroke-width="2"/>
      <line x1="54.5" y1="0" x2="54.5" y2="109" stroke="#e2d6bf" stroke-width="1.5" stroke-dasharray="4 4"/>
      <line x1="0" y1="54.5" x2="109" y2="54.5" stroke="#e2d6bf" stroke-width="1.5" stroke-dasharray="4 4"/>
    </svg>
  `;

  @ViewChild('strokeHost', { static: false }) strokeHost!: ElementRef;

  // Tiêm thêm ChangeDetectorRef để ép Angular cập nhật view
  constructor(private vocabService: VocabularyClientService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadNotebookData();
  }

  // === SỬA LẠI HÀM NẠP: GỌI SONG SONG CHUẨN CÚ PHÁP ĐỂ SỬA LỖI Expected 1 arguments ===
  loadNotebookData(): void {
    this.statusLine = 'Đang tải dữ liệu Hán tự...';

    // Đặt lại state danh mục mặc định ban đầu
    this.categoryList = [{ key: 'all', label: 'Tất cả' }];

    // Tạo mảng các Observable, sử dụng 'of' để bọc lót nếu API lỗi
    const requests = this.kanjiLessons.map(id =>
      this.vocabService.getCardsAsync(id).pipe(map(vocabularies => vocabularies || []))
    );

    // Gom cụm gọi song song
    forkJoin(requests).subscribe({
      next: (results) => {
        // Đặt lại list từ vựng chính
        this.vocabularyList = [];

        results.forEach((vocabularies, index) => {
          const lessonId = this.kanjiLessons[index];

          if (vocabularies && vocabularies.length > 0) {
            // Lọc: Chỉ lấy từ vựng có Kanji
            const validKanjiWords = vocabularies.filter(v => v.kanji && v.kanji.trim() !== '');

            if (validKanjiWords.length > 0) {
              this.vocabularyList.push(...validKanjiWords);

              // Thêm nút bài học động
              if (this.lessonNames[lessonId]) {
                this.categoryList.push({
                  key: lessonId.toString(),
                  label: this.lessonNames[lessonId]
                });
              }
            }
          }
        });

        this.statusLine = 'Nạp dữ liệu từ Database thành công.';
        this.filterKanjiGrid(); // Cập nhật lưới hiển thị

        // 🔥 SỬA LỖI PHẢI CLICK MỚI HIỆN: Tăng độ trễ setTimeout lên 300ms
        // Đảm bảo DOM cột phải đã dựng xong hoàn toàn, ô #strokeHost tồn tại.
        if (this.vocabularyList.length > 0) {
          setTimeout(() => {
            // Tự động gán và gọi nạp chữ đầu tiên mượt mà
            this.selectWordCard(this.vocabularyList[0]);
            this.cdr.detectChanges(); // Ép cập nhật lại view
          }, 300);
        } else {
          this.statusLine = 'Không tìm thấy dữ liệu chữ Hán trong các bài học này.';
        }
      },
      error: () => {
        this.statusLine = 'Lỗi kết nối nghiêm trọng đến Vocabulary API.';
      }
    });
  }

  selectCategory(categoryKey: string): void {
    this.activeCategory = categoryKey;
    this.filterKanjiGrid();
  }

  filterKanjiGrid(): void {
    if (this.activeCategory === 'all') {
      this.filteredKanjiList = this.vocabularyList;
    } else {
      this.filteredKanjiList = this.vocabularyList.filter(v => v.lessonId.toString() === this.activeCategory);
    }
  }

  selectWordCard(word: VocabularyModel): void {
    this.selectedWord = word;
    // Đợi DOM cập nhật xong trong 1 tick, sau đó gọi lấy nét vẽ
    setTimeout(() => {
      if (word.kanji) this.loadStrokeOrderSvg(word.kanji);
    });
  }

  getSinoVietnamese(meaning: string, type: 'sino' | 'pure'): string {
    if (!meaning) return '';
    const matches = meaning.match(/\[(.*?)\]\s*(.*)/);
    if (type === 'sino') return matches ? matches[1] : 'HÁN TỰ';
    return matches ? matches[2] : meaning;
  }

  async loadStrokeOrderSvg(char: string): Promise<void> {
    if (!this.strokeHost || !char) return;
    this.statusLine = 'Đang tải hoạt ảnh nét vẽ vector...';

    const hex = char.codePointAt(0)!.toString(16).padStart(5, '0');
    const url = `https://cdn.jsdelivr.net/gh/KanjiVG/kanjivg@master/kanji/${hex}.svg`;

    try {
      const response = await fetch(url);
      if (!response.ok) throw new Error();

      const svgText = await response.text();
      const parser = new DOMParser();
      const doc = parser.parseFromString(svgText, 'image/svg+xml');
      const svgEl = doc.querySelector('svg');

      if (svgEl) {
        svgEl.removeAttribute('width');
        svgEl.removeAttribute('height');

        const hostElement = this.strokeHost.nativeElement;
        hostElement.innerHTML = '';
        hostElement.appendChild(svgEl);

        this.animateStrokes(svgEl);
        this.statusLine = 'Tải nét vẽ từ KanjiVG thành công.';
      }
    } catch (err) {
      // Nếu không tải được SVG, hiện chữ font hệ thống làm đại diện
      this.strokeHost.nativeElement.innerHTML = `
        <div style="display:flex;align-items:center;justify-content:center;height:100%;font-family:var(--font-display);font-size:2.5rem;color:var(--ink-soft);">${char}</div>
      `;
      this.statusLine = 'Hiển thị Font tĩnh hệ thống (Mất kết nối file hoạt ảnh SVG).';
    }
  }

  animateStrokes(svgEl: SVGElement): void {
    const paths = svgEl.querySelectorAll('g[id^="kvg:StrokePaths"] path') as NodeListOf<SVGPathElement>;
    paths.forEach(p => {
      const len = p.getTotalLength();
      p.style.transition = 'none';
      p.style.strokeDasharray = len.toString();
      p.style.strokeDashoffset = len.toString();
    });

    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        paths.forEach((p, i) => {
          p.style.transition = `stroke-dashoffset 0.5s ease-in-out ${i * 0.55}s`;
          p.style.strokeDashoffset = '0';
        });
      });
    });
  }

  replayAnimation(): void {
    if (!this.strokeHost) return;
    const svg = this.strokeHost.nativeElement.querySelector('svg');
    if (svg) this.animateStrokes(svg);
  }

  speakJapanese(text: string): void {
    if ('speechSynthesis' in window) {
      window.speechSynthesis.cancel();
      const utterance = new SpeechSynthesisUtterance(text);
      utterance.lang = 'ja-JP';
      utterance.rate = 0.85;
      window.speechSynthesis.speak(utterance);
    } else {
      alert('Hệ thống TTS SpeechSynthesis không được hỗ trợ.');
    }
  }
}