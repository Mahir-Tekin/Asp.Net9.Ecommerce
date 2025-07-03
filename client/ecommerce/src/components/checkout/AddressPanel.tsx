'use client';
import { useEffect, useState } from 'react';
import AddressCard from './AddressCard';
import AddressFormModal from './AddressFormModal';
import type { Address } from '@/types/address';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';

interface AddressPanelProps {
  onSelect: (address: Address) => void;
  selectedAddressId?: string;
}

export default function AddressPanel({ onSelect, selectedAddressId }: AddressPanelProps) {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);

  // Fetch addresses on mount and when requested
  const fetchAddresses = async () => {
    setLoading(true);
    const token = localStorage.getItem('accessToken');
    const res = await fetch(`${API_URL}/api/Addresses`, {
      headers: token ? { Authorization: `Bearer ${token}` } : {},
    });
    const data = await res.json();
    setAddresses(Array.isArray(data) ? data : []);
    setLoading(false);
  };

  useEffect(() => {
    fetchAddresses();
  }, []);

  return (
    <section className="mb-8">
      <div className="flex items-center justify-between mb-2">
        <h2 className="text-lg font-semibold">Shipping Address</h2>
        <button className="text-green-600 hover:underline" onClick={() => setShowForm(true)}>
          Add Address
        </button>
      </div>
      {loading ? (
        <div>Loading addresses...</div>
      ) : addresses.length === 0 ? (
        <div>No addresses found. Please add one.</div>
      ) : (
        <div className="flex flex-col gap-2">
          {addresses.map(addr => (
            <AddressCard
              key={addr.id}
              address={addr}
              selected={selectedAddressId === addr.id}
              onSelect={() => onSelect(addr)}
            />
          ))}
        </div>
      )}
      {showForm && (
        <AddressFormModal
          onSuccess={() => {
            setShowForm(false);
            fetchAddresses();
          }}
        />
      )}
    </section>
  );
}
