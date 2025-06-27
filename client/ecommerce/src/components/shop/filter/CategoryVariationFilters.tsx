'use client';

import React, { useEffect, useState } from 'react';
import { useProductFilters } from '@/context/ProductFilterContext';
import { fetchCategoryDetails, CategoryDetails, VariationType } from './CategoryDetails.api';

const CategoryVariationFilters: React.FC = () => {
  const { filters, setFilters } = useProductFilters();
  const [categoryDetails, setCategoryDetails] = useState<CategoryDetails | null>(null);
  const [loading, setLoading] = useState(false);

  // Fetch category details when categoryId changes
  useEffect(() => {
    if (filters.categoryId) {
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

  // Reset variation filters when category changes
  useEffect(() => {
    setFilters(prev => ({
      ...prev,
      variationFilters: {}
    }));
  }, [filters.categoryId, setFilters]);

  // Handle variation filter changes using variation type names and option values
  // This matches the backend API format: ?color=red,blue&size=M,L

  const handleVariationFilterChange = (variationTypeName: string, optionValue: string, checked: boolean) => {
    setFilters(prev => {
      const currentFilters = prev.variationFilters || {};
      const currentOptions = currentFilters[variationTypeName] || [];
      
      let newOptions: string[];
      if (checked) {
        newOptions = [...currentOptions, optionValue];
      } else {
        newOptions = currentOptions.filter(value => value !== optionValue);
      }

      const newVariationFilters = {
        ...currentFilters,
        [variationTypeName]: newOptions
      };

      // Remove empty arrays to keep the object clean
      if (newOptions.length === 0) {
        delete newVariationFilters[variationTypeName];
      }

      return {
        ...prev,
        variationFilters: newVariationFilters
      };
    });
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
    <div className="mb-4">
      <h4 className="text-md font-medium mb-3">Category Filters</h4>
      {categoryDetails.variationTypes.map((variationType: VariationType) => (
        <div key={variationType.id} className="mb-4">
          <h5 className="text-sm font-medium mb-2">{variationType.name}</h5>
          <div className="space-y-2">
            {variationType.options.map((option) => {
              const isChecked = filters.variationFilters?.[variationType.name]?.includes(option.value) || false;
              return (
                <label key={option.id} className="flex items-center space-x-2 text-sm">
                  <input
                    type="checkbox"
                    checked={isChecked}
                    onChange={(e) => handleVariationFilterChange(
                      variationType.name, 
                      option.value, 
                      e.target.checked
                    )}
                    className="rounded border-gray-300"
                  />
                  <span>{option.displayValue}</span>
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
