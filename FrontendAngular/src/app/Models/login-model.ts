export interface LoginViewModel {
    email: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    message: string;
    // Thêm các trường khác tùy vào API của bạn
}