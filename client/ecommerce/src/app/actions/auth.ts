'use server'

import { cookies } from 'next/headers';
import { 
    LoginRequest, 
    RegisterRequest, 
    AuthResponse, 
    AuthActionResponse,
    ApiError,
    User
} from '@/types/auth';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

async function handleApiResponse(response: Response): Promise<AuthActionResponse> {
    if (!response.ok) {
        const errorData: ApiError = await response.json();
        if (errorData.type === 'Validation') {
            return {
                success: false,
                error: errorData.message,
                validationErrors: errorData.errors
            };
        }
        return {
            success: false,
            error: errorData.message || 'An error occurred during authentication'
        };
    }

    const data: AuthResponse = await response.json();
    
    // Set cookies
    const cookieStore = await cookies();
    cookieStore.set('accessToken', data.accessToken, {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
        maxAge: 60 * 60 // 1 hour
    });
    
    cookieStore.set('refreshToken', data.refreshToken, {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
        path: '/api/auth/refresh',
        maxAge: 7 * 24 * 60 * 60 // 7 days
    });

    return {
        success: true,
        user: {
            email: data.email,
            fullName: data.fullName,
            roles: data.roles
        },
        accessToken: data.accessToken
    };
}

export async function login(credentials: LoginRequest): Promise<AuthActionResponse> {
    try {
        const response = await fetch(`${API_URL}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(credentials),
        });

        if (!response.ok) {
            console.error('Login failed with status:', response.status);
            const errorData = await response.text();
            console.error('Error response:', errorData);
            
            try {
                const jsonError: ApiError = JSON.parse(errorData);
                if (jsonError.type === 'Validation') {
                    return {
                        success: false,
                        error: jsonError.message,
                        validationErrors: jsonError.errors
                    };
                }
                return {
                    success: false,
                    error: jsonError.message || 'An error occurred during login'
                };
            } catch (e) {
                console.error('Failed to parse error response as JSON:', e);
                return {
                    success: false,
                    error: `Server error (${response.status}): ${response.statusText}`
                };
            }
        }

        return handleApiResponse(response);
    } catch (error) {
        console.error('Login error:', error);
        if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
            return {
                success: false,
                error: 'Unable to connect to the authentication service. Please check your internet connection and try again.'
            };
        }
        return {
            success: false,
            error: 'Failed to connect to the authentication service. Please try again later.'
        };
    }
}

export async function register(userData: RegisterRequest): Promise<AuthActionResponse> {
    try {
        const response = await fetch(`${API_URL}/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData),
        });

        return handleApiResponse(response);
    } catch (error) {
        return {
            success: false,
            error: 'Failed to connect to the registration service'
        };
    }
}

export async function logout(): Promise<void> {
    const cookieStore = await cookies();
    cookieStore.delete('accessToken');
    cookieStore.delete('refreshToken');
}

export async function refreshTokens(): Promise<boolean> {
    try {
        const response = await fetch(`${API_URL}/Auth/refresh`, {
            method: 'POST',
            credentials: 'include', // Important for cookies
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            return false;
        }

        const data: AuthResponse = await response.json();
        
        // Set cookies
        const cookieStore = await cookies();
        cookieStore.set('accessToken', data.accessToken, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            maxAge: 60 * 60 // 1 hour
        });
        
        cookieStore.set('refreshToken', data.refreshToken, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            path: '/api/auth/refresh',
            maxAge: 7 * 24 * 60 * 60 // 7 days
        });

        return true;
    } catch (error) {
        console.error('Token refresh failed:', error);
        return false;
    }
}

export async function getMe(): Promise<User | null> {
    try {
        const cookieStore = await cookies();
        const accessToken = cookieStore.get('accessToken')?.value;

        const response = await fetch(`${API_URL}/Auth/me`, {
            headers: {
                'Content-Type': 'application/json',
                ...(accessToken ? { 'Authorization': `Bearer ${accessToken}` } : {})
            }
        });

        if (!response.ok) {
            return null;
        }

        const userData = await response.json();
        return {
            email: userData.email,
            fullName: userData.fullName,
            roles: userData.roles
        };
    } catch (error) {
        console.error('Failed to fetch user data:', error);
        return null;
    }
}

// Form Actions
export type AuthState = {
    error: string | null;
    success: boolean;
    validationErrors?: { propertyName: string; errorMessage: string }[];
};

export async function loginFormAction(prevState: any, formData: FormData): Promise<AuthState> {
    const credentials: LoginRequest = {
        email: formData.get('email') as string,
        password: formData.get('password') as string,
        rememberMe: formData.get('rememberMe') === 'true'
    };

    // Basic validation
    if (!credentials.email) {
        return {
            success: false,
            error: 'Email is required',
            validationErrors: [{
                propertyName: 'Email',
                errorMessage: 'Email is required'
            }]
        };
    }

    if (!credentials.password) {
        return {
            success: false,
            error: 'Password is required',
            validationErrors: [{
                propertyName: 'Password',
                errorMessage: 'Password is required'
            }]
        };
    }

    const result = await login(credentials);
    
    return {
        success: result.success,
        error: result.error || null,
        validationErrors: result.validationErrors
    };
}