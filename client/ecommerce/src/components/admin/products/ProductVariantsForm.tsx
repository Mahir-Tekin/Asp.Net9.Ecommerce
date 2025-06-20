// ProductVariantsForm.tsx
'use client';

import { useEffect, useState } from 'react';

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

export interface Variant {
  sku: string;
  name: string;
  price: number | null;
  stockQuantity: number;
  trackInventory: boolean;
  selectedOptions: { [variationTypeId: string]: string };
  useCustomPrice?: boolean;
}

interface ProductVariantsFormProps {
  variationTypes: VariationType[];
  selectedVariationTypeIds: string[];
  selectedOptionsByType: { [variationTypeId: string]: string[] };
  basePrice: number;
  onChange: (variants: Variant[]) => void;
  fieldErrors?: { [key: string]: string };
}

function getCombinations(arrays: string[][]): string[][] {
  if (arrays.length === 0) return [[]];
  const [first, ...rest] = arrays;
  const restComb = getCombinations(rest);
  return first.flatMap(f => restComb.map(r => [f, ...r]));
}

export default function ProductVariantsForm({
  variationTypes,
  selectedVariationTypeIds,
  selectedOptionsByType,
  basePrice,
  onChange,
  fieldErrors = {},
}: ProductVariantsFormProps) {
  const [variants, setVariants] = useState<Variant[]>([]);

  // Generate variants when variation types change
  useEffect(() => {
    if (selectedVariationTypeIds.length === 0) {
      setVariants([
        {
          sku: '',
          name: '',
          price: null,
          stockQuantity: 0,
          trackInventory: true,
          selectedOptions: {},
          useCustomPrice: false,
        },
      ]);
      return;
    }

    const selectedTypes = selectedVariationTypeIds
      .map(id => variationTypes.find(vt => vt.id === id))
      .filter(Boolean) as VariationType[];

    const optionsArrays = selectedTypes.map(vt => {
      const selectedOptIds = selectedOptionsByType[vt.id];
      return (selectedOptIds && selectedOptIds.length > 0)
        ? vt.options.filter(opt => selectedOptIds.includes(opt.id)).map(opt => opt.id)
        : [];
    });

    if (optionsArrays.some(arr => arr.length === 0)) {
      setVariants([]);
      return;
    }

    const combinations = getCombinations(optionsArrays);

    setVariants(prevVariants => {
      const newVariants: Variant[] = combinations.map(optionIds => {
        const selectedOptions: { [variationTypeId: string]: string } = {};
        optionIds.forEach((optId, idx) => {
          selectedOptions[selectedTypes[idx].id] = optId;
        });
        // Eski varyantı bul ve custom fiyatı koru
        const existing = prevVariants.find(v =>
          JSON.stringify(v.selectedOptions) === JSON.stringify(selectedOptions)
        );
        return existing
          ? { ...existing }
          : {
              sku: '',
              name: '',
              price: null,
              stockQuantity: 0,
              trackInventory: true,
              selectedOptions,
              useCustomPrice: false,
            };
      });
      return newVariants;
    });
  }, [selectedVariationTypeIds, variationTypes, basePrice, selectedOptionsByType]);

  // Notify parent on change
  useEffect(() => {
    onChange(variants);
  }, [variants, onChange]);

  const handleVariantChange = (idx: number, field: keyof Variant, value: any) => {
    setVariants(prev =>
      prev.map((v, i) => {
        if (i !== idx) return v;
        if (field === 'price') {
          return { ...v, price: value, useCustomPrice: true };
        }
        return { ...v, [field]: value };
      })
    );
  };

  const handleCustomPriceToggle = (idx: number, checked: boolean) => {
    setVariants(prev =>
      prev.map((v, i) => {
        if (i !== idx) return v;
        if (checked) {
          return { ...v, useCustomPrice: true, price: v.price ?? basePrice };
        } else {
          return { ...v, useCustomPrice: false, price: null };
        }
      })
    );
  };

  return (
    <div>
      <h4 className="font-semibold mb-2 mt-6">Product Variants</h4>
      <div className="overflow-x-auto">
        <table className="min-w-full border text-sm">
          <thead>
            <tr>
              {selectedVariationTypeIds.map(vtId => {
                const vt = variationTypes.find(v => v.id === vtId);
                return <th key={vtId} className="border px-2 py-1">{vt?.displayName}</th>;
              })}
              <th className="border px-2 py-1">SKU</th>
              <th className="border px-2 py-1">Name</th>
              <th className="border px-2 py-1">Price</th>
              <th className="border px-2 py-1">Stock</th>
              <th className="border px-2 py-1">Track Inventory</th>
            </tr>
          </thead>
          <tbody>
            {variants.map((variant, idx) => (
              <tr key={idx}>
                {selectedVariationTypeIds.map(vtId => {
                  const vt = variationTypes.find(v => v.id === vtId);
                  const optId = variant.selectedOptions[vtId];
                  const opt = vt?.options.find(o => o.id === optId);
                  return (
                    <td key={vtId} className="border px-2 py-1">
                      {opt?.displayValue || '-'}
                    </td>
                  );
                })}
                <td className="border px-2 py-1">
                  <input
                    type="text"
                    value={variant.sku}
                    onChange={e => handleVariantChange(idx, 'sku', e.target.value)}
                    className={`border rounded p-1 w-24 ${fieldErrors[`variant-sku-${idx}`] ? 'border-red-500' : ''}`}
                  />
                  {fieldErrors[`variant-sku-${idx}`] && (
                    <div className="text-red-500 text-xs mt-1">{fieldErrors[`variant-sku-${idx}`]}</div>
                  )}
                </td>
                <td className="border px-2 py-1">
                  <input
                    type="text"
                    value={variant.name}
                    onChange={e => handleVariantChange(idx, 'name', e.target.value)}
                    className={`border rounded p-1 w-24 ${fieldErrors[`variant-name-${idx}`] ? 'border-red-500' : ''}`}
                  />
                  {fieldErrors[`variant-name-${idx}`] && (
                    <div className="text-red-500 text-xs mt-1">{fieldErrors[`variant-name-${idx}`]}</div>
                  )}
                </td>
                <td className="border px-2 py-1">
                  <div className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={!!variant.useCustomPrice}
                      onChange={e => handleCustomPriceToggle(idx, e.target.checked)}
                      id={`custom-price-${idx}`}
                    />
                    <label htmlFor={`custom-price-${idx}`} className="text-xs select-none">Custom Price</label>
                  </div>
                  <input
                    type="number"
                    value={variant.useCustomPrice ? (variant.price ?? '') : ''}
                    onChange={e => handleVariantChange(idx, 'price', e.target.value === '' ? null : Number(e.target.value))}
                    className={`border rounded p-1 w-20 mt-1 ${fieldErrors[`variant-price-${idx}`] ? 'border-red-500' : ''}`}
                    min={0}
                    step={0.01}
                    disabled={!variant.useCustomPrice}
                    placeholder={variant.useCustomPrice ? '' : `Use main price (${basePrice})`}
                  />
                  {fieldErrors[`variant-price-${idx}`] && (
                    <div className="text-red-500 text-xs mt-1">{fieldErrors[`variant-price-${idx}`]}</div>
                  )}
                </td>
                <td className="border px-2 py-1">
                  <input
                    type="number"
                    value={variant.stockQuantity}
                    onChange={e => handleVariantChange(idx, 'stockQuantity', Number(e.target.value))}
                    className={`border rounded p-1 w-16 ${fieldErrors[`variant-stock-${idx}`] ? 'border-red-500' : ''}`}
                    min={0}
                  />
                  {fieldErrors[`variant-stock-${idx}`] && (
                    <div className="text-red-500 text-xs mt-1">{fieldErrors[`variant-stock-${idx}`]}</div>
                  )}
                </td>
                <td className="border px-2 py-1 text-center">
                  <input
                    type="checkbox"
                    checked={variant.trackInventory}
                    onChange={e => handleVariantChange(idx, 'trackInventory', e.target.checked)}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
