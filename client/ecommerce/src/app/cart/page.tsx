'use client';
import { useCart } from '@/context/CartContext';
import { useToast } from '@/context/ToastContext';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { HiMinus, HiPlus, HiTrash, HiShoppingBag } from 'react-icons/hi2';

export default function CartPage() {
  const { cartItems, cartTotal, updateQuantity, removeFromCart, clearCart } = useCart();
  const { addToast } = useToast();
  const router = useRouter();

  const handleRemoveItem = (variantId: string, itemName: string) => {
    removeFromCart(variantId);
    addToast(`${itemName} removed from cart`, 'info');
  };

  const handleClearCart = () => {
    clearCart();
    addToast('Cart cleared', 'info');
  };

  const handleUpdateQuantity = (variantId: string, newQuantity: number, itemName: string) => {
    updateQuantity(variantId, newQuantity);
    if (newQuantity === 0) {
      addToast(`${itemName} removed from cart`, 'info');
    }
  };

  if (cartItems.length === 0) {
    return (
      <div className="bg-gray-50 min-h-[calc(100vh-140px)]">
        <div className="max-w-7xl mx-auto px-4 py-12">
          <div className="max-w-md mx-auto text-center bg-white rounded-xl shadow-lg p-8">
            <div className="mb-6">
              <HiShoppingBag className="w-16 h-16 text-gray-400 mx-auto mb-4" />
              <h1 className="text-2xl font-bold text-gray-800 mb-2">Your Cart is Empty</h1>
              <p className="text-gray-600">Looks like you haven't added anything to your cart yet.</p>
            </div>
            <Link 
              href="/" 
              className="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-300 shadow-lg hover:shadow-xl"
            >
              <HiShoppingBag className="w-5 h-5 mr-2" />
              Continue Shopping
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-gray-50 min-h-[calc(100vh-140px)]">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Shopping Cart</h1>
          <p className="text-gray-600">{cartItems.length} item{cartItems.length !== 1 ? 's' : ''} in your cart</p>
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Cart Items */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-xl shadow-lg overflow-hidden">
              <div className="divide-y divide-gray-200">
                {cartItems.map(item => (
                  <div key={item.variantId} className="p-6 hover:bg-gray-50 transition-colors">
                    <div className="flex gap-4">
                      {/* Product Image */}
                      <div className="flex-shrink-0">
                        {item.image ? (
                          (() => {
                            const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
                            const src = item.image.startsWith('http') ? item.image : `${API_URL}${item.image}`;
                            return (
                              <Image 
                                src={src} 
                                alt={item.name} 
                                width={100} 
                                height={100} 
                                className="w-20 h-20 object-cover rounded-lg border border-gray-200" 
                              />
                            );
                          })()
                        ) : (
                          <div className="w-20 h-20 bg-gray-100 rounded-lg flex items-center justify-center">
                            <span className="text-gray-400 text-2xl">4e6</span>
                          </div>
                        )}
                      </div>

                      {/* Product Info */}
                      <div className="flex-1 min-w-0">
                        <Link 
                          href={`/product/${item.slug}`} 
                          className="font-semibold text-gray-800 hover:text-blue-600 transition-colors block mb-1 truncate"
                        >
                          {item.name}
                        </Link>
                        {item.variantName && item.variantName !== item.name && (
                          <p className="text-sm text-gray-500 mb-2">{item.variantName}</p>
                        )}
                        <p className="text-lg font-bold text-gray-900">${item.price.toFixed(2)}</p>
                      </div>

                      {/* Quantity Controls */}
                      <div className="flex flex-col items-end gap-3">
                        <div className="flex items-center border border-gray-300 rounded-lg">
                          <button 
                            onClick={() => handleUpdateQuantity(item.variantId, item.quantity - 1, item.name)}
                            disabled={item.quantity <= 1}
                            className="p-2 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                          >
                            <HiMinus className="w-4 h-4" />
                          </button>
                          <span className="px-4 py-2 min-w-[3rem] text-center font-medium">{item.quantity}</span>
                          <button 
                            onClick={() => handleUpdateQuantity(item.variantId, item.quantity + 1, item.name)}
                            className="p-2 hover:bg-gray-100 transition-colors"
                          >
                            <HiPlus className="w-4 h-4" />
                          </button>
                        </div>
                        
                        {/* Item Total & Remove */}
                        <div className="text-right">
                          <p className="font-bold text-lg text-gray-900 mb-1">
                            ${(item.price * item.quantity).toFixed(2)}
                          </p>
                          <button 
                            onClick={() => handleRemoveItem(item.variantId, item.name)}
                            className="text-red-500 hover:text-red-700 text-sm font-medium transition-colors flex items-center gap-1"
                          >
                            <HiTrash className="w-4 h-4" />
                            Remove
                          </button>
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Cart Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-xl shadow-lg p-6 sticky top-8">
              <h2 className="text-xl font-bold text-gray-800 mb-6">Order Summary</h2>
              
              <div className="space-y-4 mb-6">
                <div className="flex justify-between text-gray-600">
                  <span>Subtotal ({cartItems.reduce((sum, item) => sum + item.quantity, 0)} items)</span>
                  <span>${cartTotal.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Shipping</span>
                  <span className="text-green-600 font-medium">Free</span>
                </div>
                <div className="border-t pt-4">
                  <div className="flex justify-between text-xl font-bold text-gray-800">
                    <span>Total</span>
                    <span>${cartTotal.toFixed(2)}</span>
                  </div>
                </div>
              </div>

              <button
                className="w-full bg-gradient-to-r from-purple-600 to-blue-600 text-white py-3 px-6 rounded-lg font-semibold hover:from-purple-700 hover:to-blue-700 transition-all duration-300 shadow-lg hover:shadow-xl mb-4"
                disabled={cartItems.length === 0}
                onClick={() => router.push('/checkout')}
              >
                Proceed to Checkout
              </button>

              <div className="text-center space-y-3">
                <Link 
                  href="/" 
                  className="block text-purple-600 hover:text-purple-700 font-medium transition-colors"
                >
                  Continue Shopping
                </Link>
                <button 
                  onClick={handleClearCart}
                  className="block w-full text-gray-500 hover:text-red-500 text-sm transition-colors"
                >
                  Clear Cart
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
