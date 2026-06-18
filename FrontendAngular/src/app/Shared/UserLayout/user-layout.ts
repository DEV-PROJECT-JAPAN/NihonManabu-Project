import { CommonModule } from '@angular/common';
import { Component, HostListener, ChangeDetectorRef } from '@angular/core'; // 🌟 Thêm ChangeDetectorRef
import { RouterModule, Router } from '@angular/router';
import { TranslationService } from '../TranslationService';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './user-layout.html'
})
export class UserLayoutComponent {

    isBotActive: boolean = false; // Trạng thái bật/tắt toàn cục của trợ lý robot
    isBubbleOpen: boolean = false; // Trạng thái ẩn/hiện bong bóng kết quả
    bubbleContent: string = '';    // Nội dung văn bản hiển thị trong bong bóng
    isLoading: boolean = false;     // Trạng thái kiểm soát vòng quay và đèn LED nạp dữ liệu

    private debounceTimeout: any;

    // 🌟 Inject ChangeDetectorRef vào constructor để ép giao diện render ngay khi có data
    constructor(
        private _router: Router,
        private translationService: TranslationService,
        private cdr: ChangeDetectorRef
    ) { }

    goToVocabulary(): void {
        this._router.navigate(['/vocabulary/levels']);
    }

    goToGrammar(): void {
        this._router.navigate(['/grammar/levels']);
    }
    goToKanji(): void {
        this._router.navigate(['kanji']);
    }
    checkActive(moduleName: string): boolean {
        return this._router.url.includes(moduleName);
    }



    /**
     * 🌟 GIẢI PHÁP TỐI ƯU NHẤT: Quay lại dùng selectionchange nhưng xử lý triệt để bất đồng bộ
     * Bôi đen đến đâu, thả chuột là tự động nhận chữ và gọi API hiển thị ngay lập tức!
     */
    @HostListener('document:selectionchange')
    onTextSelected() {
        if (!this.isBotActive) return;

        clearTimeout(this.debounceTimeout);

        this.debounceTimeout = setTimeout(async () => {
            const selection = window.getSelection();
            if (!selection) return;

            const selectedText = selection.toString().trim();

            // 🌟 QUAN TRỌNG: Nếu click ra ngoài làm text rỗng, nhưng bong bóng ĐANG MỞ thì giữ nguyên kết quả cũ, KHÔNG tra lại bậy bạ.
            if (selectedText.length === 0) return;

            // Kiểm tra vùng an toàn tránh tự kích hoạt khi chọn trúng chữ trong con bot hoặc bong bóng
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);
                const startNode = range.startContainer.parentElement;
                const endNode = range.endContainer.parentElement;

                if (startNode?.closest('#anime-mascot-container') || endNode?.closest('#anime-mascot-container') ||
                    startNode?.closest('#aiThoughtBubble') || endNode?.closest('#aiThoughtBubble')) {
                    return;
                }
            }

            // BẬT BONG BÓNG VÀ TRẠNG THÁI LOADING ĐỒNG THỜI
            this.isLoading = true;
            this.isBubbleOpen = true;
            this.bubbleContent = '';
            this.cdr.detectChanges(); // Ép Angular render giao diện hiển thị "Đang tra cứu..." liền

            try {
                const result = await this.translationService.processAiTranslationAsync(selectedText);

                if (!this.isBotActive) return;

                // Nếu không phải tiếng Nhật hoặc quá ngắn, đóng im lặng
                if (result.isSilentCancel) {
                    this.closeBubble();
                    return;
                }

                // Tắt trạng thái loading sau khi có kết quả
                this.isLoading = false;

                if (result.isSuccess && result.data) {
                    this.bubbleContent = this.formatMarkdown(result.data);
                } else {
                    this.bubbleContent = '<div style="color: #fc8181; font-weight: bold; padding: 5px 0;">⚠️ Hệ thống đang bận hoặc có lỗi xảy ra. Vui lòng thử lại sau!</div>';
                }

                this.cdr.detectChanges(); // Ép Angular render kết quả dịch / hoặc thông báo lỗi ngay lập tức

            } catch (err) {
                this.isLoading = false;
                this.bubbleContent = '<div style="color: #fc8181; font-weight: bold; padding: 5px 0;">⚠️ Hệ thống hiện tại đã quá tải. Vui lòng thử lại sau!</div>';
                console.error("Lỗi hệ thống tra cứu:", err);
                this.cdr.detectChanges();
            }
        }, 300); // Đặt 300ms vừa đủ để trình duyệt bắt kịp vùng chữ được chọn
    }

    /**
     * Định dạng các ký tự Markdown cơ bản của AI trả về thành mã HTML tương thích hệ thống
     */
    /**
      * Định dạng các ký tự Markdown phức tạp của AI trả về thành mã HTML cao cấp, trực quan
      */
    formatMarkdown(text: string): string {
        if (!text) return "";

        let html = text;

        // 1. Chuyển đổi đường kẻ ngang --- thành thẻ <hr> có style
        html = html.replace(/[\s\n]---[\s\n]/g, '<hr class="bubble-hr">');

        // 2. Chuyển tiêu đề phụ ### thành h3 kèm icon
        html = html.replace(/### (.*)/g, '<h3 class="bubble-h3">$1</h3>');

        // 3. Biến đổi ký tự ** thành cặp thẻ strong bôi đậm màu sắc nổi bật
        html = html.replace(/\*\*(.*?)\*\*/g, '<strong class="bubble-strong">$1</strong>');

        // 4. Xử lý các dòng có gạch đầu dòng dạng "* " thành các hàng bullet đẹp
        html = html.replace(/^\* (.*)/gm, '<div class="bullet-item"><span class="bullet-dot">•</span> <span class="bullet-text">$1</span></div>');

        // 5. Thêm cấu trúc lùi đầu dòng cho các dòng phân tích chi tiết (có khoảng trống ở đầu dòng)
        html = html.replace(/^   \* (.*)/gm, '<div class="sub-bullet-item">$1</div>');
        html = html.replace(/^  \* (.*)/gm, '<div class="sub-bullet-item">$1</div>');

        // 6. Tự động highlight nhanh một số từ khóa đặc biệt để tạo giao diện sống động
        html = html.replace(/(N5|N4|N3|N2|N1)/g, '<span class="jlpt-badge">$1</span>');
        html = html.replace(/(Lịch sự|Trang trọng|Thân mật|Suồng sã)/g, '<span class="context-tag">$1</span>');

        return html;
    }

    closeBubble() {
        this.isBubbleOpen = false;
        this.isLoading = false;
        this.bubbleContent = '';
        this.cdr.detectChanges();
    }
}