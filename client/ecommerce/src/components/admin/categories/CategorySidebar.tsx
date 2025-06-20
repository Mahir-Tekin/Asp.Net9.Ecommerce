'use client';

import React, { useState, useEffect } from 'react';
import axios from 'axios';

interface Category {
  id: string;
  name: string;
  subCategories: Category[];
}

interface CategorySidebarProps {
  selectedId: string | null;
  onSelect: (id: string) => void;
  onCreateNew: () => void;
}

export default function CategorySidebar({ selectedId, onSelect, onCreateNew }: CategorySidebarProps) {
  const [categories, setCategories] = useState<Category[]>([]);
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/Categories/admin`, {
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

  const renderCategories = (categories: Category[]) => {
    return (
      <ul className="space-y-1">
        {categories.map(category => (
          <li key={category.id} className="relative">
            <div
              onClick={() => onSelect(category.id)}
              className={`cursor-pointer p-2 rounded hover:bg-indigo-100 ${
                selectedId === category.id ? 'bg-indigo-200 font-medium' : ''
              }`}
            >
              {category.name}
            </div>
            {category.subCategories && category.subCategories.length > 0 && (
              <div className="ml-4">
                {renderCategories(category.subCategories)}
              </div>
            )}
          </li>
        ))}
      </ul>
    );
  };

  return (
    <div className="w-64 border-r p-4 bg-gray-50">
      <div className="flex justify-between items-center mb-4">
        <h3 className="text-lg font-semibold">Categories</h3>
        <button
          onClick={onCreateNew}
          className="text-sm text-indigo-600 hover:underline"
        >
          + New
        </button>
      </div>
      {renderCategories(categories)}
    </div>
  );
}
