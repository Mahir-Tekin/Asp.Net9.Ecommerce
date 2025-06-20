'use client';

import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { User, AuthContextType, RegisterRequest } from '@/types/auth';
import { 
    login as loginAction, 
    logout as logoutAction, 
    register as registerAction,
    refreshTokens,
    getMe
} from '@/app/actions/auth';
import axios, { AxiosInstance } from 'axios';
import { useRouter } from 'next/navigation';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const router = useRouter();


    // Handle refresh token
    const handleRefreshToken = useCallback(async () => {
        try {
            const success = await refreshTokens();
            if (!success) {
                throw new Error('Refresh failed');
            }
            return true;
        } catch (error) {
            setUser(null);
            throw error;
        }
    }, []);



    const checkAuth = useCallback(async () => {
        try {
            console.log('Checking authentication...');
            const userData = await getMe();
            console.log("user set to", userData);
            setUser(userData);
            if (userData) {
                console.log('User authenticated:', userData);
            } else {
                console.log('No user data returned');
            }
        } catch (error: any) {
            if (!error.redirect) {
                console.error('Auth check failed:', error);
                setUser(null);
                console.log('User not authenticated - error occurred');
            }
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        console.log('AuthContext mounted - checking authentication');
        checkAuth();
    }, [checkAuth]);

    const login = async (email: string, password: string, rememberMe: boolean) => {
        const result = await loginAction({
            email,
            password,
            rememberMe
        });

        if (result.success && result.user) {
            setUser(result.user);
            if (result.accessToken) {
                localStorage.setItem('accessToken', result.accessToken);
            }
            return { success: true };
        }

        return { 
            success: false, 
            error: result.error,
            validationErrors: result.validationErrors
        };
    };

    const register = async (data: RegisterRequest) => {
        const result = await registerAction(data);

        if (result.success && result.user) {
            setUser(result.user);
            if (result.accessToken) {
                localStorage.setItem('accessToken', result.accessToken);
            }
            return { success: true };
        }

        return {
            success: false,
            error: result.error,
            validationErrors: result.validationErrors
        };
    };

    const logout = async () => {
        await logoutAction();
        localStorage.removeItem('accessToken');
        setUser(null);
        router.push('/login');
    };

    const value = {
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        register,
        logout,
        refreshToken: handleRefreshToken
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}