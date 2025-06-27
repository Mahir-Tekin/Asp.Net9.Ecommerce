'use client';
import { useState } from 'react';

interface AdminOrderDetailsProps {
  order: any | null;
}

const ORDER_STATUSES = [
  'Pending',
  'Paid',
  'Shipped',
  'Delivered',
  'Cancelled',
];

export default function AdminOrderDetails({ order }: AdminOrderDetailsProps) {
  const [updating, setUpdating] = useState(false);
  const [updateError, setUpdateError] = useState<string | null>(null);
  const [status, setStatus] = useState(order?.status || '');

  if (!order) return <div className="p-8 text-gray-400">Select an order to view details.</div>;

  const handleStatusChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newStatus = e.target.value;
    setStatus(newStatus);
    setUpdating(true);
    setUpdateError(null);
    try {
      const token = localStorage.getItem('accessToken');
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Order/admin/${order.id}/status`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: JSON.stringify({ newStatus }),
        }
      );
      if (!res.ok) throw new Error('Failed to update status');
    } catch (err: any) {
      setUpdateError(err.message || 'Unknown error');
    } finally {
      setUpdating(false);
    }
  };

  return (
    <div className="flex-1 p-4">
      <h2 className="text-lg font-bold mb-4">Order Details</h2>
      <div className="mb-2"><span className="font-semibold">Order ID:</span> {order.id}</div>
      <div className="mb-2 flex items-center gap-2">
        <span className="font-semibold">Status:</span>
        <select
          value={status}
          onChange={handleStatusChange}
          className="border rounded px-2 py-1"
          disabled={updating}
        >
          {ORDER_STATUSES.map(s => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>
        {updating && <span className="text-xs text-gray-500 ml-2">Updating...</span>}
      </div>
      {updateError && <div className="text-red-500 text-sm mb-2">{updateError}</div>}
      <div className="mb-2"><span className="font-semibold">Created:</span> {new Date(order.createdAt).toLocaleString()}</div>
      <div className="mb-2"><span className="font-semibold">Total Amount:</span> ${order.totalAmount?.toFixed(2) ?? '-'}</div>
      <div className="mb-4">
        <span className="font-semibold">Items:</span>
        <div className="flex flex-col gap-2 mt-2">
          {order.items && order.items.length > 0 ? (
            order.items.map((item: any) => (
              <div key={item.productVariantId} className="flex items-center gap-4 border-b pb-2 last:border-b-0">
                {item.imageUrl && (
                  <img src={item.imageUrl} alt={item.productName} width={40} height={40} className="rounded" />
                )}
                <div className="flex-1">
                  <span className="font-semibold">{item.productName}</span>
                  {item.variantName && <span className="ml-2 text-sm text-gray-500">{item.variantName}</span>}
                </div>
                <div>x{item.quantity}</div>
                <div className="w-20 text-right text-sm">${item.unitPrice?.toFixed(2) ?? '-'}</div>
              </div>
            ))
          ) : (
            <div className="text-gray-400">No items</div>
          )}
        </div>
      </div>
      <div className="mb-4">
        <span className="font-semibold">Shipping Address:</span>
        {order.shippingAddress ? (
          <div className="ml-2 text-sm text-gray-700">
            <div><span className="font-semibold">{order.shippingAddress.addressTitle}</span></div>
            <div>{order.shippingAddress.firstName} {order.shippingAddress.lastName}</div>
            <div>{order.shippingAddress.phoneNumber}</div>
            <div>{order.shippingAddress.addressLine}</div>
            <div>{order.shippingAddress.neighborhood}, {order.shippingAddress.district}, {order.shippingAddress.city}</div>
          </div>
        ) : (
          <div className="ml-2 text-gray-400">No shipping address</div>
        )}
      </div>
    </div>
  );
}
