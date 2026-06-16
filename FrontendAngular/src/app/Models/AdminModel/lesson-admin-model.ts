export interface LessonAdminModel {
    id: number;
    levelId: number;
    title: string;
    order: number;

    // Thường Admin sẽ cần thêm 2 trường thời gian này để quản lý
    createdAt: string;
    updatedAt: string;
}