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
  const [categories, setCategories] = useState<Category[]>([]);
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
          { variationTypeId, isRequired: false }
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
            {categories.map(category => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Variation Types
          </label>
          <div className="flex flex-col gap-2">
            {variationTypes.map((variation) => (
              <label key={variation.id} className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={formData.variationTypes.some(v => v.variationTypeId === variation.id)}
                  onChange={e => handleVariationTypeChange(variation.id, e.target.checked)}
                />
                {variation.displayName || variation.name}
              </label>
            ))}
          </div>
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

export default CreateCategoryForm;