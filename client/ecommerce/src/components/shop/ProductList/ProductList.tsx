'use client';

import React, { useEffect, useState } from 'react';
import ProductCard from '../ProductCard/ProductCard';
import { fetchShopProductList, ShopProductListItem } from './ProductList.api';

export default function ProductList() {
  const [products, setProducts] = useState<ShopProductListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    fetchShopProductList()
      .then((res) => setProducts(res.items))
      .catch(() => setError('Failed to fetch products.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div>Loading products...</div>;
  if (error) return <div>{error}</div>;
  if (products.length === 0) return <div>No products found.</div>;

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-6">
      {products.map((product) => (
        <ProductCard
          key={product.id}
          id={product.id}
          name={product.name}
          mainImage={product.mainImage}
          lowestPrice={product.lowestPrice}
          lowestOldPrice={product.lowestOldPrice}
        />
      ))}
    </div>
  );
}