'use client';

import { useState, useEffect, useRef } from 'react';
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
      if (variant.price === undefined || variant.price === null || variant.price <= 0) {
        errors.push(`Price must be greater than 0 for variant #${idx + 1}.`);
        fieldErrs[`variant-price-${idx}`] = 'Price must be greater than 0.';
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
      onSuccess();
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
    <div>
      <h3 className="text-lg font-semibold mb-4">Create Product</h3>
      {/* Error summary */}
      {(frontendErrors.length > 0 || backendErrors.length > 0) && (
        <div className="bg-red-50 border border-red-400 text-red-700 rounded p-3 mb-4">
          <div className="font-semibold mb-1">Please fix the following errors:</div>
          <ul className="list-disc pl-5">
            {frontendErrors.map((err, i) => <li key={"fe-"+i}>{err}</li>)}
            {backendErrors.map((err, i) => <li key={"be-"+i}>{err}</li>)}
          </ul>
        </div>
      )}
      <form ref={formRef} onSubmit={handleSubmit} className="space-y-4">

        {/* Name */}
        <div>
          <label className="block text-sm font-medium mb-1">Name</label>
          <input
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            className={`w-full border rounded p-2 ${fieldErrors['name'] ? 'border-red-500' : ''}`}
            required
          />
          {fieldErrors['name'] && <div className="text-red-500 text-xs mt-1">{fieldErrors['name']}</div>}
        </div>

        {/* Description */}
        <div>
          <label className="block text-sm font-medium mb-1">Description</label>
          <textarea value={description} onChange={e => setDescription(e.target.value)} className="w-full border rounded p-2" rows={3} />
        </div>

        {/* Base Price */}
        <div>
          <label className="block text-sm font-medium mb-1">Base Price</label>
          <input
            type="number"
            value={basePrice}
            onChange={e => setBasePrice(Number(e.target.value))}
            className={`w-full border rounded p-2 ${fieldErrors['basePrice'] ? 'border-red-500' : ''}`}
            min={0}
            step={0.01}
            required
          />
          {fieldErrors['basePrice'] && <div className="text-red-500 text-xs mt-1">{fieldErrors['basePrice']}</div>}
        </div>

        {/* Category */}
        <div>
          <label className="block text-sm font-medium mb-1">Category</label>
          {loadingCategories ? (
            <div>Loading categories...</div>
          ) : (
            <>
              <select
                value={categoryId}
                onChange={e => setCategoryId(e.target.value)}
                className={`w-full border rounded p-2 ${fieldErrors['categoryId'] ? 'border-red-500' : ''}`}
                required
              >
                <option value="">Select a category</option>
                {flattenLeafCategories(categories).map(cat => (
                  <option key={cat.id} value={cat.id}>{cat.name}</option>
                ))}
              </select>
              {fieldErrors['categoryId'] && <div className="text-red-500 text-xs mt-1">{fieldErrors['categoryId']}</div>}
            </>
          )}
        </div>

      {/* Product Images */}
      <div>
        <ProductImagesManager images={images} setImages={setImages} />
        {fieldErrors['images'] && <div className="text-red-500 text-xs mt-1">{fieldErrors['images']}</div>}
      </div>

        {/* Variation Types */}
        <div>
          <label className="block text-sm font-medium mb-1">Variation Types</label>
          {loadingVariationTypes ? (
            <div>Loading variation types...</div>
          ) : (
            <>
              <button type="button" className="bg-indigo-500 text-white px-3 py-1 rounded" onClick={() => setShowVariationModal(true)}>
                Select Variation Types
              </button>



              <SelectedVariationsPanel
                selectedVariationTypes={variationTypes.filter(vt => selectedVariationTypeIds.includes(vt.id))}
                selectedOptionsByType={selectedOptionsByType}
                onEditOptions={(vtId: string) => setEditingOptionsTypeId(vtId)}
                onRemoveType={(vtId: string) => setSelectedVariationTypeIds(prev => prev.filter(id => id !== vtId))}
              />

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
            </>
          )}
        </div>

        {/* Product Variants Form */}
        {/* Combinations will be generated only based on the selected options */}
        <ProductVariantsForm
          variationTypes={variationTypes}
          selectedVariationTypeIds={selectedVariationTypeIds}
          selectedOptionsByType={selectedOptionsByType}
          basePrice={basePrice}
          onChange={setVariants}
          fieldErrors={fieldErrors}
        />

        {/* Submit Buttons */}
        <div className="flex gap-2">
          <button type="submit" className="bg-indigo-600 text-white px-4 py-2 rounded">Save</button>
          <button type="button" onClick={onCancel} className="bg-gray-300 px-4 py-2 rounded">Cancel</button>
        </div>

      </form>
    </div>
  );
}
