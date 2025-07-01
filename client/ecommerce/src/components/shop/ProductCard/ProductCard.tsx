'use client';

import { useState, useEffect } from 'react';
import Image from 'next/image';
import Link from 'next/link';
import { fetchProductBySlug } from '@/components/product/ProductDetails.api';
import { useCart } from '@/context/CartContext';
import { useToast } from '@/context/ToastContext';
import { HiHeart } from 'react-icons/hi2';

interface ProductCardProps {
  id: string;
  name: string;
  mainImage: string | null;
  variantCount: number;
  lowestPrice: number;
  lowestOldPrice?: number | null;
  slug: string;
  hasStock: boolean;
  totalStock: number;
  averageRating: number;
  reviewCount: number;
}

export default function ProductCard({
  id,
  name,
  mainImage,
  variantCount,
  lowestPrice,
  lowestOldPrice,
  slug,
  hasStock,
  totalStock,
  averageRating,
  reviewCount,
}: ProductCardProps) {
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isWishlisted, setIsWishlisted] = useState(false);
  const { addToCart } = useCart();
  const { addToast } = useToast();

  // Check if product is wishlisted on mount
  useEffect(() => {
    const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
    setIsWishlisted(wishlist.includes(id));
  }, [id]);

  const hasDiscount =
    typeof lowestOldPrice === 'number' &&
    lowestOldPrice > 0 &&
    lowestOldPrice > lowestPrice;

  // Handler for adding single variant to cart
  const handleAddToCart = async () => {
    if (variantCount !== 1 || !hasStock || isAddingToCart) return;
    
    setIsAddingToCart(true);
    try {
      const productDetails = await fetchProductBySlug(slug);
      if (productDetails && productDetails.variants.length === 1) {
        const variant = productDetails.variants[0];
        addToCart({
          productId: id,
          slug: slug,
          variantId: variant.id,
          name: name,
          variantName: variant.name,
          price: variant.price,
          quantity: 1,
          image: mainImage || undefined,
        });
        
        // Show success toast
        addToast(`${name} added to cart!`, 'success');
      }
    } catch (error) {
      console.error('Failed to add to cart:', error);
      // Show error toast
      addToast('Failed to add product to cart. Please try again.', 'error');
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleWishlist = (e: React.MouseEvent) => {
    e.preventDefault(); // Prevent navigation when clicking wishlist
    e.stopPropagation();
    
    const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
    
    if (isWishlisted) {
      const updatedWishlist = wishlist.filter((productId: string) => productId !== id);
      localStorage.setItem('wishlist', JSON.stringify(updatedWishlist));
      setIsWishlisted(false);
      addToast('Removed from wishlist', 'info');
    } else {
      const updatedWishlist = [...wishlist, id];
      localStorage.setItem('wishlist', JSON.stringify(updatedWishlist));
      setIsWishlisted(true);
      addToast('Added to wishlist', 'success');
    }
  };

  // Helper function to render star rating
  const renderStars = (rating: number) => {
    const stars = [];
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    
    for (let i = 0; i < 5; i++) {
      if (i < fullStars) {
        stars.push(
          <span key={i} className="text-yellow-400">★</span>
        );
      } else if (i === fullStars && hasHalfStar) {
        stars.push(
          <span key={i} className="text-yellow-400">★</span>
        );
      } else {
        stars.push(
          <span key={i} className="text-gray-300">☆</span>
        );
      }
    }
    return stars;
  };

  return (
    <div className="bg-white rounded-xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden border border-gray-100 group">
      <Link href={`/product/${slug}`} className="block">
        <div className="relative overflow-hidden">
          {mainImage ? (
            (() => {
              const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
              const src = mainImage.startsWith('http') ? mainImage : `${API_URL}${mainImage}`;
              return (
                <Image
                  src={src}
                  alt={name}
                  width={300}
                  height={240}
                  className="w-full h-48 object-cover group-hover:scale-105 transition-transform duration-300"
                />
              );
            })()
          ) : (
            <div className="w-full h-48 bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
              <div className="text-center">
                <div className="text-4xl text-gray-400 mb-2">📦</div>
                <span className="text-gray-500 text-sm">No Image</span>
              </div>
            </div>
          )}
          {!hasStock && (
            <div className="absolute inset-0 bg-gray-900 bg-opacity-50 flex items-center justify-center">
              <span className="bg-red-500 text-white px-3 py-1 rounded-full text-sm font-semibold">
                Out of Stock
              </span>
            </div>
          )}
          {/* Wishlist Button */}
          <button
            onClick={handleWishlist}
            className={`absolute top-3 right-3 p-2 rounded-full transition-all duration-300 ${
              isWishlisted 
                ? 'bg-red-500 text-white shadow-lg' 
                : 'bg-white/80 text-gray-600 hover:bg-red-500 hover:text-white hover:shadow-lg'
            }`}
            title={isWishlisted ? 'Remove from wishlist' : 'Add to wishlist'}
          >
            <HiHeart className={`w-4 h-4 ${isWishlisted ? 'fill-current' : ''}`} />
          </button>
        </div>
      </Link>
      
      <div className="p-4">
        <Link href={`/product/${slug}`}>
          <h3 className="font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-blue-600 transition-colors duration-200">
            {name}
          </h3>
        </Link>
        
        {/* Rating and review count */}
        <div className="flex items-center gap-2 mb-3">
          {reviewCount > 0 ? (
            <>
              <div className="flex text-sm">
                {renderStars(averageRating)}
              </div>
              <span className="text-xs text-gray-500">({reviewCount})</span>
            </>
          ) : (
            <span className="text-xs text-gray-400">No reviews yet</span>
          )}
        </div>
        
        {/* Price section */}
        <div className="mb-3">
          <div className="flex items-center gap-2 mb-1">
            <span className="text-xl font-bold text-gray-900">
              ${lowestPrice.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
            </span>
            {hasDiscount && (
              <span className="text-sm line-through text-gray-500">
                ${lowestOldPrice?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
              </span>
            )}
          </div>
          
          {/* Stock and variant info */}
          <div className="flex items-center justify-between text-xs">
            <span>
              {hasStock ? (
                totalStock <= 9 ? (
                  <span className="text-red-600 font-medium">⚠ Last {totalStock} items</span>
                ) : (
                  <span className="text-green-600 font-medium">✓ In Stock</span>
                )
              ) : (
                <span className="text-red-600 font-medium">✗ Out of Stock</span>
              )}
            </span>
          </div>
        </div>
        
        {/* Action buttons */}
        <div className="space-y-2">
          {variantCount > 1 ? (
            // Multiple variants - show variant count and view details button
            <>
              <div className="text-center">
                <span className="inline-block bg-blue-100 text-blue-700 px-3 py-1 rounded-full text-xs font-semibold">
                  {variantCount} variants available
                </span>
              </div>
              <Link
                href={`/product/${slug}`}
                className="block w-full bg-blue-600 text-white text-center py-2.5 px-3 rounded-lg hover:bg-blue-700 transition-colors text-sm font-medium"
              >
                View Details
              </Link>
            </>
          ) : (
            // Single variant - show add to cart and view details
            <div className="flex gap-2">
              <Link
                href={`/product/${slug}`}
                className="flex-1 bg-gray-100 text-gray-700 text-center py-2 px-3 rounded-lg hover:bg-gray-200 transition-colors text-sm font-medium"
              >
                View Details
              </Link>
              <button
                onClick={handleAddToCart}
                disabled={!hasStock || isAddingToCart}
                className={`flex-1 py-2 px-3 rounded-lg text-sm font-medium transition-colors ${
                  hasStock && !isAddingToCart
                    ? 'bg-green-600 text-white hover:bg-green-700' 
                    : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                }`}
                type="button"
                title={
                  !hasStock 
                    ? "Out of stock" 
                    : isAddingToCart 
                    ? "Adding to cart..." 
                    : "Add to Cart"
                }
              >
                {isAddingToCart ? 'Adding...' : 'Add to Cart'}
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}