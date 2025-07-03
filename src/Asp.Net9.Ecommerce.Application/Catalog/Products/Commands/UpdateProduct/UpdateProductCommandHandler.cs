using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
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
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.NotFound("Product not found");
                }

                // 2. Update basic product information
                var updateResult = product.Update(request.Name, request.Description ?? string.Empty);
                if (updateResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return updateResult;
                }

                // 3. Update price if changed
                if (product.BasePrice != request.BasePrice)
                {
                    var oldBasePrice = product.BasePrice;
                    var priceUpdateResult = product.UpdatePrice(request.BasePrice);
                    if (priceUpdateResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return priceUpdateResult;
                    }

                    // Update OldPrice for variants that inherit from base price
                    foreach (var variant in product.Variants)
                    {
                        variant.UpdateOldPriceForBasePriceChange(oldBasePrice, request.BasePrice);
                    }
                }

                // 4. Update active status if changed
                if (product.IsActive != request.IsActive)
                {
                    var stateChangeResult = request.IsActive ? product.Activate() : product.Deactivate();
                    if (stateChangeResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return stateChangeResult;
                    }
                }

                // 5. Update variants
                foreach (var variantInfo in request.Variants)
                {
                    var variant = product.Variants.FirstOrDefault(v => v.Id == variantInfo.Id);
                    if (variant == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.NotFound($"Variant with ID {variantInfo.Id} not found");
                    }

                    // Update variant name
                    if (variant.Name != variantInfo.Name)
                    {
                        var nameUpdateResult = variant.UpdateName(variantInfo.Name);
                        if (nameUpdateResult.IsFailure)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return nameUpdateResult;
                        }
                    }

                    // Update variant price
                    if (variantInfo.Price.HasValue)
                    {
                        var priceUpdateResult = variant.UpdatePrice(variantInfo.Price);
                        if (priceUpdateResult.IsFailure)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return priceUpdateResult;
                        }
                    }

                    // Update stock if tracking inventory
                    if (variantInfo.StockQuantity.HasValue && variant.TrackInventory)
                    {
                        var stockUpdateResult = variant.UpdateStock(variantInfo.StockQuantity.Value);
                        if (stockUpdateResult.IsFailure)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return stockUpdateResult;
                        }
                    }

                    // Update variant options (using new normalized structure)
                    if (variantInfo.SelectedOptions != null)
                    {
                        // Get current variant options
                        var currentOptions = variant.GetVariantOptions();
                        
                        // Remove options that are no longer selected
                        foreach (var currentOption in currentOptions)
                        {
                            if (!variantInfo.SelectedOptions.ContainsKey(currentOption.Key))
                            {
                                var removeResult = variant.RemoveOption(currentOption.Key);
                                if (removeResult.IsFailure)
                                {
                                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                                    return removeResult;
                                }
                            }
                        }
                        
                        // Add or update selected options
                        foreach (var selectedOption in variantInfo.SelectedOptions)
                        {
                            if (!currentOptions.TryGetValue(selectedOption.Key, out var currentOptionId) || 
                                currentOptionId != selectedOption.Value)
                            {
                                var addResult = variant.AddOption(selectedOption.Key, selectedOption.Value);
                                if (addResult.IsFailure)
                                {
                                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                                    return addResult;
                                }
                            }
                        }
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