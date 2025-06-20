using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetAllCategoriesForAdmin
{
    public class GetAllCategoriesForAdminQuery : IRequest<Result<List<CategoryNestedDto>>>
    {
        // Empty as we don't need any parameters
    }
} 