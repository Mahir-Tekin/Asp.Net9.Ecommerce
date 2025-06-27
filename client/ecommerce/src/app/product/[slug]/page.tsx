import { notFound } from 'next/navigation';
import { fetchProductBySlug } from '@/components/product/ProductDetails.api';
import ProductDetails from '@/components/product/ProductDetails';

export default async function ProductDetailPage({ params }: { params: { slug: string } }) {
  const product = await fetchProductBySlug(params.slug);
  if (!product) return notFound();

  return <ProductDetails product={product} />;
}
