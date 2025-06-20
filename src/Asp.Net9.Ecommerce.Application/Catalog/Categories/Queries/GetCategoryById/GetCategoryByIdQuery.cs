using MediatR;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
 
namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryNestedDto>;
} 