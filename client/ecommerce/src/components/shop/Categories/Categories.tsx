import React, { useEffect, useState } from 'react';
import { useProductFilters } from '@/context/ProductFilterContext';
import { fetchCategories, type Category } from './Categories.api';

const CategoryList: React.FC<{
  categories: Category[];
  expanded: string | null;
  setExpanded: (id: string | null) => void;
  selectedId: string | null;
  onSelect: (id: string) => void;
}> = ({ categories, expanded, setExpanded, selectedId, onSelect }) => (
  <ul className="space-y-2">
    {categories.map((cat) => {
      const isLeaf = !cat.subCategories || cat.subCategories.length === 0;
      const isExpanded = expanded === cat.id;
      return (
        <li key={cat.id}>
          <div className="flex items-center">
            <button
              className={`flex-1 text-left px-2 py-1 rounded ${isLeaf && selectedId === cat.id ? 'bg-blue-200 font-bold' : ''} ${!isLeaf && isExpanded ? 'bg-gray-100' : ''}`}
              onClick={() => {
                if (isLeaf) onSelect(cat.id);
                else setExpanded(isExpanded ? null : cat.id);
              }}
              disabled={!isLeaf && (!cat.subCategories || cat.subCategories.length === 0)}
            >
              {cat.name}
            </button>
            {!isLeaf && (
              <span className="ml-2 cursor-pointer" onClick={() => setExpanded(isExpanded ? null : cat.id)}>
                {isExpanded ? '-' : '+'}
              </span>
            )}
          </div>
          {!isLeaf && isExpanded && (
            <div className="ml-4 mt-1">
              <CategoryList
                categories={cat.subCategories}
                expanded={expanded}
                setExpanded={setExpanded}
                selectedId={selectedId}
                onSelect={onSelect}
              />
            </div>
          )}
        </li>
      );
    })}
  </ul>
);

const Categories: React.FC = () => {
  const { filters, setFilters } = useProductFilters();
  const [categories, setCategories] = useState<Category[]>([]);
  const [expanded, setExpanded] = useState<string | null>(null);
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

  const handleSelect = (id: string | null) => {
    setFilters((prev: any) => ({ ...prev, categoryId: id }));
  };

  if (loading) return <div>Loading categories...</div>;
  if (error) return <div>{error}</div>;

  return (
    <nav>
      <ul className="space-y-2">
        <li>
          <button
            className={`w-full text-left px-2 py-1 rounded ${!filters.categoryId ? 'bg-blue-100 font-bold' : ''}`}
            onClick={() => handleSelect(null)}
          >
            All
          </button>
        </li>
      </ul>
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
