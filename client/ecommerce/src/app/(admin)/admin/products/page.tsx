// Products admin page with two-column layout and placeholder components
'use client';

import { useState } from 'react';
import ProductList from '@/components/admin/products/ProductList';
import ProductDetails from '@/components/admin/products/ProductDetails';
import CreateProduct from '@/components/admin/products/CreateProduct';
import EditProduct from '@/components/admin/products/EditProduct';

export default function ProductsPage() {
  const [view, setView] = useState<'details' | 'create' | 'edit'>('details');
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);

  const refreshList = () => {
    // Logic to refresh product list (to be implemented)
  };

  return (
    <div style={{ display: 'flex', gap: '1rem' }}>
      {/* Left: Product List */}
      <div style={{ flex: 1 }}>
        <ProductList
          onSelect={(id) => {
            setSelectedProductId(id);
            setView('details');
          }}
          onCreate={() => setView('create')}
        />
      </div>
      {/* Right: Conditional */}
      <div style={{ flex: 2 }}>
        {view === 'details' && selectedProductId && (
          <ProductDetails
            id={selectedProductId}
            onEdit={() => setView('edit')}
            onDelete={refreshList}
          />
        )}
        {view === 'create' && (
          <CreateProduct
            onSuccess={refreshList}
            onCancel={() => setView('details')}
          />
        )}
        {view === 'edit' && selectedProductId && (
          <EditProduct
            id={selectedProductId}
            onSuccess={refreshList}
            onCancel={() => setView('details')}
          />
        )}
      </div>
    </div>
  );
}
