'use client';

import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface VariationType {
  id: string;
  displayName: string;
}

interface ListVariationTypesProps {
  onSelect: (id: string) => void;
  onCreate: () => void;
}

export default function ListVariationTypes({ onSelect, onCreate }: ListVariationTypesProps) {
  const [variationTypes, setVariationTypes] = useState<VariationType[]>([]);
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchVariationTypes = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/variation-types`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        setVariationTypes(response.data);
      } catch (error) {
        console.error('Failed to fetch variation types:', error);
      }
    };
    fetchVariationTypes();
  }, [API_URL]);

  return (
    <div className="p-4 border-r bg-gray-50">
      <div className="flex justify-between items-center mb-4">
        <h3 className="text-lg font-semibold">Variation Types</h3>
        <button
          onClick={onCreate}
          className="text-sm text-indigo-600 hover:underline"
        >
          + New
        </button>
      </div>
      <ul className="space-y-2">
        {variationTypes.map((type) => (
          <li
            key={type.id}
            onClick={() => onSelect(type.id)}
            className="cursor-pointer p-2 rounded hover:bg-indigo-100"
          >
            {type.displayName}
          </li>
        ))}
      </ul>
    </div>
  );
}