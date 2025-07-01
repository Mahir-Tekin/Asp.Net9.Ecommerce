'use client';
import axios from 'axios';
import React, { useState, useEffect } from 'react';
import { generateSlug } from '@/utils/string';

interface CreateCategoryFormProps {
  onCancel: () => void;
  onSuccess: () => void;
}

interface VariationTypeSelection {
  variationTypeId: string;
  isRequired: boolean;
}

interface ParentCategory {
  id: string;
  name: string;
}

interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  options: Array<{
    id: string;
    value: string;
    displayValue: string;
    sortOrder: number;
  }>;
}

interface Category {
  id: string;
  name: string;
}

interface CategoryFormData {
  name: string;
  description: string;
  slug: string;
  parentCategoryId: string | null;
  variationTypes: VariationTypeSelection[];
}

export function CreateCategoryForm({ onCancel, onSuccess }: CreateCategoryFormProps) {
  const [formData, setFormData] = useState<CategoryFormData>({
    name: '',
    description: '',
    slug: '',
    parentCategoryId: null,
    variationTypes: []
  });
  const [variationTypes, setVariationTypes] = useState<VariationType[]>([]);
  const [categories, setCategories] = useState<any[]>([]); // Accept nested categories
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    // Fetch variation types on mount
    const fetchVariationTypes = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/variation-types`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setVariationTypes(response.data);
      } catch (error) {
        console.error('Failed to fetch variation types:', error);
      }
    };
    fetchVariationTypes();

    // Fetch categories on mount
    const fetchCategories = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/Categories`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setCategories(response.data);
      } catch (error) {
        console.error('Failed to fetch categories:', error);
      }
    };
    fetchCategories();
  }, [API_URL]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.post(`${API_URL}/api/categories`, formData, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        console.log(response.data.error);
        if (response.status !== 201) {
            throw new Error('Failed to create category');
            
        }
        onSuccess();
    } catch (error) {
        console.error('Failed to create category:', error);
    }
};

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
      ...(name === 'name' ? { slug: generateSlug(value) } : {})
    }));
  };

  const handleVariationTypeChange = (variationTypeId: string, checked: boolean) => {
    setFormData(prev => {
      let updatedVariationTypes = prev.variationTypes;
      if (checked) {
        // Add to array
        updatedVariationTypes = [
          ...prev.variationTypes,
          { variationTypeId, isRequired: true }
        ];
      } else {
        // Remove from array
        updatedVariationTypes = prev.variationTypes.filter(v => v.variationTypeId !== variationTypeId);
      }
      return {
        ...prev,
        variationTypes: updatedVariationTypes
      };
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

  return (
    <div className="p-6 border rounded bg-white shadow">
      <h2 className="text-xl font-semibold mb-4">Create New Category</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
            Name
          </label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            required
            className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-indigo-500"
          />
        </div>

        <div>
          <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
            Description
          </label>
          <textarea
            id="description"
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            rows={4}
            className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-indigo-500"
          />
        </div>

        <div>
          <label htmlFor="parentCategory" className="block text-sm font-medium text-gray-700 mb-1">
            Parent Category
          </label>
          <select
            id="parentCategory"
            name="parentCategoryId"
            value={formData.parentCategoryId || ''}
            onChange={e => setFormData(prev => ({ ...prev, parentCategoryId: e.target.value || null }))}
            className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-1 focus:ring-indigo-500"
          >
            <option value="">Set as Root Category</option>
            {/* Flatten categories for dropdown */}
            {flattenCategories(categories).map(category => (
              <option key={category.id} value={category.id}>
                {category.displayNameForDropdown}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Variation Types
          </label>
          <div className="space-y-3 max-h-64 overflow-y-auto border border-gray-200 rounded p-3">
            {variationTypes
              .filter(vt => vt.isActive) // Only show active variation types
              .map((vt) => {
                const isSelected = formData.variationTypes.some(v => v.variationTypeId === vt.id);
                const selectedVt = formData.variationTypes.find(v => v.variationTypeId === vt.id);
                
                return (
                  <div key={vt.id} className="border border-gray-100 rounded p-3">
                    <div className="flex items-center mb-2">
                      <input
                        type="checkbox"
                        id={`vt-${vt.id}`}
                        checked={isSelected}
                        onChange={e => handleVariationTypeChange(vt.id, e.target.checked)}
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

        <div className="flex justify-end space-x-3 mt-6">
          <button
            type="button"
            onClick={onCancel}
            className="px-4 py-2 border rounded-md hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700"
          >
            Create Category
          </button>
        </div>
      </form>
    </div>
  );
}


// Helper to flatten nested categories for dropdown
function flattenCategories(categories: any[], prefix = ""): any[] {
  let result: any[] = [];
  for (const cat of categories) {
    result.push({
      ...cat,
      displayNameForDropdown: prefix ? `${prefix} > ${cat.name}` : cat.name
    });
    if (cat.subCategories && cat.subCategories.length > 0) {
      result = result.concat(flattenCategories(cat.subCategories, prefix ? `${prefix} > ${cat.name}` : cat.name));
    }
  }
  return result;
}

export default CreateCategoryForm;