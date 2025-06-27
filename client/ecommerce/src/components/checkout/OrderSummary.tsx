import { useCart } from '@/context/CartContext';
import Image from 'next/image';
import Link from 'next/link';

export default function OrderSummary() {
  const { cartItems, cartTotal } = useCart();

  if (cartItems.length === 0) {
    return (
      <div className="max-w-2xl mx-auto p-8 text-center">
        <h2 className="text-xl font-bold mb-4">No items in your order</h2>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto p-8 border rounded bg-white">
      <h2 className="text-xl font-bold mb-6">Order Summary</h2>
      <div className="flex flex-col gap-4">
        {cartItems.map(item => (
          <div key={item.variantId} className="flex items-center gap-4 border-b pb-4">
            {item.image && (
              <Image src={item.image} alt={item.name} width={60} height={60} className="rounded" />
            )}
            <div className="flex-1">
              <Link href={`/shop/${item.slug}`} className="font-semibold hover:underline">{item.name}</Link>
              {item.variantName && <div className="text-sm text-gray-500">{item.variantName}</div>}
              <div className="text-sm text-gray-700">${item.price.toFixed(2)}</div>
            </div>
            <div className="w-16 text-center">x{item.quantity}</div>
            <div className="w-20 text-right font-medium">${(item.price * item.quantity).toFixed(2)}</div>
          </div>
        ))}
      </div>
      <div className="flex justify-end mt-8">
        <div className="text-lg font-bold">Total: ${cartTotal.toFixed(2)}</div>
      </div>
    </div>
  );
}
