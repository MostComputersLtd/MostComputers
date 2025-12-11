using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.Text.Json.Serialization;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services.ProductProperties;
internal sealed class ProductPropertyService : IProductPropertyService
{
    public ProductPropertyService(
        IProductPropertyCrudService productPropertyCrudService,
        IProductCharacteristicService productCharacteristicService,
        IProductRepository productRepository,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        IProductToHtmlProductService productToHtmlProductService,
        IProductHtmlService productHtmlService,
        IProductImageService productImageService,
        ITransactionExecuteService transactionExecuteService)
    {
        _productPropertyCrudService = productPropertyCrudService;
        _productCharacteristicService = productCharacteristicService;
        _productRepository = productRepository;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _productToHtmlProductService = productToHtmlProductService;
        _productHtmlService = productHtmlService;
        _productImageService = productImageService;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IProductPropertyCrudService _productPropertyCrudService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductRepository _productRepository;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly IProductToHtmlProductService _productToHtmlProductService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly IProductImageService _productImageService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public async Task<List<IGrouping<int, ProductProperty>>> GetAllAsync()
    {
        List<ProductProperty> allProperties = await _productPropertyCrudService.GetAllAsync();

        return allProperties.GroupBy(x => x.ProductId)
            .ToList();
    }

    public async Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _productPropertyCrudService.GetAllInProductsAsync(productIds);
    }

    public async Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _productPropertyCrudService.GetCountOfAllInProductsAsync(productIds);
    }

    public async Task<List<ProductProperty>> GetAllInProductAsync(int productId)
    {
        if (productId <= 0) return new();

        return await _productPropertyCrudService.GetAllInProductAsync(productId);
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        if (productId <= 0) return 0;

        return await _productPropertyCrudService.GetCountOfAllInProductAsync(productId);
    }

    public async Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId)
    {
        if (productId <= 0) return null;

        return await _productPropertyCrudService.GetByProductAndCharacteristicIdAsync(productId, characteristicId);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(
        ServiceProductPropertyByCharacteristicIdCreateRequest createRequest,
        string createUserName)
    {
        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => InsertInternalAsync(createRequest, createUserName),
        //    result => result.IsT0);

        return await InsertInternalAsync(createRequest, createUserName);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInternalAsync(
        ServiceProductPropertyByCharacteristicIdCreateRequest createRequest,
        string createUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await _productPropertyCrudService.InsertAsync(createRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                createRequest.ProductId, ProductNewStatus.WorkInProgress, createUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                statusId => result.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        return await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(createRequest.ProductId, localDBTransactionScope);
    }

    //private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInternalAsync(
    //    ServiceProductPropertyByCharacteristicIdCreateRequest createRequest,
    //    string createUserName)
    //{
    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> result
    //        = await _productPropertyCrudService.InsertAsync(createRequest);

    //    if (!result.IsT0) return result;

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            createRequest.ProductId, ProductNewStatus.WorkInProgress, createUserName);

    //    if (!upsertProductStatusResult.IsT0)
    //    {
    //        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //            statusId => result.AsT0,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
    //        = await UpdateImagesProductHtmlToCurrentProductDataAsync(createRequest.ProductId);

    //    return updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //        isSuccessful => result.AsT0,
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(
        ProductPropertyUpdateRequest updateRequest,
        string updateUserName)
    {
        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpdateInternalAsync(updateRequest, updateUserName),
        //    result => result.IsT0);

        return await UpdateInternalAsync(updateRequest, updateUserName);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInternalAsync(
        ProductPropertyUpdateRequest updateRequest,
        string updateUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpdateAsync(updateRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                updateRequest.ProductId, ProductNewStatus.WorkInProgress, updateUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                statusId => result.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        return await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(updateRequest.ProductId, localDBTransactionScope);
    }

    //private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInternalAsync(
    //    ProductPropertyUpdateRequest updateRequest,
    //    string updateUserName)
    //{
    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpdateAsync(updateRequest);

    //    if (!result.IsT0) return result;

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            updateRequest.ProductId, ProductNewStatus.WorkInProgress, updateUserName);

    //    if (!upsertProductStatusResult.IsT0)
    //    {
    //        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //            statusId => result.AsT0,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
    //        = await UpdateImagesProductHtmlToCurrentProductDataAsync(updateRequest.ProductId);

    //    return updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //        isSuccessful =>
    //        {
    //            if (!isSuccessful) return new UnexpectedFailureResult();

    //            return result.AsT0;
    //        },
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(
        ProductPropertyUpdateRequest upsertRequest,
        string upsertUserName)
    {
        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpsertInternalAsync(upsertRequest, upsertUserName),
        //    result => result.IsT0);

        return await UpsertInternalAsync(upsertRequest, upsertUserName);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInternalAsync(
        ProductPropertyUpdateRequest upsertRequest,
        string upsertUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpsertAsync(upsertRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                upsertRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                statusId => result.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        return await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(upsertRequest.ProductId, localDBTransactionScope);
    }

    //private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInternalAsync(
    //   ProductPropertyUpdateRequest upsertRequest,
    //   string upsertUserName)
    //{
    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productPropertyCrudService.UpsertAsync(upsertRequest);

    //    if (!result.IsT0) return result;

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            upsertRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

    //    if (!upsertProductStatusResult.IsT0)
    //    {
    //        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //            statusId => result.AsT0,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
    //        = await UpdateImagesProductHtmlToCurrentProductDataAsync(upsertRequest.ProductId);

    //    return updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //        isSuccessful => result.AsT0,
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdAsync(
        int productId, int oldCharacteristicId, int newCharacteristicId, string changeUserName)
    {
        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => ChangePropertyCharacteristicIdInternalAsync(productId, oldCharacteristicId, newCharacteristicId, changeUserName),
        //    result => result.IsT0);

        return await ChangePropertyCharacteristicIdInternalAsync(productId, oldCharacteristicId, newCharacteristicId, changeUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdInternalAsync(
        int productId, int currentCharacteristicId, int newCharacteristicId, string changeUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> result
            = await _productPropertyCrudService.ChangePropertyCharacteristicIdAsync(productId, currentCharacteristicId, newCharacteristicId);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, changeUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
                statusId => result.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
            = await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(productId, localDBTransactionScope);

        return updateImagesHtmlDataResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    }

    //private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdInternalAsync(
    //    int productId, int currentCharacteristicId, int newCharacteristicId, string changeUserName)
    //{
    //    OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> result
    //        = await _productPropertyCrudService.ChangePropertyCharacteristicIdAsync(productId, currentCharacteristicId, newCharacteristicId);

    //    if (!result.IsT0) return result;

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            productId, ProductNewStatus.WorkInProgress, changeUserName);

    //    if (!upsertProductStatusResult.IsT0)
    //    {
    //        return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
    //            statusId => result.AsT0,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
    //        = await UpdateImagesProductHtmlToCurrentProductDataAsync(productId);

    //    return updateImagesProductHtmlResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
    //        isSuccessful =>
    //        {
    //            if (!isSuccessful) return new UnexpectedFailureResult();

    //            return result.AsT0;
    //        },
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(
        ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest, string upsertUserName)
    {
        //using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        //using (TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
        //{
            OneOf<Success, ValidationResult, UnexpectedFailureResult> result
                = await _productPropertyCrudService.UpsertAllProductPropertiesAsync(productPropertyChangeAllForProductRequest);

            if (!result.IsT0)
            {
                return result;
            }

            OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
               = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                   productPropertyChangeAllForProductRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

            if (!upsertProductStatusResult.IsT0)
            {
                return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                    statusId => result.AsT0,
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            //localDBTransactionScope.Complete();
        //}

        HtmlProductsData productHtmlData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync([productPropertyChangeAllForProductRequest.ProductId]);

        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

        if (!getProductHtmlResult.IsT0)
        {
            return new UnexpectedFailureResult();
        }

        //using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
            = await _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productPropertyChangeAllForProductRequest.ProductId, getProductHtmlResult.AsT0);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResultMapped
            = updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                isSuccessful =>
                {
                    return new Success();
                },
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);

        if (!updateImagesProductHtmlResultMapped.IsT0) return updateImagesProductHtmlResultMapped;

        //replicationDBTransactionScope.Complete();

        //localDBTransactionScope.Complete();
        
        return updateImagesProductHtmlResultMapped;
    }

    //public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(
    //    ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest, string upsertUserName)
    //{
    //    return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
    //        () => UpsertAllProductPropertiesInternalAsync(productPropertyChangeAllForProductRequest, upsertUserName),
    //        result => result.IsT0);
    //}

    //public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesInternalAsync(
    //    ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest, string upsertUserName)
    //{
    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> result
    //        = await _productPropertyCrudService.UpsertAllProductPropertiesAsync(productPropertyChangeAllForProductRequest);

    //    if (!result.IsT0) return result;

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //       = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //           productPropertyChangeAllForProductRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

    //    if (!upsertProductStatusResult.IsT0)
    //    {
    //        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //            statusId => result.AsT0,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
    //        = await UpdateImagesProductHtmlToCurrentProductDataAsync(productPropertyChangeAllForProductRequest.ProductId);

    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResultMapped
    //        = updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //            isSuccessful =>
    //            {
    //                return result.AsT0;
    //            },
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);

    //    return updateImagesProductHtmlResultMapped;
    //}

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAsync(
        int productId, int characteristicId, string deleteUserName)
    {
        if (productId <= 0 || characteristicId <= 0) return new NotFound();

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteInternalAsync(productId, characteristicId, deleteUserName),
        //    result => result.Match(
        //        success => true,
        //        notFound => true,
        //        validationResult => false,
        //        unexpectedFailureResult => false));

        return await DeleteInternalAsync(productId, characteristicId, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteInternalAsync(
        int productId, int characteristicId, string deleteUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        bool result = await _productPropertyCrudService.DeleteAsync(productId, characteristicId);

        if (!result) return new NotFound();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, deleteUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
                statusId => new Success(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
            = await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(productId, localDBTransactionScope);

        return updateImagesHtmlDataResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForProductAsync(
        int productId, string deleteUserName)
    {
        if (productId <= 0) return new NotFound();

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteAllForProductInternalAsync(productId, deleteUserName),
        //    result => result.Match(
        //        success => true,
        //        notFound => true,
        //        validationResult => false,
        //        unexpectedFailureResult => false));

        return await DeleteAllForProductInternalAsync(productId, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForProductInternalAsync(
        int productId, string deleteUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        bool result = await _productPropertyCrudService.DeleteAllForProductAsync(productId);

        if (!result) return new NotFound();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, deleteUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
                statusId => new Success(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
            = await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(productId, localDBTransactionScope);

        return updateImagesHtmlDataResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForCharacteristicAsync(
        int characteristicId, string deleteUserName)
    {
        if (characteristicId <= 0) return new NotFound();

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteAllForCharacteristicInternalAsync(characteristicId, deleteUserName),
        //    result => result.Match(
        //        success => true,
        //        notFound => true,
        //        validationResult => false,
        //        unexpectedFailureResult => false));

        return await DeleteAllForCharacteristicInternalAsync(characteristicId, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForCharacteristicInternalAsync(
        int characteristicId, string deleteUserName)
    {
        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        List<ProductProperty> productProperties = await _productPropertyCrudService.GetAllAsync();

        IEnumerable<ProductProperty> relatedProductProperties = productProperties
            .Where(property => property.ProductCharacteristicId == characteristicId);

        List<int> relatedProductIds = relatedProductProperties
            .Select(x => x.ProductId)
            .ToList();

        bool result = await _productPropertyCrudService.DeleteAllForCharacteristicAsync(characteristicId);

        if (!result) return new NotFound();

        foreach (int productId in relatedProductIds)
        {
            OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
                = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                    productId, ProductNewStatus.WorkInProgress, deleteUserName);

            if (!upsertProductStatusResult.IsT0)
            {
                return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
                    statusId => new Success(),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }
        }

        foreach (int productId in relatedProductIds)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
                = await UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(productId);

            if (!updateImagesHtmlDataResult.IsT0)
            {
                return updateImagesHtmlDataResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
            }
        }

        localDBTransactionScope.Complete();

        return new Success();
    }

    //private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForCharacteristicInternalAsync(
    //    int characteristicId, string deleteUserName)
    //{
    //    List<ProductProperty> productProperties = await _productPropertyCrudService.GetAllAsync();

    //    IEnumerable<ProductProperty> relatedProductProperties = productProperties
    //        .Where(property => property.ProductCharacteristicId == characteristicId);

    //    List<int> relatedProductIds = relatedProductProperties
    //        .Select(x => x.ProductId)
    //        .ToList();

    //    bool result = await _productPropertyCrudService.DeleteAllForCharacteristicAsync(characteristicId);

    //    if (!result) return new NotFound();

    //    foreach (int productId in relatedProductIds)
    //    {
    //        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //                productId, ProductNewStatus.WorkInProgress, deleteUserName);

    //        if (!upsertProductStatusResult.IsT0)
    //        {
    //            return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
    //                statusId => new Success(),
    //                validationResult => validationResult,
    //                unexpectedFailureResult => unexpectedFailureResult);
    //        }
    //    }

    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
    //        = await UpdateImagesProductHtmlToCurrentProductsDataAsync(relatedProductIds);

    //    return updateImagesHtmlDataResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    //}

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateImagesHtmlDataSeparatelyWithTransactionsAsync(
        int productId,
        TransactionScope? localDBTransactionScope = null)
    {
        HtmlProductsData productHtmlData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync([productId]);

        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

        if (!getProductHtmlResult.IsT0)
        {
            return new UnexpectedFailureResult();
        }

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResult
            = await _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, getProductHtmlResult.AsT0);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesProductHtmlResultMapped
            = updateImagesProductHtmlResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                isSuccessful =>
                {
                    return new Success();
                },
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);

        if (!updateImagesProductHtmlResultMapped.IsT0) return updateImagesProductHtmlResultMapped;

        replicationDBTransactionScope.Complete();

        localDBTransactionScope?.Complete();

        return updateImagesProductHtmlResultMapped;
    }

    //private async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateImagesProductHtmlToCurrentProductDataAsync(int productId)
    //{
    //    HtmlProductsData productHtmlData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync([productId]);

    //    return await UpdateImagesProductHtmlToCurrentProductDataAsync(productId, productHtmlData);
    //}

    //private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateImagesProductHtmlToCurrentProductsDataAsync(List<int> productIds)
    //{
    //    foreach (int productId in productIds)
    //    {   
    //        HtmlProductsData productHtmlData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync([productId]);

    //        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

    //        if (!getProductHtmlResult.IsT0)
    //        {
    //            return new UnexpectedFailureResult();
    //        }

    //        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateProductHtmlInImagesResult
    //            = await _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, getProductHtmlResult.AsT0);

    //        if (!updateProductHtmlInImagesResult.IsT0)
    //        {
    //            return updateProductHtmlInImagesResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
    //                isSuccessful => new UnexpectedFailureResult(),
    //                validationResult => validationResult,
    //                unexpectedFailureResult => unexpectedFailureResult);
    //        }
    //    }

    //    return new Success();
    //}

    //private async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateImagesProductHtmlToCurrentProductDataAsync(int productId, HtmlProductsData productHtmlData)
    //{
    //    OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

    //    if (!getProductHtmlResult.IsT0)
    //    {
    //        return new UnexpectedFailureResult();
    //    }

    //    OneOf<bool, ValidationResult, UnexpectedFailureResult> updateProductHtmlInImagesResult
    //        = await _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, getProductHtmlResult.AsT0);

    //    return updateProductHtmlInImagesResult;
    //}
}