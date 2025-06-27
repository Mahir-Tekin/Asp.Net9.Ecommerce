// API helper for shop ProductList

export interface ShopProductListItem {
  id: string;
  name: string;
  mainImage: string | null;
  lowestPrice: number;
  lowestOldPrice?: number | null;
  slug: string;
}

export interface ShopProductListResponse {
  items: ShopProductListItem[];
  total?: number;
  totalItems?: number;  // Add this property
  totalPages?: number;
  currentPage?: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

export async function fetchShopProductList({
  page = 1,
  pageSize = 20,
  searchTerm = '',
  categoryId = null,
  minPrice,
  maxPrice,
  variationFilters = {},
  sortBy = '',
}: {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string | null;
  minPrice?: number | '';
  maxPrice?: number | '';
  variationFilters?: { [variationTypeName: string]: string[] };
  sortBy?: string;
} = {}): Promise<ShopProductListResponse> {
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
  });
  
  if (searchTerm) {
    params.append('searchTerm', searchTerm);
  }
  if (categoryId) {
    params.append('categoryId', categoryId);
  }
  if (minPrice !== undefined && minPrice !== '') {
    params.append('minPrice', String(minPrice));
  }
  if (maxPrice !== undefined && maxPrice !== '') {
    params.append('maxPrice', String(maxPrice));
  }
  if (sortBy) {
    params.append('sortBy', sortBy);
  }
  
  // Add variation filters to URL parameters
  // Backend expects format: ?color=red,blue&size=M,L&brand=nike,adidas
  if (variationFilters && Object.keys(variationFilters).length > 0) {
    Object.entries(variationFilters).forEach(([variationTypeName, optionValues]) => {
      if (optionValues.length > 0) {
        // Send as comma-separated values: color=red,blue
        params.append(variationTypeName, optionValues.join(','));
      }
    });
  }
  
  const res = await fetch(`${API_URL}/Products?${params.toString()}`);
  if (!res.ok) throw new Error('Failed to fetch product list');
  return res.json();
}
