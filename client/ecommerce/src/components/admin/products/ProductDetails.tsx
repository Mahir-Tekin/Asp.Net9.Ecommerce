'use client';

import { useState, useEffect, useCallback } from 'react';
import Image from 'next/image';
import { ChevronDownIcon, ChevronUpIcon, PencilIcon, TrashIcon, EyeIcon, EyeSlashIcon, ArrowLeftIcon } from '@heroicons/react/24/outline';

interface ProductImage {
  url: string;
  altText: string;
  isMain: boolean;
}

interface VariationOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

interface VariationType {
  id: string;
  name: string;
  options: VariationOption[];
}

interface ProductVariant {
  id: string;
  sku: string;
  name: string;
  price: number;
  oldPrice: number;
  stockQuantity: number;
  trackInventory: boolean;
  isActive: boolean;
  selectedOptions: Record<string, string>;
}

interface Product {
  id: string;
  name: string;
  slug: string;
  description: string;
  basePrice: number;
  mainImage: string;
  images: ProductImage[];
  categoryName: string;
  categoryId: string;
  isActive: boolean;
  createdAt: string;
  variationTypes: VariationType[];
  variants: ProductVariant[];
  totalStock: number;
  hasStock: boolean;
  lowestPrice: number;
  lowestOldPrice: number;
  averageRating: number;
  reviewCount: number;
}

interface ProductDetailsProps {
  id: string;
  onEdit: () => void;
  onDelete: () => void;
  onBack: () => void;
}

export default function ProductDetails({ id, onEdit, onDelete, onBack }: ProductDetailsProps) {
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [expandedSections, setExpandedSections] = useState({
    images: false,
    variations: false,
    variants: false,
    inventory: false
  });

  const fetchProduct = useCallback(async () => {
    try {
      setLoading(true);
      const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
      const token = typeof window !== 'undefined' ? localStorage.getItem('accessToken') : undefined;
      const response = await fetch(`${API_URL}/api/Products/${id}`, {
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
      });
      if (!response.ok) {
        throw new Error('Failed to fetch product');
      }
      const data = await response.json();
      setProduct(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    fetchProduct();
  }, [id, fetchProduct]);

  const toggleSection = (section: keyof typeof expandedSections) => {
    setExpandedSections(prev => ({
      ...prev,
      [section]: !prev[section]
    }));
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  if (loading) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="space-y-3">
            <div className="h-4 bg-gray-200 rounded"></div>
            <div className="h-4 bg-gray-200 rounded w-5/6"></div>
            <div className="h-4 bg-gray-200 rounded w-4/6"></div>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div className="text-red-600">
          <h3 className="text-lg font-medium mb-2">Error Loading Product</h3>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div className="text-gray-500">Product not found</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <div className="flex items-center gap-3 mb-2">
              <button
                onClick={onBack}
                className="inline-flex items-center text-gray-500 hover:text-gray-700 transition-colors"
              >
                <ArrowLeftIcon className="w-5 h-5 mr-1" />
                Back to Products
              </button>
            </div>
            <div className="flex items-center gap-3 mb-2">
              <h2 className="text-2xl font-bold text-gray-900">{product.name}</h2>
              <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                product.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
              }`}>
                {product.isActive ? (
                  <>
                    <EyeIcon className="w-3 h-3 mr-1" />
                    Active
                  </>
                ) : (
                  <>
                    <EyeSlashIcon className="w-3 h-3 mr-1" />
                    Inactive
                  </>
                )}
              </span>
            </div>
            <p className="text-sm text-gray-500 mb-2">Slug: {product.slug}</p>
            <p className="text-gray-600 mb-4">{product.description}</p>
            <div className="flex items-center gap-6 text-sm text-gray-500">
              <span>Category: <span className="font-medium text-gray-900">{product.categoryName}</span></span>
              <span>Created: {formatDate(product.createdAt)}</span>
              <span>ID: {product.id}</span>
            </div>
          </div>
          <div className="flex gap-2 ml-4">
            <button
              onClick={onEdit}
              className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <PencilIcon className="w-4 h-4 mr-1.5" />
              Edit
            </button>
            <button
              onClick={onDelete}
              className="inline-flex items-center px-3 py-2 border border-red-300 shadow-sm text-sm leading-4 font-medium rounded-md text-red-700 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
            >
              <TrashIcon className="w-4 h-4 mr-1.5" />
              Delete
            </button>
          </div>
        </div>
      </div>

      {/* Pricing & Rating */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Pricing & Performance</h3>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Base Price</label>
            <p className="text-lg font-semibold text-gray-900">{formatCurrency(product.basePrice)}</p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Lowest Price</label>
            <p className="text-lg font-semibold text-green-600">{formatCurrency(product.lowestPrice)}</p>
            {product.lowestOldPrice > 0 && (
              <p className="text-sm text-gray-500 line-through">{formatCurrency(product.lowestOldPrice)}</p>
            )}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Average Rating</label>
            <div className="flex items-center">
              <span className="text-lg font-semibold text-gray-900">{product.averageRating.toFixed(1)}</span>
              <span className="text-yellow-400 ml-1">★</span>
              <span className="text-sm text-gray-500 ml-2">({product.reviewCount} reviews)</span>
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Stock Status</label>
            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
              product.hasStock ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
            }`}>
              {product.hasStock ? 'In Stock' : 'Out of Stock'}
            </span>
            <p className="text-sm text-gray-500 mt-1">Total: {product.totalStock} units</p>
          </div>
        </div>
      </div>

      {/* Images */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200">
        <div 
          className="flex items-center justify-between p-6 cursor-pointer border-b border-gray-200"
          onClick={() => toggleSection('images')}
        >
          <h3 className="text-lg font-semibold text-gray-900">
            Images ({product.images.length})
          </h3>
          {expandedSections.images ? (
            <ChevronUpIcon className="w-5 h-5 text-gray-500" />
          ) : (
            <ChevronDownIcon className="w-5 h-5 text-gray-500" />
          )}
        </div>
        {expandedSections.images && (
          <div className="p-6">
            <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
              {product.images.map((image, index) => (
                <div key={index} className="relative group">
                  <Image
                    src={image.url}
                    alt={image.altText}
                    width={200}
                    height={96}
                    className="w-full h-24 object-cover rounded-lg border border-gray-200"
                  />
                  {image.isMain && (
                    <div className="absolute top-1 right-1">
                      <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">
                        Main
                      </span>
                    </div>
                  )}
                  <p className="text-xs text-gray-500 mt-1 truncate">{image.altText}</p>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Variation Types */}
      {product.variationTypes.length > 0 && (
        <div className="bg-white rounded-lg shadow-sm border border-gray-200">
          <div 
            className="flex items-center justify-between p-6 cursor-pointer border-b border-gray-200"
            onClick={() => toggleSection('variations')}
          >
            <h3 className="text-lg font-semibold text-gray-900">
              Variation Types ({product.variationTypes.length})
            </h3>
            {expandedSections.variations ? (
              <ChevronUpIcon className="w-5 h-5 text-gray-500" />
            ) : (
              <ChevronDownIcon className="w-5 h-5 text-gray-500" />
            )}
          </div>
          {expandedSections.variations && (
            <div className="p-6 space-y-4">
              {product.variationTypes.map((variationType) => (
                <div key={variationType.id} className="border border-gray-200 rounded-lg p-4">
                  <h4 className="font-medium text-gray-900 mb-2">{variationType.name}</h4>
                  <div className="flex flex-wrap gap-2">
                    {variationType.options
                      .sort((a, b) => a.sortOrder - b.sortOrder)
                      .map((option) => (
                        <span
                          key={option.id}
                          className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800"
                        >
                          {option.displayValue}
                        </span>
                      ))}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Variants */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200">
        <div 
          className="flex items-center justify-between p-6 cursor-pointer border-b border-gray-200"
          onClick={() => toggleSection('variants')}
        >
          <h3 className="text-lg font-semibold text-gray-900">
            Product Variants ({product.variants.length})
          </h3>
          {expandedSections.variants ? (
            <ChevronUpIcon className="w-5 h-5 text-gray-500" />
          ) : (
            <ChevronDownIcon className="w-5 h-5 text-gray-500" />
          )}
        </div>
        {expandedSections.variants && (
          <div className="p-6">
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Variant
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      SKU
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Price
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Stock
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {product.variants.map((variant) => (
                    <tr key={variant.id}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div>
                          <div className="text-sm font-medium text-gray-900">{variant.name}</div>
                          <div className="text-sm text-gray-500">
                            {Object.values(variant.selectedOptions).join(' • ')}
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {variant.sku}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900">
                          {formatCurrency(variant.price)}
                        </div>
                        {variant.oldPrice > 0 && (
                          <div className="text-sm text-gray-500 line-through">
                            {formatCurrency(variant.oldPrice)}
                          </div>
                        )}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-900">
                          {variant.trackInventory ? variant.stockQuantity : 'Not tracked'}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          variant.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}>
                          {variant.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
