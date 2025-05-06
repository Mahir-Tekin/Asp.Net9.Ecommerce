using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product ID is required");
        }
    }
} 