'use client';
import React from 'react';
import type { Address } from '@/types/address';

interface AddressCardProps {
  address: Address;
  selected: boolean;
  onSelect: () => void;
}

const AddressCard: React.FC<AddressCardProps> = ({ address, selected, onSelect }) => {
  return (
    <div
      className={`border rounded p-3 flex items-center gap-3 cursor-pointer transition-colors ${selected ? 'border-green-600 bg-green-50' : 'border-gray-300 bg-white'}`}
      onClick={onSelect}
      tabIndex={0}
      role="button"
      aria-pressed={selected}
    >
      <input
        type="radio"
        name="address"
        checked={selected}
        onChange={onSelect}
        className="accent-green-600"
        tabIndex={-1}
      />
      <div>
        <div className="font-medium flex items-center gap-2">
          {address.addressTitle}
          {address.isMain && <span className="ml-2 px-2 py-0.5 text-xs bg-green-200 text-green-800 rounded">Main</span>}
        </div>
        <div className="text-sm text-gray-700 font-semibold">{address.firstName} {address.lastName} ({address.phoneNumber})</div>
        <div className="text-sm text-gray-600">
          {address.addressLine}, {address.neighborhood}, {address.district}, {address.city}
        </div>
      </div>
    </div>
  );
};

export default AddressCard;
