using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.UpdateVariationType
{
    public record UpdateVariationTypeCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public bool IsActive { get; init; }
        public List<VariationOptionUpdateInfo> Options { get; init; } = new();
    }

    public record VariationOptionUpdateInfo
    {
        public string Value { get; init; }
        public string DisplayValue { get; init; }
        public int SortOrder { get; init; }
    }
} 