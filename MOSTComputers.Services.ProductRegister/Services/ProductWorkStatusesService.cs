using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using FluentValidation.Results;
using FluentValidation;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Validation;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class ProductWorkStatusesService : IProductWorkStatusesService
{
    public ProductWorkStatusesService(
        IProductWorkStatusesRepository productWorkStatusesRepository,
        IValidator<ProductWorkStatusesCreateRequest>? createRequestValidator = null,
        IValidator<ProductWorkStatusesUpdateByIdRequest>? updateByIdRequestValidator = null,
        IValidator<ProductWorkStatusesUpdateByProductIdRequest>? updateByProductIdRequestValidator = null)
    {
        _productWorkStatusesRepository = productWorkStatusesRepository;
        _createRequestValidator = createRequestValidator;
        _updateByIdRequestValidator = updateByIdRequestValidator;
        _updateByProductIdRequestValidator = updateByProductIdRequestValidator;
    }

    private readonly IProductWorkStatusesRepository _productWorkStatusesRepository;

    private readonly IValidator<ProductWorkStatusesCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductWorkStatusesUpdateByIdRequest>? _updateByIdRequestValidator;
    private readonly IValidator<ProductWorkStatusesUpdateByProductIdRequest>? _updateByProductIdRequestValidator;

    public IEnumerable<ProductWorkStatuses> GetAll()
    {
        return _productWorkStatusesRepository.GetAll();
    }

    public ProductWorkStatuses? GetByProductId(int productId)
    {
        return _productWorkStatusesRepository.GetByProductId(productId);
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum)
    {
        return _productWorkStatusesRepository.GetAllWithProductNewStatus(productNewStatusEnum);
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum)
    {
        return _productWorkStatusesRepository.GetAllWithProductXmlStatus(productXmlStatusEnum);
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithReadyForImageInsert(bool readyForImageInsert)
    {
        return _productWorkStatusesRepository.GetAllWithReadyForImageInsert(readyForImageInsert);
    }

    public ProductWorkStatuses? GetById(int productWorkStatusesId)
    {
        return _productWorkStatusesRepository.GetById(productWorkStatusesId);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest,
        IValidator<ProductWorkStatusesCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productWorkStatusesRepository.InsertIfItDoesntExist(createRequest);
    }

    public OneOf<bool, ValidationResult> UpdateById(ProductWorkStatusesUpdateByIdRequest updateRequest,
        IValidator<ProductWorkStatusesUpdateByIdRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateByIdRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productWorkStatusesRepository.UpdateById(updateRequest);
    }

    public OneOf<bool, ValidationResult> UpdateByProductId(ProductWorkStatusesUpdateByProductIdRequest updateRequest,
        IValidator<ProductWorkStatusesUpdateByProductIdRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateByProductIdRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productWorkStatusesRepository.UpdateByProductId(updateRequest);
    }

    public bool DeleteAll()
    {
        return _productWorkStatusesRepository.DeleteAll();
    }

    public bool DeleteByProductId(int productId)
    {
        return _productWorkStatusesRepository.DeleteByProductId(productId);
    }

    public bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum)
    {
        return _productWorkStatusesRepository.DeleteAllWithProductNewStatus(productNewStatusEnum);
    }

    public bool DeleteAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum)
    {
        return _productWorkStatusesRepository.DeleteAllWithProductXmlStatus(productXmlStatusEnum);
    }

    public bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert)
    {
        return _productWorkStatusesRepository.DeleteAllWithReadyForImageInsert(readyForImageInsert);
    }

    public bool DeleteById(int productWorkStatusesId)
    {
        return _productWorkStatusesRepository.DeleteById(productWorkStatusesId);
    }
}