import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    private apiKey = "hajfjaf"; // Thay API Key của bạn vào đây
    private url = `https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key=${this.apiKey}`;

    // Kiểm tra văn bản chứa tiếng Nhật
    isJapaneseText(text: string): boolean {
        const jpRegex = /[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF]/;
        return jpRegex.test(text);
    }

    // Gửi request sang Gemini API với cơ chế Retry
    async processAiTranslationAsync(text: string): Promise<{ isSuccess: boolean; data?: string; isSilentCancel?: boolean; errorMessage?: string }> {
        const cleanText = text.trim();

        if (cleanText.length < 2 || !this.isJapaneseText(cleanText)) {
            return { isSuccess: false, isSilentCancel: true };
        }

        const systemPrompt = `Bạn là một giáo viên dạy tiếng Nhật chuyên nghiệp và là chuyên gia ngôn ngữ học.\n` +
            `Hãy phân tích chuyên sâu đoạn văn bản tiếng Nhật sau: "${cleanText}" và phản hồi CHÍNH XÁC 100% theo cấu trúc Markdown dưới đây. Tuyệt đối không viết lời mở đầu hoặc kết luận luyên thuyên:\n\n` +
            `### 🎯 BẢN DỊCH CHUẨN TRỰC QUAN\n` +
            `* **Nghĩa thoát ý:** (Dịch câu mượt mà, thuần Việt, đúng văn phong đời sống)\n` +
            `* **Cách đọc toàn câu:** (Furigana hoặc Kanji kèm Hiragana trong ngoặc - Phiên âm Romaji)\n` +
            `* **Bối cảnh sử dụng:** (Giải thích ngắn gọn câu này dùng trong trường hợp nào: Lịch sự, trang trọng, thân mật, hay suồng sã)\n\n` +
            `--- \n\n` +
            `### 📝 BÓC TÁCH NGỮ PHÁP & TỪ VỰNG CHÍNH\n` +
            `Phân tích các từ vựng cốt lõi hoặc cấu trúc ngữ pháp quan trọng xuất hiện (Tối đa 3 thành phần):\n\n` +
            `* **Thành phần:** (Chữ Kanji/Gốc gốc)\n` +
            `  * **Cách đọc & Loại từ:** (Hiragana - Nghĩa tiếng Việt đầy đủ)\n` +
            `  * **Cấp độ JLPT:** (N5/N4/N3/N2/N1 hoặc Từ vựng thông dụng)\n` +
            `  * **Ví dụ minh họa:** (Câu tiếng Nhật ngắn, thực tế)\n` +
            `  * **Nghĩa ví dụ:** (Bản dịch câu ví dụ)`;

        const requestData = { contents: [{ parts: [{ text: systemPrompt }] }] };
        let maxRetries = 3;
        let delay = 2000;

        for (let i = 0; i < maxRetries; i++) {
            try {
                const response = await fetch(this.url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(requestData)
                });

                const jsonResult = await response.json();

                if (response.ok) {
                    return { isSuccess: true, data: jsonResult.candidates[0].content.parts[0].text };
                }

                const isRateLimited = response.status === 429 || response.status === 503 ||
                    (jsonResult.error && jsonResult.error.message.includes("quota"));

                if (isRateLimited && i < maxRetries - 1) {
                    await new Promise(resolve => setTimeout(resolve, delay));
                    delay *= 2;
                    continue;
                }

                return { isSuccess: false, errorMessage: jsonResult.error?.message || "Google API đang bận." };
            } catch (err: any) {
                if (i < maxRetries - 1) {
                    await new Promise(resolve => setTimeout(resolve, delay));
                    continue;
                }
                return { isSuccess: false, errorMessage: err.message };
            }
        }
        return { isSuccess: false, errorMessage: "Thất bại sau nhiều lần thử." };
    }
}