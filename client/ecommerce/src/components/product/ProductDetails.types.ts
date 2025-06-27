export interface ProductImage {
  url: string;
  altText: string;
  isMain: boolean;
}

export interface ProductOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

export interface ProductVariationType {
  id: string;
  name: string;
  options: ProductOption[];
}

export interface ProductVariant {
  id: string;
  sku: string;
  name: string;
  price: number;
  oldPrice: number;
  stockQuantity: number;
  trackInventory: boolean;
  isActive: boolean;
  selectedOptions: Record<string, string>; // { [variationTypeId]: optionId }
}

export interface ProductDetail {
  id: string;
  name: string;
  slug: string;
  description: string;
  basePrice: number;
  mainImage: string;
  images: ProductImage[];
  categoryName: string;
  categoryId: string;
  isActive: boolean;
  createdAt: string;
  variationTypes: ProductVariationType[];
  variants: ProductVariant[];
  totalStock: number;
  hasStock: boolean;
  lowestPrice: number;
  lowestOldPrice: number;
}
