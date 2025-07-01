// Products admin page with single-view layout for better space efficiency
'use client';

import { useState } from 'react';
import ProductList from '@/components/admin/products/ProductList';
import ProductDetails from '@/components/admin/products/ProductDetails';
import CreateProduct from '@/components/admin/products/CreateProduct';
import EditProduct from '@/components/admin/products/EditProduct';

type ViewType = 'list' | 'details' | 'create' | 'edit';

export default function ProductsPage() {
  const [view, setView] = useState<ViewType>('list');
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);

  const handleProductSelect = (id: string) => {
    setSelectedProductId(id);
    setView('details');
  };

  const handleCreateNew = () => {
    setSelectedProductId(null);
    setView('create');
  };

  const handleEdit = () => {
    setView('edit');
  };

  const handleSuccess = () => {
    setView('list');
    setSelectedProductId(null);
  };

  const handleCancel = () => {
    if (view === 'create') {
      setView('list');
    } else {
      setView('details');
    }
  };

  const handleBackToList = () => {
    setView('list');
    setSelectedProductId(null);
  };

  const handleDelete = () => {
    setView('list');
    setSelectedProductId(null);
  };

  return (
    <div className="max-w-7xl mx-auto">
      {/* Breadcrumb Navigation */}
      <div className="mb-6">
        <nav className="flex items-center space-x-2 text-sm text-gray-500">
          <button
            onClick={handleBackToList}
            className={`hover:text-gray-700 ${view === 'list' ? 'text-gray-900 font-medium' : ''}`}
          >
            Products
          </button>
          {view === 'details' && (
            <>
              <span>/</span>
              <span className="text-gray-900 font-medium">Product Details</span>
            </>
          )}
          {view === 'create' && (
            <>
              <span>/</span>
              <span className="text-gray-900 font-medium">Create Product</span>
            </>
          )}
          {view === 'edit' && (
            <>
              <span>/</span>
              <button onClick={() => setView('details')} className="hover:text-gray-700">
                Product Details
              </button>
              <span>/</span>
              <span className="text-gray-900 font-medium">Edit Product</span>
            </>
          )}
        </nav>
      </div>

      {/* Main Content */}
      <div className="transition-all duration-200 ease-in-out">
        {view === 'list' && (
          <ProductList
            onSelect={handleProductSelect}
            onCreate={handleCreateNew}
          />
        )}

        {view === 'details' && selectedProductId && (
          <ProductDetails
            id={selectedProductId}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onBack={handleBackToList}
          />
        )}

        {view === 'create' && (
          <CreateProduct
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        )}

        {view === 'edit' && selectedProductId && (
          <EditProduct
            id={selectedProductId}
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        )}
      </div>
    </div>
  );
}
