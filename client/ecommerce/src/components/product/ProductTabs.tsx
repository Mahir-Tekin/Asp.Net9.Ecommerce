'use client';

import React, { useState } from 'react';
import ProductDescription from './ProductDescription';
import ProductReviews from './ProductReviews';
import { ProductDetail } from './ProductDetails.types';

interface ProductTabsProps {
  product: ProductDetail;
}

export default function ProductTabs({ product }: ProductTabsProps) {
  const [activeTab, setActiveTab] = useState<'description' | 'reviews'>('description');

  return (
    <div className="mt-8">
      {/* Tab Navigation */}
      <div className="border-b border-gray-200">
        <nav className="flex space-x-8">
          <button
            onClick={() => setActiveTab('description')}
            className={`py-2 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'description'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Description
          </button>
          <button
            onClick={() => setActiveTab('reviews')}
            className={`py-2 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'reviews'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Reviews ({product.reviewCount})
          </button>
        </nav>
      </div>

      {/* Tab Content */}
      <div className="mt-6">
        {activeTab === 'description' && (
          <ProductDescription description={product.description} />
        )}
        {activeTab === 'reviews' && (
          <ProductReviews 
            productId={product.id}
            averageRating={product.averageRating}
            reviewCount={product.reviewCount}
          />
        )}
      </div>
    </div>
  );
}
