'use client';
import { useCart } from '@/context/CartContext';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';

export default function CartPage() {
  const { cartItems, cartTotal, updateQuantity, removeFromCart, clearCart } = useCart();
  const router = useRouter();

  if (cartItems.length === 0) {
    return (
      <div className="max-w-2xl mx-auto p-8 text-center">
        <h1 className="text-2xl font-bold mb-4">Your Cart is Empty</h1>
        <Link href="/shop" className="text-green-600 hover:underline">Continue Shopping</Link>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto p-8">
      <h1 className="text-2xl font-bold mb-6">Shopping Cart</h1>
      <div className="flex flex-col gap-4">
        {cartItems.map(item => (
          <div key={item.variantId} className="flex items-center gap-4 border-b pb-4">
            {item.image && (
              <Image src={item.image} alt={item.name} width={80} height={80} className="rounded" />
            )}
            <div className="flex-1">
              <Link href={`/shop/${item.slug}`} className="font-semibold hover:underline">{item.name}</Link>
              {item.variantName && <div className="text-sm text-gray-500">{item.variantName}</div>}
              <div className="text-sm text-gray-700">${item.price.toFixed(2)}</div>
            </div>
            <div className="flex items-center gap-2">
              <button onClick={() => updateQuantity(item.variantId, item.quantity - 1)} disabled={item.quantity <= 1} className="px-2 py-1 border rounded">-</button>
              <span>{item.quantity}</span>
              <button onClick={() => updateQuantity(item.variantId, item.quantity + 1)} className="px-2 py-1 border rounded">+</button>
            </div>
            <div className="w-20 text-right">${(item.price * item.quantity).toFixed(2)}</div>
            <button onClick={() => removeFromCart(item.variantId)} className="ml-2 text-red-500 hover:underline">Remove</button>
          </div>
        ))}
      </div>
      <div className="flex justify-between items-center mt-8">
        <button onClick={clearCart} className="text-sm text-gray-500 hover:underline">Clear Cart</button>
        <div className="text-xl font-bold">Total: ${cartTotal.toFixed(2)}</div>
      </div>
      <button
        className="mt-6 w-full bg-green-600 text-white py-3 rounded hover:bg-green-700 text-lg font-semibold"
        disabled={cartItems.length === 0}
        onClick={() => router.push('/checkout')}
      >
        Proceed to Checkout
      </button>
    </div>
  );
}
