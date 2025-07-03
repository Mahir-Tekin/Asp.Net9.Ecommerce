'use client';

import { useState } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import Header from "@/components/layout/Header/Header";
import Sidebar from "@/components/layout/Sidebar/Sidebar";
import Footer from "@/components/layout/Footer/Footer";
import ShopBreadcrumb from "@/components/shop/breadcrumb/ShopBreadcrumb";
import CategoryBar from "@/components/home/CategoryBar";
import { HiAdjustmentsHorizontal } from 'react-icons/hi2';

export default function ShopLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { isFiltered } = useURLFilters();
  const [showMobileSidebar, setShowMobileSidebar] = useState(false);

  return (
    <div className="min-h-screen">
      <Header />
      <div className="pt-[70px] overflow-visible">
        {!isFiltered && <CategoryBar />}
        <div className="flex">
          {isFiltered && (
            <Sidebar 
              isOpen={showMobileSidebar}
              onClose={() => setShowMobileSidebar(false)}
            />
          )}
          <main className="flex-1 bg-gray-50">
            {/* Mobile Filter Button */}
            {isFiltered && (
              <div className="md:hidden bg-white border-b border-gray-200 p-4">
                <button
                  onClick={() => setShowMobileSidebar(true)}
                  className="flex items-center gap-2 px-4 py-2 text-gray-700 hover:text-gray-900 hover:bg-gray-50 rounded-lg transition-colors border border-gray-200"
                >
                  <HiAdjustmentsHorizontal className="w-5 h-5" />
                  <span className="font-medium">Filters</span>
                </button>
              </div>
            )}
            <div className="p-6">
              {children}
            </div>
          </main>
        </div>
      </div>
      <Footer />
    </div>
  );
}