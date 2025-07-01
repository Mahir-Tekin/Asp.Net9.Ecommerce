'use client';

import { useURLFilters } from '@/hooks/useURLFilters';
import ProductList from '@/components/shop/ProductList/ProductList';
import HomePageContent from '@/components/home/HomePageContent';

export default function Home() {
  const { isFiltered } = useURLFilters();
  return isFiltered ? <ProductList /> : <HomePageContent />;
}