'use client';

import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface VariationTypeDetailsProps {
  id: string;
  onEdit: () => void;
}

interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  options: {
    id: string;
    value: string;
    displayValue: string;
    sortOrder: number;
  }[];
}

export default function VariationTypeDetails({ id, onEdit }: VariationTypeDetailsProps) {
  const [variationType, setVariationType] = useState<VariationType | null>(null);
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchVariationTypeDetails = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/variation-types/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        setVariationType(response.data);
      } catch (error) {
        console.error('Failed to fetch variation type details:', error);
      }
    };
    fetchVariationTypeDetails();
  }, [id, API_URL]);

  if (!variationType) {
    return <div>Loading...</div>;
  }

  return (
    <div className="p-4">
      <h3 className="text-lg font-semibold mb-4">Variation Type Details</h3>
      <div className="space-y-2">
        <div>
          <strong>Name:</strong> {variationType.name}
        </div>
        <div>
          <strong>Display Name:</strong> {variationType.displayName}
        </div>
        <div>
          <strong>Active:</strong> {variationType.isActive ? 'Yes' : 'No'}
        </div>
        <div>
          <strong>Created At:</strong> {new Date(variationType.createdAt).toLocaleString()}
        </div>
        <div>
          <strong>Updated At:</strong> {new Date(variationType.updatedAt).toLocaleString()}
        </div>
        <div>
          <strong>Options:</strong>
          <ul className="list-disc ml-6">
            {variationType.options.map((option) => (
              <li key={option.id}>
                {option.displayValue} ({option.value}) - Sort Order: {option.sortOrder}
              </li>
            ))}
          </ul>
        </div>
      </div>
      <button
        onClick={onEdit}
        className="mt-4 bg-indigo-600 text-white px-4 py-2 rounded"
      >
        Edit
      </button>
    </div>
  );
}