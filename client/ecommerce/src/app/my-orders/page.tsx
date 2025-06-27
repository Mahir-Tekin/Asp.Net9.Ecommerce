'use client';
import { useEffect, useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';

interface OrderItem {
  productId: string;
  productVariantId: string;
  productName: string;
  variantName: string;
  quantity: number;
  imageUrl?: string;
}

interface Order {
  id: string;
  createdAt: string;
  status: string;
  totalAmount: number;
  items: OrderItem[];
}

export default function MyOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchOrders() {
      setLoading(true);
      setError(null);
      try {
        const token = localStorage.getItem('accessToken');
        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Order/my`, {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
        });
        if (!res.ok) throw new Error('Failed to fetch orders');
        const data = await res.json();
        setOrders(Array.isArray(data) ? data : []);
      } catch (err: any) {
        setError(err.message || 'Unknown error');
      } finally {
        setLoading(false);
      }
    }
    fetchOrders();
  }, []);

  return (
    <div className="max-w-3xl mx-auto p-8">
      <h1 className="text-2xl font-bold mb-6">My Orders</h1>
      {loading && <div>Loading orders...</div>}
      {error && <div className="text-red-500 mb-4">{error}</div>}
      {!loading && orders.length === 0 && <div>No orders found.</div>}
      <div className="flex flex-col gap-8">
        {orders.map(order => (
          <div key={order.id} className="border rounded p-4 bg-white">
            <div className="flex justify-between items-center mb-2">
              <div>
                <span className="font-semibold">Order ID:</span> {order.id}
              </div>
              <div className="text-sm text-gray-500">{new Date(order.createdAt).toLocaleString()}</div>
              <div className="text-sm font-semibold">Status: {order.status}</div>
            </div>
            <div className="flex flex-col gap-2 mt-2">
              {order.items.map(item => (
                <div key={item.productVariantId} className="flex items-center gap-4 border-b pb-2 last:border-b-0">
                  {item.imageUrl && (
                    <Image src={item.imageUrl} alt={item.productName} width={50} height={50} className="rounded" />
                  )}
                  <div className="flex-1">
                    <span className="font-semibold">{item.productName}</span>
                    {item.variantName && <span className="ml-2 text-sm text-gray-500">{item.variantName}</span>}
                  </div>
                  <div>x{item.quantity}</div>
                </div>
              ))}
            </div>
            <div className="text-right mt-2 font-bold">Total: ${order.totalAmount?.toFixed(2) ?? '-'}</div>
          </div>
        ))}
      </div>
      <Link href="/shop" className="inline-block mt-8 text-green-600 hover:underline">Continue Shopping</Link>
    </div>
  );
}
