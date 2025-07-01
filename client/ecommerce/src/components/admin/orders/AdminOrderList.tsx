'use client';
interface AdminOrderListProps {
  orders: any[];
  onSelect: (order: any) => void;
  selectedOrderId: string | null;
}

export default function AdminOrderList({ orders, onSelect, selectedOrderId }: AdminOrderListProps) {
  return (
    <div className="w-1/3 border-r pr-4 overflow-y-auto bg-neutral-50">
      <h2 className="text-lg font-bold mb-4 pl-1">Orders</h2>
      {orders.length === 0 ? (
        <div className="text-gray-400 text-center mt-8">No orders found.</div>
      ) : (
        <ul className="flex flex-col gap-2">
          {orders.map((order) => (
            <li
              key={order.id}
              className={`p-4 rounded-lg cursor-pointer border transition-all flex flex-col gap-1 relative group
                ${selectedOrderId === order.id
                  ? 'bg-green-50 border-green-500 shadow-md ring-2 ring-green-200'
                  : 'bg-white border-gray-200 hover:bg-gray-100'}
              `}
              onClick={() => onSelect(order)}
            >
              <div className="flex items-center justify-between">
                <span className="font-mono text-xs text-gray-500">#{order.id}</span>
                <span className={`text-xs font-semibold px-2 py-0.5 rounded-full 
                  ${order.status === 'Completed' || order.status === 'Delivered' ? 'bg-green-100 text-green-800' :
                    order.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                    order.status === 'Cancelled' ? 'bg-red-100 text-red-700' :
                    'bg-gray-100 text-gray-700'}
                `}>
                  {order.status}
                </span>
              </div>
              <div className="flex items-center justify-between mt-1">
                <span className="text-sm font-medium text-gray-900 truncate">
                  {order.customerName || order.customerEmail || 'Customer'}
                </span>
                <span className="text-xs text-gray-500">{new Date(order.createdAt).toLocaleString()}</span>
              </div>
              <div className="flex items-center justify-between mt-1">
                <span className="text-xs text-gray-500">Total:</span>
                <span className="text-sm font-semibold text-gray-800">${order.totalAmount?.toFixed(2) ?? '-'}</span>
              </div>
              {selectedOrderId === order.id && (
                <div className="absolute left-0 top-0 h-full w-1 bg-green-500 rounded-l"></div>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
