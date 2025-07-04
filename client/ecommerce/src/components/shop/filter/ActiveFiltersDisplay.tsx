'use client';

import React, { useState, useEffect } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import { HiXMark } from 'react-icons/hi2';
import { fetchCategories, Category } from '@/components/shop/Categories/Categories.api';

export default function ActiveFiltersDisplay() {
  const { filters, removeFilter, clearAllFilters, hasActiveFilters } = useURLFilters();
  const [categories, setCategories] = useState<Category[]>([]);
  const [categoriesLoading, setCategoriesLoading] = useState(false);

  // Fetch categories for display names
  useEffect(() => {
    async function loadCategories() {
      setCategoriesLoading(true);
      try {
        const data = await fetchCategories();
        setCategories(data);
      } catch (error) {
        console.error('Failed to fetch categories:', error);
      } finally {
        setCategoriesLoading(false);
      }
    }
    
    // Fetch categories on mount if not already loaded
    if (categories.length === 0) {
      loadCategories();
    }
  }, [categories.length]);

  // Flatten categories to include subcategories for easier searching
  const flattenCategories = (cats: Category[]): Category[] => {
    const result: Category[] = [];
    cats.forEach(cat => {
      result.push(cat);
      if (cat.subCategories && cat.subCategories.length > 0) {
        result.push(...flattenCategories(cat.subCategories));
      }
    });
    return result;
  };

  // Get category name by ID
  const getCategoryName = (categoryId: string) => {
    if (categoriesLoading) return 'Loading...';
    const flatCategories = flattenCategories(categories);
    const category = flatCategories.find(c => c.id === categoryId);
    return category ? category.name : `Category ${categoryId}`;
  };

  // Format sort option for display
  const formatSortOption = (sortBy: string) => {
    const sortOptions: { [key: string]: string } = {
      'price-asc': 'Price: Low to High',
      'price-desc': 'Price: High to Low',
      'name-asc': 'Name: A to Z',
      'name-desc': 'Name: Z to A',
      'newest': 'Newest First',
      'oldest': 'Oldest First',
      'rating-desc': 'Highest Rated',
      'rating-asc': 'Lowest Rated'
    };
    return sortOptions[sortBy] || sortBy;
  };

  // Don't render anything if there are no active filters
  if (!hasActiveFilters) {
    return null;
  }

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
      <div className="flex items-center justify-between mb-3">
        <h3 className="text-sm font-medium text-gray-900">Active Filters</h3>
        <button
          onClick={clearAllFilters}
          className="text-sm text-blue-600 hover:text-blue-700 font-medium transition-colors"
        >
          Clear All
        </button>
      </div>
      
      <div className="flex flex-wrap gap-2">
        {/* Search Term Filter */}
        {filters.searchTerm && (
          <div className="inline-flex items-center gap-1 bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-blue-200">
            <span>Search: "{filters.searchTerm}"</span>
            <button
              onClick={() => removeFilter('searchTerm')}
              className="hover:bg-blue-300 rounded-full p-0.5 transition-colors"
            >
              <HiXMark className="w-3 h-3" />
            </button>
          </div>
        )}

        {/* Category Filter */}
        {filters.categoryId && filters.categoryId !== 'all' && (
          <div className="inline-flex items-center gap-1 bg-green-100 text-green-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-green-200">
            <span>Category: {getCategoryName(filters.categoryId)}</span>
            <button
              onClick={() => removeFilter('categoryId')}
              className="hover:bg-green-300 rounded-full p-0.5 transition-colors"
            >
              <HiXMark className="w-3 h-3" />
            </button>
          </div>
        )}

        {/* Price Range Filters */}
        {filters.minPrice && (
          <div className="inline-flex items-center gap-1 bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-purple-200">
            <span>Min: ${filters.minPrice}</span>
            <button
              onClick={() => removeFilter('minPrice')}
              className="hover:bg-purple-300 rounded-full p-0.5 transition-colors"
            >
              <HiXMark className="w-3 h-3" />
            </button>
          </div>
        )}

        {filters.maxPrice && (
          <div className="inline-flex items-center gap-1 bg-purple-100 text-purple-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-purple-200">
            <span>Max: ${filters.maxPrice}</span>
            <button
              onClick={() => removeFilter('maxPrice')}
              className="hover:bg-purple-300 rounded-full p-0.5 transition-colors"
            >
              <HiXMark className="w-3 h-3" />
            </button>
          </div>
        )}

        {/* Sort Filter */}
        {filters.sortBy && (
          <div className="inline-flex items-center gap-1 bg-orange-100 text-orange-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-orange-200">
            <span>Sort: {formatSortOption(filters.sortBy)}</span>
            <button
              onClick={() => removeFilter('sortBy')}
              className="hover:bg-orange-300 rounded-full p-0.5 transition-colors"
            >
              <HiXMark className="w-3 h-3" />
            </button>
          </div>
        )}

        {/* Variation Filters */}
        {filters.variationFilters && Object.entries(filters.variationFilters).map(([variationName, values]) =>
          (values as string[]).map((value) => (
            <div
              key={`${variationName}-${value}`}
              className="inline-flex items-center gap-1 bg-gray-100 text-gray-800 px-3 py-1 rounded-full text-sm transition-all hover:bg-gray-200"
            >
              <span className="capitalize">{variationName}: {value}</span>
              <button
                onClick={() => removeFilter('variation', variationName, value)}
                className="hover:bg-gray-300 rounded-full p-0.5 transition-colors"
              >
                <HiXMark className="w-3 h-3" />
              </button>
            </div>
          ))
        )}
      </div>
      
      {/* Filter count summary */}
      <div className="mt-3 pt-3 border-t border-gray-200">
        <p className="text-xs text-gray-500">
          {Object.values({
            search: filters.searchTerm ? 1 : 0,
            category: (filters.categoryId && filters.categoryId !== 'all') ? 1 : 0,
            price: (filters.minPrice || filters.maxPrice) ? 1 : 0,
            sort: filters.sortBy ? 1 : 0,
            variations: filters.variationFilters ? Object.keys(filters.variationFilters).length : 0
          }).reduce((sum, count) => sum + count, 0)} active filter{Object.values({
            search: filters.searchTerm ? 1 : 0,
            category: (filters.categoryId && filters.categoryId !== 'all') ? 1 : 0,
            price: (filters.minPrice || filters.maxPrice) ? 1 : 0,
            sort: filters.sortBy ? 1 : 0,
            variations: filters.variationFilters ? Object.keys(filters.variationFilters).length : 0
          }).reduce((sum, count) => sum + count, 0) !== 1 ? 's' : ''}
        </p>
      </div>
    </div>
  );
}
