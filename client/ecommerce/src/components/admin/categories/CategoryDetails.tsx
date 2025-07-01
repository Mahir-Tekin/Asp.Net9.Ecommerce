'use client';

import React, { useState, useEffect } from 'react';
import axios from 'axios';

interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  options: VariationOption[];
}

interface VariationOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

interface Category {
  id: string;
  name: string;
  description: string;
  slug: string;
  isActive: boolean;
  parentCategoryId: string | null;
  subCategories: Category[];
  variationTypes: VariationType[];
}

export function CategoryDetails({ categoryId, onEdit }: { categoryId: string; onEdit: () => void }) {
  const [category, setCategory] = useState<Category | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchCategory = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/Categories/${categoryId}`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setCategory(response.data);
      } catch (error) {
        console.error('Failed to fetch category details:', error);
        setError('Failed to load category details');
      } finally {
        setLoading(false);
      }
    };
    fetchCategory();
  }, [API_URL, categoryId]);

  if (loading) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow max-w-2xl">
        <div className="animate-pulse">
          <div className="h-6 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="space-y-3">
            <div className="h-4 bg-gray-200 rounded w-3/4"></div>
            <div className="h-4 bg-gray-200 rounded w-1/2"></div>
            <div className="h-4 bg-gray-200 rounded w-2/3"></div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !category) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow max-w-2xl">
        <div className="text-red-600 mb-4">{error || 'Category not found'}</div>
        <button 
          onClick={onEdit}
          className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
        >
          Back
        </button>
      </div>
    );
  }

  return (
    <div className="p-6 border rounded-lg bg-white shadow max-w-2xl">
      <div className="flex justify-between items-start mb-6">
        <h2 className="text-2xl font-semibold text-gray-900">{category.name}</h2>
        <div className="flex items-center space-x-2">
          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            category.isActive 
              ? 'bg-green-100 text-green-800' 
              : 'bg-red-100 text-red-800'
          }`}>
            {category.isActive ? 'Active' : 'Inactive'}
          </span>
          <button
            onClick={onEdit}
            className="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
            Edit
          </button>
        </div>
      </div>

      <div className="space-y-6">
        {/* Basic Information */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md">
              {category.description || 'No description provided'}
            </p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">URL Slug</label>
            <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md font-mono">
              /category/{category.slug}
            </p>
          </div>
        </div>

        {/* Parent Category */}
        {category.parentCategoryId && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Parent Category</label>
            <div className="bg-blue-50 border border-blue-200 rounded-md p-3">
              <p className="text-sm text-blue-800">
                <strong>ID:</strong> {category.parentCategoryId}
              </p>
            </div>
          </div>
        )}

        {/* Sub Categories */}
        {category.subCategories && category.subCategories.length > 0 && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Sub Categories ({category.subCategories.length})
            </label>
            <div className="border border-gray-200 rounded-md">
              <div className="bg-gray-50 px-4 py-2 border-b border-gray-200">
                <div className="grid grid-cols-3 gap-4 text-xs font-medium text-gray-500 uppercase tracking-wider">
                  <span>Name</span>
                  <span>Slug</span>
                  <span>Status</span>
                </div>
              </div>
              <div className="divide-y divide-gray-200">
                {category.subCategories.map((subCategory) => (
                  <div key={subCategory.id} className="px-4 py-3 grid grid-cols-3 gap-4">
                    <span className="text-sm font-medium text-gray-900">{subCategory.name}</span>
                    <span className="text-sm text-gray-500 font-mono">{subCategory.slug}</span>
                    <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                      subCategory.isActive 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-red-100 text-red-800'
                    }`}>
                      {subCategory.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* Variation Types */}
        {category.variationTypes && category.variationTypes.length > 0 && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Variation Types ({category.variationTypes.length})
            </label>
            <div className="space-y-3">
              {category.variationTypes.map((vt) => (
                <div key={vt.id} className="border border-gray-200 rounded-lg p-4">
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="text-sm font-medium text-gray-900">
                      {vt.displayName || vt.name}
                    </h4>
                    <div className="flex items-center space-x-2">
                      {vt.options && vt.options.length > 0 && (
                        <span className="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">
                          {vt.options.length} options
                        </span>
                      )}
                    </div>
                  </div>
                  
                  {vt.options && vt.options.length > 0 && (
                    <div>
                      <p className="text-xs text-gray-500 mb-2">Available options:</p>
                      <div className="flex flex-wrap gap-1">
                        {vt.options.slice(0, 8).map(option => (
                          <span 
                            key={option.id} 
                            className="inline-block px-2 py-1 text-xs bg-gray-100 text-gray-700 rounded border"
                          >
                            {option.displayValue || option.value}
                          </span>
                        ))}
                        {vt.options.length > 8 && (
                          <span className="text-xs text-gray-500 px-2 py-1">
                            +{vt.options.length - 8} more
                          </span>
                        )}
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        )}

        {/* No Variation Types Message */}
        {(!category.variationTypes || category.variationTypes.length === 0) && (
          <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a.997.997 0 01-1.414 0l-7-7A1.997 1.997 0 013 12V7a4 4 0 014-4z" />
            </svg>
            <h3 className="mt-2 text-sm font-medium text-gray-900">No variation types</h3>
            <p className="mt-1 text-sm text-gray-500">
              This category doesn't have any variation types assigned.
            </p>
          </div>
        )}

        {/* Category ID for reference */}
        <div className="pt-4 border-t border-gray-200">
          <p className="text-xs text-gray-500">
            <strong>Category ID:</strong> <span className="font-mono">{category.id}</span>
          </p>
        </div>
      </div>
    </div>
  );
}

export default CategoryDetails;
