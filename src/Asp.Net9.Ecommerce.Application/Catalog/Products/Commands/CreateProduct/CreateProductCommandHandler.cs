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

                // 1. Validate category exists and is active
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
                if (category == null)
                {
                    return Result.NotFound<Guid>("Category not found");
                }
                if (!category.IsActive)
                {
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("CategoryId", "Category is not active") }));
                }

                // 2. Validate at least one variant is provided
                if (request.Variants == null || !request.Variants.Any())
                {
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variants", "At least one variant is required") }));
                }

                // 3. Check for duplicate SKUs
                var duplicateSKUs = request.Variants
                    .GroupBy(v => v.SKU)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateSKUs.Any())
                {
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variants", 
                            $"Duplicate SKUs found: {string.Join(", ", duplicateSKUs)}") }));
                }

                // 4. Check if any SKU already exists in database
                foreach (var variant in request.Variants)
                {
                    if (await _unitOfWork.Products.ExistsBySKUAsync(variant.SKU, cancellationToken))
                    {
                        return Result.Failure<Guid>(ErrorResponse.ValidationError(
                            new List<ValidationError> { new("SKU", $"A product with SKU '{variant.SKU}' already exists") }));
                    }
                }

                // 5. Create variant types if provided
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

                // 6. Create variant data list
                var variantDataList = request.Variants.Select(v => new Product.VariantData
                {
                    SKU = v.SKU,
                    Name = v.Name,
                    Price = v.Price,
                    Variations = v.Variations ?? new Dictionary<string, string>()
                }).ToList();

                // 7. Create product
                var productResult = Product.Create(
                    request.Name,
                    request.Description,
                    request.BasePrice,
                    request.CategoryId,
                    variantTypes,
                    variantDataList);

                if (productResult.IsFailure)
                    return Result.Failure<Guid>(productResult.Error);

                var product = productResult.Value;

                // 8. Set inventory for variants
                foreach (var variant in product.Variants.Zip(request.Variants, (v, info) => (Variant: v, Info: info)))
                {
                    if (variant.Info.TrackInventory)
                    {
                        var updateStockResult = variant.Variant.UpdateStock(variant.Info.StockQuantity);
                        if (updateStockResult.IsFailure)
                            return Result.Failure<Guid>(updateStockResult.Error);
                    }
                }

                // 9. Save to database
                var addResult = await _unitOfWork.Products.AddAsync(product, cancellationToken);
                if (addResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<Guid>(addResult.Error);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success(product.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<Guid>(ErrorResponse.General(
                    $"An error occurred while creating the product: {ex.Message}",
                    "PRODUCT_CREATION_ERROR"));
            }
        }
    }
} 