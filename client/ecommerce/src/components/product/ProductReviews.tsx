'use client';

import React, { useState, useEffect } from 'react';
import { fetchProductReviews, voteOnReview } from './ProductReviews.api';
import { Review, ReviewsResponse } from './ProductDetails.types';
import Pagination from '../shop/pagination/Pagination';
import ReviewForm from './ReviewForm';
import { useAuth } from '@/context/AuthContext';

interface ProductReviewsProps {
  productId: string;
  averageRating: number;
  reviewCount: number;
}

export default function ProductReviews({ 
  productId, 
  averageRating, 
  reviewCount 
}: ProductReviewsProps) {
  const [reviewsData, setReviewsData] = useState<ReviewsResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [showReviewForm, setShowReviewForm] = useState(false);
  const [votingStates, setVotingStates] = useState<Record<string, boolean>>({});
  const [voteErrors, setVoteErrors] = useState<Record<string, string>>({});
  const pageSize = 5;
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    if (productId) {
      loadReviews();
    }
  }, [productId, currentPage]);

  const loadReviews = async () => {
    try {
      setLoading(true);
      const response = await fetchProductReviews({
        productId,
        page: currentPage,
        pageSize,
      });
      setReviewsData(response);
    } catch (err) {
      setError('Failed to load reviews');
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    // Scroll to reviews section when page changes
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleReviewSubmitSuccess = () => {
    // Reload reviews after successful submission
    loadReviews();
  };

  const handleWriteReviewClick = () => {
    if (!isAuthenticated) {
      // Redirect to login page
      window.location.href = '/login';
      return;
    }
    setShowReviewForm(true);
  };

  const handleVote = async (reviewId: string, helpful: boolean) => {
    if (!isAuthenticated) {
      // Redirect to login page
      window.location.href = '/login';
      return;
    }

    // Check if already voting on this review
    if (votingStates[reviewId]) {
      return;
    }

    try {
      setVotingStates(prev => ({ ...prev, [reviewId]: true }));
      setVoteErrors(prev => ({ ...prev, [reviewId]: '' })); // Clear any previous errors
      
      await voteOnReview({
        productId,
        reviewId,
        helpful
      });

      // Reload reviews to get updated vote counts
      loadReviews();
    } catch (err: any) {
      console.error('Failed to vote on review:', err);
      
      // Handle backend validation errors
      try {
        const errorResponse = JSON.parse(err.message);
        let errorMessage = 'Failed to vote on review. Please try again.';
        
        if (errorResponse.code === 'VALIDATION_ERROR' && errorResponse.errors?.length > 0) {
          const voteError = errorResponse.errors.find((e: any) => e.propertyName === 'Vote');
          if (voteError) {
            errorMessage = voteError.errorMessage; // "You cannot vote on your own review"
          } else {
            errorMessage = errorResponse.message || errorMessage;
          }
        } else if (errorResponse.message) {
          errorMessage = errorResponse.message;
        }
        
        setVoteErrors(prev => ({ ...prev, [reviewId]: errorMessage }));
        
        // Clear error after 5 seconds
        setTimeout(() => {
          setVoteErrors(prev => ({ ...prev, [reviewId]: '' }));
        }, 5000);
        
      } catch (parseError) {
        // If error message is not JSON, handle as plain text
        setVoteErrors(prev => ({ ...prev, [reviewId]: 'Failed to vote on review. Please try again.' }));
        setTimeout(() => {
          setVoteErrors(prev => ({ ...prev, [reviewId]: '' }));
        }, 5000);
      }
    } finally {
      setVotingStates(prev => ({ ...prev, [reviewId]: false }));
    }
  };

  // Helper function to render star rating
  const renderStars = (rating: number) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <span 
          key={i} 
          className={i <= rating ? 'text-yellow-400' : 'text-gray-300'}
        >
          ★
        </span>
      );
    }
    return stars;
  };

  if (loading) {
    return <div className="text-center py-8">Loading reviews...</div>;
  }

  if (error) {
    return <div className="text-center py-8 text-red-600">{error}</div>;
  }

  return (
    <div>
      {/* Review Summary with Rating Breakdown */}
      <div className="mb-8 p-6 bg-gray-50 rounded-lg">
        <div className="grid md:grid-cols-2 gap-6">
          {/* Overall Rating */}
          <div className="flex items-center gap-4">
            <div className="text-4xl font-bold text-gray-900">
              {reviewsData?.averageRating?.toFixed(1) || averageRating.toFixed(1)}
            </div>
            <div>
              <div className="flex text-xl mb-1">
                {renderStars(Math.round(reviewsData?.averageRating || averageRating))}
              </div>
              <div className="text-sm text-gray-600">
                Based on {reviewsData?.ratingSummary?.totalReviews || reviewCount} review{(reviewsData?.ratingSummary?.totalReviews || reviewCount) !== 1 ? 's' : ''}
              </div>
            </div>
          </div>

          {/* Rating Breakdown */}
          {reviewsData?.ratingSummary && (
            <div className="space-y-2">
              {[5, 4, 3, 2, 1].map((rating) => {
                const key = rating === 5 ? 'fiveStars' : 
                           rating === 4 ? 'fourStars' : 
                           rating === 3 ? 'threeStars' : 
                           rating === 2 ? 'twoStars' : 'oneStar';
                const percentage = rating === 5 ? reviewsData.ratingSummary.fiveStarPercentage :
                                 rating === 4 ? reviewsData.ratingSummary.fourStarPercentage :
                                 rating === 3 ? reviewsData.ratingSummary.threeStarPercentage :
                                 rating === 2 ? reviewsData.ratingSummary.twoStarPercentage :
                                 reviewsData.ratingSummary.oneStarPercentage;
                
                return (
                  <div key={rating} className="flex items-center gap-2">
                    <span className="text-sm w-4">{rating}</span>
                    <span className="text-yellow-400">★</span>
                    <div className="flex-1 bg-gray-200 rounded-full h-2">
                      <div 
                        className="bg-yellow-400 h-2 rounded-full" 
                        style={{ width: `${percentage}%` }}
                      ></div>
                    </div>
                    <span className="text-sm text-gray-600 w-12">
                      {Math.round(percentage)}%
                    </span>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* Reviews List */}
      {!reviewsData?.items || reviewsData.items.length === 0 ? (
        <div className="text-center py-8 text-gray-500">
          No reviews yet. Be the first to review this product!
        </div>
      ) : (
        <div className="space-y-6">
          {reviewsData.items.map((review: Review) => (
            <div key={review.id} className="border-b border-gray-200 pb-6">
              <div className="flex items-start justify-between mb-2">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <span className="font-medium text-gray-900">{review.reviewerName}</span>
                    {review.isVerifiedPurchase && (
                      <span className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded">
                        Verified Purchase
                      </span>
                    )}
                  </div>
                  <div className="flex text-sm">
                    {renderStars(review.rating)}
                  </div>
                </div>
                <div className="text-sm text-gray-500">
                  {new Date(review.createdAt).toLocaleDateString()}
                </div>
              </div>
              {review.title && (
                <h4 className="font-medium text-gray-900 mb-2">{review.title}</h4>
              )}
              <p className="text-gray-700 mb-3">{review.comment}</p>
              
              {/* Helpfulness votes */}
              <div className="space-y-2">
                <div className="flex items-center justify-between text-sm">
                  <div className="text-gray-600">
                    {review.totalHelpfulnessVotes > 0 ? (
                      <span>
                        {review.helpfulVotes} of {review.totalHelpfulnessVotes} found this helpful
                      </span>
                    ) : (
                      <span>Was this review helpful?</span>
                    )}
                  </div>
                  <div className="flex gap-2">
                    <button 
                      className={`px-3 py-1 rounded-md border transition-colors ${
                        votingStates[review.id] 
                          ? 'text-gray-400 border-gray-200 cursor-not-allowed' 
                          : isAuthenticated
                            ? 'text-green-600 border-green-200 hover:bg-green-50 hover:border-green-300'
                            : 'text-gray-400 border-gray-200 cursor-not-allowed'
                      }`}
                      onClick={() => handleVote(review.id, true)}
                      disabled={votingStates[review.id] || !isAuthenticated}
                      title={!isAuthenticated ? 'Please log in to vote' : 'Mark as helpful'}
                    >
                      {votingStates[review.id] ? 'Voting...' : '👍 Helpful'}
                    </button>
                    <button 
                      className={`px-3 py-1 rounded-md border transition-colors ${
                        votingStates[review.id] 
                          ? 'text-gray-400 border-gray-200 cursor-not-allowed' 
                          : isAuthenticated
                            ? 'text-red-600 border-red-200 hover:bg-red-50 hover:border-red-300'
                            : 'text-gray-400 border-gray-200 cursor-not-allowed'
                      }`}
                      onClick={() => handleVote(review.id, false)}
                      disabled={votingStates[review.id] || !isAuthenticated}
                      title={!isAuthenticated ? 'Please log in to vote' : 'Mark as not helpful'}
                    >
                      {votingStates[review.id] ? 'Voting...' : '👎 Not Helpful'}
                    </button>
                  </div>
                </div>
                
                {/* Error message for voting */}
                {voteErrors[review.id] && (
                  <div className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-md border border-red-200">
                    {voteErrors[review.id]}
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination */}
      {reviewsData && reviewsData.totalPages > 1 && (
        <div className="mt-8">
          <Pagination
            currentPage={reviewsData.currentPage}
            totalPages={reviewsData.totalPages}
            total={reviewsData.totalItems}
            pageSize={pageSize}
            onPageChange={handlePageChange}
          />
        </div>
      )}

      {/* Write Review Button */}
      <div className="mt-8 pt-6 border-t border-gray-200">
        <button 
          className="bg-indigo-600 text-white px-6 py-2 rounded-md hover:bg-indigo-700 transition-colors"
          onClick={handleWriteReviewClick}
        >
          {isAuthenticated ? 'Write a Review' : 'Log in to Write a Review'}
        </button>
      </div>

      {/* Review Form Modal */}
      <ReviewForm
        productId={productId}
        isOpen={showReviewForm}
        onClose={() => setShowReviewForm(false)}
        onSubmitSuccess={handleReviewSubmitSuccess}
      />
    </div>
  );
}
