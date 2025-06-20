
// Admin ProductList component: minimal, performant, English only
'use client';

import React, { useEffect, useState } from 'react';
import { fetchAdminProductList } from './ProductList.api';
import type { AdminProductListItem } from './ProductList.types';

interface ProductListProps {
  onSelect: (id: string) => void;
  onCreate: () => void;
}

export default function ProductList({ onSelect, onCreate }: ProductListProps) {
  // State for product list, loading, error, filters, and pagination
  const [products, setProducts] = useState<AdminProductListItem[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [hasNextPage, setHasNextPage] = useState(false);
  const [hasPreviousPage, setHasPreviousPage] = useState(false);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);


  // Fetch product list from API
  useEffect(() => {
    setLoading(true);
    setError(null);
    fetchAdminProductList({ page, pageSize, searchTerm: search, categoryId })
      .then((res) => {
        setProducts(res.items);
        setTotal(res.total || res.totalItems || 0);
        setHasNextPage(res.hasNextPage ?? false);
        setHasPreviousPage(res.hasPreviousPage ?? false);
        setTotalPages(res.totalPages ?? 1);
        if (typeof res.currentPage === 'number') setPage(res.currentPage);
      })
      .catch((err) => {
        setError(err.message || 'Failed to load products');
      })
      .finally(() => setLoading(false));
  }, [page, pageSize, search, categoryId]);

  // Fetch categories for filter (minimal, only id/name)
  const [categories, setCategories] = useState<{ id: string; name: string }[]>([]);
  useEffect(() => {
    async function fetchCategories() {
      try {
        const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';
        const token = typeof window !== 'undefined' ? localStorage.getItem('accessToken') : undefined;
        const res = await fetch(`${API_URL}/Categories/admin`, {
          headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {})
          }
        });
        if (!res.ok) throw new Error('Failed to fetch categories');
        const data = await res.json();
        // Flatten categories to leaf nodes only
        function flatten(cats: any[]): { id: string; name: string }[] {
          return cats.flatMap((cat) =>
            cat.subCategories && cat.subCategories.length > 0
              ? flatten(cat.subCategories)
              : [{ id: cat.id, name: cat.name }]
          );
        }
        setCategories(flatten(data));
      } catch (e: any) {
        setCategories([]);
      }
    }
    fetchCategories();
  }, []);

  // Handlers
  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };
  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCategoryId(e.target.value);
    setPage(1);
  };

  // UI
  return (
    <div className="bg-white p-4 rounded shadow">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold">Product List</h3>
        <button
          onClick={onCreate}
          className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
        >
          Create Product
        </button>
      </div>
      <div className="flex gap-2 mb-4">
        <input
          type="text"
          placeholder="Search by name..."
          value={search}
          onChange={handleSearchChange}
          className="border rounded px-2 py-1 w-48"
        />
        <select
          value={categoryId}
          onChange={handleCategoryChange}
          className="border rounded px-2 py-1"
        >
          <option value="">All Categories</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>{cat.name}</option>
          ))}
        </select>
      </div>
      {loading ? (
        <div>Loading products...</div>
      ) : error ? (
        <div className="text-red-500">{error}</div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm border">
            <thead className="bg-gray-100">
              <tr>
                <th className="px-2 py-1 border">Image</th>
                <th className="px-2 py-1 border">Name</th>
                <th className="px-2 py-1 border">Price</th>
                <th className="px-2 py-1 border">Stock</th>
                <th className="px-2 py-1 border">Variants</th>
                <th className="px-2 py-1 border">Active</th>
                <th className="px-2 py-1 border">Created</th>
                <th className="px-2 py-1 border">Action</th>
              </tr>
            </thead>
            <tbody>
              {products.length === 0 ? (
                <tr>
                  <td colSpan={8} className="text-center py-4">No products found.</td>
                </tr>
              ) : (
                products.map((product) => (
                  <tr key={product.id} className="hover:bg-gray-50 cursor-pointer">
                    <td className="border px-2 py-1">
                      {product.mainImage ? (
                        <img
                          src={product.mainImage}
                          alt={product.name}
                          style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 4 }}
                        />
                      ) : (
                        <div style={{ width: 48, height: 48 }} className="bg-gray-200 flex items-center justify-center rounded text-gray-400">-</div>
                      )}
                    </td>
                    <td className="border px-2 py-1 font-medium" onClick={() => onSelect(product.id)}>
                      {product.name}
                    </td>
                    <td className="border px-2 py-1">
                      <span className="font-semibold">${product.lowestPrice.toFixed(2)}</span>
                      {product.lowestOldPrice && product.lowestOldPrice > product.lowestPrice && (
                        <span className="ml-2 line-through text-gray-400">${product.lowestOldPrice.toFixed(2)}</span>
                      )}
                    </td>
                    <td className="border px-2 py-1">
                      {product.hasStock ? (
                        <span className="text-green-600 font-semibold">In Stock</span>
                      ) : (
                        <span className="text-red-500">Out</span>
                      )}
                    </td>
                    <td className="border px-2 py-1 text-center">{product.variantCount}</td>
                    <td className="border px-2 py-1 text-center">
                      {product.isActive ? (
                        <span className="text-green-600">Yes</span>
                      ) : (
                        <span className="text-gray-400">No</span>
                      )}
                    </td>
                    <td className="border px-2 py-1 text-xs">{new Date(product.createdAt).toLocaleDateString()}</td>
                    <td className="border px-2 py-1">
                      <button
                        className="text-indigo-600 hover:underline text-sm"
                        onClick={() => onSelect(product.id)}
                      >
                        Details
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
      {/* Pagination */}
      <div className="flex items-center justify-between mt-4">
        <div>
          Page {total === 0 ? 0 : page} of {totalPages}
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={!hasPreviousPage}
            className="px-2 py-1 border rounded disabled:opacity-50"
          >
            Previous
          </button>
          <button
            onClick={() => setPage((p) => p + 1)}
            disabled={!hasNextPage}
            className="px-2 py-1 border rounded disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
