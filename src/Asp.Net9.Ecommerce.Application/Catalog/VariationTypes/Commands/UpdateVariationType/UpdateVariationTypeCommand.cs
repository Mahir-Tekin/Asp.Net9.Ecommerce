using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.UpdateVariationType
{
    public record UpdateVariationTypeCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string DisplayName { get; init; }
        public bool IsActive { get; init; }
        public List<VariationOptionUpdateInfo> Options { get; init; } = new();
    }

    public record VariationOptionUpdateInfo
    {
        public Guid? Id { get; init; } // Null for new options, set for existing options to update
        public required string Value { get; init; }
        public required string DisplayValue { get; init; }
        public int SortOrder { get; init; }
    }
} 