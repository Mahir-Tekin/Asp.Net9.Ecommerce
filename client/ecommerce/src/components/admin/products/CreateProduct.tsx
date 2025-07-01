'use client';

import { useState, useEffect, useRef } from 'react';
import { ArrowLeftIcon, PlusIcon, XMarkIcon, ExclamationTriangleIcon } from '@heroicons/react/24/outline';
import VariantSelectionModal from './VariantSelectionModal';
import ProductVariantsForm, { Variant } from './ProductVariantsForm';
import SelectedVariationsPanel from './SelectedVariationsPanel';
import VariationOptionsEditModal from './VariationOptionsEditModal';
import ProductImagesManager from './ProductImagesManager';
import axios from 'axios';
import { ProductImage } from './types';

interface VariationOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
  options: VariationOption[];
}

interface Category {
  id: string;
  name: string;
  subCategories?: Category[];
}

interface CreateProductProps {
  onSuccess: () => void;
  onCancel: () => void;
}

export default function CreateProduct({ onSuccess, onCancel }: CreateProductProps) {
  // Success feedback state
  const [success, setSuccess] = useState<string | null>(null);
  // ERROR STATE
  const [frontendErrors, setFrontendErrors] = useState<string[]>([]);
  const [fieldErrors, setFieldErrors] = useState<{ [key: string]: string }>({});
  const [backendErrors, setBackendErrors] = useState<string[]>([]);
  const formRef = useRef<HTMLFormElement>(null);

  // PRODUCT STATE
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [basePrice, setBasePrice] = useState(0);
  const [categoryId, setCategoryId] = useState('');
  const [variants, setVariants] = useState<Variant[]>([]);

  // IMAGES STATE
  const [images, setImages] = useState<ProductImage[]>([]);

  // CATEGORY STATE
  const [categories, setCategories] = useState<Category[]>([]);
  const [loadingCategories, setLoadingCategories] = useState(true);

  // VARIATION STATE
  const [variationTypes, setVariationTypes] = useState<VariationType[]>([]);
  const [loadingVariationTypes, setLoadingVariationTypes] = useState(true);
  const [selectedVariationTypeIds, setSelectedVariationTypeIds] = useState<string[]>([]);
  const [showVariationModal, setShowVariationModal] = useState(false);
  // Option selection state: { [variationTypeId]: string[] }
  const [selectedOptionsByType, setSelectedOptionsByType] = useState<{ [variationTypeId: string]: string[] }>({});
  // State for tracking which variation type's options are being edited
  const [editingOptionsTypeId, setEditingOptionsTypeId] = useState<string | null>(null);

  // If a selected variation type is removed, also remove its options
  useEffect(() => {
    setSelectedOptionsByType(prev => {
      const updated: { [variationTypeId: string]: string[] } = {};
      selectedVariationTypeIds.forEach(id => {
        if (prev[id]) updated[id] = prev[id];
      });
      return updated;
    });
  }, [selectedVariationTypeIds]);

  // FETCH CATEGORIES
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/Categories`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setCategories(response.data);
      } catch (error) {
        console.error('Failed to fetch categories:', error);
      } finally {
        setLoadingCategories(false);
      }
    };
    fetchCategories();
  }, []);

  // FETCH VARIATION TYPES
  useEffect(() => {
    const fetchVariationTypes = async () => {
      try {
        const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
        const token = localStorage.getItem('accessToken');
        const response = await axios.get(`${API_URL}/api/variation-types`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setVariationTypes(response.data);
      } catch (error) {
        console.error('Failed to fetch variation types:', error);
      } finally {
        setLoadingVariationTypes(false);
      }
    };
    fetchVariationTypes();
  }, []);

  // FLATTEN LEAF CATEGORIES
  const flattenLeafCategories = (cats: Category[], prefix = ''): { id: string; name: string }[] =>
    cats.flatMap(cat =>
      cat.subCategories && cat.subCategories.length > 0
        ? flattenLeafCategories(cat.subCategories, prefix + cat.name + ' / ')
        : [{ id: cat.id, name: prefix + cat.name }]
    );


  // Validation helpers
  function validateSKU(sku: string) {
    // Example: at least 3 chars, alphanumeric and dashes/underscores
    return /^[A-Za-z0-9-_]{3,}$/.test(sku);
  }

  function validateForm() {
    const errors: string[] = [];
    const fieldErrs: { [key: string]: string } = {};

    if (!name.trim()) {
      errors.push('Product name is required.');
      fieldErrs['name'] = 'Product name is required.';
    }
    if (!categoryId) {
      errors.push('Category is required.');
      fieldErrs['categoryId'] = 'Category is required.';
    }
    if (!basePrice || basePrice <= 0) {
      errors.push('Base price must be greater than 0.');
      fieldErrs['basePrice'] = 'Base price must be greater than 0.';
    }
    if (!images || images.length === 0) {
      errors.push('At least one product image is required.');
      fieldErrs['images'] = 'At least one product image is required.';
    }
    // Validate variants
    variants.forEach((variant, idx) => {
      if (!variant.sku.trim()) {
        errors.push(`SKU is required for variant #${idx + 1}.`);
        fieldErrs[`variant-sku-${idx}`] = 'SKU is required.';
      } else if (!validateSKU(variant.sku)) {
        errors.push(`SKU format is invalid for variant #${idx + 1}.`);
        fieldErrs[`variant-sku-${idx}`] = 'SKU format is invalid.';
      }
      if (!variant.name.trim()) {
        errors.push(`Name is required for variant #${idx + 1}.`);
        fieldErrs[`variant-name-${idx}`] = 'Name is required.';
      }
      // Only validate custom prices when useCustomPrice is true
      if (variant.useCustomPrice && (variant.price === undefined || variant.price === null || variant.price <= 0)) {
        errors.push(`Custom price must be greater than 0 for variant #${idx + 1}.`);
        fieldErrs[`variant-price-${idx}`] = 'Custom price must be greater than 0.';
      }
      if (variant.stockQuantity === undefined || variant.stockQuantity === null || variant.stockQuantity < 0) {
        errors.push(`Stock quantity must be 0 or more for variant #${idx + 1}.`);
        fieldErrs[`variant-stock-${idx}`] = 'Stock quantity must be 0 or more.';
      }
    });
    return { errors, fieldErrs };
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFrontendErrors([]);
    setFieldErrors({});
    setBackendErrors([]);

    const { errors, fieldErrs } = validateForm();
    if (errors.length > 0) {
      setFrontendErrors(errors);
      setFieldErrors(fieldErrs);
      // Log validation errors for debugging
      console.error('Validation errors:', errors, fieldErrs);
      // Scroll to first error
      setTimeout(() => {
        if (formRef.current) {
          const el = formRef.current.querySelector('.border-red-500');
          if (el && el.scrollIntoView) el.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
      }, 100);
      return;
    }

    const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
    const token = localStorage.getItem('accessToken');

    // Varyantlarda useCustomPrice false ise price alanını null gönder, true ise price değerini gönder
    const preparedVariants = variants.map(v => {
      if (v.useCustomPrice) {
        return { ...v, price: v.price };
      } else {
        // price alanını hiç göndermeyelim (backend ana fiyatı uygular)
        const { price, ...rest } = v;
        return rest;
      }
    });

    const payload = {
      name,
      description,
      basePrice,
      categoryId,
      variants: preparedVariants,
      variantTypeIds: selectedVariationTypeIds,
      images,
    };

    try {
      await axios.post(`${API_URL}/api/Products`, payload, {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      });
      setSuccess('Product created successfully!');
      setTimeout(() => {
        setSuccess(null);
        onSuccess();
      }, 1200);
    } catch (err: any) {
      // Log the full server response for debugging
      if (err?.response) {
        // Log the full error object and errors array in detail
        console.error('Product creation error:', JSON.stringify(err.response.data, null, 2));
        // Try to extract backend validation errors
        if (Array.isArray(err.response.data?.errors)) {
          setBackendErrors(err.response.data.errors.map((e: any) => e.errorMessage || e.message || JSON.stringify(e)));
        } else if (err.response.data?.message) {
          setBackendErrors([err.response.data.message]);
        } else {
          setBackendErrors(['Product creation failed.']);
        }
      } else {
        setBackendErrors(['Product creation failed.']);
      }
    }
  };


  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200">
      {/* Header */}
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-center gap-3 mb-2">
          <button
            onClick={onCancel}
            className="inline-flex items-center text-gray-500 hover:text-gray-700 transition-colors"
          >
            <ArrowLeftIcon className="w-5 h-5 mr-1" />
            Back
          </button>
        </div>
        <h2 className="text-2xl font-bold text-gray-900">Create New Product</h2>
        <p className="text-gray-600">Add a new product to your inventory with variants and pricing</p>
      </div>

      {/* Success Message */}
      {success && (
        <div className="p-6 bg-green-50 border-b border-green-200">
          <div className="flex items-start">
            <svg className="w-5 h-5 text-green-500 mt-0.5 mr-3 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" /></svg>
            <div className="flex-1">
              <h3 className="text-sm font-medium text-green-800 mb-2">Product created successfully!</h3>
            </div>
          </div>
        </div>
      )}
      {/* Error Messages */}
      {(frontendErrors.length > 0 || backendErrors.length > 0) && (
        <div className="p-6 bg-red-50 border-b border-red-200">
          <div className="flex items-start">
            <ExclamationTriangleIcon className="w-5 h-5 text-red-400 mt-0.5 mr-3 flex-shrink-0" />
            <div className="flex-1">
              <h3 className="text-sm font-medium text-red-800 mb-2">Please fix the following errors:</h3>
              <ul className="text-sm text-red-700 space-y-1">
                {frontendErrors.map((err, i) => (
                  <li key={`fe-${i}`} className="flex items-start">
                    <span className="w-1 h-1 bg-red-400 rounded-full mt-2 mr-2 flex-shrink-0"></span>
                    {err}
                  </li>
                ))}
                {backendErrors.map((err, i) => (
                  <li key={`be-${i}`} className="flex items-start">
                    <span className="w-1 h-1 bg-red-400 rounded-full mt-2 mr-2 flex-shrink-0"></span>
                    {err}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}

      <form ref={formRef} onSubmit={handleSubmit} className="p-6 space-y-8" noValidate>
        {/* Basic Information Section */}
        <div>
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Basic Information</h3>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div>
              <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                Product Name *
              </label>
              <input
                type="text"
                id="name"
                value={name}
                onChange={e => setName(e.target.value)}
                className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                  fieldErrors['name'] ? 'border-red-500' : 'border-gray-300'
                }`}
                placeholder="Enter product name"
                required
              />
              {fieldErrors['name'] && (
                <p className="mt-1 text-sm text-red-600">{fieldErrors['name']}</p>
              )}
            </div>

            <div>
              <label htmlFor="basePrice" className="block text-sm font-medium text-gray-700 mb-2">
                Base Price *
              </label>
              <div className="relative">
                <span className="absolute left-3 top-2 text-gray-500">$</span>
                <input
                  type="number"
                  id="basePrice"
                  value={basePrice}
                  onChange={e => setBasePrice(Number(e.target.value))}
                  className={`w-full pl-8 pr-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    fieldErrors['basePrice'] ? 'border-red-500' : 'border-gray-300'
                  }`}
                  placeholder="0.00"
                  min={0}
                  step={0.01}
                  required
                />
              </div>
              {fieldErrors['basePrice'] && (
                <p className="mt-1 text-sm text-red-600">{fieldErrors['basePrice']}</p>
              )}
            </div>
          </div>

          <div className="mt-6">
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
              Description
            </label>
            <textarea
              id="description"
              value={description}
              onChange={e => setDescription(e.target.value)}
              rows={4}
              className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              placeholder="Enter product description"
            />
          </div>

          <div className="mt-6">
            <label htmlFor="categoryId" className="block text-sm font-medium text-gray-700 mb-2">
              Category *
            </label>
            {loadingCategories ? (
              <div className="flex items-center py-3">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600 mr-3"></div>
                <span className="text-gray-500">Loading categories...</span>
              </div>
            ) : (
              <>
                <select
                  id="categoryId"
                  value={categoryId}
                  onChange={e => setCategoryId(e.target.value)}
                  className={`w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                    fieldErrors['categoryId'] ? 'border-red-500' : 'border-gray-300'
                  }`}
                  required
                >
                  <option value="">Select a category</option>
                  {flattenLeafCategories(categories).map(cat => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
                {fieldErrors['categoryId'] && (
                  <p className="mt-1 text-sm text-red-600">{fieldErrors['categoryId']}</p>
                )}
              </>
            )}
          </div>
        </div>

        {/* Product Images Section */}
        <div>
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Product Images</h3>
          <div className={`border-2 border-dashed rounded-lg p-6 ${
            fieldErrors['images'] ? 'border-red-300 bg-red-50' : 'border-gray-300 bg-gray-50'
          }`}>
            <ProductImagesManager images={images} setImages={setImages} />
            {fieldErrors['images'] && (
              <p className="mt-2 text-sm text-red-600">{fieldErrors['images']}</p>
            )}
          </div>
        </div>

        {/* Variation Types Section */}
        <div>
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-900">Variation Types</h3>
            {!loadingVariationTypes && (
              <button
                type="button"
                onClick={() => setShowVariationModal(true)}
                className="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <PlusIcon className="w-4 h-4 mr-1.5" />
                Add Variation Types
              </button>
            )}
          </div>

          {loadingVariationTypes ? (
            <div className="flex items-center py-6 justify-center bg-gray-50 rounded-lg">
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600 mr-3"></div>
              <span className="text-gray-500">Loading variation types...</span>
            </div>
          ) : (
            <div className="bg-gray-50 rounded-lg p-6">
              <SelectedVariationsPanel
                selectedVariationTypes={variationTypes.filter(vt => selectedVariationTypeIds.includes(vt.id))}
                selectedOptionsByType={selectedOptionsByType}
                onEditOptions={(vtId: string) => setEditingOptionsTypeId(vtId)}
                onRemoveType={(vtId: string) => setSelectedVariationTypeIds(prev => prev.filter(id => id !== vtId))}
              />

              {selectedVariationTypeIds.length === 0 && (
                <div className="text-center py-6">
                  <p className="text-gray-500 mb-3">No variation types selected</p>
                  <p className="text-sm text-gray-400">
                    Add variation types like Size, Color, Material to create product variants
                  </p>
                </div>
              )}
            </div>
          )}

          {/* Modal for editing options of a variation type */}
          {editingOptionsTypeId && (
            (() => {
              const vt = variationTypes.find(v => v.id === editingOptionsTypeId);
              if (!vt) return null;
              return (
                <VariationOptionsEditModal
                  variationType={vt}
                  selectedOptionIds={selectedOptionsByType[vt.id] || []}
                  onSave={(selected: string[]) => {
                    setSelectedOptionsByType(prev => ({ ...prev, [vt.id]: selected }));
                  }}
                  onClose={() => setEditingOptionsTypeId(null)}
                />
              );
            })()
          )}

          {showVariationModal && (
            <VariantSelectionModal
              variationTypes={variationTypes}
              selectedIds={selectedVariationTypeIds}
              onChange={setSelectedVariationTypeIds}
              onClose={() => setShowVariationModal(false)}
            />
          )}
        </div>

        {/* Product Variants Section */}
        <div>
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Product Variants</h3>
          <div className="bg-gray-50 rounded-lg p-6">
            <ProductVariantsForm
              variationTypes={variationTypes}
              selectedVariationTypeIds={selectedVariationTypeIds}
              selectedOptionsByType={selectedOptionsByType}
              basePrice={basePrice}
              onChange={setVariants}
              fieldErrors={fieldErrors}
            />
          </div>
        </div>

        {/* Form Actions */}
        <div className="flex items-center justify-end gap-3 pt-6 border-t border-gray-200">
          <button
            type="button"
            onClick={onCancel}
            className="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Create Product
          </button>
        </div>
      </form>
    </div>
  );
}
