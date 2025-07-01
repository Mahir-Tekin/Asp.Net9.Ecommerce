'use client';

import { useURLFilters } from '@/hooks/useURLFilters';
import Header from "@/components/layout/Header/Header";
import Sidebar from "@/components/layout/Sidebar/Sidebar";
import Footer from "@/components/layout/Footer/Footer";
import ShopBreadcrumb from "@/components/shop/breadcrumb/ShopBreadcrumb";
import CategoryBar from "@/components/home/CategoryBar";

export default function ShopLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { isFiltered } = useURLFilters();

  return (
    <div className="min-h-screen">
      <Header />
      <div className="pt-[70px] overflow-visible">
        {!isFiltered && <CategoryBar />}
        <div className="flex">
          {isFiltered && <Sidebar />}
          <main className="flex-1 bg-gray-50 p-6">
            {children}
          </main>
        </div>
      </div>
      <Footer />
    </div>
  );
}