'use client';

import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface UpdateVariationTypeProps {
  id: string;
  onSuccess: () => void;
}

interface Option {
  id?: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

export default function UpdateVariationType({ id, onSuccess }: UpdateVariationTypeProps) {
  const [name, setName] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [isActive, setIsActive] = useState(true);
  const [options, setOptions] = useState<Option[]>([]);
  const [newOption, setNewOption] = useState<Option>({ value: '', displayValue: '', sortOrder: 0 });
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

  useEffect(() => {
    const fetchVariationType = async () => {
      try {
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/variation-types/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const { name, displayName, isActive, options } = response.data;
        setName(name);
        setDisplayName(displayName);
        setIsActive(isActive);
        setOptions(options);
      } catch (error) {
        console.error('Failed to fetch variation type:', error);
      }
    };
    fetchVariationType();
  }, [id, API_URL]);

  const handleAddOption = () => {
    setOptions([...options, newOption]);
    setNewOption({ value: '', displayValue: '', sortOrder: 0 });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('accessToken');
      await axios.put(
        `${API_URL}/api/variation-types/${id}`,
        { name, displayName, isActive, options },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      onSuccess();
    } catch (error) {
      console.error('Failed to update variation type:', error);
    }
  };

  return (
    <div className="p-4">
      <h3 className="text-lg font-semibold mb-4">Update Variation Type</h3>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border rounded p-2"
            required
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
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Active</label>
          <input
            type="checkbox"
            checked={isActive}
            onChange={(e) => setIsActive(e.target.checked)}
            className="mr-2"
          />
          <span>{isActive ? 'Yes' : 'No'}</span>
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
            />
            <input
              type="text"
              placeholder="Display Value"
              value={newOption.displayValue}
              onChange={(e) => setNewOption({ ...newOption, displayValue: e.target.value })}
              className="border rounded p-2"
            />
            <input
              type="number"
              placeholder="Sort Order"
              value={newOption.sortOrder}
              onChange={(e) => setNewOption({ ...newOption, sortOrder: parseInt(e.target.value) })}
              className="border rounded p-2"
            />
            <button
              type="button"
              onClick={handleAddOption}
              className="bg-indigo-600 text-white px-4 py-2 rounded"
            >
              Add Option
            </button>
          </div>
        </div>
        <button
          type="submit"
          className="bg-indigo-600 text-white px-4 py-2 rounded"
        >
          Update
        </button>
      </form>
    </div>
  );
}