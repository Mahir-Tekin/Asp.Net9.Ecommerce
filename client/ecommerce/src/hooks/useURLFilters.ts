'use client';

import { useRouter, useSearchParams, usePathname } from 'next/navigation';
import { useMemo, useCallback } from 'react';

// Type for product filters
export interface ProductFilters {
  searchTerm: string;
  categoryId: string | null;
  minPrice?: number | '';
  maxPrice?: number | '';
  variationFilters?: { [variationTypeName: string]: string[] };
  sortBy?: string;
  [key: string]: any;
}

export function useURLFilters() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();

  // Parse filters from current URL
  const filters = useMemo((): ProductFilters => {
    const urlFilters: ProductFilters = {
      searchTerm: searchParams.get('search') || '',
      categoryId: searchParams.get('category'),
      minPrice: searchParams.get('minPrice') ? Number(searchParams.get('minPrice')) : '',
      maxPrice: searchParams.get('maxPrice') ? Number(searchParams.get('maxPrice')) : '',
      variationFilters: {},
      sortBy: searchParams.get('sort') || '',
    };

    // Parse variation filters from URL
    searchParams.forEach((value, key) => {
      if (key.startsWith('variation_')) {
        const variationName = key.replace('variation_', '');
        urlFilters.variationFilters = urlFilters.variationFilters || {};
        urlFilters.variationFilters[variationName] = value.split(',');
      }
    });

    return urlFilters;
  }, [searchParams]);

  // Check if we have any filters that should show product list instead of homepage
  const isFiltered = useMemo(() => {
    return (
      (typeof filters.searchTerm === 'string' && filters.searchTerm.trim() !== '') ||
      (typeof filters.categoryId === 'string' && filters.categoryId !== null && filters.categoryId.trim() !== '')
    );
  }, [filters.searchTerm, filters.categoryId]);

  // Function to update URL with new filters
  const updateFilters = useCallback((newFilters: Partial<ProductFilters>) => {
    const params = new URLSearchParams();
    
    // Merge current filters with new ones
    const updatedFilters = { ...filters, ...newFilters };
    
    // Add non-empty filters to URL
    if (updatedFilters.searchTerm?.trim()) {
      params.set('search', updatedFilters.searchTerm.trim());
    }
    
    if (updatedFilters.categoryId?.trim()) {
      params.set('category', updatedFilters.categoryId.trim());
    }
    
    if (updatedFilters.minPrice !== '' && updatedFilters.minPrice !== undefined && Number(updatedFilters.minPrice) > 0) {
      params.set('minPrice', updatedFilters.minPrice.toString());
    }
    
    if (updatedFilters.maxPrice !== '' && updatedFilters.maxPrice !== undefined && Number(updatedFilters.maxPrice) > 0) {
      params.set('maxPrice', updatedFilters.maxPrice.toString());
    }
    
    if (updatedFilters.sortBy?.trim()) {
      params.set('sort', updatedFilters.sortBy.trim());
    }
    
    // Add variation filters
    if (updatedFilters.variationFilters) {
      Object.entries(updatedFilters.variationFilters).forEach(([variationName, values]) => {
        if (values && values.length > 0) {
          params.set(`variation_${variationName}`, values.join(','));
        }
      });
    }
    
    const newURL = params.toString() ? `${pathname}?${params.toString()}` : pathname;
    router.replace(newURL, { scroll: false });
  }, [filters, pathname, router]);

  // Function to remove a specific filter
  const removeFilter = useCallback((filterType: string, variationName?: string, valueToRemove?: string) => {
    const updatedFilters = { ...filters };
    
    switch (filterType) {
      case 'searchTerm':
        updatedFilters.searchTerm = '';
        break;
      case 'categoryId':
        updatedFilters.categoryId = null;
        break;
      case 'minPrice':
        updatedFilters.minPrice = '';
        break;
      case 'maxPrice':
        updatedFilters.maxPrice = '';
        break;
      case 'sortBy':
        updatedFilters.sortBy = '';
        break;
      case 'variation':
        if (variationName && valueToRemove && updatedFilters.variationFilters) {
          const updatedVariationFilters = { ...updatedFilters.variationFilters };
          if (updatedVariationFilters[variationName]) {
            updatedVariationFilters[variationName] = updatedVariationFilters[variationName].filter(
              (value) => value !== valueToRemove
            );
            if (updatedVariationFilters[variationName].length === 0) {
              delete updatedVariationFilters[variationName];
            }
          }
          updatedFilters.variationFilters = updatedVariationFilters;
        }
        break;
    }
    
    updateFilters(updatedFilters);
  }, [filters, updateFilters]);

  // Function to clear all filters
  const clearAllFilters = useCallback(() => {
    router.replace(pathname, { scroll: false });
  }, [pathname, router]);

  return {
    filters,
    isFiltered,
    updateFilters,
    removeFilter,
    clearAllFilters
  };
}
