export interface GrammarAdminModel {
    id: number;
    createdAt: Date;
    updatedAt: Date;

    // Dữ liệu cốt lõi phục vụ Form nhập liệu
    lessonId: number;
    structure: string;
    explanation: string;
}