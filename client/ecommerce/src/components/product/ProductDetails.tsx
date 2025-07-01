'use client';
import { useState, useEffect } from 'react';
import ProductImageGallery from './ProductImageGallery';
import ProductTitle from './ProductTitle';
import ProductPrice from './ProductPrice';
import ProductOptionPanel from './ProductOptionPanel';
import AddToCartButton from './AddToCartButton';
import ProductDescription from './ProductDescription';
import ProductTabs from './ProductTabs';
import RatingDisplay from './RatingDisplay';
import { ProductDetail, ProductVariant } from './ProductDetails.types';
import { useCart } from '@/context/CartContext';
import { useToast } from '@/context/ToastContext';
import { HiHeart, HiShare, HiShieldCheck, HiTruck, HiArrowPath } from 'react-icons/hi2';

interface ProductDetailsProps {
  product: ProductDetail;
}

export default function ProductDetails({ product }: ProductDetailsProps) {
  const [selectedVariant, setSelectedVariant] = useState<ProductVariant | undefined>(undefined);
  const [isWishlisted, setIsWishlisted] = useState(false);
  const { addToCart } = useCart();
  const { addToast } = useToast();

  // Log all received variants on mount
  useEffect(() => {
    console.log('All received variants:', product.variants);
  }, [product.variants]);

  // Check if product is wishlisted on mount
  useEffect(() => {
    const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
    setIsWishlisted(wishlist.includes(product.id));
  }, [product.id]);

  // Log selected variant for debugging
  useEffect(() => {
    console.log('Selected variant:', selectedVariant);
  }, [selectedVariant]);

  // Helper to determine if in stock
  const inStock = selectedVariant ? selectedVariant.stockQuantity > 0 : product.hasStock;
  
  // Get stock quantity for display
  const stockQuantity = selectedVariant ? selectedVariant.stockQuantity : product.totalStock;

  // Handler for add to cart
  const handleAddToCart = () => {
    if (!selectedVariant) return;
    
    try {
      addToCart({
        productId: product.id,
        slug: product.slug,
        variantId: selectedVariant.id,
        name: product.name,
        variantName: selectedVariant.name,
        price: selectedVariant.price,
        quantity: 1,
        image: product.mainImage,
      });
      
      // Show success toast
      const variantText = selectedVariant.name !== product.name ? ` (${selectedVariant.name})` : '';
      addToast(`${product.name}${variantText} added to cart!`, 'success');
    } catch (error) {
      console.error('Failed to add to cart:', error);
      addToast('Failed to add product to cart. Please try again.', 'error');
    }
  };

  const handleWishlist = () => {
    const wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
    
    if (isWishlisted) {
      // Remove from wishlist
      const updatedWishlist = wishlist.filter((id: string) => id !== product.id);
      localStorage.setItem('wishlist', JSON.stringify(updatedWishlist));
      setIsWishlisted(false);
      addToast('Removed from wishlist', 'info');
    } else {
      // Add to wishlist
      const updatedWishlist = [...wishlist, product.id];
      localStorage.setItem('wishlist', JSON.stringify(updatedWishlist));
      setIsWishlisted(true);
      addToast('Added to wishlist', 'success');
    }
  };

  const handleShare = () => {
    if (navigator.share) {
      navigator.share({
        title: product.name,
        url: window.location.href,
      });
    } else {
      navigator.clipboard.writeText(window.location.href);
      addToast('Product link copied to clipboard!', 'success');
    }
  };

  return (
    <div className="bg-gray-50 min-h-[calc(100vh-140px)]">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Main Product Section */}
        <div className="bg-white rounded-xl shadow-lg overflow-hidden mb-8">
          <div className="grid lg:grid-cols-2 gap-8 p-8">
            {/* Left Column - Images */}
            <div className="space-y-4">
              <ProductImageGallery images={product.images} />
              
              {/* Product Features */}
              <div className="grid grid-cols-3 gap-4 pt-6 border-t border-gray-200">
                <div className="text-center">
                  <HiTruck className="w-8 h-8 text-green-600 mx-auto mb-2" />
                  <p className="text-sm font-medium text-gray-800">Free Shipping</p>
                  <p className="text-xs text-gray-500">On orders over $50</p>
                </div>
                <div className="text-center">
                  <HiShieldCheck className="w-8 h-8 text-blue-600 mx-auto mb-2" />
                  <p className="text-sm font-medium text-gray-800">Warranty</p>
                  <p className="text-xs text-gray-500">1 year guarantee</p>
                </div>
                <div className="text-center">
                  <HiArrowPath className="w-8 h-8 text-purple-600 mx-auto mb-2" />
                  <p className="text-sm font-medium text-gray-800">Easy Returns</p>
                  <p className="text-xs text-gray-500">30-day policy</p>
                </div>
              </div>
            </div>

            {/* Right Column - Product Info */}
            <div className="space-y-6">
              {/* Header Section */}
              <div>
                <ProductTitle
                  name={product.name}
                  variantName={selectedVariant ? selectedVariant.name : undefined}
                />
                
                <div className="flex items-center justify-between mt-4">
                  <RatingDisplay 
                    averageRating={product.averageRating}
                    reviewCount={product.reviewCount}
                  />
                  
                  {/* Action Buttons */}
                  <div className="flex items-center gap-3">
                    <button
                      onClick={handleWishlist}
                      className={`p-2 rounded-full transition-colors ${
                        isWishlisted 
                          ? 'bg-red-100 text-red-600' 
                          : 'bg-gray-100 text-gray-600 hover:bg-red-100 hover:text-red-600'
                      }`}
                      title={isWishlisted ? 'Remove from wishlist' : 'Add to wishlist'}
                    >
                      <HiHeart className={`w-5 h-5 ${isWishlisted ? 'fill-current' : ''}`} />
                    </button>
                    <button
                      onClick={handleShare}
                      className="p-2 rounded-full bg-gray-100 text-gray-600 hover:bg-blue-100 hover:text-blue-600 transition-colors"
                      title="Share product"
                    >
                      <HiShare className="w-5 h-5" />
                    </button>
                  </div>
                </div>
              </div>

              {/* Price Section */}
              <div className="bg-gray-50 rounded-lg p-4">
                <ProductPrice
                  price={selectedVariant ? selectedVariant.price : undefined}
                  oldPrice={selectedVariant ? selectedVariant.oldPrice : undefined}
                />
                
                {/* Stock Status */}
                <div className="mt-3">
                  {inStock ? (
                    stockQuantity <= 10 ? (
                      <p className="text-orange-600 font-medium text-sm">
                        ⚠️ Only {stockQuantity} left in stock!
                      </p>
                    ) : (
                      <p className="text-green-600 font-medium text-sm">
                        ✅ In stock ({stockQuantity} available)
                      </p>
                    )
                  ) : (
                    <p className="text-red-600 font-medium text-sm">
                      ❌ Out of stock
                    </p>
                  )}
                </div>
              </div>

              {/* Product Options */}
              <div className="space-y-4">
                <ProductOptionPanel
                  variationTypes={product.variationTypes}
                  variants={product.variants}
                  onVariantChange={setSelectedVariant}
                />
              </div>

              {/* Add to Cart Section */}
              <div className="space-y-4 pt-4 border-t border-gray-200">
                <AddToCartButton inStock={inStock} onClick={handleAddToCart} />
                
                {/* Quick Product Info */}
                <div className="bg-blue-50 rounded-lg p-4 space-y-2">
                  <h4 className="font-semibold text-gray-800 text-sm">Product Highlights:</h4>
                  <ul className="text-sm text-gray-600 space-y-1">
                    <li>• Free shipping on orders over $50</li>
                    <li>• 30-day money-back guarantee</li>
                    <li>• Secure payment processing</li>
                    <li>• Customer support 24/7</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Product Tabs Section */}
        <div className="bg-white rounded-xl shadow-lg p-6">
          <ProductTabs product={product} />
        </div>
      </div>
    </div>
  );
}
