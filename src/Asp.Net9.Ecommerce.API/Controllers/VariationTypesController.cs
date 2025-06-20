using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.CreateVariationType;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.DeleteVariationType;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.UpdateVariationType;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetAllVariationTypes;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetVariationTypeById;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/variation-types")]
    [Authorize(Roles = "Admin")]
    public class VariationTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public VariationTypesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all variation types
        /// </summary>
        /// <response code="200">Returns the list of variation types</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<VariationTypeResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var query = new GetAllVariationTypesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Retrieves a specific variation type by ID
        /// </summary>
        /// <param name="id">The ID of the variation type</param>
        /// <response code="200">Returns the requested variation type</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="404">Variation type not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VariationTypeResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetVariationTypeByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Creates a new variation type
        /// </summary>
        /// <param name="request">The variation type creation request</param>
        /// <response code="200">Returns the ID of the created variation type</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="409">A variation type with the same name already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateVariationTypeRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateVariationTypeCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Updates an existing variation type
        /// </summary>
        /// <param name="id">The ID of the variation type to update</param>
        /// <param name="request">The variation type update request</param>
        /// <response code="200">Variation type updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="404">Variation type not found</response>
        /// <response code="409">A variation type with the same name already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateVariationTypeRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateVariationTypeCommand>(request);
            command = command with { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Deletes a variation type
        /// </summary>
        /// <param name="id">The ID of the variation type to delete</param>
        /// <response code="200">Variation type deleted successfully</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not authorized</response>
        /// <response code="404">Variation type not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteVariationTypeCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
} 