'use client';
import AdminOrderList from '@/components/admin/orders/AdminOrderList';
import AdminOrderDetails from '@/components/admin/orders/AdminOrderDetails';
import { useEffect, useState } from 'react';

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<any[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [selectedOrder, setSelectedOrder] = useState<any | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchOrders() {
      setLoading(true);
      setError(null);
      try {
        const token = localStorage.getItem('accessToken');
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Order/admin?page=1&pageSize=20`,
          {
            headers: token ? { Authorization: `Bearer ${token}` } : {},
          }
        );
        if (!res.ok) throw new Error('Failed to fetch orders');
        const data = await res.json();
        console.log('Admin orders response:', data); // debug log
        setOrders(Array.isArray(data.orders) ? data.orders : []);
      } catch (err: any) {
        setError(err.message || 'Unknown error');
      } finally {
        setLoading(false);
      }
    }
    fetchOrders();
  }, []);

  useEffect(() => {
    if (!selectedOrderId) {
      setSelectedOrder(null);
      return;
    }
    async function fetchOrderDetails() {
      setDetailsLoading(true);
      setDetailsError(null);
      try {
        const token = localStorage.getItem('accessToken');
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Order/admin/${selectedOrderId}`,
          {
            headers: token ? { Authorization: `Bearer ${token}` } : {},
          }
        );
        if (!res.ok) throw new Error('Failed to fetch order details');
        const data = await res.json();
        setSelectedOrder(data);
      } catch (err: any) {
        setDetailsError(err.message || 'Unknown error');
        setSelectedOrder(null);
      } finally {
        setDetailsLoading(false);
      }
    }
    fetchOrderDetails();
  }, [selectedOrderId]);

  return (
    <div className="flex max-w-5xl mx-auto p-8 min-h-[500px] bg-white rounded shadow">
      <div className="flex-1 flex">
        {loading ? (
          <div className="p-8">Loading orders...</div>
        ) : error ? (
          <div className="p-8 text-red-500">{error}</div>
        ) : (
          <AdminOrderList orders={orders} onSelect={order => setSelectedOrderId(order.id)} selectedOrderId={selectedOrderId} />
        )}
        {detailsLoading ? (
          <div className="flex-1 p-8">Loading order details...</div>
        ) : detailsError ? (
          <div className="flex-1 p-8 text-red-500">{detailsError}</div>
        ) : (
          <AdminOrderDetails order={selectedOrder} />
        )}
      </div>
    </div>
  );
}
