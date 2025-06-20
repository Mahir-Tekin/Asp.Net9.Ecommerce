// Types for the admin ProductList component

export interface AdminProductListItem {
  id: string;
  name: string;
  mainImage: string | null;
  lowestPrice: number;
  lowestOldPrice?: number | null;
  hasStock: boolean;
  variantCount: number;
  isActive: boolean;
  createdAt: string;
  categoryName?: string;
}

export interface AdminProductListResponse {
  items: AdminProductListItem[];
  total?: number;
  totalItems?: number;
  page?: number;
  pageSize?: number;
  currentPage?: number;
  totalPages?: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}
