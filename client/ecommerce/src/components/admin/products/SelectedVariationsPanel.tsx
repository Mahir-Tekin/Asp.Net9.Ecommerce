import React from 'react';
import { VariationType, VariationOption } from './types';
import SelectedVariationTypeItem from './SelectedVariationTypeItem';

interface SelectedVariationsPanelProps {
  selectedVariationTypes: VariationType[];
  selectedOptionsByType: { [variationTypeId: string]: string[] };
  onEditOptions: (variationTypeId: string) => void;
  onRemoveType: (variationTypeId: string) => void;
}

const SelectedVariationsPanel: React.FC<SelectedVariationsPanelProps> = ({
  selectedVariationTypes,
  selectedOptionsByType,
  onEditOptions,
  onRemoveType,
}) => {
  return (
    <div className="border rounded p-3 bg-gray-50">
      <h5 className="font-semibold mb-2 text-sm">Selected Variations</h5>
      {selectedVariationTypes.length === 0 ? (
        <div className="text-xs text-gray-400">No variation types selected.</div>
      ) : (
        <div className="space-y-3">
          {selectedVariationTypes.map(type => (
            <SelectedVariationTypeItem
              key={type.id}
              variationType={type}
              selectedOptionIds={selectedOptionsByType[type.id] || []}
              onEditOptions={() => onEditOptions(type.id)}
              onRemoveType={() => onRemoveType(type.id)}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default SelectedVariationsPanel;
