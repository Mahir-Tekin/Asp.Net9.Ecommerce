'use server';

import { cookies } from 'next/headers';
import { AuthResponse } from '@/types/auth';

// Base API URL for all requests
const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

// Token refresh state management
let isRefreshing = false;
let failedQueue: { resolve: (token: string) => void; reject: (error: any) => void; }[] = [];

/**
 * Process queued requests after token refresh
 * This handles multiple requests that failed due to token expiration
 */
function processFailedQueue(error: any = null, token: string | null = null) {
    failedQueue.forEach(promise => {
        if (error) {
            promise.reject(error);
        } else if (token) {
            promise.resolve(token);
        }
    });
    failedQueue = [];
}

/**
 * Response type including tokens for cookie management
 */
type ApiResponse = {
    response: Response;
    cookies?: {
        accessToken?: { value: string; maxAge: number };
        refreshToken?: { value: string; maxAge: number };
    };
}

/**
 * Refresh the access token using the refresh token
 * Returns the new tokens for cookie management
 */
async function refreshTokens(): Promise<{ accessToken: string; refreshToken: string } | null> {
    const cookieStore = await cookies();
    try {
        const refreshToken = cookieStore.get('refreshToken')?.value;

        if (!refreshToken) {
            throw new Error('No refresh token available');
        }

        const response = await fetch(`${API_URL}/Auth/refresh`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ refreshToken }),
        });

        if (!response.ok) {
            throw new Error('Token refresh failed');
        }

        const data: AuthResponse = await response.json();
        
        // Instead of setting cookies directly, return the tokens
        return {
            accessToken: data.accessToken,
            refreshToken: data.refreshToken
        };
    } catch (error) {
        // Clear cookies on refresh failure
        cookieStore.delete('accessToken');
        cookieStore.delete('refreshToken');
        throw error;
    }
}

/**
 * Main API call function that handles authentication and token refresh
 * Returns both the response and any new tokens that need to be set as cookies
 */
export async function apiCall(endpoint: string, options: RequestInit = {}): Promise<ApiResponse> {
    const cookieStore = await cookies();
    const accessToken = cookieStore.get('accessToken')?.value;

    const config: RequestInit = {
        ...options,
        headers: {
            ...options.headers,
            'Content-Type': 'application/json',
            ...(accessToken ? { 'Authorization': `Bearer ${accessToken}` } : {}),
        },
    };

    try {
        console.log(`Making API call to: ${API_URL}${endpoint}`);
        let response = await fetch(`${API_URL}${endpoint}`, config);
        let newTokens;

        if (response.status === 401) {
            if (!isRefreshing) {
                isRefreshing = true;
                try {
                    newTokens = await refreshTokens();
                    if (newTokens) {
                        config.headers = {
                            ...config.headers,
                            'Authorization': `Bearer ${newTokens.accessToken}`,
                        };
                        response = await fetch(`${API_URL}${endpoint}`, config);
                        processFailedQueue(null, newTokens.accessToken);
                    }
                } catch (error) {
                    processFailedQueue(error);
                    throw error;
                } finally {
                    isRefreshing = false;
                }
            } else {
                try {
                    const newToken = await new Promise<string>((resolve, reject) => {
                        failedQueue.push({ resolve, reject });
                    });
                    config.headers = {
                        ...config.headers,
                        'Authorization': `Bearer ${newToken}`,
                    };
                    response = await fetch(`${API_URL}${endpoint}`, config);
                } catch (error) {
                    throw error;
                }
            }
        }

        // Return both the response and any new tokens
        return {
            response,
            ...(newTokens && {
                cookies: {
                    accessToken: { 
                        value: newTokens.accessToken,
                        maxAge: 60 * 60 // 1 hour
                    },
                    refreshToken: { 
                        value: newTokens.refreshToken,
                        maxAge: 7 * 24 * 60 * 60 // 7 days
                    }
                }
            })
        };
    } catch (error) {
        throw error;
    }
} 