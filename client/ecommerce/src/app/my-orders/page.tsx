'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { 
  HiShoppingBag, 
  HiClock, 
  HiTruck, 
  HiCheckCircle, 
  HiXCircle, 
  HiArrowLeft, 
  HiEye,
  HiCalendarDays,
  HiCurrencyDollar,
  HiExclamationTriangle
} from 'react-icons/hi2';

interface OrderItem {
  productId: string;
  productVariantId: string;
  productName: string;
  productSlug: string;
  variantName: string;
  quantity: number;
  unitPrice: number;
  imageUrl?: string;
}

interface ShippingAddress {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  city: string;
  district: string;
  neighborhood: string;
  addressLine: string;
  addressTitle: string;
}

interface Order {
  id: string;
  createdAt: string;
  status: string;
  totalAmount: number;
  items: OrderItem[];
  shippingAddress: ShippingAddress;
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
        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Order/my`, {
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

  const getStatusIcon = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return <HiClock className="w-5 h-5 text-yellow-600" />;
      case 'processing':
        return <HiTruck className="w-5 h-5 text-blue-600" />;
      case 'shipped':
        return <HiTruck className="w-5 h-5 text-blue-600" />;
      case 'delivered':
        return <HiCheckCircle className="w-5 h-5 text-green-600" />;
      case 'cancelled':
        return <HiXCircle className="w-5 h-5 text-red-600" />;
      default:
        return <HiShoppingBag className="w-5 h-5 text-gray-600" />;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'processing':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'shipped':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'delivered':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'cancelled':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 py-8">
          {/* Header Skeleton */}
          <div className="mb-8">
            <div className="h-8 w-32 bg-gray-200 rounded animate-pulse mb-2"></div>
            <div className="h-6 w-64 bg-gray-200 rounded animate-pulse"></div>
          </div>

          {/* Orders Skeleton */}
          <div className="space-y-6">
            {[1, 2, 3].map((i) => (
              <div key={i} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <div className="flex justify-between items-center mb-4">
                  <div className="h-6 w-32 bg-gray-200 rounded animate-pulse"></div>
                  <div className="h-8 w-24 bg-gray-200 rounded animate-pulse"></div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
                  <div className="h-4 bg-gray-200 rounded animate-pulse"></div>
                  <div className="h-4 bg-gray-200 rounded animate-pulse"></div>
                  <div className="h-4 bg-gray-200 rounded animate-pulse"></div>
                </div>
                <div className="h-32 bg-gray-200 rounded animate-pulse"></div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="text-center">
            <HiExclamationTriangle className="w-16 h-16 text-red-500 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-900 mb-2">Unable to Load Orders</h2>
            <p className="text-gray-600 mb-6">{error}</p>
            <button
              onClick={() => window.location.reload()}
              className="px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center gap-4 mb-4">
            <Link
              href="/"
              className="flex items-center gap-2 text-gray-600 hover:text-purple-600 transition-colors"
            >
              <HiArrowLeft className="w-5 h-5" />
              <span>Back to Shop</span>
            </Link>
          </div>
          
          <div className="flex items-center gap-3 mb-2">
            <HiShoppingBag className="w-8 h-8 text-purple-600" />
            <h1 className="text-3xl font-bold text-gray-900">My Orders</h1>
          </div>
          <p className="text-gray-600">Track and manage your order history</p>
        </div>

        {/* Orders List */}
        {orders.length === 0 ? (
          <div className="text-center py-16">
            <HiShoppingBag className="w-20 h-20 text-gray-300 mx-auto mb-6" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No Orders Yet</h3>
            <p className="text-gray-600 mb-6">Start shopping to see your orders here</p>
            <Link
              href="/"
              className="inline-flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200"
            >
              <HiShoppingBag className="w-5 h-5" />
              Start Shopping
            </Link>
          </div>
        ) : (
          <div className="space-y-6">
            {orders.map((order) => (
              <div key={order.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
                {/* Order Header */}
                <div className="p-6 border-b border-gray-100">
                  <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-4">
                      <div className="flex items-center gap-2">
                        {getStatusIcon(order.status)}
                        <span className={`px-3 py-1 rounded-full text-sm font-medium border ${getStatusColor(order.status)}`}>
                          {order.status.charAt(0).toUpperCase() + order.status.slice(1)}
                        </span>
                      </div>
                      <div className="hidden md:block h-6 w-px bg-gray-300"></div>
                      <div className="text-sm text-gray-600">
                        Order #{order.id.slice(-8).toUpperCase()}
                      </div>
                    </div>
                    
                    <div className="flex items-center gap-6">
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <HiCalendarDays className="w-4 h-4" />
                        {formatDate(order.createdAt)}
                      </div>
                      <div className="flex items-center gap-2">
                        <HiCurrencyDollar className="w-5 h-5 text-green-600" />
                        <span className="text-lg font-semibold text-gray-900">
                          {formatPrice(order.totalAmount)}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Order Items */}
                <div className="p-6">
                  <div className="grid gap-4">
                    {order.items.map((item, index) => (
                      <div key={index} className="flex items-center gap-4 p-4 bg-gray-50 rounded-lg">
                        <div className="w-16 h-16 bg-gray-200 rounded-lg overflow-hidden flex-shrink-0">
                          {item.imageUrl ? (
                            (() => {
                              const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
                              const src = item.imageUrl.startsWith('http') ? item.imageUrl : `${API_URL}${item.imageUrl}`;
                              return (
                                <Image
                                  src={src}
                                  alt={item.productName}
                                  width={64}
                                  height={64}
                                  className="w-full h-full object-cover"
                                />
                              );
                            })()
                          ) : (
                            <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-200 to-gray-300">
                              <HiShoppingBag className="w-6 h-6 text-gray-400" />
                            </div>
                          )}
                        </div>
                        <div className="flex-1 min-w-0">
                          <h4 className="font-semibold text-gray-900 truncate">{item.productName}</h4>
                          <p className="text-sm text-gray-600">{item.variantName}</p>
                          <div className="flex items-center gap-4 mt-1">
                            <p className="text-sm text-gray-500">Qty: {item.quantity}</p>
                            <p className="text-sm font-medium text-gray-900">{formatPrice(item.unitPrice)} each</p>
                            <p className="text-sm font-semibold text-purple-600">{formatPrice(item.unitPrice * item.quantity)} total</p>
                          </div>
                        </div>
                        <Link
                          href={`/product/${item.productSlug}`}
                          className="flex items-center gap-2 px-3 py-2 text-purple-600 hover:text-purple-700 hover:bg-purple-50 rounded-lg transition-colors text-sm font-medium"
                        >
                          <HiEye className="w-4 h-4" />
                          View
                        </Link>
                      </div>
                    ))}
                  </div>

                  {/* Shipping Address */}
                  {order.shippingAddress && (
                    <div className="mt-6 pt-6 border-t border-gray-100">
                      <h4 className="text-sm font-semibold text-gray-900 mb-3">Shipping Address</h4>
                      <div className="bg-gray-50 rounded-lg p-4">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <p className="font-medium text-gray-900">
                              {order.shippingAddress.firstName} {order.shippingAddress.lastName}
                            </p>
                            <p className="text-sm text-gray-600 mt-1">{order.shippingAddress.phoneNumber}</p>
                          </div>
                          <div>
                            <p className="text-sm font-medium text-gray-900">{order.shippingAddress.addressTitle}</p>
                            <p className="text-sm text-gray-600 mt-1">
                              {order.shippingAddress.addressLine}
                            </p>
                            <p className="text-sm text-gray-600">
                              {order.shippingAddress.neighborhood}, {order.shippingAddress.district}
                            </p>
                            <p className="text-sm text-gray-600">
                              {order.shippingAddress.city}
                            </p>
                          </div>
                        </div>
                      </div>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
