using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
            var query = new GetProductsQuery
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                HasStock = hasStock,
                IsActive = isActive,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">The ID of the product</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The product details</returns>
        /// <response code="200">Returns the product details</response>
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
    }
} 