'use client';

import { useState, useEffect } from 'react';
import Categories from '@/components/shop/Categories/Categories';
import ProductFiltersPanel from '@/components/shop/filter/ProductFiltersPanel';
import { HiXMark, HiAdjustmentsHorizontal } from 'react-icons/hi2';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
  className?: string;
}

export default function Sidebar({ isOpen = true, onClose, className = '' }: SidebarProps) {
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };
    
    checkMobile();
    window.addEventListener('resize', checkMobile);
    return () => window.removeEventListener('resize', checkMobile);
  }, []);

  // Prevent body scroll when mobile sidebar is open
  useEffect(() => {
    if (isMobile && isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
    
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, [isMobile, isOpen]);

  const sidebarContent = (
    <div className="p-4 h-full">
      {/* Mobile Header */}
      {isMobile && (
        <div className="flex items-center justify-between mb-4 pb-4 border-b border-gray-200">
          <div className="flex items-center gap-2">
            <HiAdjustmentsHorizontal className="w-5 h-5 text-gray-600" />
            <h2 className="text-lg font-semibold text-gray-800">Filters</h2>
          </div>
          {onClose && (
            <button
              onClick={onClose}
              className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <HiXMark className="w-5 h-5 text-gray-600" />
            </button>
          )}
        </div>
      )}

      {/* Desktop Header */}
      {!isMobile && (
        <h2 className="text-lg font-semibold mb-4 text-gray-800">Categories</h2>
      )}
      
      <div className="space-y-6">
        <Categories />
        <ProductFiltersPanel />
      </div>
    </div>
  );

  if (isMobile) {
    return (
      <>
        {/* Mobile Overlay */}
        {isOpen && (
          <div 
            className="fixed inset-0 bg-black/50 z-40"
            onClick={onClose}
          />
        )}
        
        {/* Mobile Sidebar */}
        <aside 
          className={`fixed top-[70px] left-0 w-80 h-[calc(100vh-70px)] bg-white shadow-xl z-50 transform transition-transform duration-300 overflow-y-auto ${
            isOpen ? 'translate-x-0' : '-translate-x-full'
          } ${className}`}
        >
          {sidebarContent}
        </aside>
      </>
    );
  }

  // Desktop Sidebar - always visible when not mobile
  return (
    <aside className={`w-64 bg-white h-[calc(100vh-70px)] shadow-md overflow-y-auto border-r border-gray-200 sticky top-[70px] ${className}`}>
      {sidebarContent}
    </aside>
  );
}