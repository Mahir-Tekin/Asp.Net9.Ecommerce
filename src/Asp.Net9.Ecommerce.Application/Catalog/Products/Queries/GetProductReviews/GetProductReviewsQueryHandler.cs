using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductReviews
{
    public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, Result<ProductReviewListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public GetProductReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<ProductReviewListResponse>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            // First, check if the product exists
            var productExists = await _unitOfWork.Products.ExistsAsync(request.ProductId, cancellationToken);
            if (!productExists)
            {
                return Result.Failure<ProductReviewListResponse>(
                    ErrorResponse.NotFound($"Product with ID {request.ProductId} was not found."));
            }

            // Get paginated reviews with user information
            var (reviews, totalCount, averageRating, ratingSummary) = await _unitOfWork.Products
                .GetProductReviewsAsync(
                    request.ProductId,
                    request.SortBy,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

            // Map reviews to DTOs
            var reviewDtos = _mapper.Map<List<ProductReviewDto>>(reviews);

            // Populate reviewer names from Identity service
            foreach (var reviewDto in reviewDtos)
            {
                var review = reviews.FirstOrDefault(r => r.Id == reviewDto.Id);
                if (review != null)
                {
                    var userDetailsResult = await _identityService.GetUserDetailsAsync(review.UserId.ToString());
                    if (userDetailsResult.IsSuccess)
                    {
                        var (firstName, lastName, _) = userDetailsResult.Value;
                        reviewDto.ReviewerName = CensorUserName(firstName, lastName);
                    }
                    else
                    {
                        reviewDto.ReviewerName = "Anonymous User";
                    }
                }
            }

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var hasNextPage = request.PageNumber < totalPages;
            var hasPreviousPage = request.PageNumber > 1;

            var response = new ProductReviewListResponse
            {
                Items = reviewDtos,
                TotalItems = totalCount,
                TotalPages = totalPages,
                CurrentPage = request.PageNumber,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
                ProductId = request.ProductId,
                AverageRating = averageRating,
                RatingSummary = ratingSummary
            };

            return Result.Success(response);
        }

        // Helper method to censor user name (show first name + first letter of last name)
        private static string CensorUserName(string? firstName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return "Anonymous User";

            var censoredFirstName = firstName;
            var lastNameInitial = !string.IsNullOrWhiteSpace(lastName) ? lastName[0].ToString() + "****" : "";
            
            return string.IsNullOrEmpty(lastNameInitial) 
                ? censoredFirstName 
                : $"{censoredFirstName} {lastNameInitial}";
        }
    }
}
