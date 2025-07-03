// API helper for admin ProductList
import { AdminProductListResponse } from './ProductList.types';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

export async function fetchAdminProductList({
  page = 1,
  pageSize = 20,
  searchTerm = '',
  categoryId = ''
}: {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
}): Promise<AdminProductListResponse> {
  const token = typeof window !== 'undefined' ? localStorage.getItem('accessToken') : undefined;
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
    ...(searchTerm ? { searchTerm } : {}),
    ...(categoryId ? { categoryId } : {})
  });
  const res = await fetch(`${API_URL}/api/Products?${params.toString()}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    }
  });
  if (!res.ok) throw new Error('Failed to fetch product list');
  return res.json();
}
