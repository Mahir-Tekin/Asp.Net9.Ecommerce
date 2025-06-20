using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand : IRequest<Result<Guid>>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Slug { get; init; }
        public Guid? ParentCategoryId { get; init; }
        public List<CategoryVariationTypeInfo> VariationTypes { get; init; } = new();
    }
} 