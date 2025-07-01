using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands
{
    public class CreateProductReviewCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public CreateProductReviewDto Dto { get; set; }

        public CreateProductReviewCommand(Guid userId, CreateProductReviewDto dto)
        {
            UserId = userId;
            Dto = dto;
        }
    }
}
