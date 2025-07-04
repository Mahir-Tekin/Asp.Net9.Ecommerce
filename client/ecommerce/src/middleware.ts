import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

// Define path configurations
const PUBLIC_PATHS = ['/', '/login', '/register'];
const PUBLIC_DYNAMIC_PATHS = [/^\/product\/[^/]+$/]; // Dynamic product details route
const ADMIN_PATHS = ['/admin'];
const PROTECTED_PATHS = ['/checkout', '/my-orders', '/favorites']; // Paths that require authentication
const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

// Helper function to verify user role
async function verifyUserRole(token: string): Promise<{ isValid: boolean; isAdmin: boolean; userData?: any }> {
    try {
        const response = await fetch(`${API_URL}/api/Auth/me`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            return { isValid: false, isAdmin: false };
        }

        const userData = await response.json();
        return {
            isValid: true,
            isAdmin: userData.roles?.includes('Admin') || false,
            userData
        };
    } catch (error) {
        console.error('Error verifying user role:', error);
        return { isValid: false, isAdmin: false };
    }
}

export async function middleware(request: NextRequest) {
    const path = request.nextUrl.pathname;
    
    // Check if current path is public, protected, or admin
    const isPublicPath = PUBLIC_PATHS.includes(path) || 
                        PUBLIC_DYNAMIC_PATHS.some((re) => re.test(path)) ||
                        path.startsWith('/api') || 
                        path.startsWith('/_next') ||
                        path === '/favicon.ico' ||
                        path === '/cart'; // Cart is accessible to everyone
    const isAdminPath = ADMIN_PATHS.some(adminPath => path.startsWith(adminPath));
    const isProtectedPath = PROTECTED_PATHS.some(protectedPath => path.startsWith(protectedPath));
    
    // Get the token from cookies (for server-side verification)
    const token = request.cookies.get('accessToken')?.value;

    // Handle public paths
    if (isPublicPath) {
        if (token) {
            // Verify token and check user role
            const { isValid, isAdmin } = await verifyUserRole(token);
            
            if (isValid && isAdmin) {
                // Redirect admin users from public pages to admin dashboard
                if (path === '/' || path === '/login' || path === '/register') {
                    return NextResponse.redirect(new URL('/admin/dashboard', request.url));
                }
            } else if (isValid) {
                // Redirect authenticated non-admin users away from auth pages
                if (path === '/login' || path === '/register') {
                    return NextResponse.redirect(new URL('/', request.url));
                }
            }
            // If token is invalid, let them access public pages (will be handled by AuthContext)
        }
        return NextResponse.next();
    }

    // Handle protected paths - redirect to login if not authenticated
    if (isProtectedPath && !token) {
        const redirectUrl = new URL('/login', request.url);
        redirectUrl.searchParams.set('redirect', path);
        return NextResponse.redirect(redirectUrl);
    }

    // Handle admin paths
    if (isAdminPath) {
        if (!token) {
            return NextResponse.redirect(new URL('/login', request.url));
        }
        
        const { isValid, isAdmin } = await verifyUserRole(token);
        
        if (!isValid) {
            return NextResponse.redirect(new URL('/login', request.url));
        }

        if (!isAdmin) {
            // If user is authenticated but not admin, redirect to home
            return NextResponse.redirect(new URL('/', request.url));
        }
    }

    // Allow the request to proceed
    return NextResponse.next();
}

// See "Matching Paths" below to learn more
export const config = {
    matcher: [
        /*
         * Match all request paths except for the ones starting with:
         * - api (API routes)
         * - _next/static (static files)
         * - _next/image (image optimization files)
         * - favicon.ico (favicon file)
         */
        '/((?!api|_next/static|_next/image|favicon.ico).*)',
    ],
}