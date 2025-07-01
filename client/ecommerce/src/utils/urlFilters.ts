// Utility functions for URL filter management
export function createFilterURL(filters: {
  searchTerm?: string;
  categoryId?: string | null;
  minPrice?: number | string;
  maxPrice?: number | string;
  sortBy?: string;
  variationFilters?: { [key: string]: string[] };
}): string {
  const params = new URLSearchParams();
  
  if (filters.searchTerm?.trim()) {
    params.set('search', filters.searchTerm.trim());
  }
  
  if (filters.categoryId?.trim()) {
    params.set('category', filters.categoryId.trim());
  }
  
  if (filters.minPrice && filters.minPrice !== '' && filters.minPrice !== 0) {
    params.set('minPrice', filters.minPrice.toString());
  }
  
  if (filters.maxPrice && filters.maxPrice !== '' && filters.maxPrice !== 0) {
    params.set('maxPrice', filters.maxPrice.toString());
  }
  
  if (filters.sortBy?.trim()) {
    params.set('sort', filters.sortBy.trim());
  }
  
  // Add variation filters
  if (filters.variationFilters) {
    Object.entries(filters.variationFilters).forEach(([variationName, values]) => {
      if (values && values.length > 0) {
        params.set(`variation_${variationName}`, values.join(','));
      }
    });
  }
  
  return params.toString() ? `/?${params.toString()}` : '/';
}

export function parseURLFilters(searchParams: URLSearchParams) {
  const filters: any = {
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
      filters.variationFilters = filters.variationFilters || {};
      filters.variationFilters[variationName] = value.split(',');
    }
  });

  return filters;
}
