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
                    return Result.NotFound<Guid>("Category not found");
                if (!category.IsActive)
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("CategoryId", "Category is not active") }));

                // 2. Validate at least one variant is provided
                if (request.Variants == null || !request.Variants.Any())
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("Variants", "At least one variant is required") }));

                // 3. Check for duplicate SKUs
                var duplicateSKUs = request.Variants.GroupBy(v => v.SKU).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateSKUs.Any())
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("Variants", $"Duplicate SKUs found: {string.Join(", ", duplicateSKUs)}") }));

                // 4. Check if any SKU already exists in database
                foreach (var variant in request.Variants)
                {
                    if (await _unitOfWork.Products.ExistsBySKUAsync(variant.SKU, cancellationToken))
                        return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("SKU", $"A product with SKU '{variant.SKU}' already exists") }));
                }

                // 4.1. Check if slug already exists
                var slugToCheck = Product.GenerateSlug(request.Name);
                if (await _unitOfWork.Products.ExistsBySlugAsync(slugToCheck, cancellationToken))
                {
                    return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("Slug", $"A product with slug '{slugToCheck}' already exists") }));
                }

                // 5. Load variant types and options by ID
                var variantTypes = new List<VariationType>();
                var variantTypeDict = new Dictionary<Guid, VariationType>();
                if (request.VariantTypeIds != null && request.VariantTypeIds.Any())
                {
                    foreach (var typeId in request.VariantTypeIds)
                    {
                        var type = await _unitOfWork.VariationTypes.GetByIdWithOptionsAsync(typeId, cancellationToken);
                        if (type == null)
                            return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("VariantTypeId", $"Variant type with ID '{typeId}' not found") }));
                        if (!type.IsActive)
                            return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("VariantTypeId", $"Variant type with ID '{typeId}' is not active") }));
                        variantTypes.Add(type);
                        variantTypeDict[typeId] = type;
                    }
                }

                // 6. Validate and build variant data list
                var variantDataList = new List<Product.VariantData>();
                foreach (var v in request.Variants)
                {
                    // Validate SelectedOptions
                    if (variantTypes.Any())
                    {
                        // Must have all required variant types
                        var missingTypes = variantTypes.Select(t => t.Id).Except(v.SelectedOptions.Keys).ToList();
                        if (missingTypes.Any())
                        {
                            return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> {
                                new("SelectedOptions", $"Variant '{v.SKU}' is missing options for types: {string.Join(", ", missingTypes)}")
                            }));
                        }
                        // Validate all option IDs
                        foreach (var (typeId, optionId) in v.SelectedOptions)
                        {
                            if (!variantTypeDict.TryGetValue(typeId, out var type))
                                return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("SelectedOptions", $"Variant '{v.SKU}' references unknown variant type ID '{typeId}'") }));
                            if (!type.Options.Any(o => o.Id == optionId))
                                return Result.Failure<Guid>(ErrorResponse.ValidationError(new List<ValidationError> { new("SelectedOptions", $"Variant '{v.SKU}' references invalid option ID '{optionId}' for type '{type.Name}'") }));
                        }
                    }
                    variantDataList.Add(new Product.VariantData
                    {
                        SKU = v.SKU,
                        Name = v.Name,
                        Price = v.Price,
                        SelectedOptions = v.SelectedOptions ?? new Dictionary<Guid, Guid>()
                    });
                }

                

                // 7. Create product
                var productResult = Product.Create(
                    request.Name,
                    request.Description,
                    request.BasePrice,
                    request.CategoryId,
                    variantTypes,
                    variantDataList,
                    request.Images?.Select(img => new Product.ImageData
                    {
                        Url = img.Url,
                        AltText = img.AltText,
                        IsMain = img.IsMain
                    }).ToList(),
                    null // always generate slug from name
                );

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