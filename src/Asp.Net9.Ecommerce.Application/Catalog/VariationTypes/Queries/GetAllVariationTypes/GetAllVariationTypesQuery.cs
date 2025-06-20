using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetAllVariationTypes
{
    public record GetAllVariationTypesQuery : IRequest<Result<List<VariationTypeResponse>>>
    {
    }
} 