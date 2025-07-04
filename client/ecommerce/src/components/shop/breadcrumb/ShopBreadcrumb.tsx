'use client';

import React, { useEffect, useState } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import Breadcrumb, { BreadcrumbItem } from '@/components/ui/Breadcrumb';
import { fetchCategories, type Category } from '@/components/shop/Categories/Categories.api';

export default function ShopBreadcrumb() {
  const { filters, updateFilters, clearAllFilters } = useURLFilters();
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    fetchCategories()
      .then(setCategories)
      .catch((err) => {
        console.error('Failed to fetch categories for breadcrumb:', err);
      });
  }, []);

  // Helper function to find a category by ID in the hierarchical structure
  const findCategoryById = (categoryId: string, categories: Category[]): Category | null => {
    for (const category of categories) {
      if (category.id === categoryId) {
        return category;
      }
      if (category.subCategories && category.subCategories.length > 0) {
        const found = findCategoryById(categoryId, category.subCategories);
        if (found) return found;
      }
    }
    return null;
  };

  // Helper function to build the category path (parent → child)
  const buildCategoryPath = (categoryId: string, categories: Category[]): Category[] => {
    const path: Category[] = [];
    
    const findPath = (targetId: string, cats: Category[], currentPath: Category[]): boolean => {
      for (const cat of cats) {
        const newPath = [...currentPath, cat];
        
        if (cat.id === targetId) {
          path.push(...newPath);
          return true;
        }
        
        if (cat.subCategories && cat.subCategories.length > 0) {
          if (findPath(targetId, cat.subCategories, newPath)) {
            return true;
          }
        }
      }
      return false;
    };

    findPath(categoryId, categories, []);
    return path;
  };

  const handleResetFilters = () => {
    clearAllFilters();
  };

  const generateBreadcrumbs = (): BreadcrumbItem[] => {
    const items: BreadcrumbItem[] = [
      { 
        label: 'Home', 
        href: '/',
        onClick: handleResetFilters
      }
    ];

    // If we're in shop but no filters applied, or if category is "all"
    if ((!filters.categoryId || filters.categoryId === 'all') && !filters.searchTerm) {
      items.push({ label: 'All Products', isActive: true });
      return items;
    }

    // If there's a category selected (and not "all"), build the category path
    if (filters.categoryId && filters.categoryId !== 'all' && categories.length > 0) {
      const categoryPath = buildCategoryPath(filters.categoryId, categories);
      
      categoryPath.forEach((category, index) => {
        const isLast = index === categoryPath.length - 1;
        const isActive = isLast && !filters.searchTerm;
        
        items.push({
          label: category.name,
          // No href or onClick - categories are display-only since backend only supports leaf categories
          isActive
        });
      });
    }

    // If there's a search term, add it
    if (filters.searchTerm) {
      items.push({ 
        label: `Search: "${filters.searchTerm}"`, 
        isActive: !filters.minPrice && !filters.maxPrice && !filters.sortBy && 
                  (!filters.variationFilters || Object.keys(filters.variationFilters).length === 0)
      });
    }

    // Add price range information
    if (filters.minPrice || filters.maxPrice) {
      let priceLabel = 'Price: ';
      if (filters.minPrice && filters.maxPrice) {
        priceLabel += `$${filters.minPrice} - $${filters.maxPrice}`;
      } else if (filters.minPrice) {
        priceLabel += `From $${filters.minPrice}`;
      } else if (filters.maxPrice) {
        priceLabel += `Up to $${filters.maxPrice}`;
      }
      
      items.push({ 
        label: priceLabel, 
        isActive: !filters.sortBy && (!filters.variationFilters || Object.keys(filters.variationFilters).length === 0)
      });
    }

    // Add variation filters
    if (filters.variationFilters && Object.keys(filters.variationFilters).length > 0) {
      const variationLabels = Object.entries(filters.variationFilters)
        .filter(([_, values]) => values && values.length > 0)
        .map(([name, values]) => `${name}: ${values.join(', ')}`);
      
      if (variationLabels.length > 0) {
        items.push({ 
          label: variationLabels.join(' • '), 
          isActive: !filters.sortBy
        });
      }
    }

    // Add sort information
    if (filters.sortBy) {
      const sortNames: { [key: string]: string } = {
        'name_asc': 'A-Z',
        'name_desc': 'Z-A', 
        'price_asc': 'Price ↑',
        'price_desc': 'Price ↓',
        'rating_desc': 'Top Rated',
        'newest': 'Newest',
        'oldest': 'Oldest'
      };
      
      items.push({ 
        label: `Sorted: ${sortNames[filters.sortBy] || filters.sortBy}`, 
        isActive: true
      });
    }

    return items;
  };

  const breadcrumbs = generateBreadcrumbs();

  return (
    <div className="bg-gray-50 border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 py-3">
        <Breadcrumb items={breadcrumbs} />
      </div>
    </div>
  );
}
