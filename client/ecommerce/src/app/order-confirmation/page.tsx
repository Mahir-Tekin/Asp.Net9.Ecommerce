'use client';
import Link from 'next/link';

export default function OrderConfirmationPage() {
  return (
    <div className="max-w-2xl mx-auto p-8 text-center">
      <h1 className="text-3xl font-bold mb-4 text-green-700">Thank you for your order!</h1>
      <p className="mb-6 text-lg">Your order has been placed successfully. We will send you a confirmation email soon.</p>
      <Link href="/" className="inline-block bg-green-600 text-white px-6 py-3 rounded hover:bg-green-700 font-semibold">Continue Shopping</Link>
    </div>
  );
}
