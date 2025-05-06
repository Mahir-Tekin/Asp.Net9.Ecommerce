using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
    }
} 