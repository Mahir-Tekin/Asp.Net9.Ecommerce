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
  displayName: string;
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
  averageRating: number;
  reviewCount: number;
}

// Review-related types
export interface Review {
  id: string;
  productId: string;
  userId: string;
  rating: number;
  title: string;
  comment: string;
  helpfulVotes: number;
  unhelpfulVotes: number;
  totalHelpfulnessVotes: number;
  helpfulnessRatio: number;
  reviewerName: string;
  isVerifiedPurchase: boolean;
  createdAt: string;
}

export interface RatingSummary {
  fiveStars: number;
  fourStars: number;
  threeStars: number;
  twoStars: number;
  oneStar: number;
  totalReviews: number;
  fiveStarPercentage: number;
  fourStarPercentage: number;
  threeStarPercentage: number;
  twoStarPercentage: number;
  oneStarPercentage: number;
}

export interface ReviewsResponse {
  items: Review[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  productId: string;
  averageRating: number;
  ratingSummary: RatingSummary;
}
