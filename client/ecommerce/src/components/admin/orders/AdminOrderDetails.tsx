'use client';
import { useState } from 'react';
import Image from 'next/image';

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

  if (!order) return (
    <div className="flex-1 flex items-center justify-center text-gray-400 h-full min-h-[300px]">
      Select an order to view details.
    </div>
  );

  const handleStatusChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newStatus = e.target.value;
    setStatus(newStatus);
    setUpdating(true);
    setUpdateError(null);
    try {
      const token = localStorage.getItem('accessToken');
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Order/admin/${order.id}/status`,
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
    <div className="flex-1 flex justify-center items-start bg-neutral-50 min-h-[100vh] py-10">
      <div className="w-full max-w-2xl bg-white rounded-2xl shadow-lg border border-neutral-200 p-8">
        <h2 className="text-2xl font-bold mb-8 text-neutral-900 tracking-tight">Order Details</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <div>
            <div className="text-xs text-neutral-500 mb-1">Order ID</div>
            <div className="font-mono text-base font-semibold text-neutral-800">#{order.id}</div>
          </div>
          <div>
            <div className="text-xs text-neutral-500 mb-1">Status</div>
            <div className="flex items-center gap-2">
              <select
                value={status}
                onChange={handleStatusChange}
                className="border rounded px-2 py-1 min-w-[120px] focus:ring-2 focus:ring-green-200"
                disabled={updating}
              >
                {ORDER_STATUSES.map(s => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
              <span className={`text-xs font-semibold px-2 py-0.5 rounded-full border 
                ${status === 'Delivered' || status === 'Completed' ? 'bg-green-50 text-green-700 border-green-200' :
                  status === 'Pending' ? 'bg-yellow-50 text-yellow-700 border-yellow-200' :
                  status === 'Cancelled' ? 'bg-red-50 text-red-700 border-red-200' :
                  'bg-gray-50 text-gray-700 border-gray-200'}
              `}>
                {status}
              </span>
              {updating && <span className="text-xs text-gray-400 ml-2">Updating...</span>}
            </div>
            {updateError && <div className="text-red-500 text-xs mt-1">{updateError}</div>}
          </div>
          <div>
            <div className="text-xs text-neutral-500 mb-1">Created</div>
            <div className="text-base text-neutral-800">{new Date(order.createdAt).toLocaleString()}</div>
          </div>
          <div>
            <div className="text-xs text-neutral-500 mb-1">Total Amount</div>
            <div className="text-base font-semibold text-neutral-900">${order.totalAmount?.toFixed(2) ?? '-'}</div>
          </div>
        </div>

        <div className="mb-8">
          <div className="font-semibold mb-3 text-neutral-700 text-lg border-b pb-1">Items</div>
          <div className="flex flex-col gap-3 mt-2">
            {order.items && order.items.length > 0 ? (
              order.items.map((item: any) => (
                <div key={item.productVariantId} className="flex items-center gap-4 border-b pb-3 last:border-b-0">
                  {item.imageUrl && (() => {
                    const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
                    const src = item.imageUrl.startsWith('http') ? item.imageUrl : `${API_URL}${item.imageUrl}`;
                    return (
                      <Image src={src} alt={item.productName} width={48} height={48} className="rounded border bg-neutral-100 object-cover" />
                    );
                  })()}
                  <div className="flex-1">
                    <span className="font-semibold text-neutral-900">{item.productName}</span>
                    {item.variantName && <span className="ml-2 text-sm text-gray-500">{item.variantName}</span>}
                  </div>
                  <div className="text-xs text-gray-500">x{item.quantity}</div>
                  <div className="w-20 text-right text-sm font-medium text-neutral-800">${item.unitPrice?.toFixed(2) ?? '-'}</div>
                </div>
              ))
            ) : (
              <div className="text-gray-400">No items</div>
            )}
          </div>
        </div>

        <div className="mb-2">
          <div className="font-semibold mb-2 text-neutral-700 text-lg border-b pb-1">Shipping Address</div>
          {order.shippingAddress ? (
            <div className="ml-2 text-sm text-neutral-700 space-y-1">
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
    </div>
  );
}
