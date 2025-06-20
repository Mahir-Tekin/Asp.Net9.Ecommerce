import { AxiosInstance } from 'axios';

// Request Types
export interface LoginRequest {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface RegisterRequest {
    email: string;
    password: string;
    confirmPassword: string;
}

// Response Types
export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    userId: string;
    email: string;
    fullName: string;
    roles: string[];
    refreshTokenExpiryTime: string;
}

// User Type (for client-side usage)
export interface User {
    email: string;
    fullName: string;
    roles: string[];
}

// Error Response Type
export interface ApiError {
    message: string;
    code: string;
    type: 'Validation' | 'Error';
    errors?: ValidationError[];
}

export interface ValidationError {
    propertyName: string;
    errorMessage: string;
}

// Auth Context Type
export interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    refreshToken: () => Promise<boolean>;
    login: (email: string, password: string, rememberMe: boolean) => Promise<{
        success: boolean;
        error?: string;
        validationErrors?: ValidationError[];
    }>;
    register: (data: RegisterRequest) => Promise<{
        success: boolean;
        error?: string;
        validationErrors?: ValidationError[];
    }>;
    logout: () => Promise<void>;
}

// Action Response Types
export interface AuthActionResponse {
    success: boolean;
    user?: User;
    error?: string;
    validationErrors?: ValidationError[];
    accessToken?: string;
}

// Auth State Type
export interface AuthState {
    error: string | null;
    success: boolean;
    validationErrors?: ValidationError[];
} 