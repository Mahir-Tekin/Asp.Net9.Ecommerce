using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.DeleteCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries;
using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetAllCategoriesForAdmin;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetCategoryById;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CategoriesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="request">The category creation request</param>
        /// <returns>The ID of the created category</returns>
        /// <response code="201">Returns the ID of the created category</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="409">If a category with the same slug already exists</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var command = _mapper.Map<CreateCategoryCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return result.ToActionResult();

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Gets all active categories in a nested tree structure
        /// </summary>
        /// <remarks>
        /// Returns a hierarchical structure of categories where:
        /// - Only active categories are included
        /// - Root categories (no parent) are at the top level
        /// - Each category includes its subcategories recursively
        /// </remarks>
        /// <response code="200">Returns the list of categories in a tree structure</response>
        /// <response code="500">If there was an internal error while processing the request</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryNestedDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ResponseCache(Duration = 60)] // Cache for 1 minute
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets all categories (including inactive) in a nested tree structure for admin
        /// </summary>
        /// <remarks>
        /// Returns a hierarchical structure of all categories where:
        /// - Both active and inactive categories are included
        /// - Root categories (no parent) are at the top level
        /// - Each category includes its subcategories recursively
        /// </remarks>
        /// <response code="200">Returns the list of all categories in a tree structure</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="500">If there was an internal error while processing the request</response>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<CategoryNestedDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllCategoriesForAdminQuery(), cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="id">The ID of the category to get</param>
        /// <returns>The category details</returns>
        /// <response code="200">Returns the category details</response>
        /// <response code="404">If the category was not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryNestedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var categoryResult = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

            if (categoryResult == null)
                return Result.NotFound("Category not found").ToActionResult();

            return Result.Success(categoryResult).ToActionResult();
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">The ID of the category to update</param>
        /// <param name="request">The category update request</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the category was successfully updated</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="404">If the category was not found</response>
        /// <response code="409">If a category with the same slug already exists</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateCategoryCommand>(request);
            command = command with { Id = id };
            
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsSuccess)
                return NoContent();

            return result.ToActionResult();
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id">The ID of the category to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the category was successfully deleted</response>
        /// <response code="400">If the category has subcategories</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="404">If the category was not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            
            if (result.IsSuccess)
                return NoContent();

            return result.ToActionResult();
        }
    }
} 