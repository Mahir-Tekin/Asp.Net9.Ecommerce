// API helper for product reviews

import { Review, ReviewsResponse } from './ProductDetails.types';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

export async function fetchProductReviews({
  productId,
  page = 1,
  pageSize = 10,
  sortBy = '',
}: {
  productId: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
}): Promise<ReviewsResponse> {
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
  });

  if (sortBy) {
    params.append('sortBy', sortBy);
  }

  const res = await fetch(`${API_URL}/Products/${productId}/reviews?${params.toString()}`);
  if (!res.ok) throw new Error('Failed to fetch product reviews');
  return res.json();
}

export async function submitReview({
  productId,
  rating,
  title,
  comment,
}: {
  productId: string;
  rating: number;
  title: string;
  comment: string;
}): Promise<Review> {
  // Get access token from localStorage
  const accessToken = localStorage.getItem('accessToken');
  
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (accessToken) {
    headers['Authorization'] = `Bearer ${accessToken}`;
  }

  const res = await fetch(`${API_URL}/Products/${productId}/reviews`, {
    method: 'POST',
    headers,
    body: JSON.stringify({
      rating,
      title,
      comment,
    }),
  });

  if (!res.ok) {
    const errorData = await res.text();
    // Try to parse as JSON first, if that fails use the raw text
    try {
      const errorJson = JSON.parse(errorData);
      throw new Error(JSON.stringify(errorJson));
    } catch (parseError) {
      throw new Error(errorData || 'Failed to submit review');
    }
  }
  
  return res.json();
}

export async function voteOnReview({
  productId,
  reviewId,
  helpful
}: {
  productId: string;
  reviewId: string;
  helpful: boolean;
}): Promise<void> {
  // Get access token from localStorage
  const accessToken = localStorage.getItem('accessToken');
  
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (accessToken) {
    headers['Authorization'] = `Bearer ${accessToken}`;
  }

  const res = await fetch(`${API_URL}/Products/${productId}/reviews/${reviewId}/vote`, {
    method: 'POST',
    headers,
    body: JSON.stringify({
      helpful,
    }),
  });

  if (!res.ok) {
    const errorData = await res.text();
    // Try to parse as JSON first, if that fails use the raw text
    try {
      const errorJson = JSON.parse(errorData);
      throw new Error(JSON.stringify(errorJson));
    } catch (parseError) {
      throw new Error(errorData || 'Failed to vote on review');
    }
  }
}
