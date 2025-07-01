'use client';

import { useState, useMemo, useEffect } from 'react';
import { ProductVariationType, ProductVariant } from './ProductDetails.types';
import ProductOptionSelector from './ProductOptionSelector';

// Helper: get only the option IDs used by at least one variant (regardless of stock) for a given variation type
function getUsedOptionIdsAnyStock(variationTypeId: string, variants: ProductVariant[]) {
  const used = new Set<string>();
  variants.forEach(variant => {
    if (variant.selectedOptions[variationTypeId]) {
      used.add(variant.selectedOptions[variationTypeId]);
    }
  });
  return used;
}

interface ProductOptionPanelProps {
  variationTypes: ProductVariationType[];
  variants: ProductVariant[];
  onVariantChange?: (variant: ProductVariant | undefined) => void;
}

function getLowestPricedVariant(variants: ProductVariant[]) {
  return variants
    .filter(v => v.isActive && v.stockQuantity > 0)
    .reduce((min, v) => (min && min.price <= v.price ? min : v), undefined as ProductVariant | undefined);
}


export default function ProductOptionPanel({ variationTypes, variants, onVariantChange }: ProductOptionPanelProps) {
  const lowestVariant = getLowestPricedVariant(variants);
  const [selectedOptions, setSelectedOptions] = useState<{ [variationTypeId: string]: string }>(
    lowestVariant ? { ...lowestVariant.selectedOptions } : {}
  );

  // Find the selected variant based on selected options
  const selectedVariant = useMemo(() => {
    return variants.find(variant =>
      Object.entries(variant.selectedOptions).every(
        ([typeId, optionId]) => selectedOptions[typeId] === optionId
      ) &&
      Object.keys(selectedOptions).length === variationTypes.length
    );
  }, [selectedOptions, variants, variationTypes.length]);

  // Notify parent of variant change
  useEffect(() => {
    onVariantChange?.(selectedVariant);
  }, [selectedVariant, onVariantChange]);

  // Determine if an option should be disabled
  function isOptionDisabled(variationTypeId: string, optionId: string) {
    // Simulate selection with this option
    const simulated = { ...selectedOptions, [variationTypeId]: optionId };
    // Find if any variant matches simulated selection and is in stock
    return !variants.some(variant =>
      variant.isActive &&
      variant.stockQuantity > 0 &&
      Object.entries(simulated).every(
        ([typeId, optId]) => variant.selectedOptions[typeId] === optId
      )
    );
  }

  return (
    <div className="mb-4">
      {variationTypes.map((variationType) => {
        // Only show options that are used by at least one variant (regardless of stock)
        const usedOptionIds = getUsedOptionIdsAnyStock(variationType.id, variants);
        const filteredOptions = variationType.options.filter(option => usedOptionIds.has(option.id));
        if (filteredOptions.length === 0) return null; // Hide variation type if no options are used
        // Pass a shallow copy of variationType with filtered options
        return (
          <ProductOptionSelector
            key={variationType.id}
            variationType={{ ...variationType, options: filteredOptions }}
            selectedOptionId={selectedOptions[variationType.id]}
            onSelectOption={(optionId) =>
              setSelectedOptions((prev) => ({ ...prev, [variationType.id]: optionId }))
            }
            isOptionDisabled={(optionId) => isOptionDisabled(variationType.id, optionId)}
          />
        );
      })}
    </div>
  );
}
