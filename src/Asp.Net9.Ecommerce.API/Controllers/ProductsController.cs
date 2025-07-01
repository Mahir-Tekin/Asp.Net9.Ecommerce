using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.VoteOnProductReview;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProductReview;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProductReview;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductBySlug;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductReviews;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ProductsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all active products with their summaries
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter products by name, description, or SKU</param>
        /// <param name="categoryId">Optional category ID to filter products</param>
        /// <param name="minPrice">Optional minimum price filter</param>
        /// <param name="maxPrice">Optional maximum price filter</param>
        /// <param name="hasStock">Optional filter for products with stock</param>
        /// <param name="isActive">Optional filter for active/inactive products</param>
        /// <param name="sortBy">Optional sorting parameter (default: CreatedAtDesc)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of product summaries</returns>
        /// <response code="200">Returns the paginated list of products</response>
        /// <remarks>
        /// <para><strong>Dynamic Variation Filtering:</strong></para>
        /// <para>You can filter by any variation type using query parameters. The system dynamically detects variation filters.</para>
        /// <para><strong>Examples:</strong></para>
        /// <list type="bullet">
        /// <item>Filter by color: <c>?color=red,blue</c></item>
        /// <item>Filter by size: <c>?size=M,L,XL</c></item>
        /// <item>Filter by brand: <c>?brand=nike,adidas</c></item>
        /// <item>Filter by material: <c>?material=cotton,silk</c></item>
        /// <item>Multiple variations: <c>?color=red&amp;size=M,L&amp;material=cotton</c></item>
        /// </list>
        /// <para><strong>How it works:</strong> Any query parameter not in the standard list will be treated as a variation filter.</para>
        /// <para><strong>Discovery:</strong> Use <c>GET /api/categories/{id}/filters</c> to discover available variation types and options for a specific category.</para>
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? hasStock = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] ProductSortBy sortBy = ProductSortBy.CreatedAtDesc,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // Parse variation filters from query parameters
            var variationFilters = ParseVariationFiltersFromQuery();

            var query = new GetProductsQuery
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                HasStock = hasStock,
                IsActive = isActive,
                VariationFilters = variationFilters,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
                var allParams = Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
    Console.WriteLine($"Request params: {string.Join(", ", allParams.Select(kv => $"{kv.Key}={kv.Value}"))}");
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        private List<VariationFilter>? ParseVariationFiltersFromQuery()
        {
            var variationFilters = new List<VariationFilter>();
            
            // Get all query parameters that are not standard parameters
            var standardParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "searchTerm", "categoryId", "minPrice", "maxPrice", 
                "hasStock", "isActive", "sortBy", "pageNumber", "pageSize"
                // Note: color, size, brand are intentionally NOT in this list 
                // so they get processed as variation filters
            };

            foreach (var param in Request.Query)
            {
                if (!standardParams.Contains(param.Key))
                {
                    // This is likely a variation filter (e.g., color=red,blue)
                    var variationTypeName = param.Key;
                    var optionValues = param.Value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                              .Select(v => v.Trim())
                                                              .Where(v => !string.IsNullOrEmpty(v))
                                                              .ToList();

                    if (optionValues.Any())
                    {
                        variationFilters.Add(new VariationFilter
                        {
                            VariationTypeName = variationTypeName,
                            OptionValues = optionValues
                        });
                    }
                }
            }

            return variationFilters.Any() ? variationFilters : null;
        }

        /// <summary>
        /// Advanced product search with complex filtering (alternative to GET with JSON body support)
        /// </summary>
        /// <param name="request">The search request with all filter options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of product summaries</returns>
        /// <response code="200">Returns the paginated list of products</response>
        /// <remarks>
        /// Use this endpoint when you need to send complex filter objects or prefer JSON body over query parameters.
        /// For simple filtering, use the GET endpoint instead.
        /// </remarks>
        [HttpPost("search")]
        [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromBody] GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">The ID of the product</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The product details including images and review statistics</returns>
        /// <response code="200">Returns the product details with images, average rating, and review count</response>
        /// <response code="404">If the product was not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Creates a new product with variants
        /// </summary>
        /// <param name="request">The product creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ID of the created product</returns>
        /// <response code="201">Returns the ID of the created product</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="409">If a variant SKU already exists</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateProductCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

            return result.ToActionResult();
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">The ID of the product to update</param>
        /// <param name="request">The product update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the product was updated successfully</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the product was not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateProductCommand>(request);
            command = command with { Id = id }; // Set ID from route

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return NoContent();

            return result.ToActionResult();
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the product was deleted successfully</response>
        /// <response code="404">If the product was not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return NoContent();

            return result.ToActionResult();
        }


        
        /// <summary>
        /// Uploads a product image and returns its public URL
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <returns>The public URL of the uploaded image</returns>
        /// <response code="200">Returns the image URL</response>
        /// <response code="400">If the file is invalid</response>
        [HttpPost("images/upload")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProductImage( IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ErrorResponse.ValidationError(new List<ValidationError> { new("File", "No file uploaded.") }));

            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(ErrorResponse.ValidationError(new List<ValidationError> { new("File", "Unsupported file type.") }));


            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return BadRequest(ErrorResponse.ValidationError(new List<ValidationError> { new("File", "File size exceeds 5MB limit.") }));

            // Ensure the directory exists
            var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(imagesDir))
                Directory.CreateDirectory(imagesDir);

            // Generate a unique file name
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(imagesDir, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Build the public URL (relative to wwwroot)
            var url = $"/images/products/{fileName}";
            return Ok(url);
        }

        /// <summary>
        /// Gets a product by slug (for SEO-friendly URLs)
        /// </summary>
        /// <param name="slug">The slug of the product</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The product details including review statistics</returns>
        /// <response code="200">Returns the product details with average rating and review count</response>
        /// <response code="404">If the product was not found</response>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductBySlugQuery(slug), cancellationToken);
            return result.ToActionResult();
        }

        #region Product Reviews

        /// <summary>
        /// Creates a new review for a product
        /// </summary>
        /// <param name="productId">The ID of the product to review</param>
        /// <param name="request">The review creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ID of the created review</returns>
        /// <response code="201">Returns the ID of the created review</response>
        /// <response code="400">If the request is invalid or user is not eligible</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the product was not found</response>
        /// <response code="409">If the user has already reviewed this product</response>
        [HttpPost("{productId}/reviews")]
        [Authorize]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateReview(
            Guid productId, 
            [FromBody] CreateProductReviewDto request, 
            CancellationToken cancellationToken)
        {
            // Get the current user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user token"));
            }

            // Ensure the productId from the route matches the request
            request.ProductId = productId;

            var command = new CreateProductReviewCommand(userGuid, request);
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess)
                return CreatedAtAction(
                    nameof(GetProductReviews), 
                    new { productId = productId }, 
                    new { reviewId = result.Value });

            return result.ToActionResult();
        }

        /// <summary>
        /// Gets all reviews for a product with pagination
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <param name="sortBy">Sort by rating, date, or helpfulness (default: newest first)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of product reviews</returns>
        /// <response code="200">Returns the paginated list of reviews</response>
        /// <response code="404">If the product was not found</response>
        [HttpGet("{productId}/reviews")]
        [ProducesResponseType(typeof(ProductReviewListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews(
            Guid productId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ReviewSortBy sortBy = ReviewSortBy.Newest,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductReviewsQuery
            {
                ProductId = productId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="reviewId">The ID of the review to update</param>
        /// <param name="request">The review update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the review was updated successfully</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the review author</response>
        /// <response code="404">If the product or review was not found</response>
        [HttpPut("{productId}/reviews/{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview(
            Guid productId,
            Guid reviewId,
            [FromBody] UpdateProductReviewDto request,
            CancellationToken cancellationToken)
        {
            // Get the current user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user token"));
            }

            var command = new UpdateProductReviewCommand(productId, reviewId, userGuid, request);
            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        /// <summary>
        /// Deletes a review
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="reviewId">The ID of the review to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the review was deleted successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the review author</response>
        /// <response code="404">If the product or review was not found</response>
        [HttpDelete("{productId}/reviews/{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(
            Guid productId,
            Guid reviewId,
            CancellationToken cancellationToken)
        {
            // Get the current user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user token"));
            }

            var command = new DeleteProductReviewCommand(productId, reviewId, userGuid);
            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        /// <summary>
        /// Votes on the helpfulness of a review
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="reviewId">The ID of the review</param>
        /// <param name="helpful">True for helpful, false for unhelpful</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the vote was recorded successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the product or review was not found</response>
        [HttpPost("{productId}/reviews/{reviewId}/vote")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VoteOnReview(
            Guid productId,
            Guid reviewId,
            [FromQuery] bool helpful,
            CancellationToken cancellationToken)
        {
            // Get the current user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(ErrorResponse.Unauthorized("Invalid user token"));
            }

            var voteType = helpful ? Domain.Catalog.VoteType.Helpful : Domain.Catalog.VoteType.Unhelpful;
            var command = new VoteOnProductReviewCommand(productId, reviewId, userGuid, voteType);
            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        #endregion
    }
}