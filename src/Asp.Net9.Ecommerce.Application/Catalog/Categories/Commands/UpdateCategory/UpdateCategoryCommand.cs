using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Slug { get; init; }
        public bool IsActive { get; init; }
        public List<CategoryVariationTypeInfo> VariationTypes { get; init; } = new();
    }
} 