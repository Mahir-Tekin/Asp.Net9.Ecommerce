'use client';

import React from 'react';
import { useProductFilters } from '@/context/ProductFilterContext';

const sortOptions = [
  { value: '', label: 'Default' },
  { value: 'PriceAsc', label: 'Price: Low to High' },
  { value: 'PriceDesc', label: 'Price: High to Low' },
  { value: 'CreatedAtDesc', label: 'Newest First' },
  { value: 'CreatedAtAsc', label: 'Oldest First' },
];

export default function ProductSortDropdown() {
  const { filters, setFilters } = useProductFilters();

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setFilters((prev) => ({
      ...prev,
      sortBy: e.target.value,
    }));
  };

  return (
    <div className="flex items-center gap-2 mb-4">
      <label htmlFor="sort" className="text-sm font-medium text-gray-700">
        Sort by:
      </label>
      <select
        id="sort"
        value={filters.sortBy || ''}
        onChange={handleSortChange}
        className="border border-gray-300 rounded-md px-3 py-1 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
      >
        {sortOptions.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </div>
  );
}
