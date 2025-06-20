using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.DeleteVariationType
{
    public record DeleteVariationTypeCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
    }
} 