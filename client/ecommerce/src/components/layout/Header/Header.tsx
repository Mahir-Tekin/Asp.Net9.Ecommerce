'use client';

import Link from 'next/link';
import { useAuth } from '@/context/AuthContext';
import { useRouter } from 'next/navigation';
import { useProductFilters } from '@/context/ProductFilterContext';

export default function Header() {
  const { user, logout } = useAuth();
  const router = useRouter();
  const { filters, setFilters } = useProductFilters();

  const handleLogout = async () => {
    await logout();
    router.push('/login');
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFilters((prev: any) => ({ ...prev, searchTerm: e.target.value }));
  };

  return (
    <header className="fixed top-0 left-0 right-0 h-16 bg-white shadow-sm z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-full">
        <div className="flex justify-between items-center h-full">
          {/* Logo/Home Link */}
          <Link href="/" className="text-xl font-bold text-gray-800">
            E-Commerce
          </Link>

          {/* Search Bar */}
          <div className="flex-1 flex justify-center mx-8">
            <input
              type="text"
              placeholder="Search products..."
              value={filters.searchTerm}
              onChange={handleSearchChange}
              className="w-full max-w-md px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Cart Link */}
          <Link
            href="/cart"
            className="relative text-gray-600 hover:text-gray-900 px-4 py-2 text-sm font-medium"
          >
            Cart
          </Link>

          {/* Auth Buttons */}
          <div className="flex items-center space-x-4">
            {user ? (
              <div className="flex items-center space-x-4">
                <span className="text-gray-600">{user.email}</span>
                {user.roles.includes('Admin') && (
                  <Link
                    href="/dashboard"
                    className="text-gray-600 hover:text-gray-900"
                  >
                    Dashboard
                  </Link>
                )}
                <button
                  onClick={handleLogout}
                  className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  Logout
                </button>
              </div>
            ) : (
              <>
                <Link
                  href="/login"
                  className="text-gray-600 hover:text-gray-900 px-4 py-2 text-sm font-medium"
                >
                  Login
                </Link>
                <Link
                  href="/register"
                  className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  Register
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}