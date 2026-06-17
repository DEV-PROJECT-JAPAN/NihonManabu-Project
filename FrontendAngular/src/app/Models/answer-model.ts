export interface AnswerModel {
    id: number;
    questionId: number;
    text: string;
    isCorrect: boolean;
    displayOrder: string; // Đồng bộ kiểu chuỗi (string) từ Đội trưởng
}