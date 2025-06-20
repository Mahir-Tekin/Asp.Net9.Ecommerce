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
  variationTypes: { variationTypeId: string; isRequired: boolean }[];
}

export function CategoryDetails({ categoryId, onEdit }: { categoryId: string; onEdit: () => void }) {
  const [category, setCategory] = useState<Category | null>(null);
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
      }
    };
    fetchCategory();
  }, [API_URL, categoryId]);

  if (!category) {
    return <div>Loading...</div>;
  }

  return (
    <div className="p-4 border rounded bg-white shadow">
      <h2 className="text-xl font-semibold mb-2">{category.name}</h2>
      <p><strong>Description:</strong> {category.description}</p>
      <p><strong>Slug:</strong> {category.slug}</p>
      <p><strong>Active:</strong> {category.isActive ? 'Yes' : 'No'}</p>
      <p><strong>Parent Category ID:</strong> {category.parentCategoryId || 'None'}</p>
      <div className="mt-4">
        <button onClick={onEdit} className="bg-blue-500 text-white px-3 py-1 rounded">Edit</button>
      </div>
    </div>
  );
}

export default CategoryDetails;
