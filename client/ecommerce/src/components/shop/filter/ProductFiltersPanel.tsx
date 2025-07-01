'use client';
import React from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import CategoryVariationFilters from './CategoryVariationFilters';

const ProductFiltersPanel: React.FC = () => {
  const { filters, updateFilters } = useURLFilters();

  const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    console.log('Price change event:', e.target.name, e.target.value);
    const { name, value } = e.target;
    updateFilters({
      [name]: value === '' ? '' : Number(value),
    });
  };

  return (
    <div className="space-y-6 mt-6">
      <h3 className="text-lg font-semibold text-gray-800 border-b border-gray-200 pb-2">Filters</h3>
      
      {/* Price Range Filter */}
      <div>
        <label className="block text-sm font-semibold mb-3 text-gray-800">Price Range</label>
        <div className="flex gap-2 items-center">
          <input
            type="number"
            name="minPrice"
            value={filters.minPrice ?? ''}
            onChange={handlePriceChange}
            placeholder="Min"
            className="w-20 border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            min={0}
          />
          <span className="text-gray-500">-</span>
          <input
            type="number"
            name="maxPrice"
            value={filters.maxPrice ?? ''}
            onChange={handlePriceChange}
            placeholder="Max"
            className="w-20 border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            min={0}
          />
        </div>
      </div>

      {/* Dynamic Category-Based Variation Filters */}
      <CategoryVariationFilters />
    </div>
  );
};

export default ProductFiltersPanel;
