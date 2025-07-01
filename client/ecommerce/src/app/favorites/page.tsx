'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { 
  HiHeart, 
  HiArrowLeft, 
  HiShoppingBag,
  HiOutlineHeart,
  HiExclamationTriangle,
  HiTrash
} from 'react-icons/hi2';
import { useToast } from '@/context/ToastContext';

interface FavoriteProduct {
  id: string;
  slug: string;
  name: string;
  basePrice: number;
  lowestPrice: number;
  lowestOldPrice?: number;
  mainImage?: string;
  totalStock: number;
  hasStock: boolean;
  isActive: boolean;
}

export default function MyFavoritesPage() {
  const [favorites, setFavorites] = useState<FavoriteProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { addToast } = useToast();

  useEffect(() => {
    async function fetchFavoriteProducts() {
      setLoading(true);
      setError(null);
      
      try {
        // Get favorite IDs from localStorage
        const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
        
        if (wishlist.length === 0) {
          setFavorites([]);
          setLoading(false);
          return;
        }

        // Fetch product details for each favorite
        const favoriteProducts: FavoriteProduct[] = [];
        
        for (const id of wishlist) {
          try {
            const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Products/${id}`);
            if (res.ok) {
              const product = await res.json();
              
              // Debug: Log the product data to see what images we're getting
              console.log('Product data:', {
                name: product.name,
                mainImage: product.mainImage,
                images: product.images
              });
              
              // Get the main image - prefer mainImage, fallback to first image in array
              const getMainImageUrl = (product: any) => {
                if (product.mainImage) return product.mainImage;
                if (product.images && product.images.length > 0) {
                  const mainImg = product.images.find((img: any) => img.isMain);
                  return mainImg?.url || product.images[0]?.url;
                }
                return null;
              };

              favoriteProducts.push({
                id: product.id,
                slug: product.slug,
                name: product.name,
                basePrice: product.basePrice,
                lowestPrice: product.lowestPrice,
                lowestOldPrice: product.lowestOldPrice,
                mainImage: getMainImageUrl(product),
                totalStock: product.totalStock,
                hasStock: product.hasStock,
                isActive: product.isActive
              });
            }
          } catch (err) {
            console.error(`Failed to fetch product ${id}:`, err);
          }
        }
        
        setFavorites(favoriteProducts);
      } catch (err: any) {
        setError(err.message || 'Failed to load favorites');
      } finally {
        setLoading(false);
      }
    }

    fetchFavoriteProducts();
  }, []);

  const removeFavorite = (id: string) => {
    const currentWishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
    const updatedWishlist = currentWishlist.filter((item: string) => item !== id);
    localStorage.setItem('wishlist', JSON.stringify(updatedWishlist));
    
    // Update local state
    setFavorites(prev => prev.filter(item => item.id !== id));
    
    // Dispatch storage event to update other components
    window.dispatchEvent(new Event('storage'));
    
    addToast('Removed from favorites', 'success');
  };

  const clearAllFavorites = () => {
    localStorage.setItem('wishlist', JSON.stringify([]));
    setFavorites([]);
    window.dispatchEvent(new Event('storage'));
    addToast('All favorites cleared', 'success');
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 py-8">
          {/* Header Skeleton */}
          <div className="mb-8">
            <div className="h-8 w-32 bg-gray-200 rounded animate-pulse mb-2"></div>
            <div className="h-6 w-64 bg-gray-200 rounded animate-pulse"></div>
          </div>

          {/* Products Skeleton */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="bg-white rounded-xl shadow-sm border border-gray-200 p-4">
                <div className="aspect-square bg-gray-200 rounded-lg animate-pulse mb-4"></div>
                <div className="h-4 bg-gray-200 rounded animate-pulse mb-2"></div>
                <div className="h-6 bg-gray-200 rounded animate-pulse mb-2"></div>
                <div className="h-10 bg-gray-200 rounded animate-pulse"></div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 py-8">
          <div className="text-center">
            <HiExclamationTriangle className="w-16 h-16 text-red-500 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-900 mb-2">Unable to Load Favorites</h2>
            <p className="text-gray-600 mb-6">{error}</p>
            <button
              onClick={() => window.location.reload()}
              className="px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center gap-4 mb-4">
            <Link
              href="/"
              className="flex items-center gap-2 text-gray-600 hover:text-purple-600 transition-colors"
            >
              <HiArrowLeft className="w-5 h-5" />
              <span>Back to Shop</span>
            </Link>
          </div>
          
          <div className="flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <HiHeart className="w-8 h-8 text-red-500" />
                <h1 className="text-3xl font-bold text-gray-900">My Favorites</h1>
              </div>
              <p className="text-gray-600">
                {favorites.length > 0 
                  ? `${favorites.length} favorite product${favorites.length !== 1 ? 's' : ''}`
                  : 'Your favorite products will appear here'
                }
              </p>
            </div>
            
            {favorites.length > 0 && (
              <button
                onClick={clearAllFavorites}
                className="flex items-center gap-2 px-4 py-2 text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors text-sm font-medium"
              >
                <HiTrash className="w-4 h-4" />
                Clear All
              </button>
            )}
          </div>
        </div>

        {/* Favorites Grid */}
        {favorites.length === 0 ? (
          <div className="text-center py-16">
            <HiOutlineHeart className="w-20 h-20 text-gray-300 mx-auto mb-6" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">No Favorites Yet</h3>
            <p className="text-gray-600 mb-6">
              Start browsing and add products to your favorites by clicking the heart icon
            </p>
            <Link
              href="/"
              className="inline-flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200"
            >
              <HiShoppingBag className="w-5 h-5" />
              Start Shopping
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {favorites.map((product) => (
              <div key={product.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow group">
                {/* Product Image */}
                <div className="relative aspect-square bg-gray-100">
                  <Link href={`/product/${product.slug}`}>
                    {product.mainImage ? (
                      <Image
                        src={product.mainImage}
                        alt={product.name}
                        fill
                        className="object-cover group-hover:scale-105 transition-transform duration-300"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-200 to-gray-300">
                        <HiShoppingBag className="w-16 h-16 text-gray-400" />
                      </div>
                    )}
                  </Link>
                  
                  {/* Remove from favorites button */}
                  <button
                    onClick={() => removeFavorite(product.id)}
                    className="absolute top-3 right-3 p-2 bg-white/90 backdrop-blur-sm rounded-full shadow-md hover:bg-white transition-colors"
                  >
                    <HiHeart className="w-5 h-5 text-red-500" />
                  </button>

                  {/* Discount Badge */}
                  {product.lowestOldPrice && product.lowestOldPrice > product.lowestPrice && (
                    <div className="absolute top-3 left-3 bg-red-500 text-white px-2 py-1 rounded-full text-xs font-semibold">
                      {Math.round(((product.lowestOldPrice - product.lowestPrice) / product.lowestOldPrice) * 100)}% OFF
                    </div>
                  )}

                  {/* Stock Status */}
                  {!product.isActive || !product.hasStock && (
                    <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                      <span className="bg-white text-gray-900 px-3 py-1 rounded-full text-sm font-medium">
                        {!product.isActive ? 'Unavailable' : 'Out of Stock'}
                      </span>
                    </div>
                  )}
                </div>

                {/* Product Info */}
                <div className="p-4">
                  <Link href={`/product/${product.slug}`}>
                    <h3 className="font-semibold text-gray-900 hover:text-purple-600 transition-colors mb-2 line-clamp-2">
                      {product.name}
                    </h3>
                  </Link>
                  
                  {/* Price */}
                  <div className="flex items-center gap-2 mb-4">
                    {product.lowestOldPrice && product.lowestOldPrice > product.lowestPrice ? (
                      <>
                        <span className="text-lg font-bold text-gray-900">
                          {formatPrice(product.lowestPrice)}
                        </span>
                        <span className="text-sm text-gray-500 line-through">
                          {formatPrice(product.lowestOldPrice)}
                        </span>
                      </>
                    ) : (
                      <span className="text-lg font-bold text-gray-900">
                        {formatPrice(product.lowestPrice)}
                      </span>
                    )}
                  </div>

                  {/* Stock Status */}
                  {product.isActive && product.hasStock && (
                    <div className="text-sm text-gray-600 mb-3">
                      {product.totalStock < 10 && product.totalStock > 0 && (
                        <span className="text-orange-600 font-medium">
                          Only {product.totalStock} left!
                        </span>
                      )}
                    </div>
                  )}

                  {/* Action Button */}
                  <Link
                    href={`/product/${product.slug}`}
                    className="w-full bg-gradient-to-r from-purple-600 to-blue-600 text-white py-2 px-4 rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200 text-center font-medium block"
                  >
                    View Product
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
