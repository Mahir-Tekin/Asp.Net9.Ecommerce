import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

// Define path configurations
const PUBLIC_PATHS = ['/', '/login', '/register','/cart'];
const PUBLIC_DYNAMIC_PATHS = [/^\/product\/[^/]+$/]; // Add dynamic product details route
const ADMIN_PATHS = ['/admin', '/admin/dashboard'];
const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

export async function middleware(request: NextRequest) {
    const path = request.nextUrl.pathname;
    
    // Check if current path is public or admin
    const isPublicPath = PUBLIC_PATHS.includes(path) || 
                        PUBLIC_DYNAMIC_PATHS.some((re) => re.test(path)) ||
                        path.startsWith('/api') || 
                        path.startsWith('/_next') ||
                        path === '/favicon.ico';
    const isAdminPath = ADMIN_PATHS.some(adminPath => path.startsWith(adminPath));
    
    // Get the token from cookies
    const token = request.cookies.get('accessToken')?.value;

    // Allow public paths without token
    if (isPublicPath) {
        if (token) {
            // Check user role
            try {
                const response = await fetch(`${API_URL}/Auth/me`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    const userData = await response.json();
                    if (userData.roles.includes('Admin') && (path === '/' || path === '/login' || path === '/register')) {
                        return NextResponse.redirect(new URL('/admin/products', request.url));
                    }
                }
            } catch (error) {
                // Ignore error, just proceed
            }
            // Only redirect to home if trying to access login/register while logged in (non-admin)
            if (path === '/login' || path === '/register') {
                return NextResponse.redirect(new URL('/', request.url));
            }
        }
        return NextResponse.next();
    }

    // Check authentication for protected routes
    if (!token) {
        return NextResponse.redirect(new URL('/login', request.url));
    }

    // Additional role check for admin paths
    if (isAdminPath) {
        try {
            const response = await fetch(`${API_URL}/Auth/me`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            
            if (!response.ok) {
                return NextResponse.redirect(new URL('/login', request.url));
            }

            const userData = await response.json();
            
            if (!userData.roles.includes('Admin')) {
                // If user is authenticated but not admin, redirect to home
                return NextResponse.redirect(new URL('/', request.url));
            }
        } catch (error) {
            console.error('Error checking user role:', error);
            return NextResponse.redirect(new URL('/login', request.url));
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