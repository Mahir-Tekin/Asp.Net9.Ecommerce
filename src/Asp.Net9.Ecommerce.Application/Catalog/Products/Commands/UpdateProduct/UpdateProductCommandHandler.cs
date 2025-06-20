using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Get product with variants
                var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(request.Id, cancellationToken);
                if (product == null)
                    return Result.NotFound("Product not found");

                // 2. Update basic product information
                var updateResult = product.Update(request.Name, request.Description);
                if (updateResult.IsFailure)
                    return updateResult;

                // 3. Update price if changed
                if (product.BasePrice != request.BasePrice)
                {
                    var priceUpdateResult = product.UpdatePrice(request.BasePrice);
                    if (priceUpdateResult.IsFailure)
                        return priceUpdateResult;
                }

                // 4. Update active status if changed
                if (product.IsActive != request.IsActive)
                {
                    var stateChangeResult = request.IsActive ? product.Activate() : product.Deactivate();
                    if (stateChangeResult.IsFailure)
                        return stateChangeResult;
                }

                // 5. Update variants
                foreach (var variantInfo in request.Variants)
                {
                    var variant = product.Variants.FirstOrDefault(v => v.Id == variantInfo.Id);
                    if (variant == null)
                        return Result.NotFound($"Variant with ID {variantInfo.Id} not found");

                    // Update variant name
                    if (variant.Name != variantInfo.Name)
                    {
                        var nameUpdateResult = variant.UpdateName(variantInfo.Name);
                        if (nameUpdateResult.IsFailure)
                            return nameUpdateResult;
                    }

                    // Update variant price
                    if (variantInfo.Price.HasValue)
                    {
                        var priceUpdateResult = variant.UpdatePrice(variantInfo.Price);
                        if (priceUpdateResult.IsFailure)
                            return priceUpdateResult;
                    }

                    // Update stock if tracking inventory
                    if (variantInfo.StockQuantity.HasValue && variant.TrackInventory)
                    {
                        var stockUpdateResult = variant.UpdateStock(variantInfo.StockQuantity.Value);
                        if (stockUpdateResult.IsFailure)
                            return stockUpdateResult;
                    }

                    // Update variations
                    if (variantInfo.Variations != null)
                    {
                        // TODO: Refactor for new normalized variant/option structure
                        // Commented out legacy variation update logic to prevent errors
                        // var currentVariations = variant.GetVariations();
                        // foreach (var oldVariation in currentVariations)
                        // {
                        //     if (!variantInfo.Variations.ContainsKey(oldVariation.Key))
                        //     {
                        //         var removeResult = variant.RemoveVariation(oldVariation.Key);
                        //         if (removeResult.IsFailure)
                        //             return removeResult;
                        //     }
                        // }
                        //
                        // // Add or update new variations
                        // foreach (var newVariation in variantInfo.Variations)
                        // {
                        //     if (!currentVariations.TryGetValue(newVariation.Key, out var currentValue) || 
                        //         currentValue != newVariation.Value)
                        //     {
                        //         var addResult = variant.AddVariation(newVariation.Key, newVariation.Value);
                        //         if (addResult.IsFailure)
                        //             return addResult;
                        //     }
                        // }
                    }
                }

                // 6. Update in repository
                var updateProductResult = await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                if (updateProductResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return updateProductResult;
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(ErrorResponse.General(
                    $"An error occurred while updating the product: {ex.Message}",
                    "PRODUCT_UPDATE_ERROR"));
            }
        }
    }
} 