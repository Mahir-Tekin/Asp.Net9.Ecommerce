import React, { useEffect, useState } from 'react';
import { useURLFilters } from '@/hooks/useURLFilters';
import { fetchCategories, type Category } from './Categories.api';
import { HiChevronDown, HiChevronRight } from 'react-icons/hi2';

type ExpandedSet = Set<string>;
const CategoryList: React.FC<{
  categories: Category[];
  expanded: ExpandedSet;
  setExpanded: React.Dispatch<React.SetStateAction<ExpandedSet>>;
  selectedId: string | null;
  onSelect: (id: string) => void;
  level?: number;
}> = ({ categories, expanded, setExpanded, selectedId, onSelect, level = 0 }) => (
  <ul className={`space-y-1 ${level > 0 ? 'ml-4 mt-2' : ''}`}>
    {categories.map((cat) => {
      const isLeaf = !cat.subCategories || cat.subCategories.length === 0;
      const isExpanded = expanded.has(cat.id);
      const isSelected = selectedId === cat.id;
      return (
        <li key={cat.id}>
          <div className="flex items-center">
            <button
              className={`flex-1 text-left px-3 py-2 rounded-md transition-all duration-200 text-sm ${
                isSelected 
                  ? 'bg-blue-100 text-blue-700 font-semibold border border-blue-200' 
                  : isExpanded
                  ? 'bg-gray-100 text-gray-700 hover:bg-gray-150'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-800'
              }`}
              onClick={() => {
                if (isLeaf) {
                  onSelect(cat.id);
                } else {
                  setExpanded(prev => {
                    const next = new Set(prev);
                    if (next.has(cat.id)) {
                      next.delete(cat.id);
                    } else {
                      next.add(cat.id);
                    }
                    return next;
                  });
                }
              }}
            >
              <div className="flex items-center justify-between">
                <span>{cat.name}</span>
                {!isLeaf && (
                  <div className="flex items-center space-x-2">
                    {cat.subCategories && (
                      <span className="text-xs text-gray-400 bg-gray-200 px-2 py-0.5 rounded-full">
                        {cat.subCategories.length}
                      </span>
                    )}
                    {isExpanded ? (
                      <HiChevronDown className="h-4 w-4 text-gray-400" />
                    ) : (
                      <HiChevronRight className="h-4 w-4 text-gray-400" />
                    )}
                  </div>
                )}
              </div>
            </button>
          </div>
          {!isLeaf && isExpanded && (
            <div className="overflow-hidden">
              <CategoryList
                categories={cat.subCategories}
                expanded={expanded}
                setExpanded={setExpanded}
                selectedId={selectedId}
                onSelect={onSelect}
                level={level + 1}
              />
            </div>
          )}
        </li>
      );
    })}
  </ul>
);

const Categories: React.FC = () => {
  const { filters, updateFilters } = useURLFilters();
  const [categories, setCategories] = useState<Category[]>([]);
  const [expanded, setExpanded] = useState<ExpandedSet>(new Set());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchCategories()
      .then(setCategories)
      .catch((err) => {
        const isDev = process.env.NODE_ENV === 'development';
        setError(isDev ? err.message : 'Failed to load categories');
      })
      .finally(() => setLoading(false));
  }, []);

  // Auto-expand all parent categories if a subcategory is selected
  useEffect(() => {
    if (filters.categoryId && categories.length > 0) {
      const findParentPath = (categoryId: string, cats: Category[], path: string[] = []): string[] | null => {
        for (const cat of cats) {
          if (cat.id === categoryId) {
            return path;
          }
          if (cat.subCategories) {
            const found = findParentPath(categoryId, cat.subCategories, [...path, cat.id]);
            if (found) return found;
          }
        }
        return null;
      };
      const parentPath = findParentPath(filters.categoryId, categories);
      if (parentPath) {
        setExpanded(new Set(parentPath));
      }
    }
  }, [filters.categoryId, categories]);

  const handleSelect = (id: string | null) => {
    updateFilters({ categoryId: id });
  };

  if (loading) {
    return (
      <div className="space-y-2">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="h-8 bg-gray-200 rounded animate-pulse" />
        ))}
      </div>
    );
  }
  
  if (error) {
    return (
      <div className="text-red-600 text-sm bg-red-50 p-3 rounded-md border border-red-200">
        {error}
      </div>
    );
  }

  return (
    <nav className="space-y-1">
      <CategoryList
        categories={categories}
        expanded={expanded}
        setExpanded={setExpanded}
        selectedId={filters.categoryId}
        onSelect={handleSelect}
      />
    </nav>
  );
};

export default Categories;
