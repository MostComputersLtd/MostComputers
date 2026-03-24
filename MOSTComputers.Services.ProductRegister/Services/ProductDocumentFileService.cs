using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductDocument;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Utils.OneOf.MappingExtensions;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductDocumentConstraints;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class ProductDocumentFileService : IProductDocumentFileService
{
    private const string _productIdInvalidError = "Product id does not match existing product ids";

    private readonly IProductDocumentRepository _productDocumentRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocalFileManagementService _localFileManagementService;
    private readonly IValidator<ServiceProductDocumentCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServiceProductDocumentUpdateRequest>? _updateRequestValidator;

    public ProductDocumentFileService(
        IProductDocumentRepository productDocumentRepository,
        IProductRepository productRepository,
        [FromKeyedServices(ConfigureServices.ProductDocumentLocalFileManagementServiceKey)] ILocalFileManagementService localFileManagementService,
        IValidator<ServiceProductDocumentCreateRequest>? createRequestValidator = null,
        IValidator<ServiceProductDocumentUpdateRequest>? updateRequestValidator = null)
    {
        _productDocumentRepository = productDocumentRepository;
        _productRepository = productRepository;
        _localFileManagementService = localFileManagementService;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    public async Task<List<ProductDocument>> GetAllForProductAsync(int productId)
    {
        if (productId < 0) return new();

        return await _productDocumentRepository.GetAllForProductAsync(productId);
    }

    public async Task<ProductDocument?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _productDocumentRepository.GetByIdAsync(id);
    }

    public async Task<Stream?> GetFileStreamByIdAsync(int id)
    {
        if (id <= 0) return null;

        ProductDocument? productDocument = await _productDocumentRepository.GetByIdAsync(id);

        if (productDocument == null) return null;

        return _localFileManagementService.GetFileStream(productDocument.FileName);
    }

    public Stream? GetFileStreamByFileName(string fullFileName)
    {
        return _localFileManagementService.GetFileStream(fullFileName);
    }

    public async Task<OneOf<ProductDocument, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(
        ServiceProductDocumentCreateRequest createRequest)
    {
        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        createRequest.FileExtension = createRequest.FileExtension.Trim().ToLowerInvariant();

        if (createRequest.FileExtension.StartsWith('.'))
        {
            createRequest.FileExtension = createRequest.FileExtension[1..];
        }

        if (_createRequestValidator != null)
        {
            ValidationResult validationResult = _createRequestValidator.Validate(createRequest);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }
        }

        bool productExists = await _productRepository.DoesProductExistAsync(createRequest.ProductId);

        if (!productExists)
        {
            ValidationFailure error = new(nameof(createRequest.ProductId), _productIdInvalidError);

            return new ValidationResult([error]);
        }

        ProductDocumentCreateRequest productDocumentCreateRequest = new()
        {
            ProductId = createRequest.ProductId,
            FileExtension = createRequest.FileExtension,
            Description = createRequest.Description,
        };

        OneOf<ProductDocument, UnexpectedFailureResult> recordCreateResult = await _productDocumentRepository.InsertAsync(productDocumentCreateRequest);

        if (!recordCreateResult.IsT0)
        {
            return recordCreateResult.Map<ProductDocument, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
        }

        ProductDocument productDocument = recordCreateResult.AsT0;

        OneOf<Success, FileAlreadyExistsResult> fileCreateResult
            = await _localFileManagementService.AddFileAsync(productDocument.FileName, createRequest.FileData);

        if (!fileCreateResult.IsT0)
        {
            return fileCreateResult.AsT1;
        }

        transactionScope.Complete();

        return productDocument;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServiceProductDocumentUpdateRequest updateRequest)
    {
        if (_updateRequestValidator != null)
        {
            ValidationResult validationResult = _updateRequestValidator.Validate(updateRequest);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }
        }

        ProductDocumentUpdateRequest innerUpdateRequest = new()
        {
            IdOrFileName = updateRequest.IdOrFileName,
            Description = updateRequest.Description,
        };

        OneOf<Success, NotFound> result = await _productDocumentRepository.UpdateAsync(innerUpdateRequest);

        return result.Map<Success, NotFound, ValidationResult>();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(OneOf<int, string> idOrFileName)
    {
        if (idOrFileName.IsT0 && idOrFileName.AsT0 <= 0) return new NotFound();

        if (idOrFileName.IsT1
            && (string.IsNullOrWhiteSpace(idOrFileName.AsT1) || idOrFileName.AsT1.Length > FileNameMaxLength))
        {
            return new NotFound();
        }

        return await _productDocumentRepository.DeleteAsync(idOrFileName);
    }
}