'use client';

// ProductFilterContext for managing product filters across the shop UI
import React, { createContext, useContext, useState, useMemo, useEffect, useRef } from 'react';
import { useRouter, useSearchParams, usePathname } from 'next/navigation';

// Type for product filters
export interface ProductFilters {
  searchTerm: string;
  categoryId: string | null;
  minPrice?: number | '';
  maxPrice?: number | '';
  variationFilters?: { [variationTypeName: string]: string[] }; // Dynamic variation filters
  sortBy?: string; // Sorting option
  [key: string]: any; // For dynamic filters in the future
}


// Context type for product filters
export interface ProductFilterContextType {
  filters: ProductFilters;
  setFilters: React.Dispatch<React.SetStateAction<ProductFilters>>;
  isFiltered: boolean;
}

const ProductFilterContext = createContext<ProductFilterContextType | undefined>(undefined);

export const ProductFilterProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();
  
  // Keep track of the last URL we pushed to prevent loops
  const lastPushedURL = useRef<string>('');
  
  const [filters, setFilters] = useState<ProductFilters>({
    searchTerm: '',
    categoryId: null,
    minPrice: '',
    maxPrice: '',
    variationFilters: {},
    sortBy: '', // Default no sorting
  });

  // Separate function to update URL without triggering state update
  const updateURL = (filtersToSync: ProductFilters) => {
    const params = new URLSearchParams();
    
    // Add non-empty filters to URL
    if (filtersToSync.searchTerm?.trim()) {
      params.set('search', filtersToSync.searchTerm.trim());
    }
    
    if (filtersToSync.categoryId?.trim()) {
      params.set('category', filtersToSync.categoryId.trim());
    }
    
    if (filtersToSync.minPrice !== '' && filtersToSync.minPrice !== undefined && Number(filtersToSync.minPrice) > 0) {
      params.set('minPrice', filtersToSync.minPrice.toString());
    }
    
    if (filtersToSync.maxPrice !== '' && filtersToSync.maxPrice !== undefined && Number(filtersToSync.maxPrice) > 0) {
      params.set('maxPrice', filtersToSync.maxPrice.toString());
    }
    
    if (filtersToSync.sortBy?.trim()) {
      params.set('sort', filtersToSync.sortBy.trim());
    }
    
    // Add variation filters
    if (filtersToSync.variationFilters) {
      Object.entries(filtersToSync.variationFilters).forEach(([variationName, values]) => {
        if (values && values.length > 0) {
          params.set(`variation_${variationName}`, values.join(','));
        }
      });
    }
    
    const newURL = params.toString() ? `${pathname}?${params.toString()}` : pathname;
    
    // Only update URL if it's different from what we last pushed
    if (newURL !== lastPushedURL.current) {
      lastPushedURL.current = newURL;
      router.replace(newURL, { scroll: false });
    }
  };

  // Initialize filters from URL on mount and when URL changes
  useEffect(() => {
    const currentURL = searchParams.toString() ? `${pathname}?${searchParams.toString()}` : pathname;
    
    // Don't update state if this URL change was caused by us
    if (currentURL === lastPushedURL.current) {
      return;
    }
    
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

    // Update filters state without triggering URL update
    setFilters(urlFilters);
  }, [searchParams, pathname]);

  // Custom setFilters that also updates URL
  const setFiltersWithURL = (newFilters: React.SetStateAction<ProductFilters>) => {
    setFilters((prevFilters) => {
      const updatedFilters = typeof newFilters === 'function' ? newFilters(prevFilters) : newFilters;
      
      // Update URL to match new filters
      updateURL(updatedFilters);
      
      return updatedFilters;
    });
  };

  // Only consider searchTerm and categoryId for isFiltered (homepage logic)
  const isFiltered = useMemo(() => {
    return (
      (typeof filters.searchTerm === 'string' && filters.searchTerm.trim() !== '') ||
      (typeof filters.categoryId === 'string' && filters.categoryId !== null && filters.categoryId.trim() !== '')
    );
  }, [filters.searchTerm, filters.categoryId]);

  return (
    <ProductFilterContext.Provider value={{ filters, setFilters: setFiltersWithURL, isFiltered }}>
      {children}
    </ProductFilterContext.Provider>
  );
};

export function useProductFilters() {
  const context = useContext(ProductFilterContext);
  if (!context) {
    throw new Error('useProductFilters must be used within a ProductFilterProvider');
  }
  return context;
}
