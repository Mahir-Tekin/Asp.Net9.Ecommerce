interface RatingDisplayProps {
  averageRating: number;
  reviewCount: number;
  className?: string;
}

export default function RatingDisplay({ 
  averageRating, 
  reviewCount, 
  className = '' 
}: RatingDisplayProps) {
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

  if (reviewCount === 0) {
    return (
      <div className={`flex items-center gap-2 ${className}`}>
        <div className="flex text-sm text-gray-300">
          {renderStars(0)}
        </div>
        <span className="text-sm text-gray-500">No reviews yet</span>
      </div>
    );
  }

  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <div className="flex text-sm">
        {renderStars(averageRating)}
      </div>
      <span className="text-sm text-gray-600">
        {averageRating.toFixed(1)} ({reviewCount} review{reviewCount !== 1 ? 's' : ''})
      </span>
    </div>
  );
}
