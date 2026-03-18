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
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Utils.OneOf.MappingExtensions;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class ProductDocumentFileService : IProductDocumentFileService
{
    private const string _productIdInvalidError = "Product id does not match existing product ids";

    private readonly IProductDocumentRepository _productDocumentRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocalFileManagementService _localFileManagementService;
    private readonly IValidator<ServiceProductDocumentCreateRequest>? _createRequestValidator;

    public ProductDocumentFileService(
        IProductDocumentRepository productDocumentRepository,
        IProductRepository productRepository,
        [FromKeyedServices(ConfigureServices.ProductDocumentLocalFileManagementServiceKey)] ILocalFileManagementService localFileManagementService,
        IValidator<ServiceProductDocumentCreateRequest>? createRequestValidator = null)
    {
        _productDocumentRepository = productDocumentRepository;
        _productRepository = productRepository;
        _localFileManagementService = localFileManagementService;
        _createRequestValidator = createRequestValidator;
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
}