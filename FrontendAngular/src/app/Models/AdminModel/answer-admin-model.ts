export interface AnswerAdminModel {
    id: number;
    questionId: number;

    // Đổi 'Text' thành 'content' để đồng bộ khớp với cấu trúc mảng answers ở QuestionAdminModel
    text: string;

    isCorrect: boolean;

    // Thứ tự hiển thị (A, B, C, D...)
    displayOrder: string;

    orderId: number;

    // Các thuộc tính ngày tháng tùy chọn (Nullable)
    createdAt?: Date;
    updatedAt?: Date;
}