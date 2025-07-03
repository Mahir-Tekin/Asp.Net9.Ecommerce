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

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

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
            
            if (userData) {
                setUser(userData);
                console.log('User authenticated:', userData);
                
                // Handle admin redirection if on inappropriate pages
                const currentPath = window.location.pathname;
                if (userData.roles.includes('Admin') && 
                    (currentPath === '/' || currentPath === '/login' || currentPath === '/register')) {
                    router.push('/admin/dashboard');
                }
            } else {
                setUser(null);
                console.log('No user data returned');
            }
        } catch (error: any) {
            console.error('Auth check failed:', error);
            setUser(null);
            console.log('User not authenticated - error occurred');
            
            // Only redirect to login if we're on a protected route
            const currentPath = window.location.pathname;
            const isProtectedRoute = !['/', '/login', '/register', '/cart'].includes(currentPath) && 
                                   !currentPath.startsWith('/product/') &&
                                   !currentPath.startsWith('/api');
            
            if (isProtectedRoute && !error.redirect) {
                router.push('/login');
            }
        } finally {
            setIsLoading(false);
        }
    }, [router]);

    useEffect(() => {
        console.log('AuthContext mounted - checking authentication');
        checkAuth();
    }, [checkAuth]);

    // Handle role-based redirects when user state changes
    useEffect(() => {
        if (!isLoading && user) {
            const currentPath = window.location.pathname;
            
            // Redirect admin users away from non-admin pages
            if (user.roles.includes('Admin') && 
                (currentPath === '/' || currentPath === '/login' || currentPath === '/register')) {
                router.push('/admin/dashboard');
            }
            // Redirect non-admin users away from admin pages
            else if (!user.roles.includes('Admin') && currentPath.startsWith('/admin')) {
                router.push('/');
            }
        }
    }, [user, isLoading, router]);

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
            
            // Redirect admin users to admin dashboard
            if (result.user.roles.includes('Admin')) {
                router.push('/admin/dashboard');
            } else {
                router.push('/');
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
            
            // Redirect admin users to admin dashboard, regular users to home
            if (result.user.roles.includes('Admin')) {
                router.push('/admin/dashboard');
            } else {
                router.push('/');
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
        try {
            await logoutAction();
            localStorage.removeItem('accessToken');
            setUser(null);
            router.push('/login');
        } catch (error) {
            console.error('Logout error:', error);
            // Even if logout fails, clear local state and redirect
            localStorage.removeItem('accessToken');
            setUser(null);
            router.push('/login');
        }
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