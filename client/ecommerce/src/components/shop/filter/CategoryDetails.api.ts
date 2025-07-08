// API helper for fetching category details with variation types

export interface VariationOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

export interface VariationType {
  id: string;
  name: string;
  displayName: string;
  options: VariationOption[];
}

export interface SubCategory {
  id: string;
  name: string;
  description: string;
  slug: string;
  isActive: boolean;
  parentCategoryId: string;
  subCategories: SubCategory[];
  variationTypes: VariationType[];
}

export interface CategoryDetails {
  id: string;
  name: string;
  description: string;
  slug: string;
  isActive: boolean;
  parentCategoryId?: string;
  subCategories: SubCategory[];
  variationTypes: VariationType[];
}

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

export async function fetchCategoryDetails(categoryId: string): Promise<CategoryDetails> {
  const res = await fetch(`${API_URL}/api/Categories/${categoryId}`);
  if (!res.ok) throw new Error('Failed to fetch category details');
  return res.json();
}
