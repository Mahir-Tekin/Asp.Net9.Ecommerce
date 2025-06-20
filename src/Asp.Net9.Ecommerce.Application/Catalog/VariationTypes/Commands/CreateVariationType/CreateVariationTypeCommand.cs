using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.CreateVariationType
{
    public record CreateVariationTypeCommand : IRequest<Result<Guid>>
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public List<VariationOptionInfo> Options { get; init; } = new();
    }

    public record VariationOptionInfo
    {
        public string Value { get; init; }
        public string DisplayValue { get; init; }
        public int SortOrder { get; init; }
    }
} 