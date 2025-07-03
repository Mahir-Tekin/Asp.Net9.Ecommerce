'use client';
import Link from 'next/link';
import { useState, Suspense } from 'react';
import AddressPanel from '@/components/checkout/AddressPanel';
import OrderSummary from '@/components/checkout/OrderSummary';
import type { Address } from '@/types/address';
import { useCart } from '@/context/CartContext';
import { useRouter } from 'next/navigation';

function CheckoutPageContent() {
  const [selectedAddress, setSelectedAddress] = useState<Address | null>(null);
  const { cartItems, clearCart } = useCart();
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handlePlaceOrder = async () => {
    if (!selectedAddress) {
      setError('Please select a shipping address.');
      return;
    }
    if (cartItems.length === 0) {
      setError('Your cart is empty.');
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem('accessToken');
      const payload = {
        shippingAddress: {
          firstName: selectedAddress.firstName,
          lastName: selectedAddress.lastName,
          phoneNumber: selectedAddress.phoneNumber,
          city: selectedAddress.city,
          district: selectedAddress.district,
          neighborhood: selectedAddress.neighborhood,
          addressLine: selectedAddress.addressLine,
          addressTitle: selectedAddress.addressTitle,
        },
        items: cartItems.map(item => ({
          productId: item.productId,
          productVariantId: item.variantId,
          productName: item.name,
          productSlug: item.slug,
          variantName: item.variantName,
          quantity: item.quantity,
          unitPrice: item.price,
          imageUrl: item.image,
        })),
      };
      const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Order`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(payload),
      });
      if (!res.ok) throw new Error('Order placement failed');
      clearCart();
      router.push('/order-confirmation');
    } catch (err: any) {
      setError(err.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto p-8">
      <h1 className="text-2xl font-bold mb-6">Checkout</h1>
      <AddressPanel
        onSelect={setSelectedAddress}
        selectedAddressId={selectedAddress?.id}
      />
      <div className="my-8" />
      <OrderSummary />
      {error && <div className="text-red-500 my-4">{error}</div>}
      <button
        className="mt-4 w-full bg-green-600 text-white py-3 rounded hover:bg-green-700 text-lg font-semibold disabled:opacity-50"
        onClick={handlePlaceOrder}
        disabled={loading || !selectedAddress || cartItems.length === 0}
      >
        {loading ? 'Placing Order...' : 'Place Order'}
      </button>
      <Link href="/cart" className="text-green-600 hover:underline block mt-4">Back to Cart</Link>
    </div>
  );
}

export default function CheckoutPage() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <CheckoutPageContent />
    </Suspense>
  );
}
