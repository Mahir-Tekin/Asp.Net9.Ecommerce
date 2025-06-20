// API helper for shop ProductList

export interface ShopProductListItem {
  id: string;
  name: string;
  mainImage: string | null;
  lowestPrice: number;
  lowestOldPrice?: number | null;
}

export interface ShopProductListResponse {
  items: ShopProductListItem[];
  total?: number;
  totalPages?: number;
  currentPage?: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
}

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

export async function fetchShopProductList({
  page = 1,
  pageSize = 20,
}: {
  page?: number;
  pageSize?: number;
} = {}): Promise<ShopProductListResponse> {
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
  });
  const res = await fetch(`${API_URL}/Products?${params.toString()}`);
  if (!res.ok) throw new Error('Failed to fetch product list');
  return res.json();
}
