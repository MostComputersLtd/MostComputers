using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.Transactions;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;
internal sealed class ProductImageService : IProductImageService
{
    public ProductImageService(
        IProductImageAndFileService productImageAndFileService,
        IPromotionProductFileInfoService promotionProductFileInfoService,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        ITransactionExecuteService transactionExecuteService)
    {
        _productImageAndFileService = productImageAndFileService;
        _promotionProductFileInfoService = promotionProductFileInfoService;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IProductImageAndFileService _productImageAndFileService;
    private readonly IPromotionProductFileInfoService _promotionProductFileInfoService;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync()
    {
        return await _productImageAndFileService.GetAllWithoutFileDataAsync();
    }

    public async Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync()
    {
        return await _productImageAndFileService.GetAllFirstImagesForAllProductsAsync();
    }

    public async Task<List<ProductImage>> GetAllInProductAsync(int productId)
    {
        return await _productImageAndFileService.GetAllInProductAsync(productId);
    }

    public async Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageAndFileService.GetAllInProductsAsync(productIds);
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds)
    {
        return await _productImageAndFileService.GetAllInProductsWithoutFileDataAsync(productIds);
    }

    public async Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId)
    {
        return await _productImageAndFileService.GetAllInProductWithoutFileDataAsync(productId);
    }

    public async Task<ProductImage?> GetByIdInAllImagesAsync(int id)
    {
        return await _productImageAndFileService.GetByIdInAllImagesAsync(id);
    }

    public async Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id)
    {
        return await _productImageAndFileService.GetByIdInAllImagesWithoutFileDataAsync(id);
    }

    public async Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId)
    {
        return await _productImageAndFileService.GetByProductIdInFirstImagesAsync(productId);
    }

    public async Task<bool> DoesProductImageExistAsync(int imageId)
    {
        return await _productImageAndFileService.DoesProductImageExistAsync(imageId);
    }

    public async Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(List<int> productIds)
    {
        return await _productImageAndFileService.DoProductsHaveImagesInFirstImagesAsync(productIds);
    }

    public async Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId)
    {
        return await _productImageAndFileService.DoesProductHaveImageInFirstImagesAsync(productId);
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        return await _productImageAndFileService.GetCountOfAllInProductAsync(productId);
    }

    public async Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageAndFileService.GetCountOfAllInProductsAsync(productIds);
    }

    public async Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(List<int> productIds)
    {
        return await _productImageAndFileService.GetFirstImagesForSelectionOfProductsAsync(productIds);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesAsync(
        ServiceProductImageCreateRequest createRequest, string createUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertInAllImagesInternalAsync(createRequest, createUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesInternalAsync(
        ServiceProductImageCreateRequest createRequest, string createUserName)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.InsertInAllImagesAsync(createRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                createRequest.ProductId, ProductNewStatus.WorkInProgress, createUserName);

        return upsertProductStatusResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileAsync(ProductImageWithFileCreateRequest productImageWithFileCreateRequest)
    {
        using TransactionScope replicationDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _productImageAndFileService.InsertInAllImagesWithFileAsync(productImageWithFileCreateRequest);

        if (!result.IsT0) return result;

        replicationDBWorkTransactionScope.Complete();

        return result;
    }

    //public Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileAsync(ProductImageWithFileCreateRequest productImageWithFileCreateRequest)
    //{
    //    return _productImageAndFileService.InsertInAllImagesWithFileAsync(productImageWithFileCreateRequest);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesAsync(
        ServiceProductFirstImageCreateRequest createRequest,
        string createUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertInFirstImagesInternalAsync(createRequest, createUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesInternalAsync(
        ServiceProductFirstImageCreateRequest createRequest, string createUserName)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.InsertInFirstImagesAsync(createRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                createRequest.ProductId, ProductNewStatus.WorkInProgress, createUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData)
    {
        return await _productImageAndFileService.UpdateHtmlDataInAllImagesByIdAsync(imageId, htmlData);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData)
    {
        return await _productImageAndFileService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, htmlData);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData)
    {
        return await _productImageAndFileService.UpdateHtmlDataInFirstImagesByProductIdAsync(productId, htmlData);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesAsync(
        ServiceProductImageUpdateRequest updateRequest,
        string updateUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpdateInAllImagesInternalAsync(updateRequest, updateUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesInternalAsync(
        ServiceProductImageUpdateRequest updateRequest, string updateUserName)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.UpdateInAllImagesAsync(updateRequest);

        if (!result.IsT0) return result;

        ProductImageData? updatedImage = await GetByIdInAllImagesWithoutFileDataAsync(updateRequest.Id);

        if (updatedImage?.ProductId is null) return new UnexpectedFailureResult();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                updatedImage.ProductId!.Value, ProductNewStatus.WorkInProgress, updateUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInAllImagesWithFileAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest)
    {
        using TransactionScope replicationDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _productImageAndFileService.UpdateInAllImagesWithFileAsync(productImageWithFileUpdateRequest);

        if (!result.IsT0) return result;

        replicationDBWorkTransactionScope.Complete();

        return result;
    }

    //public Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInAllImagesWithFileAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest)
    //{
    //    return _productImageAndFileService.UpdateInAllImagesWithFileAsync(productImageWithFileUpdateRequest);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesAsync(
        ServiceProductFirstImageUpdateRequest updateRequest,
        string updateUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpdateInFirstImagesInternalAsync(updateRequest, updateUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesInternalAsync(
        ServiceProductFirstImageUpdateRequest updateRequest, string updateUserName)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.UpdateInFirstImagesAsync(updateRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                updateRequest.ProductId, ProductNewStatus.WorkInProgress, updateUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesAsync(
        ProductImageUpsertRequest upsertRequest,
        string upsertUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertInAllImagesInternalAsync(upsertRequest, upsertUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesInternalAsync(
        ProductImageUpsertRequest upsertRequest, string upsertUserName)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.UpsertInAllImagesAsync(upsertRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                upsertRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

        return upsertProductStatusResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertInAllImagesWithFileAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest)
    {
        using TransactionScope replicationDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _productImageAndFileService.UpsertInAllImagesWithFileAsync(productImageWithFileUpsertRequest);

        if (!result.IsT0) return result;

        replicationDBWorkTransactionScope.Complete();

        return result;
    }

    //public Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertInAllImagesWithFileAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest)
    //{
    //    return _productImageAndFileService.UpsertInAllImagesWithFileAsync(productImageWithFileUpsertRequest);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesAsync(
        ServiceProductFirstImageUpsertRequest upsertRequest,
        string upsertUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertInFirstImagesInternalAsync(upsertRequest, upsertUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesInternalAsync(ServiceProductFirstImageUpsertRequest upsertRequest, string upsertUserName)
    {
        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = await _productImageAndFileService.UpsertInFirstImagesAsync(upsertRequest);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                upsertRequest.ProductId, ProductNewStatus.WorkInProgress, upsertUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesForProductAsync(
        int productId,
        List<ProductImageForProductUpsertRequest> imageUpsertRequests,
        string upsertUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertFirstAndAllImagesForProductInternalAsync(productId, imageUpsertRequests, upsertUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesForProductInternalAsync(
        int productId,
        List<ProductImageForProductUpsertRequest> imageUpsertRequests,
        string upsertUserName)
    {
        List<ProductImageData> currentProductImages = await _productImageAndFileService.GetAllInProductWithoutFileDataAsync(productId);

        List<PromotionProductFileInfo> relatedPromotionProductFileInfos = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

        foreach (ProductImageData image in currentProductImages)
        {
            ProductImageForProductUpsertRequest? requestWithSameId
                = imageUpsertRequests.FirstOrDefault(x => x.ExistingImageId == image.Id);

            if (requestWithSameId is not null) continue;
            
            IEnumerable<PromotionProductFileInfo> promotionProductFilesWithImage = relatedPromotionProductFileInfos
                .Where(x => x.ProductImageId == image.Id);

            foreach (PromotionProductFileInfo? promotionProductFile in promotionProductFilesWithImage)
            {
                bool deleteResult = await _promotionProductFileInfoService.DeleteAsync(promotionProductFile.Id);

                if (!deleteResult) return new UnexpectedFailureResult();
            }

            //ChangeRelatedPromotionFileInfosToNewImageId(image.Id, null, productId, upsertUserName);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result
            = await _productImageAndFileService.UpsertFirstAndAllImagesForProductAsync(productId, imageUpsertRequests);

        if (!result.IsT0) return result;

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, upsertUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            statusId => result.AsT0,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesWithFilesForProductAsync(
        int productId,
        List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
        string deleteUserName)
    {
        //return _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpsertFirstAndAllImagesWithFilesForProductInternalAsync(productId, imageAndFileNameUpsertRequests, deleteUserName),
        //    result => result.IsT0);

        return UpsertFirstAndAllImagesWithFilesForProductInternalAsync(productId, imageAndFileNameUpsertRequests, deleteUserName);
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesWithFilesForProductInternalAsync(
        int productId,
        List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
        string deleteUserName)
    {
        List<ProductImageData> currentProductImages = await _productImageAndFileService.GetAllInProductWithoutFileDataAsync(productId);

        List<PromotionProductFileInfo> relatedPromotionProductFileInfos = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        foreach (ProductImageData image in currentProductImages)
        {
            ProductImageWithFileForProductUpsertRequest? requestWithSameId
                = imageAndFileNameUpsertRequests.FirstOrDefault(x => x.ExistingImageId == image.Id);

            if (requestWithSameId is not null) continue;

            IEnumerable<PromotionProductFileInfo> promotionProductFilesWithImage = relatedPromotionProductFileInfos
               .Where(x => x.ProductImageId == image.Id);

            foreach (PromotionProductFileInfo? promotionProductFile in promotionProductFilesWithImage)
            {
                bool deleteResult = await _promotionProductFileInfoService.DeleteAsync(promotionProductFile.Id);

                if (!deleteResult) return new UnexpectedFailureResult();
            }

            //ChangeRelatedPromotionFileInfosToNewImageId(image.Id, null, productId, upsertUserName);
        }

        using TransactionScope replicationDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _productImageAndFileService.UpsertFirstAndAllImagesWithFilesForProductAsync(
            productId, imageAndFileNameUpsertRequests, deleteUserName);

        if (!result.IsT0) return result;

        replicationDBWorkTransactionScope.Complete();

        localDBTransactionScope.Complete();

        return result;
    }

    //private async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesWithFilesForProductInternalAsync(
    //    int productId,
    //    List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
    //    string deleteUserName)
    //{
    //    List<ProductImageData> currentProductImages = await _productImageAndFileService.GetAllInProductWithoutFileDataAsync(productId);

    //    List<PromotionProductFileInfo> relatedPromotionProductFileInfos = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

    //    foreach (ProductImageData image in currentProductImages)
    //    {
    //        ProductImageWithFileForProductUpsertRequest? requestWithSameId
    //            = imageAndFileNameUpsertRequests.FirstOrDefault(x => x.ExistingImageId == image.Id);

    //        if (requestWithSameId is not null) continue;

    //        IEnumerable<PromotionProductFileInfo> promotionProductFilesWithImage = relatedPromotionProductFileInfos
    //           .Where(x => x.ProductImageId == image.Id);

    //        foreach (PromotionProductFileInfo? promotionProductFile in promotionProductFilesWithImage)
    //        {
    //            bool deleteResult = await _promotionProductFileInfoService.DeleteAsync(promotionProductFile.Id);

    //            if (!deleteResult) return new UnexpectedFailureResult();
    //        }

    //        //ChangeRelatedPromotionFileInfosToNewImageId(image.Id, null, productId, upsertUserName);
    //    }

    //    return await _productImageAndFileService.UpsertFirstAndAllImagesWithFilesForProductAsync(
    //        productId, imageAndFileNameUpsertRequests, deleteUserName);
    //}

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllImagesForProductAsync(
        int productId, string deleteUserName)
    {
        if (productId <= 0) return new NotFound();

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteAllImagesForProductInternalAsync(productId, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllImagesForProductInternalAsync(
        int productId, string deleteUserName)
    {
        bool result = await _productImageAndFileService.DeleteAllImagesForProductAsync(productId);

        if (!result) return new NotFound();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, deleteUserName);

        return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteInAllImagesByIdAsync(
        int id, string deleteUserName)
    {
        if (id <= 0) return new NotFound();

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteInAllImagesByIdInternalAsync(id, deleteUserName),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteInAllImagesByIdInternalAsync(
        int id, string deleteUserName)
    {
        ProductImageData? image = await GetByIdInAllImagesWithoutFileDataAsync(id);

        if (image is null) return new NotFound();

        if (image.ProductId is null) return new UnexpectedFailureResult();

        bool isImageDeleted = await _productImageAndFileService.DeleteInAllImagesByIdAsync(id);

        if (!isImageDeleted) return new NotFound();

        OneOf<Success, NotFound, UnexpectedFailureResult> deleteRelatedPromotionProductFiles
            = await DeleteRelatedPromotionFileInfosAsync(image.ProductId.Value, id);

        if (!deleteRelatedPromotionProductFiles.IsT0 && !deleteRelatedPromotionProductFiles.IsT1)
        {
            return deleteRelatedPromotionProductFiles.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
                success => success,
                notFound => new Success(),
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                image.ProductId.Value, ProductNewStatus.WorkInProgress, deleteUserName);

        return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);

        //OneOf<Success, NotFound, ValidationResult> updatePromotionProductFilesResult
        //    = ChangeRelatedPromotionFileInfosToNewImageId(id, null, image.ProductId.Value, deleteUserName);

        //return updatePromotionProductFilesResult.Map<Success, NotFound, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileAsync(
        int id, string deleteUserName)
    {
        if (id <= 0) return new NotFound();

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteInAllImagesByIdWithFileInternalAsync(id, deleteUserName),
        //    result => result.IsT0);

        return await DeleteInAllImagesByIdWithFileInternalAsync(id, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileInternalAsync(
        int id, string deleteUserName)
    {
        ProductImageData? image = await GetByIdInAllImagesWithoutFileDataAsync(id);

        if (image is null) return new NotFound();

        if (image.ProductId is null) return new UnexpectedFailureResult();

        using TransactionScope replicatedDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult> deleteImageWithFileResult
            = await _productImageAndFileService.DeleteInAllImagesByIdWithFileAsync(id, deleteUserName);

        if (!deleteImageWithFileResult.IsT0)
        {
            return deleteImageWithFileResult.Map<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>();
        }

        using TransactionScope localDBWorkTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, UnexpectedFailureResult> deleteRelatedPromotionProductFilesResult = await DeleteRelatedPromotionFileInfosAsync(image.ProductId.Value, id);

        OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult> deleteRelatedPromotionProductFilesResultMapped
            = deleteRelatedPromotionProductFilesResult.Match<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                success => success,
                notFound => new Success(),
                unexpectedFailureResult => unexpectedFailureResult);

        if (!deleteRelatedPromotionProductFilesResultMapped.IsT0) return deleteRelatedPromotionProductFilesResultMapped;

        localDBWorkTransactionScope.Complete();

        replicatedDBWorkTransactionScope.Complete();

        return deleteRelatedPromotionProductFilesResultMapped;

        //OneOf<Success, NotFound, ValidationResult> updatePromotionProductFilesResult
        //    = ChangeRelatedPromotionFileInfosToNewImageId(id, null, image.ProductId.Value, deleteUserName);

        //return updatePromotionProductFilesResult.Map<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteInFirstImagesByProductIdAsync(int productId, string deleteUserName)
    {
        if (productId <= 0) return new NotFound();

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteInFirstImagesByProductIdInternalAsync(productId, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteInFirstImagesByProductIdInternalAsync(int productId, string deleteUserName)
    {
        bool result = await _productImageAndFileService.DeleteInFirstImagesByProductIdAsync(productId);

        if (!result) return new NotFound();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, deleteUserName);

        return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    //private OneOf<Success, NotFound, ValidationResult> ChangeRelatedPromotionFileInfosToNewImageId(
    //    int imageId, int? newImageId, int productId, string deleteUserName)
    //{
    //    IEnumerable<PromotionProductFileInfo> relatedPromotionProductFileInfos = _promotionProductFileInfoService.GetAllForProduct(productId)
    //        .Where(x => x.ProductImageId == imageId);

    //    foreach (PromotionProductFileInfo promotionProductFileInfo in relatedPromotionProductFileInfos)
    //    {
    //        ServicePromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
    //        {
    //            Id = promotionProductFileInfo.Id,
    //            Active = promotionProductFileInfo.Active,
    //            ProductImageId = newImageId,
    //            ValidFrom = promotionProductFileInfo.ValidFrom,
    //            ValidTo = promotionProductFileInfo.ValidTo,
    //            UpdateUserName = deleteUserName,
    //        };

    //        OneOf<Success, NotFound, ValidationResult> updateResult = _promotionProductFileInfoService.Update(promotionProductFileInfoUpdateRequest);

    //        if (!updateResult.IsT0) return updateResult;
    //    }

    //    return new Success();
    //}

    private async Task<OneOf<Success, NotFound, UnexpectedFailureResult>> DeleteRelatedPromotionFileInfosAsync(int productId, int imageId)
    {
        List<PromotionProductFileInfo> promotionProductFilesForProduct = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

        IEnumerable<PromotionProductFileInfo> promotionProductFilesWithImage = promotionProductFilesForProduct
            .Where(x => x.ProductImageId == imageId);

        if (!promotionProductFilesWithImage.Any()) return new NotFound();

        foreach (PromotionProductFileInfo? promotionProductFile in promotionProductFilesWithImage)
        {
            bool deleteResult = await _promotionProductFileInfoService.DeleteAsync(promotionProductFile.Id);

            if (!deleteResult) return new UnexpectedFailureResult();
        }

        return new Success();
    }
}