
class TranslationClientService {
    constructor() {
        this.apiKey = "Tuổi cc lấy API T";
        this.url = `https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key=${this.apiKey}`;
    }

    /**
     * HÀM BỔ TRỢ: Kiểm tra xem văn bản được bôi đen có thực sự chứa ký tự tiếng Nhật hay không.
     * Mục đích: Loại bỏ hoàn toàn các yêu cầu (Request) rác khi người dùng vô tình bôi đen chữ tiếng Việt, tiếng Anh hoặc các khoảng trắng.
     */
    isJapaneseText(text) {
        // Biểu thức chính quy (Regex) quét các dải mã Unicode đặc trưng của: Chữ Kanji (Hán tự), chữ Hiragana và Katakana (Chữ mềm/chữ cứng)
        const jpRegex = /[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF]/;
        // Trả về true nếu trong chuỗi bôi đen có chứa ít nhất 1 ký tự tiếng Nhật, ngược lại trả về false
        return jpRegex.test(text);
    }

    /**
     * HÀM CHÍNH: Nhận chữ bôi đen, đóng gói prompt hệ thống và tiến hành gửi fetch sang Google Gemini API
     */
    async processAiTranslationAsync(requestModel) {
        if (!requestModel || !requestModel.text) {
            return { isSuccess: false, errorMessage: "Dữ liệu trống." };
        }
        const cleanText = requestModel.text.trim();

        // 🛑 BỘ LỌC BẢO VỆ 1: Nếu chuỗi quá ngắn (dưới 2 ký tự) hoặc KHÔNG chứa chữ Nhật thì lập tức HỦY request ngay tại đây!
        // Việc chặn đứng này giúp cứu nguy cho hạn mức (Quota) miễn phí của bạn không bị cạn kiệt vì những cú click chuột nhầm.
        if (cleanText.length < 2 || !this.isJapaneseText(cleanText)) {
            // Trả về cờ "isSilentCancel" để Frontend tự biết đường đóng bong bóng trong im lặng, không hiện lỗi
            return { isSuccess: false, isSilentCancel: true };
        }

        // Đúc "khuôn mẫu" Prompt hệ thống nhằm ép AI đóng vai giáo viên, bắt nó phải trả về định dạng Markdown chính xác theo cấu trúc thiết kế giao diện
        const systemPrompt = `Bạn là một giáo viên dạy tiếng Nhật chuyên nghiệp.\n` +
            `Let phân tích đoạn văn bản tiếng Nhật sau: "${cleanText}" và phản hồi CHÍNH XÁC theo cấu trúc Markdown dưới đây. Không viết lời mở đầu luyên thuyên:\n\n` +
            `### 🎯 BẢN DỊCH CHUẨN TRỰC QUAN\n` +
            `* **Nghĩa thoát ý:** (Dịch câu mượt mà, thuần Việt)\n` +
            `* **Cách đọc:** (Phiên âm Romaji hoặc Kanji kèm Hiragana toàn câu)\n\n` +
            `--- \n\n` +
            `### 📝 BÓC TÁCH NGỮ PHÁP & TỪ VỰNG CHÍNH\n` +
            `Liệt kê các từ vựng quan trọng hoặc cấu trúc ngữ pháp xuất hiện (Tối đa 3 thành phần chính):\n\n` +
            `1. **Từ/Cụm từ:** (Chữ gốc)\n` +
            `   * **Cách đọc & Loại từ:** (Hiragana - Nghĩa tiếng Việt)\n` +
            `   * **Ví dụ minh họa:** (Câu tiếng Nhật ngắn)\n` +
            `   * **Nghĩa ví dụ:** (Bản dịch câu ví dụ)`;


        const requestData = {
            contents: [{ parts: [{ text: systemPrompt }] }]
        };


        let maxRetries = 3;
        let delay = 2000;
        // Vòng lặp tối đa 3 lần để cố gắng kết nối và lấy bằng được câu dịch từ Google về
        for (let i = 0; i < maxRetries; i++) {
            try {
                // Bắn HTTP POST ngầm truyền chuỗi dữ liệu JSON sang server Google AI
                const response = await fetch(this.url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(requestData)
                });

                // Chuyển đổi phản hồi của Google về dạng đối tượng JSON để bóc tách kết quả
                const jsonResult = await response.json();

                // TRƯỜNG HỢP A: Kết nối thành công rực rỡ (Mã HTTP 200 OK)
                if (response.ok) {
                    return {
                        isSuccess: true,
                        // Bốc đúng ngách dữ liệu chứa chuỗi văn bản câu dịch bóc tách của AI gửi về cho Frontend xài
                        data: jsonResult.candidates[0].content.parts[0].text
                    };
                }

                // TRƯỜNG HỢP B: Gặp sự cố dính lỗi 429 (Bị giới hạn số lần bấm/phút) hoặc 503 (Server phía Google quá tải)
                const isRateLimited = response.status === 429 || response.status === 503 ||
                    (jsonResult.error && jsonResult.error.message.includes("quota"));

                // Nếu thực sự bị nghẽn hạn mức VÀ vẫn còn lượt thử lại (chưa vượt quá 3 lần)
                if (isRateLimited && i < maxRetries - 1) {
                    // Ra lệnh cho luồng chạy đứng im bất động ngủ ngầm (setTimeout) để chờ hết thời gian phạt block của Google
                    await new Promise(resolve => setTimeout(resolve, delay));
                    delay *= 2; // CƠ CHẾ NHÂN ĐÔI: Lần sau bắt đợi lâu hơn để an toàn (Ví dụ: Lần 1 đợi 2s -> Lần 2 đợi 4s)
                    continue;   // Kích hoạt từ khóa "continue" để nhảy sang lượt lặp tiếp theo, tự động nộp lại form ngầm
                }

                // Nếu đã thử lại hết 3 lần mà vẫn thất bại, lấy câu chửi lỗi chính thức từ Google, không thì báo câu mặc định
                const errMessage = jsonResult.error ? jsonResult.error.message : "Google API đang bận.";
                return { isSuccess: false, errorMessage: errMessage };

            } catch (err) {
                // KHỐI BẮT LỖI MẠNG (Mất kết nối internet giữa chừng, rớt mạng...)
                // Nếu vẫn còn lượt thử lại thì tiếp tục bắt luồng đứng đợi rồi quay vòng lặp chạy lại ngầm
                if (i < maxRetries - 1) {
                    await new Promise(resolve => setTimeout(resolve, delay));
                    continue;
                }
                // Nếu rớt mạng hẳn dẫu đã thử hết cách, trả về thông báo lỗi hệ thống
                return { isSuccess: false, errorMessage: err.message };
            }
        }
    }
}

// Xuất lớp dịch thuật này ra biến môi trường toàn cục "window" để file script ngoài trang HTML có thể dễ dàng gọi new xài được
window.TranslationClientService = TranslationClientService;
