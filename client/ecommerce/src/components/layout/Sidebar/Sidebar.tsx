'use client';

import { useState } from 'react';
import Categories from '@/components/shop/Categories/Categories';
import ProductFiltersPanel from '@/components/shop/filter/ProductFiltersPanel';

export default function Sidebar() {
  const [isOpen, setIsOpen] = useState(true);

  return (
    <aside className="w-64 bg-white h-[calc(100vh-70px)] shadow-md overflow-y-auto border-r border-gray-200 sticky top-[70px]">
      <div className="p-4">
        
        <h2 className="text-lg font-semibold mb-4">Categories</h2>
        <Categories />
        <ProductFiltersPanel />
      </div>
    </aside>
  );
}