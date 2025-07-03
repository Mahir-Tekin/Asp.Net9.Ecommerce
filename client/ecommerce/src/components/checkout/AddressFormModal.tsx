import { useState } from 'react';
import type { Address } from '@/types/address';

interface AddressFormModalProps {
  onSuccess: () => void;
}

export default function AddressFormModal({ onSuccess }: AddressFormModalProps) {
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    phoneNumber: '',
    city: '',
    district: '',
    neighborhood: '',
    addressLine: '',
    addressTitle: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm({
      ...form,
      [name]: value,
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('accessToken');
      const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Addresses`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(form),
      });
      if (!res.ok) throw new Error('Failed to add address');
      onSuccess();
    } catch (err: any) {
      setError(err.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded shadow-md w-full max-w-md">
        <h2 className="text-lg font-bold mb-4">Add Address</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3">
          <input name="addressTitle" placeholder="Address Title" value={form.addressTitle} onChange={handleChange} className="border p-2 rounded" required />
          <input name="firstName" placeholder="First Name" value={form.firstName} onChange={handleChange} className="border p-2 rounded" required />
          <input name="lastName" placeholder="Last Name" value={form.lastName} onChange={handleChange} className="border p-2 rounded" required />
          <input name="phoneNumber" placeholder="Phone Number" value={form.phoneNumber} onChange={handleChange} className="border p-2 rounded" required />
          <input name="city" placeholder="City" value={form.city} onChange={handleChange} className="border p-2 rounded" required />
          <input name="district" placeholder="District" value={form.district} onChange={handleChange} className="border p-2 rounded" required />
          <input name="neighborhood" placeholder="Neighborhood" value={form.neighborhood} onChange={handleChange} className="border p-2 rounded" required />
          <input name="addressLine" placeholder="Address Line" value={form.addressLine} onChange={handleChange} className="border p-2 rounded" required />
          {error && <div className="text-red-500 text-sm">{error}</div>}
          <div className="flex gap-2 mt-2">
            <button type="submit" className="bg-green-600 text-white px-4 py-2 rounded" disabled={loading}>
              {loading ? 'Saving...' : 'Save'}
            </button>
            <button type="button" className="bg-gray-300 px-4 py-2 rounded" onClick={onSuccess} disabled={loading}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
