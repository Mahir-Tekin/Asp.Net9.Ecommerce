import React from 'react';

const AdminSidebar = () => {
    return (
        <aside className="w-64 min-h-screen bg-white shadow-sm">
            <nav className="mt-5 px-2">
                <a href="/admin/dashboard" 
                   className="group flex items-center px-2 py-2 text-base font-medium rounded-md text-gray-900 hover:bg-gray-50">
                    Dashboard
                </a>
                <a href="/admin/categories" 
                   className="group flex items-center px-2 py-2 text-base font-medium rounded-md text-gray-600 hover:bg-gray-50">
                    Categories
                </a>
                <a href="/admin/products" 
                   className="group flex items-center px-2 py-2 text-base font-medium rounded-md text-gray-600 hover:bg-gray-50">
                    Products
                </a>
                <a href="/admin/variation-types" 
                   className="group flex items-center px-2 py-2 text-base font-medium rounded-md text-gray-600 hover:bg-gray-50">
                    Variation Types
                </a>
                <a href="/admin/orders" 
                   className="group flex items-center px-2 py-2 text-base font-medium rounded-md text-gray-600 hover:bg-gray-50">
                    Orders
                </a>
            </nav>
        </aside>
    );
};

export default AdminSidebar;