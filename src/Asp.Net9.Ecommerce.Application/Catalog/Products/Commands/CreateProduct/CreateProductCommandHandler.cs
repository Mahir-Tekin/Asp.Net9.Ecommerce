using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Validate category exists
                if (!await _unitOfWork.Categories.ExistsByIdAsync(request.CategoryId, cancellationToken))
                {
                    return Result.NotFound<Guid>("Category not found");
                }

                // 2. Check if SKU is unique
                if (await _unitOfWork.Products.ExistsBySKUAsync(request.DefaultSKU, cancellationToken))
                {
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("DefaultSKU", "A product with this SKU already exists") }));
                }

                // 3. Create variant types if provided
                var variantTypes = new List<VariationType>();
                if (request.VariantTypes?.Any() == true)
                {
                    foreach (var typeInfo in request.VariantTypes)
                    {
                        var typeResult = VariationType.Create(typeInfo.Name, typeInfo.DisplayName);
                        if (typeResult.IsFailure)
                            return Result.Failure<Guid>(typeResult.Error);

                        var type = typeResult.Value;
                        foreach (var optionInfo in typeInfo.Options)
                        {
                            var addOptionResult = type.AddOption(optionInfo.Value, optionInfo.DisplayValue);
                            if (addOptionResult.IsFailure)
                                return Result.Failure<Guid>(addOptionResult.Error);
                        }

                        variantTypes.Add(type);
                    }
                }

                // 4. Create default variant
                var defaultVariantResult = ProductVariant.Create(
                    Guid.Empty, // This will be set when the product is created
                    request.DefaultSKU,
                    request.DefaultVariantName ?? request.Name,
                    stockQuantity: request.DefaultStockQuantity ?? 0,
                    trackInventory: request.TrackInventory);

                if (defaultVariantResult.IsFailure)
                    return Result.Failure<Guid>(defaultVariantResult.Error);

                // 5. Create product
                var productResult = Product.Create(
                    request.Name,
                    request.Description,
                    request.BasePrice,
                    request.CategoryId,
                    defaultVariantResult.Value,
                    variantTypes);

                if (productResult.IsFailure)
                    return Result.Failure<Guid>(productResult.Error);

                // 6. Save to database
                _unitOfWork.Products.Add(productResult.Value);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success(productResult.Value.Id);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
} 