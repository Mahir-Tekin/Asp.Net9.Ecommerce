using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetVariationTypeById
{
    public record GetVariationTypeByIdQuery : IRequest<Result<VariationTypeResponse>>
    {
        public Guid Id { get; init; }
    }
} 