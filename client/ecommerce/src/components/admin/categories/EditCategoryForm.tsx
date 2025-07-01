'use client';

import React, { useState, useEffect } from 'react';
import axios from 'axios';

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

interface CategoryUpdateRequest {
  name: string;
  description: string;
  slug: string;
  isActive: boolean;
  variationTypes: { variationTypeId: string; isRequired: boolean }[];
}

export function EditCategoryForm({ categoryId, onCancel, onSave }: { 
  categoryId: string; 
  onCancel: () => void;
  onSave?: () => void;
}) {
  const [category, setCategory] = useState<Category | null>(null);
  const [formData, setFormData] = useState<CategoryUpdateRequest>({
    name: '',
    description: '',
    slug: '',
    isActive: true,
    variationTypes: []
  });
  const [availableVariationTypes, setAvailableVariationTypes] = useState<VariationType[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchData = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const headers = { Authorization: `Bearer ${token}` };

        // Fetch category details
        const categoryResponse = await axios.get(`${API_URL}/api/Categories/${categoryId}`, { headers });
        const categoryData = categoryResponse.data;
        setCategory(categoryData);

        // Set form data from category
        setFormData({
          name: categoryData.name,
          description: categoryData.description,
          slug: categoryData.slug,
          isActive: categoryData.isActive,
          variationTypes: categoryData.variationTypes.map((vt: any) => ({
            variationTypeId: vt.id,
            isRequired: true // Default to required, could be extended
          }))
        });

        // Fetch available variation types
        try {
          const variationTypesResponse = await axios.get(`${API_URL}/api/variation-types`, { headers });
          setAvailableVariationTypes(variationTypesResponse.data);
        } catch (vtError) {
          console.warn('Could not fetch variation types:', vtError);
          setAvailableVariationTypes([]);
        }

        setLoading(false);
      } catch (error) {
        console.error('Failed to fetch category details:', error);
        setError('Failed to load category details');
        setLoading(false);
      }
    };

    fetchData();
  }, [API_URL, categoryId]);

  const handleInputChange = (field: keyof CategoryUpdateRequest, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleVariationTypeToggle = (variationTypeId: string) => {
    setFormData(prev => {
      const existing = prev.variationTypes.find(vt => vt.variationTypeId === variationTypeId);
      if (existing) {
        // Remove it
        return {
          ...prev,
          variationTypes: prev.variationTypes.filter(vt => vt.variationTypeId !== variationTypeId)
        };
      } else {
        // Add it
        return {
          ...prev,
          variationTypes: [...prev.variationTypes, { variationTypeId, isRequired: true }]
        };
      }
    });
  };

  const handleVariationTypeRequiredToggle = (variationTypeId: string) => {
    setFormData(prev => ({
      ...prev,
      variationTypes: prev.variationTypes.map(vt => 
        vt.variationTypeId === variationTypeId 
          ? { ...vt, isRequired: !vt.isRequired }
          : vt
      )
    }));
  };

  const generateSlug = (name: string) => {
    return name
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/(^-|-$)/g, '');
  };

  const handleNameChange = (value: string) => {
    handleInputChange('name', value);
    if (!formData.slug || formData.slug === generateSlug(formData.name)) {
      handleInputChange('slug', generateSlug(value));
    }
  };

  const handleSave = async () => {
    if (!formData.name.trim()) {
      setError('Category name is required');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const token = localStorage.getItem('accessToken');
      await axios.put(`${API_URL}/api/Categories/${categoryId}`, formData, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      onSave?.();
      onCancel();
    } catch (error: any) {
      console.error('Failed to update category:', error);
      setError(error.response?.data?.message || 'Failed to update category');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow">
        <div className="animate-pulse">
          <div className="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
          <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
          <div className="h-4 bg-gray-200 rounded w-1/2"></div>
        </div>
      </div>
    );
  }

  if (error && !category) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow">
        <div className="text-red-600 mb-4">{error}</div>
        <button 
          onClick={onCancel}
          className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
        >
          Back
        </button>
      </div>
    );
  }

  return (
    <div className="p-6 border rounded-lg bg-white shadow max-w-2xl">
      <h2 className="text-xl font-semibold mb-6">Edit Category</h2>
      
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <div className="space-y-4">
        {/* Name */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Category Name *
          </label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => handleNameChange(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter category name"
          />
        </div>

        {/* Description */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Description
          </label>
          <textarea
            value={formData.description}
            onChange={(e) => handleInputChange('description', e.target.value)}
            rows={3}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Enter category description"
          />
        </div>

        {/* Slug */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            URL Slug
          </label>
          <input
            type="text"
            value={formData.slug}
            onChange={(e) => handleInputChange('slug', e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="category-url-slug"
          />
          <p className="text-xs text-gray-500 mt-1">
            This will be used in the URL: /category/{formData.slug}
          </p>
        </div>

        {/* Active Status */}
        <div className="flex items-center">
          <input
            type="checkbox"
            id="isActive"
            checked={formData.isActive}
            onChange={(e) => handleInputChange('isActive', e.target.checked)}
            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label htmlFor="isActive" className="ml-2 block text-sm text-gray-700">
            Category is active
          </label>
        </div>

        {/* Variation Types */}
        {availableVariationTypes.length > 0 && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Variation Types
            </label>
            <div className="space-y-3 max-h-64 overflow-y-auto border border-gray-200 rounded p-3">
              {availableVariationTypes
                .filter(vt => vt.isActive) // Only show active variation types
                .map((vt) => {
                  const isSelected = formData.variationTypes.some(selected => selected.variationTypeId === vt.id);
                  const selectedVt = formData.variationTypes.find(selected => selected.variationTypeId === vt.id);
                  
                  return (
                    <div key={vt.id} className="border border-gray-100 rounded p-3">
                      <div className="flex items-center mb-2">
                        <input
                          type="checkbox"
                          id={`vt-${vt.id}`}
                          checked={isSelected}
                          onChange={() => handleVariationTypeToggle(vt.id)}
                          className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                        />
                        <label htmlFor={`vt-${vt.id}`} className="ml-2 block text-sm font-medium text-gray-700">
                          {vt.displayName || vt.name}
                        </label>
                        {vt.options && vt.options.length > 0 && (
                          <span className="text-xs text-gray-500 ml-2">
                            ({vt.options.length} options)
                          </span>
                        )}
                      </div>
                      
                      {isSelected && (
                        <div className="ml-6 flex items-center">
                          <input
                            type="checkbox"
                            id={`vt-required-${vt.id}`}
                            checked={selectedVt?.isRequired || false}
                            onChange={() => handleVariationTypeRequiredToggle(vt.id)}
                            className="h-3 w-3 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                          />
                          <label htmlFor={`vt-required-${vt.id}`} className="ml-2 block text-xs text-gray-600">
                            Required for products in this category
                          </label>
                        </div>
                      )}
                      
                      {vt.options && vt.options.length > 0 && (
                        <div className="ml-6 mt-2">
                          <p className="text-xs text-gray-500 mb-1">Available options:</p>
                          <div className="flex flex-wrap gap-1">
                            {vt.options.slice(0, 5).map(option => (
                              <span 
                                key={option.id} 
                                className="inline-block px-2 py-1 text-xs bg-gray-100 text-gray-700 rounded"
                              >
                                {option.displayValue || option.value}
                              </span>
                            ))}
                            {vt.options.length > 5 && (
                              <span className="text-xs text-gray-500">
                                +{vt.options.length - 5} more
                              </span>
                            )}
                          </div>
                        </div>
                      )}
                    </div>
                  );
                })}
            </div>
            <p className="text-xs text-gray-500 mt-1">
              Select which variation types are available for products in this category. Mark as required if products must have this variation.
            </p>
          </div>
        )}

        {/* Parent Category Info */}
        {category?.parentCategoryId && (
          <div className="bg-gray-50 p-3 rounded">
            <p className="text-sm text-gray-600">
              <strong>Parent Category ID:</strong> {category.parentCategoryId}
            </p>
          </div>
        )}

        {/* Sub Categories Info */}
        {category?.subCategories && category.subCategories.length > 0 && (
          <div className="bg-blue-50 p-3 rounded">
            <p className="text-sm text-blue-800">
              <strong>Sub Categories:</strong> {category.subCategories.length} sub-categories
            </p>
            <ul className="text-xs text-blue-600 mt-1">
              {category.subCategories.slice(0, 3).map(sub => (
                <li key={sub.id}>• {sub.name}</li>
              ))}
              {category.subCategories.length > 3 && (
                <li>• ... and {category.subCategories.length - 3} more</li>
              )}
            </ul>
          </div>
        )}
      </div>

      {/* Actions */}
      <div className="flex justify-end space-x-3 mt-6 pt-4 border-t">
        <button
          onClick={onCancel}
          className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50"
          disabled={saving}
        >
          Cancel
        </button>
        <button
          onClick={handleSave}
          disabled={saving || !formData.name.trim()}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {saving ? 'Saving...' : 'Save Changes'}
        </button>
      </div>
    </div>
  );
}

export default EditCategoryForm;
