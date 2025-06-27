'use client';

// ProductFilterContext for managing product filters across the shop UI
import React, { createContext, useContext, useState } from 'react';

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

interface ProductFilterContextType {
  filters: ProductFilters;
  setFilters: React.Dispatch<React.SetStateAction<ProductFilters>>;
}

const ProductFilterContext = createContext<ProductFilterContextType | undefined>(undefined);

export const ProductFilterProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [filters, setFilters] = useState<ProductFilters>({
    searchTerm: '',
    categoryId: null,
    minPrice: '',
    maxPrice: '',
    variationFilters: {},
    sortBy: '', // Default no sorting
  });

  return (
    <ProductFilterContext.Provider value={{ filters, setFilters }}>
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
