import { useState } from 'react';

interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
}

interface VariantSelectionModalProps {
  variationTypes: VariationType[];
  selectedIds: string[];
  onChange: (ids: string[]) => void;
  onClose: () => void;
}

export default function VariantSelectionModal({
  variationTypes,
  selectedIds,
  onChange,
  onClose,
}: VariantSelectionModalProps) {
  const [tempSelected, setTempSelected] = useState<string[]>(selectedIds);

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded shadow-lg min-w-[300px]">
        <h4 className="font-semibold mb-4">Select Variation Types</h4>
        <div className="max-h-60 overflow-y-auto">
          {variationTypes.map((vt) => (
            <label key={vt.id} className="block">
              <input
                type="checkbox"
                value={vt.id}
                checked={tempSelected.includes(vt.id)}
                onChange={e => {
                  if (e.target.checked) {
                    setTempSelected(prev => [...prev, vt.id]);
                  } else {
                    setTempSelected(prev => prev.filter(id => id !== vt.id));
                  }
                }}
              />
              <span className="ml-2">{vt.displayName}</span>
            </label>
          ))}
        </div>
        <div className="flex gap-2 mt-4">
          <button
            type="button"
            className="bg-indigo-600 text-white px-4 py-2 rounded"
            onClick={() => {
              onChange(tempSelected);
              onClose();
            }}
          >
            Confirm
          </button>
          <button
            type="button"
            className="bg-gray-300 px-4 py-2 rounded"
            onClick={onClose}
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
