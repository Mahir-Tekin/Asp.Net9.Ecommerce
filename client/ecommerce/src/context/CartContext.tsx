'use client';

import React, { createContext, useContext, useEffect, useState } from 'react';

export interface CartItem {
  productId: string;
  slug: string;
  variantId: string;
  name: string;
  variantName?: string;
  price: number;
  quantity: number;
  image?: string;
}

interface CartContextType {
  cartItems: CartItem[];
  addToCart: (item: CartItem) => void;
  removeFromCart: (variantId: string) => void;
  updateQuantity: (variantId: string, quantity: number) => void;
  clearCart: () => void;
  cartCount: number;
  cartTotal: number;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export function CartProvider({ children }: { children: React.ReactNode }) {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  // Load from localStorage on mount
  useEffect(() => {
    const stored = localStorage.getItem('cart');
    if (stored) setCartItems(JSON.parse(stored));
  }, []);

  // Save to localStorage on change
  useEffect(() => {
    localStorage.setItem('cart', JSON.stringify(cartItems));
  }, [cartItems]);

  function addToCart(item: CartItem) {
    setCartItems(prev => {
      const existing = prev.find(i => i.variantId === item.variantId);
      if (existing) {
        return prev.map(i =>
          i.variantId === item.variantId ? { ...i, quantity: i.quantity + item.quantity } : i
        );
      }
      return [...prev, item];
    });
  }

  function removeFromCart(variantId: string) {
    setCartItems(prev => prev.filter(i => i.variantId !== variantId));
  }

  function updateQuantity(variantId: string, quantity: number) {
    setCartItems(prev =>
      prev.map(i => (i.variantId === variantId ? { ...i, quantity } : i))
    );
  }

  function clearCart() {
    setCartItems([]);
  }

  const cartCount = cartItems.reduce((sum, i) => sum + i.quantity, 0);
  const cartTotal = cartItems.reduce((sum, i) => sum + i.price * i.quantity, 0);

  return (
    <CartContext.Provider value={{ cartItems, addToCart, removeFromCart, updateQuantity, clearCart, cartCount, cartTotal }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within a CartProvider');
  return ctx;
}
