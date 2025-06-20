'use client';

import { useAuth } from '@/context/AuthContext';
import AdminHeader from '@/components/layout/Header/AdminHeader';
import AdminSidebar from '@/components/layout/Sidebar/AdminSidebar';

export default function AdminLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    const { user } = useAuth();

    return (
        <div className="min-h-screen bg-gray-100">
            {/* Admin Header */}
            <AdminHeader />

            {/* Admin Content */}
            <div className="flex">
                {/* Sidebar */}
                <AdminSidebar />

                {/* Main Content */}
                <main className="flex-1 p-8">
                    {children}
                </main>
            </div>
        </div>
    );
} 