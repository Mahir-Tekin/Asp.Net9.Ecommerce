'use client';

import Link from 'next/link';
import { useAuth } from '@/context/AuthContext';
import { useCart } from '@/context/CartContext';
import { useRouter } from 'next/navigation';
import { useURLFilters } from '@/hooks/useURLFilters';
import { HiShoppingBag, HiChartBarSquare, HiHeart, HiClipboardDocumentList, HiArrowRightOnRectangle, HiChevronDown, HiUser, HiBars3, HiXMark, HiMagnifyingGlass } from 'react-icons/hi2';
import { useState, useRef, useEffect } from 'react';

export default function Header() {
  const { user, logout } = useAuth();
  const { cartCount } = useCart();
  const router = useRouter();
  const { filters, updateFilters } = useURLFilters();
  const [showUserDropdown, setShowUserDropdown] = useState(false);
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  const [favoriteCount, setFavoriteCount] = useState(0);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const mobileMenuRef = useRef<HTMLDivElement>(null);

  // Get favorite count from localStorage
  useEffect(() => {
    const updateFavoriteCount = () => {
      const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
      setFavoriteCount(wishlist.length);
    };
    
    updateFavoriteCount();
    // Listen for storage changes to update count
    window.addEventListener('storage', updateFavoriteCount);
    
    return () => window.removeEventListener('storage', updateFavoriteCount);
  }, []);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setShowUserDropdown(false);
      }
      if (mobileMenuRef.current && !mobileMenuRef.current.contains(event.target as Node)) {
        setShowMobileMenu(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Close mobile menu when screen size changes
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768) {
        setShowMobileMenu(false);
      }
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const handleLogout = async () => {
    await logout();
    setShowUserDropdown(false);
    setShowMobileMenu(false);
    router.push('/login');
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    updateFilters({ searchTerm: e.target.value });
  };

  return (
    <>
      <header className="fixed top-0 left-0 right-0 h-[70px] bg-gradient-to-r from-purple-600 via-blue-600 to-purple-700 backdrop-blur-md shadow-xl z-50 border-b border-white/10">
        <div className="max-w-7xl mx-auto h-full px-4 flex items-center justify-between gap-4">
          {/* Mobile Menu Button */}
          <button
            onClick={() => setShowMobileMenu(!showMobileMenu)}
            className="md:hidden p-2 text-white hover:bg-white/20 rounded-lg transition-colors flex-shrink-0"
          >
            {showMobileMenu ? <HiXMark className="w-6 h-6" /> : <HiBars3 className="w-6 h-6" />}
          </button>

          {/* Logo/Home Link - Hidden on small devices */}
          <Link 
            href="/" 
            className="hidden sm:block text-2xl font-bold text-white transition-all duration-300 hover:scale-105 hover:drop-shadow-lg flex-shrink-0"
          >
            E Store
          </Link>

          {/* Desktop Search Bar */}
          <div className="hidden md:flex flex-1 max-w-lg mx-4">
            <input
              type="text"
              placeholder="Search for amazing products..."
              value={filters.searchTerm}
              onChange={handleSearchChange}
              className="w-full px-5 py-3 rounded-full bg-white/90 backdrop-blur-sm border-0 outline-none text-gray-800 placeholder-gray-600 shadow-lg transition-all duration-300 focus:bg-white focus:shadow-xl focus:-translate-y-0.5"
            />
          </div>

          {/* Mobile Search Bar - Takes full space on small devices */}
          <div className="md:hidden flex-1 mx-2">
            <input
              type="text"
              placeholder="Search products..."
              value={filters.searchTerm}
              onChange={handleSearchChange}
              className="w-full px-4 py-2.5 rounded-full bg-white/90 backdrop-blur-sm border-0 outline-none text-gray-800 placeholder-gray-600 shadow-lg transition-all duration-300 focus:bg-white focus:shadow-xl text-sm"
            />
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-3 flex-shrink-0">
            {/* Cart Link */}
            <Link 
              href="/cart" 
              className="flex items-center gap-2 px-3 py-2 text-white font-medium rounded-lg bg-white/10 backdrop-blur-sm transition-all duration-300 hover:bg-white/20 hover:-translate-y-0.5 relative"
            >
              <HiShoppingBag className="w-5 h-5" />
              <span className="hidden lg:block">Cart</span>
              {cartCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-bold shadow-lg">
                  {cartCount > 99 ? '99+' : cartCount}
                </span>
              )}
            </Link>

            {/* Auth Section */}
            <div className="flex items-center gap-2">
              {user ? (
                <>
                  {user.roles.includes('Admin') && (
                    <Link 
                      href="/admin/dashboard" 
                      className="flex items-center gap-2 px-3 py-2 text-white rounded-lg bg-white/10 backdrop-blur-sm transition-all duration-300 hover:bg-white/20"
                    >
                      <HiChartBarSquare className="w-5 h-5" />
                      <span className="hidden lg:block">Dashboard</span>
                    </Link>
                  )}
                  
                  {/* User Dropdown */}
                  <div className="relative" ref={dropdownRef}>
                    <button
                      onClick={() => setShowUserDropdown(!showUserDropdown)}
                      className="flex items-center gap-2 px-4 py-2 text-white rounded-lg bg-white/10 backdrop-blur-sm transition-all duration-300 hover:bg-white/20 hover:-translate-y-0.5"
                    >
                      <HiUser className="w-5 h-5" />
                      <span className="hidden lg:block max-w-[120px] truncate">
                        {user.email.split('@')[0]}
                      </span>
                      <HiChevronDown className={`w-4 h-4 transition-transform duration-200 ${showUserDropdown ? 'rotate-180' : ''}`} />
                    </button>

                    {/* Dropdown Menu */}
                    {showUserDropdown && (
                      <div className="absolute right-0 top-full mt-2 w-64 bg-white rounded-xl shadow-xl border border-gray-200 py-2 z-50">
                        {/* User Info */}
                        <div className="px-4 py-3 border-b border-gray-100">
                          <p className="font-semibold text-gray-800 truncate">{user.email}</p>
                          <p className="text-sm text-gray-500">
                            {user.roles.includes('Admin') ? 'Administrator' : 'Customer'}
                          </p>
                        </div>

                        {/* Menu Items */}
                        <div className="py-1">
                          <Link
                            href="/my-orders"
                            onClick={() => setShowUserDropdown(false)}
                            className="flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-50 transition-colors"
                          >
                            <HiClipboardDocumentList className="w-5 h-5 text-gray-500" />
                            <span>My Orders</span>
                          </Link>
                          
                          <Link
                            href="/favorites"
                            onClick={() => setShowUserDropdown(false)}
                            className="flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-50 transition-colors"
                          >
                            <HiHeart className="w-5 h-5 text-gray-500" />
                            <span>Favorites</span>
                            {favoriteCount > 0 && (
                              <span className="ml-auto bg-red-500 text-white text-xs px-2 py-1 rounded-full">
                                {favoriteCount}
                              </span>
                            )}
                          </Link>
                        </div>

                        {/* Logout */}
                        <div className="border-t border-gray-100 pt-1">
                          <button
                            onClick={handleLogout}
                            className="flex items-center gap-3 px-4 py-3 text-red-600 hover:bg-red-50 transition-colors w-full text-left"
                          >
                            <HiArrowRightOnRectangle className="w-5 h-5" />
                            <span>Logout</span>
                          </button>
                        </div>
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <>
                  <Link 
                    href="/login" 
                    className="px-4 py-2 text-white font-medium rounded-full bg-white/10 backdrop-blur-sm border border-white/20 transition-all duration-300 hover:bg-white/20 hover:-translate-y-0.5 whitespace-nowrap"
                  >
                    Login
                  </Link>
                  <Link 
                    href="/register" 
                    className="px-4 py-2 bg-gradient-to-r from-blue-400 to-cyan-400 text-white font-semibold rounded-full transition-all duration-300 hover:-translate-y-0.5 hover:shadow-lg hover:shadow-blue-400/25 whitespace-nowrap"
                  >
                    Register
                  </Link>
                </>
              )}
            </div>
          </nav>

          {/* Mobile Cart Icon */}
          <Link 
            href="/cart" 
            className="md:hidden flex items-center p-2 text-white hover:bg-white/20 rounded-lg transition-colors relative flex-shrink-0"
          >
            <HiShoppingBag className="w-6 h-6" />
            {cartCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-bold shadow-lg">
                {cartCount > 9 ? '9+' : cartCount}
              </span>
            )}
          </Link>
        </div>

        {/* Remove the separate mobile search dropdown since we now have inline search */}
      </header>

      {/* Mobile Menu Overlay */}
      {showMobileMenu && (
        <div className="md:hidden fixed inset-0 bg-black/50 z-40" onClick={() => setShowMobileMenu(false)} />
      )}

      {/* Mobile Menu */}
      {showMobileMenu && (
        <div 
          ref={mobileMenuRef}
          className="md:hidden fixed top-[70px] left-0 w-80 h-[calc(100vh-70px)] bg-white shadow-xl z-50 transform transition-transform duration-300 overflow-y-auto"
        >
          <div className="p-4">
            {user ? (
              <>
                {/* User Info */}
                <div className="bg-gray-50 rounded-lg p-4 mb-4">
                  <p className="font-semibold text-gray-800 truncate">{user.email}</p>
                  <p className="text-sm text-gray-500">
                    {user.roles.includes('Admin') ? 'Administrator' : 'Customer'}
                  </p>
                </div>

                {/* Admin Dashboard Link */}
                {user.roles.includes('Admin') && (
                  <Link
                    href="/admin/dashboard"
                    onClick={() => setShowMobileMenu(false)}
                    className="flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-50 transition-colors rounded-lg mb-2"
                  >
                    <HiChartBarSquare className="w-5 h-5 text-gray-500" />
                    <span>Dashboard</span>
                  </Link>
                )}

                {/* Menu Items */}
                <Link
                  href="/my-orders"
                  onClick={() => setShowMobileMenu(false)}
                  className="flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-50 transition-colors rounded-lg mb-2"
                >
                  <HiClipboardDocumentList className="w-5 h-5 text-gray-500" />
                  <span>My Orders</span>
                </Link>
                
                <Link
                  href="/favorites"
                  onClick={() => setShowMobileMenu(false)}
                  className="flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-50 transition-colors rounded-lg mb-2"
                >
                  <HiHeart className="w-5 h-5 text-gray-500" />
                  <span>Favorites</span>
                  {favoriteCount > 0 && (
                    <span className="ml-auto bg-red-500 text-white text-xs px-2 py-1 rounded-full">
                      {favoriteCount}
                    </span>
                  )}
                </Link>

                {/* Logout */}
                <div className="border-t border-gray-100 pt-4 mt-4">
                  <button
                    onClick={handleLogout}
                    className="flex items-center gap-3 px-4 py-3 text-red-600 hover:bg-red-50 transition-colors w-full text-left rounded-lg"
                  >
                    <HiArrowRightOnRectangle className="w-5 h-5" />
                    <span>Logout</span>
                  </button>
                </div>
              </>
            ) : (
              <>
                <Link 
                  href="/login" 
                  onClick={() => setShowMobileMenu(false)}
                  className="block w-full px-4 py-3 text-center text-gray-700 font-medium rounded-lg bg-gray-100 hover:bg-gray-200 transition-colors mb-3"
                >
                  Login
                </Link>
                <Link 
                  href="/register" 
                  onClick={() => setShowMobileMenu(false)}
                  className="block w-full px-4 py-3 text-center bg-gradient-to-r from-blue-400 to-cyan-400 text-white font-semibold rounded-lg transition-all duration-300 hover:shadow-lg"
                >
                  Register
                </Link>
              </>
            )}
          </div>
        </div>
      )}
    </>
  );
}