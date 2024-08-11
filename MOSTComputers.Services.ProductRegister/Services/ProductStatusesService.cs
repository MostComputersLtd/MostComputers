using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductStatusesService : IProductStatusesService
{
    public ProductStatusesService(
        IProductStatusesRepository productStatusesRepository,
        IValidator<ProductStatusesCreateRequest>? createRequestValdator = null,
        IValidator<ProductStatusesUpdateRequest>? updateRequestValdator = null)
    {
        _productStatusesRepository = productStatusesRepository;
        _createRequestValdator = createRequestValdator;
        _updateRequestValdator = updateRequestValdator;
    }

    private readonly IProductStatusesRepository _productStatusesRepository;
    private readonly IValidator<ProductStatusesCreateRequest>? _createRequestValdator;
    private readonly IValidator<ProductStatusesUpdateRequest>? _updateRequestValdator;

    public IEnumerable<ProductStatuses> GetAll()
    {
        return _productStatusesRepository.GetAll();
    }

    public ProductStatuses? GetByProductId(int productId)
    {
        if (productId <= 0) return null;

        return _productStatusesRepository.GetByProductId(productId);
    }

    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _productStatusesRepository.GetSelectionByProductIds(productIds);
    }

    public OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValdator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        return _productStatusesRepository.InsertIfItDoesntExist(createRequest);
    }

    public OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValdator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        return _productStatusesRepository.Update(updateRequest);
    }
    public bool DeleteByProductId(int productId)
    {
        if (productId <= 0) return false;

        return _productStatusesRepository.DeleteByProductId(productId);
    }
}