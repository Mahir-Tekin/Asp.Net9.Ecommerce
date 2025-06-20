import React, { useState } from 'react';
import { VariationType } from './types'; 

interface VariationOptionsEditModalProps {
  variationType: VariationType;
  selectedOptionIds: string[];
  onSave: (selected: string[]) => void;
  onClose: () => void;
}

const VariationOptionsEditModal: React.FC<VariationOptionsEditModalProps> = ({
  variationType,
  selectedOptionIds,
  onSave,
  onClose,
}) => {
  const [selected, setSelected] = useState<string[]>(selectedOptionIds);

  const handleToggle = (id: string) => {
    setSelected(prev =>
      prev.includes(id) ? prev.filter(optId => optId !== id) : [...prev, id]
    );
  };

  const handleSave = () => {
    onSave(selected);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white rounded shadow-lg p-6 min-w-[320px]">
        <h4 className="font-semibold mb-3 text-sm">{variationType.displayName} Select Options</h4>
        <div className="space-y-2 mb-4">
          {variationType.options.map(opt => (
            <label key={opt.id} className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={selected.includes(opt.id)}
                onChange={() => handleToggle(opt.id)}
              />
              {opt.displayValue}
            </label>
          ))}
        </div>
        <div className="flex gap-2 justify-end">
          <button type="button" className="px-3 py-1 rounded bg-gray-200" onClick={onClose}>Cancel</button>
          <button type="button" className="px-3 py-1 rounded bg-indigo-600 text-white" onClick={handleSave}>Save</button>
        </div>
      </div>
    </div>
  );
};

export default VariationOptionsEditModal;
