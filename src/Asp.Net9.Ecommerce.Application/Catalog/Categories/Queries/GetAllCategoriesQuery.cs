using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<Result<List<CategoryNestedDto>>>
    {
    }
} 