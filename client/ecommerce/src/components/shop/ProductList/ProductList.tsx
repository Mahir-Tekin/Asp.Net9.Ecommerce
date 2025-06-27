'use client';

import React, { useEffect, useState } from 'react';
import ProductCard from '../ProductCard/ProductCard';
import { fetchShopProductList, ShopProductListItem } from './ProductList.api';
import { useProductFilters } from '@/context/ProductFilterContext';
import ProductSortDropdown from '../sort/ProductSortDropdown';
import Pagination from '../pagination/Pagination';

export default function ProductList() {
  const { filters } = useProductFilters();
  const [products, setProducts] = useState<ShopProductListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    setLoading(true);
    fetchShopProductList({ 
      page: currentPage,
      pageSize: pageSize,
      searchTerm: filters.searchTerm, 
      categoryId: filters.categoryId,
      minPrice: filters.minPrice,
      maxPrice: filters.maxPrice,
      variationFilters: filters.variationFilters,
      sortBy: filters.sortBy
    })
      .then((res) => {
        setProducts(res.items);
        setTotal(res.totalItems || res.total || 0);
        setTotalPages(res.totalPages || 1);
      })
      .catch(() => setError('Failed to fetch products.'))
      .finally(() => setLoading(false));
  }, [filters.searchTerm, filters.categoryId, filters.minPrice, filters.maxPrice, filters.variationFilters, filters.sortBy, currentPage, pageSize]);

  // Reset to page 1 when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [filters.searchTerm, filters.categoryId, filters.minPrice, filters.maxPrice, filters.variationFilters, filters.sortBy]);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    // Scroll to top when page changes
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  if (loading) return <div>Loading products...</div>;
  if (error) return <div>{error}</div>;
  if (products.length === 0) return <div>No products found.</div>;

  return (
    <div>
      {/* Sort dropdown at the top of product list */}
      <div className="flex justify-end mb-4">
        <ProductSortDropdown />
      </div>
      
      {/* Results info */}
      <div className="mb-4 text-sm text-gray-600">
        Showing {((currentPage - 1) * pageSize) + 1}-{Math.min(currentPage * pageSize, total)} of {total} results
      </div>
      
      {/* Product grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-6 mb-8">
        {products.map((product) => (
          <ProductCard
            key={product.id}
            id={product.id}
            name={product.name}
            mainImage={product.mainImage}
            lowestPrice={product.lowestPrice}
            lowestOldPrice={product.lowestOldPrice}
            slug={product.slug}
          />
        ))}
      </div>
      

      {/* Pagination */}
      {totalPages > 1 && (
        <Pagination
          currentPage={currentPage}
          totalPages={totalPages}
          total={total}
          pageSize={pageSize}
          onPageChange={handlePageChange}
        />
      )}
    </div>
  );
}