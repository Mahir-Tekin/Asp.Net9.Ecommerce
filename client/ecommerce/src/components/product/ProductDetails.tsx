'use client';
import { useState } from 'react';
import ProductImageGallery from './ProductImageGallery';
import ProductTitle from './ProductTitle';
import ProductPrice from './ProductPrice';
import ProductOptionPanel from './ProductOptionPanel';
import AddToCartButton from './AddToCartButton';
import ProductDescription from './ProductDescription';
import { ProductDetail, ProductVariant } from './ProductDetails.types';
import { useCart } from '@/context/CartContext';

interface ProductDetailsProps {
  product: ProductDetail;
}

export default function ProductDetails({ product }: ProductDetailsProps) {
  const [selectedVariant, setSelectedVariant] = useState<ProductVariant | undefined>(undefined);
  const { addToCart } = useCart();

  // Helper to determine if in stock
  const inStock = selectedVariant ? selectedVariant.stockQuantity > 0 : product.hasStock;

  // Handler for add to cart
  const handleAddToCart = () => {
    if (!selectedVariant) return;
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
  };

  return (
    <div className="max-w-4xl mx-auto p-8">
      <div className="flex gap-8">
        <div className="w-1/2">
          <ProductImageGallery images={product.images} />
        </div>
        <div className="w-1/2 flex flex-col">
          <ProductTitle
            name={product.name}
            variantName={selectedVariant ? selectedVariant.name : undefined}
          />
          <ProductPrice
            price={selectedVariant ? selectedVariant.price : undefined}
            oldPrice={selectedVariant ? selectedVariant.oldPrice : undefined}
          />
          <ProductOptionPanel
            variationTypes={product.variationTypes}
            variants={product.variants}
            onVariantChange={setSelectedVariant}
          />
          <AddToCartButton inStock={inStock} onClick={handleAddToCart} />
        </div>
      </div>
      <ProductDescription description={product.description} />
    </div>
  );
}
