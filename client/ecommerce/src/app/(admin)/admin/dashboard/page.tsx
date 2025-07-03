'use client';

import { useAuth } from '@/context/AuthContext';
import Link from 'next/link';

export default function AdminDashboard() {
    const { user } = useAuth();

    return (
        <div className="space-y-6">
            {/* Welcome Section */}
            <div className="bg-white rounded-lg shadow p-6">
                <h1 className="text-2xl font-bold text-gray-900 mb-2">
                    Welcome to Admin Dashboard
                </h1>
                <p className="text-gray-600">
                    Hello {user?.fullName || 'Admin'}, manage your e-commerce store from here.
                </p>
            </div>

            {/* Quick Actions Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <Link href="/admin/products" className="block">
                    <div className="bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                        <div className="flex items-center">
                            <div className="p-3 bg-blue-100 rounded-lg">
                                <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                                </svg>
                            </div>
                            <div className="ml-4">
                                <h3 className="text-lg font-medium text-gray-900">Products</h3>
                                <p className="text-gray-600">Manage your product catalog</p>
                            </div>
                        </div>
                    </div>
                </Link>

                <Link href="/admin/categories" className="block">
                    <div className="bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                        <div className="flex items-center">
                            <div className="p-3 bg-green-100 rounded-lg">
                                <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                                </svg>
                            </div>
                            <div className="ml-4">
                                <h3 className="text-lg font-medium text-gray-900">Categories</h3>
                                <p className="text-gray-600">Organize product categories</p>
                            </div>
                        </div>
                    </div>
                </Link>

                <Link href="/admin/orders" className="block">
                    <div className="bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                        <div className="flex items-center">
                            <div className="p-3 bg-purple-100 rounded-lg">
                                <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                                </svg>
                            </div>
                            <div className="ml-4">
                                <h3 className="text-lg font-medium text-gray-900">Orders</h3>
                                <p className="text-gray-600">Track and manage orders</p>
                            </div>
                        </div>
                    </div>
                </Link>

                <Link href="/admin/variation-types" className="block">
                    <div className="bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                        <div className="flex items-center">
                            <div className="p-3 bg-orange-100 rounded-lg">
                                <svg className="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 10h16M4 14h16M4 18h16" />
                                </svg>
                            </div>
                            <div className="ml-4">
                                <h3 className="text-lg font-medium text-gray-900">Variation Types</h3>
                                <p className="text-gray-600">Manage product variations</p>
                            </div>
                        </div>
                    </div>
                </Link>
            </div>

            {/* Recent Activity */}
            <div className="bg-white rounded-lg shadow p-6">
                <h2 className="text-lg font-medium text-gray-900 mb-4">Recent Activity</h2>
                <div className="text-gray-600">
                    <p>No recent activity to display.</p>
                    <p className="text-sm mt-2">
                        Start managing your store by adding products, categories, or checking orders.
                    </p>
                </div>
            </div>
        </div>
    );
}
