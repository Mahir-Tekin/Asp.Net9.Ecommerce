import { notFound } from 'next/navigation';
import { fetchProductBySlug } from '@/components/product/ProductDetails.api';
import ProductDetails from '@/components/product/ProductDetails';

export default async function ProductDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  const product = await fetchProductBySlug(slug);
  console.log('Product slug:', slug);
  console.log('Fetched product:', product);
  if (!product) return notFound();

  return <ProductDetails product={product} />;
}
