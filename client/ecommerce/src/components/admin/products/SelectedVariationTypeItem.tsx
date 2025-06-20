import React from 'react';
import { VariationType } from './types';

interface SelectedVariationTypeItemProps {
  variationType: VariationType;
  selectedOptionIds: string[];
  onEditOptions: () => void;
  onRemoveType: () => void;
}

const SelectedVariationTypeItem: React.FC<SelectedVariationTypeItemProps> = ({
  variationType,
  selectedOptionIds,
  onEditOptions,
  onRemoveType,
}) => {
  return (
    <div className="border rounded p-2 bg-white">
      <div className="flex items-center justify-between mb-1">
        <span className="font-medium text-sm">{variationType.displayName}</span>
        <div className="flex gap-2">
          <button type="button" className="text-xs text-blue-600 hover:underline" onClick={onEditOptions}>
            Düzenle
          </button>
          <button type="button" className="text-xs text-red-500 hover:underline" onClick={onRemoveType}>
            Kaldır
          </button>
        </div>
      </div>
      <div className="flex flex-wrap gap-2">
        {variationType.options
          .filter(opt => selectedOptionIds.includes(opt.id))
          .map(opt => (
            <span key={opt.id} className="bg-gray-200 px-2 py-0.5 rounded text-xs">
              {opt.displayValue}
            </span>
          ))}
        {selectedOptionIds.length === 0 && (
            <span className="text-xs text-gray-400">No options selected</span>
        )}
      </div>
    </div>
  );
};

export default SelectedVariationTypeItem;
