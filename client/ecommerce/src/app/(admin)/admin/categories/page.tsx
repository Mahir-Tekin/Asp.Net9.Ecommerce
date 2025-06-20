'use client';

import { useState } from 'react';
import CategorySidebar from '@/components/admin/categories/CategorySidebar';
import CreateCategoryForm from '@/components/admin/categories/CreateCategoryForm';
import EditCategoryForm from '@/components/admin/categories/EditCategoryForm';
import CategoryDetails from '@/components/admin/categories/CategoryDetails';
export default function CategoriesPage() {
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const [mode, setMode] = useState<'view' | 'edit' | 'create' | null>(null);

  return (
    <div className="flex h-full">
      <CategorySidebar
        selectedId={selectedCategoryId}
        onSelect={(id) => {
          setSelectedCategoryId(id);
          setMode('view');
        }}
        onCreateNew={() => {
          setSelectedCategoryId(null);
          setMode('create');
        }}
      />

      <div className="flex-1 p-4">
        {mode === 'create' && (
          <CreateCategoryForm
            onCancel={() => setMode(null)}
            onSuccess={() => setMode(null)}
          />
        )}

        {mode === 'view' && selectedCategoryId && (
          <CategoryDetails
            categoryId={selectedCategoryId}
            onEdit={() => setMode('edit')}
          />
        )}

        {mode === 'edit' && selectedCategoryId && (
          <EditCategoryForm
            categoryId={selectedCategoryId}
            onCancel={() => setMode('view')}
          />
        )}
      </div>
    </div>
  );
}