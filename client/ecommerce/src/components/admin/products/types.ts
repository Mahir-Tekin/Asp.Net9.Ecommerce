// Shared types for product variation components

export interface VariationOption {
  id: string;
  value: string;
  displayValue: string;
  sortOrder: number;
}

export interface VariationType {
  id: string;
  name: string;
  displayName: string;
  isActive: boolean;
  options: VariationOption[];
}

// Product image type for admin product creation
export interface ProductImage {
  url: string;
  altText: string;
  isMain: boolean;
}
