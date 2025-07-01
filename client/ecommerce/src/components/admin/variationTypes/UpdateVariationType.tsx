'use client';

import React, { useEffect, useState } from 'react';
import axios from 'axios';

interface UpdateVariationTypeProps {
  id: string;
  onSuccess: () => void;
  onCancel: () => void;
}

interface Option {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

export default function UpdateVariationType({ id, onSuccess, onCancel }: UpdateVariationTypeProps) {
  const [name, setName] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [isActive, setIsActive] = useState(true);
  const [options, setOptions] = useState<Option[]>([]);
  const [editingOptionId, setEditingOptionId] = useState<string | number | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
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
        setOptions(options.sort((a: Option, b: Option) => a.sortOrder - b.sortOrder));
      } catch (error) {
        console.error('Failed to fetch variation type:', error);
        setError('Failed to load variation type details');
      } finally {
        setLoading(false);
      }
    };
    fetchVariationType();
  }, [id, API_URL]);

  const handleAddOption = () => {
    const newSortOrder = options.length > 0 ? Math.max(...options.map(o => o.sortOrder)) + 1 : 0;
    const newOption: Option = {
      id: `temp-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`, // Temporary ID for new options
      value: '',
      displayValue: '',
      sortOrder: newSortOrder
    };
    const newOptions = [...options, newOption];
    setOptions(newOptions);
    setEditingOptionId(newOptions.length - 1); // Edit the new option immediately
  };

  const handleOptionChange = (index: number, field: keyof Option, value: string | number) => {
    const updatedOptions = options.map((option, i) => 
      i === index ? { ...option, [field]: value } : option
    );
    setOptions(updatedOptions);
  };

  const handleDeleteOption = (index: number) => {
    const updatedOptions = options.filter((_, i) => i !== index);
    setOptions(updatedOptions);
    setEditingOptionId(null);
  };

  const handleMoveOption = (index: number, direction: 'up' | 'down') => {
    const newOptions = [...options];
    const targetIndex = direction === 'up' ? index - 1 : index + 1;
    
    if (targetIndex >= 0 && targetIndex < newOptions.length) {
      // Swap the options
      [newOptions[index], newOptions[targetIndex]] = [newOptions[targetIndex], newOptions[index]];
      
      // Update sort orders
      newOptions.forEach((option, i) => {
        option.sortOrder = i;
      });
      
      setOptions(newOptions);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validation
    if (!name.trim() || !displayName.trim()) {
      setError('Name and display name are required');
      return;
    }

    // Check for duplicate option values
    const values = options.map(o => o.value.trim()).filter(v => v);
    const uniqueValues = new Set(values);
    if (values.length !== uniqueValues.size) {
      setError('Option values must be unique');
      return;
    }

    // Check for empty option values
    const hasEmptyOptions = options.some(o => !o.value.trim() || !o.displayValue.trim());
    if (hasEmptyOptions) {
      setError('All options must have both value and display value');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const token = localStorage.getItem('accessToken');
      
      // Update sort orders to be sequential and prepare options for API
      const sortedOptions = options.map((option, index) => {
        const apiOption: any = {
          value: option.value.trim(),
          displayValue: option.displayValue.trim(),
          sortOrder: index
        };
        
        // Only include ID if it's not a temporary ID (existing options)
        if (option.id && !option.id.startsWith('temp-')) {
          apiOption.id = option.id;
        }
        
        return apiOption;
      });

      await axios.put(
        `${API_URL}/api/variation-types/${id}`,
        { 
          name: name.trim(), 
          displayName: displayName.trim(), 
          isActive, 
          options: sortedOptions 
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
          },
        }
      );
      onSuccess();
    } catch (error: any) {
      console.error('Failed to update variation type:', error);
      setError(error.response?.data?.message || 'Failed to update variation type');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="p-6 border rounded-lg bg-white shadow max-w-4xl">
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

  return (
    <div className="p-6 border rounded-lg bg-white shadow max-w-4xl">
      <h2 className="text-2xl font-semibold mb-6 text-gray-900">Update Variation Type</h2>
      
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Basic Information */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Internal Name *
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="color"
              required
            />
            <p className="text-xs text-gray-500 mt-1">Used internally (no spaces, lowercase recommended)</p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Display Name *
            </label>
            <input
              type="text"
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Color"
              required
            />
            <p className="text-xs text-gray-500 mt-1">Shown to users</p>
          </div>
        </div>

        {/* Active Status */}
        <div className="flex items-center">
          <input
            type="checkbox"
            id="isActive"
            checked={isActive}
            onChange={(e) => setIsActive(e.target.checked)}
            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label htmlFor="isActive" className="ml-2 block text-sm text-gray-700">
            Active
          </label>
        </div>

        {/* Options */}
        <div>
          <div className="flex justify-between items-center mb-4">
            <label className="block text-sm font-medium text-gray-700">
              Options ({options.length})
            </label>
            <button
              type="button"
              onClick={handleAddOption}
              className="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Add Option
            </button>
          </div>

          {options.length > 0 ? (
            <div className="space-y-2">
              {options.map((option, index) => (
                <div key={index} className="border border-gray-200 rounded-lg p-4 bg-gray-50">
                  {editingOptionId === index ? (
                    // Edit mode
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                      <div>
                        <label className="block text-xs font-medium text-gray-600 mb-1">Internal Value</label>
                        <input
                          type="text"
                          value={option.value}
                          onChange={(e) => handleOptionChange(index, 'value', e.target.value)}
                          className="w-full px-2 py-1 text-sm border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500"
                          placeholder="red"
                        />
                      </div>
                      <div>
                        <label className="block text-xs font-medium text-gray-600 mb-1">Display Value</label>
                        <input
                          type="text"
                          value={option.displayValue}
                          onChange={(e) => handleOptionChange(index, 'displayValue', e.target.value)}
                          className="w-full px-2 py-1 text-sm border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500"
                          placeholder="Red"
                        />
                      </div>
                      <div className="flex items-end space-x-2">
                        <button
                          type="button"
                          onClick={() => setEditingOptionId(null)}
                          className="px-3 py-1 text-sm bg-green-600 text-white rounded hover:bg-green-700"
                        >
                          Done
                        </button>
                        <button
                          type="button"
                          onClick={() => handleDeleteOption(index)}
                          className="px-3 py-1 text-sm bg-red-600 text-white rounded hover:bg-red-700"
                        >
                          Delete
                        </button>
                      </div>
                    </div>
                  ) : (
                    // View mode
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-4">
                        <div className="flex items-center space-x-2">
                          <span className="text-sm font-medium text-gray-900">{option.displayValue}</span>
                          <span className="text-sm text-gray-500">({option.value})</span>
                        </div>
                        <span className="text-xs text-gray-400">Position: {index + 1}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        {/* Move buttons */}
                        <button
                          type="button"
                          onClick={() => handleMoveOption(index, 'up')}
                          disabled={index === 0}
                          className="p-1 text-gray-400 hover:text-gray-600 disabled:opacity-30 disabled:cursor-not-allowed"
                          title="Move up"
                        >
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7" />
                          </svg>
                        </button>
                        <button
                          type="button"
                          onClick={() => handleMoveOption(index, 'down')}
                          disabled={index === options.length - 1}
                          className="p-1 text-gray-400 hover:text-gray-600 disabled:opacity-30 disabled:cursor-not-allowed"
                          title="Move down"
                        >
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                          </svg>
                        </button>
                        {/* Edit button */}
                        <button
                          type="button"
                          onClick={() => setEditingOptionId(index)}
                          className="p-1 text-blue-600 hover:text-blue-800"
                          title="Edit option"
                        >
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                          </svg>
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300">
              <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M9 5H7a2 2 0 00-2 2v1a2 2 0 002 2h2m0 0h2a2 2 0 012 2v1a2 2 0 01-2 2H9m0-4h2m-2 0v4m0-4V9a2 2 0 012-2h2" />
              </svg>
              <h3 className="mt-2 text-sm font-medium text-gray-900">No options</h3>
              <p className="mt-1 text-sm text-gray-500">
                Add some options to this variation type.
              </p>
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex justify-end space-x-3 pt-4 border-t">
          <button
            type="button"
            onClick={onCancel}
            disabled={saving}
            className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500 disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={saving || !name.trim() || !displayName.trim()}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {saving ? 'Saving...' : 'Update Variation Type'}
          </button>
        </div>
      </form>
    </div>
  );
}