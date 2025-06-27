import { ProductDetail } from './ProductDetails.types';

export async function fetchProductBySlug(slug: string): Promise<ProductDetail | null> {
  const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Products/slug/${slug}`);
  if (!res.ok) return null;
  return res.json();
}
