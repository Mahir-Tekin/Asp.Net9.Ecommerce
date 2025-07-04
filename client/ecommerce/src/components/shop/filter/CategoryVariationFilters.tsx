'use client';

import React, { useEffect, useState } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import { fetchCategoryDetails, CategoryDetails, VariationType } from './CategoryDetails.api';

const CategoryVariationFilters: React.FC = () => {
  const { filters, updateFilters } = useURLFilters();
  const [categoryDetails, setCategoryDetails] = useState<CategoryDetails | null>(null);
  const [loading, setLoading] = useState(false);

  // Fetch category details when categoryId changes
  useEffect(() => {
    // Only fetch category details if categoryId exists and is not "all"
    if (filters.categoryId && filters.categoryId !== 'all') {
      setLoading(true);
      fetchCategoryDetails(filters.categoryId)
        .then(setCategoryDetails)
        .catch((error) => {
          console.error('Failed to fetch category details:', error);
          setCategoryDetails(null);
        })
        .finally(() => setLoading(false));
    } else {
      setCategoryDetails(null);
    }
  }, [filters.categoryId]);

  // Handle variation filter changes using variation type names and option values
  // This matches the backend API format: ?color=red,blue&size=M,L

  const handleVariationFilterChange = (variationTypeName: string, optionValue: string, checked: boolean) => {
    const currentFilters = filters.variationFilters || {};
    const currentOptions = currentFilters[variationTypeName] || [];
    
    let newOptions: string[];
    if (checked) {
      newOptions = [...currentOptions, optionValue];
    } else {
      newOptions = currentOptions.filter((value: string) => value !== optionValue);
    }

    const newVariationFilters = {
      ...currentFilters,
      [variationTypeName]: newOptions
    };

    // Remove empty arrays to keep the object clean
    if (newOptions.length === 0) {
      delete newVariationFilters[variationTypeName];
    }

    updateFilters({ variationFilters: newVariationFilters });
  };

  if (!filters.categoryId) {
    return null; // Don't show filters if no category is selected
  }

  if (loading) {
    return <div className="mb-4 text-sm text-gray-500">Loading filters...</div>;
  }

  if (!categoryDetails || !categoryDetails.variationTypes.length) {
    return null; // Don't show if no variation types available
  }

  return (
    <div className="space-y-4">
      {categoryDetails.variationTypes.map((variationType: VariationType) => (
        <div key={variationType.id} className="border-b border-gray-200 pb-4 last:border-b-0">
          <h5 className="text-sm font-semibold mb-3 text-gray-800">{variationType.name}</h5>
          <div className="space-y-2">
            {variationType.options.map((option) => {
              const isChecked = filters.variationFilters?.[variationType.name]?.includes(option.value) || false;
              return (
                <label key={option.id} className="flex items-center space-x-3 text-sm cursor-pointer hover:bg-gray-50 p-1 rounded">
                  <input
                    type="checkbox"
                    checked={isChecked}
                    onChange={(e) => handleVariationFilterChange(
                      variationType.name, 
                      option.value, 
                      e.target.checked
                    )}
                    className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <span className="text-gray-700">{option.displayValue}</span>
                </label>
              );
            })}
          </div>
        </div>
      ))}
    </div>
  );
};

export default CategoryVariationFilters;
