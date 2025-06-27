using Asp.Net9.Ecommerce.Shared.Results;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductBySlug
{
    public record GetProductBySlugQuery(string Slug) : IRequest<Result<ProductDetailsDto>>;
}
