// API helper for Categories

export interface Category {
  id: string;
  name: string;
  description: string;
  slug: string;
  isActive: boolean;
  parentCategoryId: string | null;
  subCategories: Category[];
  variationTypes?: any[];
}

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

export async function fetchCategories(): Promise<Category[]> {
  const res = await fetch(`${API_URL}/Categories`);
  if (!res.ok) throw new Error('Failed to fetch categories');
  const data = await res.json();
  // The API returns an array of root categories, each with subCategories
  return Array.isArray(data) ? data : [];
}
