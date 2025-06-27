using Asp.Net9.Ecommerce.Shared.Results;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById
{
    public record GetProductByIdQuery : IRequest<Result<ProductDetailsDto>>
    {
        public Guid Id { get; init; }
    }

   
} 