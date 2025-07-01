'use client';

import React from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';

const sortOptions = [
  { value: '', label: 'Default' },
  { value: 'PriceAsc', label: 'Price: Low to High' },
  { value: 'PriceDesc', label: 'Price: High to Low' },
  { value: 'CreatedAtDesc', label: 'Newest First' },
  { value: 'CreatedAtAsc', label: 'Oldest First' },
  { value: 'RatingDesc', label: 'Rating: High to Low' },
  { value: 'RatingAsc', label: 'Rating: Low to High' },
];

export default function ProductSortDropdown() {
  const { filters, updateFilters } = useURLFilters();

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    updateFilters({ sortBy: e.target.value });
  };

  return (
    <div className="flex items-center gap-2">
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
