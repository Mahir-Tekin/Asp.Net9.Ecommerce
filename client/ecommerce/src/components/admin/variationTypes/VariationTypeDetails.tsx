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
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
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
        setError('Failed to load variation type details');
      } finally {
        setLoading(false);
      }
    };
    fetchVariationTypeDetails();
  }, [id, API_URL]);

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

  if (error || !variationType) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow max-w-2xl">
        <div className="text-red-600 mb-4">{error || 'Variation type not found'}</div>
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
        <div>
          <h2 className="text-2xl font-semibold text-gray-900">{variationType.displayName}</h2>
          {variationType.displayName !== variationType.name && (
            <p className="text-sm text-gray-500 mt-1">Internal name: {variationType.name}</p>
          )}
        </div>
        <div className="flex items-center space-x-2">
          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            variationType.isActive 
              ? 'bg-green-100 text-green-800' 
              : 'bg-red-100 text-red-800'
          }`}>
            {variationType.isActive ? 'Active' : 'Inactive'}
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
            <label className="block text-sm font-medium text-gray-700 mb-1">Display Name</label>
            <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md">
              {variationType.displayName}
            </p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Internal Name</label>
            <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md font-mono">
              {variationType.name}
            </p>
          </div>
        </div>

        {/* Options */}
        {variationType.options && variationType.options.length > 0 && (
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Options ({variationType.options.length})
            </label>
            <div className="border border-gray-200 rounded-md">
              <div className="bg-gray-50 px-4 py-2 border-b border-gray-200">
                <div className="grid grid-cols-3 gap-4 text-xs font-medium text-gray-500 uppercase tracking-wider">
                  <span>Display Value</span>
                  <span>Internal Value</span>
                  <span>Sort Order</span>
                </div>
              </div>
              <div className="divide-y divide-gray-200">
                {variationType.options
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((option) => (
                  <div key={option.id} className="px-4 py-3 grid grid-cols-3 gap-4">
                    <span className="text-sm font-medium text-gray-900">{option.displayValue}</span>
                    <span className="text-sm text-gray-500 font-mono">{option.value}</span>
                    <span className="text-sm text-gray-500">{option.sortOrder}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* No Options Message */}
        {(!variationType.options || variationType.options.length === 0) && (
          <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M9 5H7a2 2 0 00-2 2v1a2 2 0 002 2h2m0 0h2a2 2 0 012 2v1a2 2 0 01-2 2H9m0-4h2m-2 0v4m0-4V9a2 2 0 012-2h2" />
            </svg>
            <h3 className="mt-2 text-sm font-medium text-gray-900">No options</h3>
            <p className="mt-1 text-sm text-gray-500">
              This variation type doesn't have any options defined.
            </p>
          </div>
        )}

        {/* Variation Type ID for reference */}
        <div className="pt-4 border-t border-gray-200">
          <p className="text-xs text-gray-500">
            <strong>Variation Type ID:</strong> <span className="font-mono">{variationType.id}</span>
          </p>
        </div>
      </div>
    </div>
  );
}