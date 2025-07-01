'use client';

import React, { useState } from 'react';
import axios from 'axios';


interface CreateVariationTypeProps {
  onSuccess: () => void;
}

export default function CreateVariationType({ onSuccess }: CreateVariationTypeProps) {
  const [name, setName] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [options, setOptions] = useState<{ value: string; displayValue: string; sortOrder: number }[]>([]);
  const [newOption, setNewOption] = useState({ value: '', displayValue: '', sortOrder: 0 });
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';



  const handleAddOption = () => {
    setOptions([...options, newOption]);
    setNewOption({ value: '', displayValue: '', sortOrder: 0 });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setSuccess(null);
    setError(null);
    try {
      const token = localStorage.getItem('accessToken');
      await axios.post(
        `${API_URL}/api/variation-types`,
        { name, displayName, options },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSuccess('Variation type created successfully!');
      setName('');
      setDisplayName('');
      setOptions([]);
      setNewOption({ value: '', displayValue: '', sortOrder: 0 });
      onSuccess();
    } catch (error) {
      setError('Failed to create variation type.');
      console.error('Failed to create variation type:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4">
      <h3 className="text-lg font-semibold mb-4">Create Variation Type</h3>
      <form onSubmit={handleSubmit} className="space-y-4">
        {success && <div className="text-green-600 bg-green-50 border border-green-200 rounded p-2">{success}</div>}
        {error && <div className="text-red-600 bg-red-50 border border-red-200 rounded p-2">{error}</div>}
        <div>
          <label className="block text-sm font-medium mb-1">Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border rounded p-2"
            required
            disabled={loading}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Display Name</label>
          <input
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            className="w-full border rounded p-2"
            required
            disabled={loading}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Options</label>
          <div className="space-y-2">
            {options.map((option, index) => (
              <div key={index} className="flex items-center space-x-2">
                <span>{option.displayValue}</span>
                <span className="text-gray-500">({option.value})</span>
                <span className="text-gray-500">Sort Order: {option.sortOrder}</span>
              </div>
            ))}
          </div>
          <div className="flex items-center space-x-2 mt-2">
            <input
              type="text"
              placeholder="Value"
              value={newOption.value}
              onChange={(e) => setNewOption({ ...newOption, value: e.target.value })}
              className="border rounded p-2"
              disabled={loading}
            />
            <input
              type="text"
              placeholder="Display Value"
              value={newOption.displayValue}
              onChange={(e) => setNewOption({ ...newOption, displayValue: e.target.value })}
              className="border rounded p-2"
              disabled={loading}
            />
            <input
              type="number"
              placeholder="Sort Order"
              value={newOption.sortOrder}
              onChange={(e) => setNewOption({ ...newOption, sortOrder: parseInt(e.target.value) })}
              className="border rounded p-2"
              disabled={loading}
            />
            <button
              type="button"
              onClick={handleAddOption}
              className="bg-indigo-600 text-white px-4 py-2 rounded"
              disabled={loading}
            >
              Add Option
            </button>
          </div>
        </div>
        <button
          type="submit"
          className="bg-indigo-600 text-white px-4 py-2 rounded"
          disabled={loading}
        >
          {loading ? 'Creating...' : 'Create'}
        </button>
      </form>
    </div>
  );
}