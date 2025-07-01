'use client';

import React, { useState } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import { createFilterURL } from '@/utils/urlFilters';
import { HiLink, HiClipboard, HiCheck } from 'react-icons/hi2';

export default function FilterURLDisplay() {
  const { filters } = useURLFilters();
  const [copied, setCopied] = useState(false);

  const hasActiveFilters = 
    filters.searchTerm ||
    filters.categoryId ||
    filters.minPrice ||
    filters.maxPrice ||
    filters.sortBy ||
    (filters.variationFilters && Object.keys(filters.variationFilters).length > 0);

  if (!hasActiveFilters) {
    return null;
  }

  const currentURL = `${window.location.origin}${createFilterURL(filters)}`;

  const copyToClipboard = async () => {
    try {
      await navigator.clipboard.writeText(currentURL);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Failed to copy URL:', err);
    }
  };

  return (
    <div className="bg-gray-50 rounded-lg border border-gray-200 p-3 mb-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2 text-sm text-gray-600">
          <HiLink className="w-4 h-4" />
          <span className="font-medium">Current Filter URL:</span>
        </div>
        <button
          onClick={copyToClipboard}
          className="flex items-center gap-1 text-xs text-blue-600 hover:text-blue-700 bg-blue-50 hover:bg-blue-100 px-2 py-1 rounded transition-colors duration-200"
        >
          {copied ? (
            <>
              <HiCheck className="w-3 h-3" />
              Copied!
            </>
          ) : (
            <>
              <HiClipboard className="w-3 h-3" />
              Copy
            </>
          )}
        </button>
      </div>
      <div className="mt-2">
        <code className="text-xs text-gray-700 bg-white px-2 py-1 rounded border break-all">
          {currentURL}
        </code>
      </div>
    </div>
  );
}
