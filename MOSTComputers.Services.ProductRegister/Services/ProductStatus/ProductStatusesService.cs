//using FluentValidation;
//using FluentValidation.Results;
//using OneOf;
//using OneOf.Types;
//using MOSTComputers.Models.Product.Models;
//using MOSTComputers.Models.Product.Models.ProductStatuses;
//using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
//using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductStatuses;
//using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;

//using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;
//using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
//using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;


//namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus;
//internal sealed class ProductStatusesService : IProductStatusesService
//{
//    public ProductStatusesService(
//        IProductRepository productRepository,
//        IProductStatusesRepository productStatusesRepository,
//        IValidator<ProductStatusesCreateRequest>? createRequestValdator = null,
//        IValidator<ProductStatusesUpdateRequest>? updateRequestValdator = null)
//    {
//        _productRepository = productRepository;
//        _productStatusesRepository = productStatusesRepository;
//        _createRequestValdator = createRequestValdator;
//        _updateRequestValdator = updateRequestValdator;
//    }

//    private readonly IProductRepository _productRepository;
//    private readonly IProductStatusesRepository _productStatusesRepository;
//    private readonly IValidator<ProductStatusesCreateRequest>? _createRequestValdator;
//    private readonly IValidator<ProductStatusesUpdateRequest>? _updateRequestValdator;

//    public IEnumerable<ProductStatuses> GetAll()
//    {
//        return _productStatusesRepository.GetAll();
//    }

//    public ProductStatuses? GetByProductId(int productId)
//    {
//        if (productId <= 0) return null;

//        return _productStatusesRepository.GetByProductId(productId);
//    }

//    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds)
//    {
//        productIds = RemoveValuesSmallerThanOne(productIds);

//        return _productStatusesRepository.GetSelectionByProductIds(productIds);
//    }

//    public async Task<OneOf<Success, ValidationResult>> InsertIfItDoesntExistAsync(ProductStatusesCreateRequest createRequest)
//    {
//        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

//        if (product is null)
//        {
//            ValidationResult productExistsValidationResult = new();

//            productExistsValidationResult.Errors.Add(new(nameof(ServiceProductPropertyByCharacteristicNameCreateRequest.ProductId),
//                "Id does not correspond to any known product"));

//            return productExistsValidationResult;
//        }

//        ValidationResult validationResult = ValidateDefault(_createRequestValdator, createRequest);

//        if (!validationResult.IsValid) return validationResult;

//        return _productStatusesRepository.InsertIfItDoesntExist(createRequest);
//    }

//    public OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest)
//    {
//        ValidationResult validationResult = ValidateDefault(_updateRequestValdator, updateRequest);

//        if (!validationResult.IsValid) return validationResult;

//        return _productStatusesRepository.Update(updateRequest);
//    }
//    public bool DeleteByProductId(int productId)
//    {
//        if (productId <= 0) return false;

//        return _productStatusesRepository.DeleteByProductId(productId);
//    }
//}