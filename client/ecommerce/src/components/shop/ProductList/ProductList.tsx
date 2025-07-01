'use client';

import React, { useEffect, useState } from 'react';
import ProductCard from '../ProductCard/ProductCard';
import { fetchShopProductList, ShopProductListItem } from './ProductList.api';
import { useURLFilters } from '@/hooks/useURLFilters';
import ProductSortDropdown from '../sort/ProductSortDropdown';
import Pagination from '../pagination/Pagination';
import ActiveFiltersDisplay from '../filter/ActiveFiltersDisplay';
import ShopBreadcrumb from '../breadcrumb/ShopBreadcrumb';

export default function ProductList() {
  const { filters } = useURLFilters();
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

  if (loading) return (
    <div className="flex items-center justify-center min-h-96">
      <div className="text-center">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p className="text-gray-600 text-lg">Loading products...</p>
      </div>
    </div>
  );
  
  if (error) return (
    <div className="flex items-center justify-center min-h-96">
      <div className="text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <p className="text-red-600 text-lg">{error}</p>
      </div>
    </div>
  );
  
  if (products.length === 0) return (
    <div className="flex items-center justify-center min-h-96">
      <div className="text-center">
        <div className="text-gray-400 text-6xl mb-4">📦</div>
        <p className="text-gray-600 text-lg">No products found</p>
        <p className="text-gray-500 text-sm mt-2">Try adjusting your filters</p>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen">
      {/* Breadcrumb */}
      <ShopBreadcrumb />
      
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Active Filters Display */}
        <ActiveFiltersDisplay />
        
        {/* Results count and sort */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-4">
          <p className="text-gray-700 font-medium">
            {total === 1 ? '1 product found' : `${total.toLocaleString()} products found`}
          </p>
          <ProductSortDropdown />
        </div>
        
        {/* Product grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-12">
          {products.map((product) => (
            <ProductCard
              key={product.id}
              id={product.id}
              name={product.name}
              mainImage={product.mainImage}
              variantCount={product.variantCount}
              lowestPrice={product.lowestPrice}
              lowestOldPrice={product.lowestOldPrice}
              slug={product.slug}
              hasStock={product.hasStock}
              totalStock={product.totalStock}
              averageRating={product.averageRating}
              reviewCount={product.reviewCount}
            />
          ))}
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-center">
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              total={total}
              pageSize={pageSize}
              onPageChange={handlePageChange}
            />
          </div>
        )}
      </div>
    </div>
  );
}