import { AnswerAdminModel } from './answer-admin-model';

export interface QuestionAdminModel {
    id: number;

    // Dấu hỏi '?' tương đương với Nullable (int?) bên C#
    grammarId?: number;

    content: string;

    // 1: Trắc nghiệm, 2: Điền vào chỗ trống, 3: Tự luận
    questionType: number;

    // Khởi tạo mảng rỗng tương đương = new List<AnswerAdminModel>()
    answers: AnswerAdminModel[];

    // Kiểu DateTime bên C# chuyển sang TS sẽ dùng kiểu dữ liệu 'Date' hoặc 'string' (ở đây dùng Date)
    createdAt?: Date;
    updatedAt?: Date;
}