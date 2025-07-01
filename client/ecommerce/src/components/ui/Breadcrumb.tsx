'use client';

import React from 'react';
import Link from 'next/link';
import { HiChevronRight } from 'react-icons/hi2';

export interface BreadcrumbItem {
  label: string;
  href?: string;
  isActive?: boolean;
  onClick?: () => void;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
  className?: string;
}

export default function Breadcrumb({ items, className = '' }: BreadcrumbProps) {
  if (items.length === 0) return null;

  return (
    <nav 
      aria-label="Breadcrumb" 
      className={`flex items-center space-x-1 text-sm text-gray-600 ${className}`}
    >
      {items.map((item, index) => (
        <React.Fragment key={index}>
          {index > 0 && (
            <HiChevronRight className="h-4 w-4 text-gray-400 flex-shrink-0" />
          )}
          
          {item.href && !item.isActive ? (
            <Link
              href={item.href}
              className="hover:text-blue-600 transition-colors duration-200 truncate max-w-32 sm:max-w-48"
              title={item.label}
              onClick={item.onClick}
            >
              {item.label}
            </Link>
          ) : item.onClick ? (
            <button
              onClick={item.onClick}
              className="hover:text-blue-600 transition-colors duration-200 truncate max-w-32 sm:max-w-48 text-left"
              title={item.label}
            >
              {item.label}
            </button>
          ) : (
            <span 
              className={`truncate max-w-32 sm:max-w-48 ${
                item.isActive ? 'text-gray-900 font-medium' : 'text-gray-600'
              }`}
              title={item.label}
            >
              {item.label}
            </span>
          )}
        </React.Fragment>
      ))}
    </nav>
  );
}
