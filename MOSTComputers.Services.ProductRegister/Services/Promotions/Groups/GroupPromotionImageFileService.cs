using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.IO;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;
internal sealed class GroupPromotionImageFileService : IGroupPromotionImageFileService
{
    public GroupPromotionImageFileService(
        IGroupPromotionImageFileDataService groupPromotionImageFileDataService,
        IGroupPromotionFileManagementService groupPromotionFileManagementService,
        IGroupPromotionImagesRepository groupPromotionImagesRepository,
        ITransactionExecuteService transactionExecuteService)
    {
        _groupPromotionImageFileDataService = groupPromotionImageFileDataService;
        _groupPromotionFileManagementService = groupPromotionFileManagementService;
        _groupPromotionImagesRepository = groupPromotionImagesRepository;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IGroupPromotionImageFileDataService _groupPromotionImageFileDataService;
    private readonly IGroupPromotionFileManagementService _groupPromotionFileManagementService;
    private readonly IGroupPromotionImagesRepository _groupPromotionImagesRepository;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public async Task<List<GroupPromotionImageFileData>> GetAllAsync()
    {
        return await _groupPromotionImageFileDataService.GetAllAsync();
    }

    public async Task<List<GroupPromotionImageFileData>> GetAllInPromotionAsync(int promotionId)
    {
        return await _groupPromotionImageFileDataService.GetAllInPromotionAsync(promotionId);
    }

    public async Task<List<GroupPromotionImageFile>> GetAllInPromotionWithFilesAsync(int promotionId)
    {
        List<GroupPromotionImageFileData> imageFileDatas = await _groupPromotionImageFileDataService.GetAllInPromotionAsync(promotionId);

        List<GroupPromotionImageFile> output = new();

        foreach (GroupPromotionImageFileData imageFileData in imageFileDatas)
        {
            Stream? fileStream = _groupPromotionFileManagementService.GetFileStream(imageFileData.FileName);

            GroupPromotionImageFile imageFile = new()
            {
                Id = imageFileData.Id,
                PromotionId = imageFileData.PromotionId,
                ImageId = imageFileData.ImageId,
                FileName = imageFileData.FileName,
                FileDataStream = fileStream,
            };

            output.Add(imageFile);
        }

        return output;
    }

    public async Task<GroupPromotionImageFileData?> GetByIdAsync(int id)
    {
        return await _groupPromotionImageFileDataService.GetByIdAsync(id);
    }

    public async Task<GroupPromotionImageFile?> GetByIdWithFileAsync(int id)
    {
        GroupPromotionImageFileData? imageFile = await _groupPromotionImageFileDataService.GetByIdAsync(id);

        if (imageFile is null) return null;

        Stream? fileStream = _groupPromotionFileManagementService.GetFileStream(imageFile.FileName);

        return new()
        {
            Id = imageFile.Id,
            PromotionId = imageFile.PromotionId,
            ImageId = imageFile.ImageId,
            FileName = imageFile.FileName,
            FileDataStream = fileStream,
        };
    }

    public async Task<GroupPromotionImageFileData?> GetByImageIdAsync(int imageId)
    {
        return await _groupPromotionImageFileDataService.GetByImageIdAsync(imageId);
    }

    public async Task<GroupPromotionImageFile?> GetByImageIdWithFileAsync(int imageId)
    {
        GroupPromotionImageFileData? imageFile = await _groupPromotionImageFileDataService.GetByImageIdAsync(imageId);

        if (imageFile is null) return null;

        Stream? fileStream = _groupPromotionFileManagementService.GetFileStream(imageFile.FileName);

        return new()
        {
            Id = imageFile.Id,
            PromotionId = imageFile.PromotionId,
            ImageId = imageFile.ImageId,
            FileName = imageFile.FileName,
            FileDataStream = fileStream,
        };
    }

    public async Task<OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(
        GroupPromotionImageFileDataCreateRequest createRequest)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertInternalAsync(createRequest),
            result => result.IsT0);
    }

    private async Task<OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertInternalAsync(
        GroupPromotionImageFileDataCreateRequest createRequest)
    {
        OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> result
            = await _groupPromotionImageFileDataService.InsertAsync(createRequest);

        if (!result.IsT0) return result;

        GroupPromotionImage? image = await _groupPromotionImagesRepository.GetByIdAsync(createRequest.ImageId);

        if (image?.Image is null) return new UnexpectedFailureResult();

        OneOf<Success, FileAlreadyExistsResult> imageFileCreateResult = await _groupPromotionFileManagementService.AddFileAsync(createRequest.FileName, image.Image);

        return imageFileCreateResult.Match<OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>>(
            success => result.AsT0,
            fileAlreadyExists => new ImageFileAlreadyExistsResult { ExistingImageId = image.Id });
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult>> UpdateAsync(
        GroupPromotionImageFileDataUpdateRequest updateRequest)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpdateInternalAsync(updateRequest),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult>> UpdateInternalAsync(
        GroupPromotionImageFileDataUpdateRequest updateRequest)
    {
        GroupPromotionImageFileData? existingImageFileData = await _groupPromotionImageFileDataService.GetByIdAsync(updateRequest.Id);

        if (existingImageFileData is null) return new NotFound();

        if (Path.GetExtension(existingImageFileData.FileName) != Path.GetExtension(updateRequest.NewFileName))
        {
            ValidationFailure validationFailure = new()
            {
                ErrorMessage = "The file extension cannot be changed.",
                PropertyName = nameof(updateRequest.NewFileName),
            };

            return new ValidationResult([validationFailure]);
        }

        OneOf<Success, NotFound, ValidationResult> result = await _groupPromotionImageFileDataService.UpdateAsync(updateRequest);

        if (!result.IsT0)
        {
            return result.Map<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult>();
        }

        OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> renameFileResult
            = _groupPromotionFileManagementService.RenameFile(existingImageFileData.FileName, updateRequest.NewFileName);

        return renameFileResult.Map<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult>();
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteAsync(int id)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteInternalAsync(id),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteInternalAsync(int id)
    {
        GroupPromotionImageFileData? existingImageFileData = await _groupPromotionImageFileDataService.GetByIdAsync(id);

        if (existingImageFileData is null) return new NotFound();

        bool deleteResult = await _groupPromotionImageFileDataService.DeleteAsync(id);

        if (!deleteResult) return new NotFound();

        OneOf<Success, FileDoesntExistResult> deleteFile = _groupPromotionFileManagementService.DeleteFile(existingImageFileData.FileName);

        return deleteFile.Map<Success, NotFound, FileDoesntExistResult>();
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteByImageIdAsync(int imageId)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteByImageIdInternalAsync(imageId),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult>> DeleteByImageIdInternalAsync(int imageId)
    {
        GroupPromotionImageFileData? existingImageFileData = await _groupPromotionImageFileDataService.GetByImageIdAsync(imageId);

        if (existingImageFileData is null) return new NotFound();

        bool deleteResult = await _groupPromotionImageFileDataService.DeleteByImageIdAsync(imageId);

        if (!deleteResult) return new NotFound();

        OneOf<Success, FileDoesntExistResult> deleteFile = _groupPromotionFileManagementService.DeleteFile(existingImageFileData.FileName);

        return deleteFile.Map<Success, NotFound, FileDoesntExistResult>();
    }
}