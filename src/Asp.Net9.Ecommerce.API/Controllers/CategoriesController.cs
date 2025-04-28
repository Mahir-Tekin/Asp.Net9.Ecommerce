using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        // Placeholder for GetById action that will be implemented later
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(); // This will be implemented later
        }
    }
} 