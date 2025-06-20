'use client';

import { useAuth } from '@/context/AuthContext';
import { useRouter } from 'next/navigation';

export default function AdminHeader() {
  const { user, logout } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    await logout();
    router.push('/login');
  };

  return (
    <header className="w-full h-16 bg-gray-900 flex items-center justify-between px-8 shadow text-white">
      <div className="text-xl font-bold">Admin Dashboard</div>
      <div className="flex items-center gap-4">
        <span className="text-gray-300">{user?.email}</span>
        <button
          onClick={handleLogout}
          className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-md text-sm font-medium"
        >
          Logout
        </button>
      </div>
    </header>
  );
} 