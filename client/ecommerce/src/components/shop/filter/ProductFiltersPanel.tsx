'use client';
import React from 'react';
import { useProductFilters } from '@/context/ProductFilterContext';
import CategoryVariationFilters from './CategoryVariationFilters';

const ProductFiltersPanel: React.FC = () => {
  const { filters, setFilters } = useProductFilters();

  const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    console.log('Price change event:', e.target.name, e.target.value);
    const { name, value } = e.target;
    setFilters((prev) => ({
      ...prev,
      [name]: value === '' ? '' : Number(value),
    }));
  };

  return (
    <aside className="mb-6 p-4 bg-gray-50 rounded shadow">
      <h3 className="text-lg font-semibold mb-4">Filters</h3>
      
      {/* Price Range Filter */}
      <div className="mb-4">
        <label className="block text-sm font-medium mb-1">Price Range</label>
        <div className="flex gap-2 items-center">
          <input
            type="number"
            name="minPrice"
            value={filters.minPrice ?? ''}
            onChange={handlePriceChange}
            placeholder="Min"
            className="w-20 border rounded px-2 py-1"
            min={0}
          />
          <span>-</span>
          <input
            type="number"
            name="maxPrice"
            value={filters.maxPrice ?? ''}
            onChange={handlePriceChange}
            placeholder="Max"
            className="w-20 border rounded px-2 py-1"
            min={0}
          />
        </div>
      </div>

      {/* Dynamic Category-Based Variation Filters */}
      <CategoryVariationFilters />
    </aside>
  );
};

export default ProductFiltersPanel;
