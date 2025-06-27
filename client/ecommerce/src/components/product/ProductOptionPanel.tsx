'use client';

import { useState, useMemo, useEffect } from 'react';
import { ProductVariationType, ProductVariant } from './ProductDetails.types';
import ProductOptionSelector from './ProductOptionSelector';

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
      {variationTypes.map((variationType) => (
        <ProductOptionSelector
          key={variationType.id}
          variationType={variationType}
          selectedOptionId={selectedOptions[variationType.id]}
          onSelectOption={(optionId) =>
            setSelectedOptions((prev) => ({ ...prev, [variationType.id]: optionId }))
          }
          isOptionDisabled={(optionId) => isOptionDisabled(variationType.id, optionId)}
        />
      ))}
    </div>
  );
}
