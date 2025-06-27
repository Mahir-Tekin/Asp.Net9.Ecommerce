import { ProductVariationType } from './ProductDetails.types';

interface ProductOptionSelectorProps {
  variationType: ProductVariationType;
  selectedOptionId?: string;
  onSelectOption: (optionId: string) => void;
  isOptionDisabled?: (optionId: string) => boolean;
}

export default function ProductOptionSelector({
  variationType,
  selectedOptionId,
  onSelectOption,
  isOptionDisabled,
}: ProductOptionSelectorProps) {
  return (
    <div className="mb-2">
      <div className="font-semibold mb-1">{variationType.name}:</div>
      <div className="flex gap-2 flex-wrap">
        {variationType.options.map((option) => {
          const disabled = isOptionDisabled ? isOptionDisabled(option.id) : false;
          const selected = selectedOptionId === option.id;
          return (
            <button
              key={option.id}
              type="button"
              className={`px-3 py-1 border rounded text-sm ${
                selected ? 'bg-indigo-600 text-white' : 'bg-gray-100 text-gray-700'
              } ${disabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-indigo-100'}`}
              onClick={() => onSelectOption(option.id)}
              disabled={disabled}
            >
              {option.displayValue}
            </button>
          );
        })}
      </div>
    </div>
  );
}
