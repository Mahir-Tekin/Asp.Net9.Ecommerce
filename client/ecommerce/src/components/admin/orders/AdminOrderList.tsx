'use client';
interface AdminOrderListProps {
  orders: any[];
  onSelect: (order: any) => void;
  selectedOrderId: string | null;
}

export default function AdminOrderList({ orders, onSelect, selectedOrderId }: AdminOrderListProps) {
  return (
    <div className="w-1/3 border-r pr-4 overflow-y-auto">
      <h2 className="text-lg font-bold mb-4">Orders</h2>
      <ul className="flex flex-col gap-2">
        {orders.map((order) => (
          <li
            key={order.id}
            className={`p-3 rounded cursor-pointer border hover:bg-gray-50 ${selectedOrderId === order.id ? 'bg-green-50 border-green-400' : ''}`}
            onClick={() => onSelect(order)}
          >
            <div className="font-semibold">{order.id}</div>
            <div className="text-xs text-gray-500">{new Date(order.createdAt).toLocaleString()}</div>
            <div className="text-xs">Status: {order.status}</div>
          </li>
        ))}
      </ul>
    </div>
  );
}
