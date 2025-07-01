'use client';

import React, { useState } from 'react';
import { submitReview } from './ProductReviews.api';
import { useAuth } from '@/context/AuthContext';

interface ReviewFormProps {
  productId: string;
  isOpen: boolean;
  onClose: () => void;
  onSubmitSuccess: () => void;
}

export default function ReviewForm({ 
  productId, 
  isOpen, 
  onClose, 
  onSubmitSuccess 
}: ReviewFormProps) {
  const [rating, setRating] = useState(0);
  const [title, setTitle] = useState('');
  const [comment, setComment] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { isAuthenticated } = useAuth();

  // Helper function to render interactive star rating
  const renderInteractiveStars = () => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <button
          key={i}
          type="button"
          onClick={() => setRating(i)}
          onMouseEnter={() => setRating(i)}
          className={`text-xl sm:text-2xl ${i <= rating ? 'text-yellow-400' : 'text-gray-300'} hover:text-yellow-400 transition-colors`}
        >
          ★
        </button>
      );
    }
    return stars;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!isAuthenticated) {
      setError('Please log in to submit a review');
      return;
    }

    if (rating === 0) {
      setError('Please select a rating');
      return;
    }

    if (!comment.trim()) {
      setError('Please write a comment');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      await submitReview({
        productId,
        rating,
        title: title.trim(),
        comment: comment.trim()
      });

      // Reset form
      setRating(0);
      setTitle('');
      setComment('');
      
      onSubmitSuccess();
      onClose();
    } catch (err: any) {
      console.log('ReviewForm error:', err.message); // Debug log
      
      // Handle backend validation errors
      try {
        const errorResponse = JSON.parse(err.message);
        console.log('Parsed error response:', errorResponse); // Debug log
        
        if (errorResponse.code === 'REVIEW_NOT_ELIGIBLE') {
          setError(errorResponse.message);
        } else if (errorResponse.code === 'REVIEW_ALREADY_EXISTS') {
          setError('You have already reviewed this product.');
        } else if (errorResponse.message) {
          setError(errorResponse.message);
        } else {
          setError('Failed to submit review. Please try again.');
        }
      } catch (parseError) {
        console.log('Error parsing JSON, handling as plain text:', err.message); // Debug log
        
        // If error message is not JSON, handle as plain text
        const errorMsg = err.message.toLowerCase();
        if (errorMsg.includes('purchase')) {
          setError('You must purchase this product to write a review.');
        } else if (errorMsg.includes('already') || errorMsg.includes('duplicate')) {
          setError('You have already reviewed this product.');
        } else if (errorMsg.includes('login') || errorMsg.includes('unauthorized')) {
          setError('Please log in to submit a review.');
        } else {
          setError('Failed to submit review. Please try again.');
        }
      }
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg w-full max-w-lg max-h-[80vh] overflow-y-auto relative">
        <div className="p-4 sm:p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg sm:text-xl font-semibold">Write a Review</h2>
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600 text-xl sm:text-2xl"
            >
              ×
            </button>
          </div>

          <form onSubmit={handleSubmit}>
            {/* Rating */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Rating *
              </label>
              <div className="flex gap-1">
                {renderInteractiveStars()}
              </div>
              {rating > 0 && (
                <p className="text-sm text-gray-600 mt-1">
                  {rating} out of 5 stars
                </p>
              )}
            </div>

            {/* Title */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Review Title (Optional)
              </label>
              <input
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Brief summary of your review"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                maxLength={100}
              />
            </div>

            {/* Comment */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Your Review *
              </label>
              <textarea
                value={comment}
                onChange={(e) => setComment(e.target.value)}
                placeholder="Share your experience with this product..."
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 text-sm resize-none"
                maxLength={1000}
                required
              />
              <p className="text-xs text-gray-500 mt-1">
                {comment.length}/1000 characters
              </p>
            </div>

            {/* Error message */}
            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
                <p className="text-sm text-red-600">{error}</p>
              </div>
            )}

            {/* Buttons */}
            <div className="flex flex-col sm:flex-row gap-3">
              <button
                type="button"
                onClick={onClose}
                className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50 transition-colors text-sm"
                disabled={loading}
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={loading || rating === 0 || !comment.trim()}
                className="flex-1 px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors text-sm"
              >
                {loading ? 'Submitting...' : 'Submit Review'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
