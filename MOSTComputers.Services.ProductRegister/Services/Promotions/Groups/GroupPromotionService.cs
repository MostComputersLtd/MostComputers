using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Validation;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Utils.Files.FileExtensionUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints;
using System.Transactions;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;

public sealed class GroupPromotionService : IGroupPromotionService
{
    private readonly IGroupPromotionContentsRepository _groupPromotionContentsRepository;
    private readonly IGroupPromotionImageCrudService _groupPromotionImageCrudService;
    private readonly IGroupPromotionImageFileService _groupPromotionImageFileService;
    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IValidator<ServiceGroupPromotionContentCreateRequest> _createRequestValidator;
    private readonly IValidator<ServiceGroupPromotionContentUpdateRequest> _updateRequestValidator;

    public GroupPromotionService(
        IGroupPromotionContentsRepository groupPromotionContentsRepository,
        IGroupPromotionImageCrudService groupPromotionImageCrudService,
        IGroupPromotionImageFileService groupPromotionImageFileService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ServiceGroupPromotionContentCreateRequest> createRequestValidator,
        IValidator<ServiceGroupPromotionContentUpdateRequest> updateRequestValidator)
    {
        _groupPromotionImageCrudService = groupPromotionImageCrudService;
        _groupPromotionImageFileService = groupPromotionImageFileService;
        _transactionExecuteService = transactionExecuteService;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _groupPromotionContentsRepository = groupPromotionContentsRepository;
    }

    public Task<List<GroupPromotionContent>> GetAllActiveAsync()
    {
        return _groupPromotionContentsRepository.GetAllActiveAsync();
    }

    public Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId)
    {
        return _groupPromotionContentsRepository.GetAllActiveInGroupAsync(groupId);
    }

    public Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds)
    {
        return _groupPromotionContentsRepository.GetAllActiveInGroupsAsync(groupIds);
    }

    public Task<List<GroupPromotionContent>> GetAllAsync()
    {
        return _groupPromotionContentsRepository.GetAllAsync();
    }

    public Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId)
    {
        return _groupPromotionContentsRepository.GetAllInGroupAsync(groupId);
    }

    public Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds)
    {
        return _groupPromotionContentsRepository.GetAllInGroupsAsync(groupIds);
    }

    public Task<GroupPromotionContent?> GetByIdAsync(int id)
    {
        return _groupPromotionContentsRepository.GetByIdAsync(id);
    }

    public async Task<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(
        ServiceGroupPromotionContentCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            InsertInternalAsync,
            result => result.IsT0,
            createRequest);
    }

    private async Task<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertInternalAsync(
        ServiceGroupPromotionContentCreateRequest createRequest)
    {
        GroupPromotionContentCreateRequest innerCreateRequest = new()
        {
            Name = createRequest.Name,
            GroupId = createRequest.GroupId,
            HtmlContent = createRequest.HtmlContent ?? string.Empty,
            StartDate = createRequest.StartDate ?? DateTime.MinValue,
            ExpirationDate = createRequest.ExpirationDate ?? DateTime.MaxValue,
            DisplayOrder = createRequest.DisplayOrder ?? 0,
            DateModified = DateTime.Now,
            Disabled = createRequest.Disabled ?? false,
            Restricted = createRequest.Restricted ?? false,
            MemberOfDefaultGroup = createRequest.MemberOfDefaultGroup ?? false,
            DefaultGroupPriority = createRequest.DefaultGroupPriority ?? 0,
        };

        OneOf<int, UnexpectedFailureResult> result = await _groupPromotionContentsRepository.InsertAsync(innerCreateRequest);

        int promotionId = result.AsT0;

        GroupPromotionCreateResult output = new()
        {
            Id = promotionId,
        };

        if (!result.IsT0 || createRequest.PromotionImageCreateRequests is null)
        {
            return result.Match<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>>(
                x => output,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        output.ImageIds = new();
        output.ImageFileIds = new();

        List<ServiceGroupPromotionImageCreateRequest> promotionImageCreateRequests = createRequest.PromotionImageCreateRequests;

        foreach (ServiceGroupPromotionImageCreateRequest promotionImageCreateRequest in promotionImageCreateRequests)
        {
            OneOf<Tuple<int, int>, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> imageInsertResult
                = await InsertGroupPromotionImageAsync(
                    promotionId,
                    promotionImageCreateRequest.Image,
                    promotionImageCreateRequest.ContentType,
                    promotionImageCreateRequest.FileExtension,
                    promotionImageCreateRequest.CustomFileNameWithoutExtension);

            if (!imageInsertResult.IsT0)
            {
                return imageInsertResult.Match<OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>>(
                    success => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                    validationResult => validationResult,
                    imageFileAlreadyExistsResult => imageFileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            Tuple<int, int> imageAndFileIds = imageInsertResult.AsT0;

            output.ImageIds.Add(imageAndFileIds.Item1);
            output.ImageFileIds.Add(imageAndFileIds.Item2);
        }

        return output;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>> UpdateAsync(
        ServiceGroupPromotionContentUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            UpdateInternalAsync,
            result => result.IsT0,
            updateRequest);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>> UpdateInternalAsync(
        ServiceGroupPromotionContentUpdateRequest updateRequest)
    {
        GroupPromotionContentUpdateRequest innerUpdateRequest = new()
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            GroupId = updateRequest.GroupId,
            HtmlContent = updateRequest.HtmlContent,
            StartDate = updateRequest.StartDate,
            ExpirationDate = updateRequest.ExpirationDate,
            DisplayOrder = updateRequest.DisplayOrder,
            DateModified = DateTime.Now,
            Disabled = updateRequest.Disabled,
            Restricted = updateRequest.Restricted,
            MemberOfDefaultGroup = updateRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = updateRequest.DefaultGroupPriority,
        };

        OneOf<Success, NotFound> result = await _groupPromotionContentsRepository.UpdateAsync(innerUpdateRequest);

        if (!result.IsT0)
        {
            return result.Map<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>();
        }

        int promotionId = updateRequest.Id;
        List<ServiceGroupPromotionImageUpsertRequest>? imageRequests = updateRequest.ImageRequests;

        List<GroupPromotionImage> existingImages = await _groupPromotionImageCrudService.GetAllInPromotionAsync(promotionId);
        List<GroupPromotionImageFileData> existingImageFiles = await _groupPromotionImageFileService.GetAllInPromotionAsync(promotionId);

        for (int i = 0; i < existingImages.Count; i++)
        {
            GroupPromotionImage groupPromotionImage = existingImages[i];

            ServiceGroupPromotionImageUpsertRequest? existingRequest
                = imageRequests?.Find(x => x.ExistingImageId == groupPromotionImage.Id);

            if (existingRequest is not null) continue;

            OneOf<Success, NotFound> imageDeleteResult = await _groupPromotionImageCrudService.DeleteAsync(groupPromotionImage.Id);

            if (!imageDeleteResult.IsT0)
            {
                return imageDeleteResult.Map<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>();
            }

            existingImages.RemoveAt(i);

            i--;
        }

        for (int i = 0; i < existingImageFiles.Count; i++)
        {
            GroupPromotionImageFileData groupPromotionImageFile = existingImageFiles[i];

            ServiceGroupPromotionImageUpsertRequest? existingRequest
                = imageRequests?.Find(x => x.ExistingImageId == groupPromotionImageFile.ImageId);

            if (existingRequest is not null) continue;

            OneOf<Success, NotFound, FileDoesntExistResult> imageFileDeleteResult
                = await _groupPromotionImageFileService.DeleteAsync(groupPromotionImageFile.Id);

            if (!imageFileDeleteResult.IsT0)
            {
                return imageFileDeleteResult.Map<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>();
            }

            existingImageFiles.RemoveAt(i);

            i--;
        }

        if (imageRequests is null
            || imageRequests.Count == 0)
        {
            return new Success();
        }

        foreach (ServiceGroupPromotionImageUpsertRequest imageUpsertRequest in imageRequests)
        {
            GroupPromotionImage? existingImage = existingImages.Find(x => x.Id == imageUpsertRequest.ExistingImageId);
            GroupPromotionImageFileData? existingImageFile = existingImageFiles.Find(x => x.ImageId == imageUpsertRequest.ExistingImageId);

            if (existingImage is null)
            {
                OneOf<Tuple<int, int>, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> imageCreateResult
                    = await InsertGroupPromotionImageAsync(
                        promotionId,
                        imageUpsertRequest.Image,
                        imageUpsertRequest.ContentType,
                        imageUpsertRequest.FileExtension,
                        imageUpsertRequest.CustomFileNameWithoutExtension);

                if (!imageCreateResult.IsT0)
                {
                    return imageCreateResult.Match<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                        success => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                        validationResult => validationResult,
                        imageFileAlreadyExistsResult => imageFileAlreadyExistsResult,
                        unexpectedFailureResult => unexpectedFailureResult);
                }

                continue;
            }

            string contentType = imageUpsertRequest.ContentType;

            int contentTypeSlashIndex = contentType.IndexOf('/');

            string contentTypeWithDotForExtension = contentType.Insert(contentTypeSlashIndex + 1, ".");

            GroupPromotionImageUpdateRequest imageUpdateRequest = new()
            {
                Id = imageUpsertRequest.ExistingImageId!.Value,
                PromotionId = promotionId,
                Image = imageUpsertRequest.Image,
                ContentType = contentTypeWithDotForExtension,
            };

            OneOf<Success, NotFound, ValidationResult> imageUpdateResult
                = await _groupPromotionImageCrudService.UpdateAsync(imageUpdateRequest);

            if (!imageUpdateResult.IsT0)
            {
                return imageUpdateResult.Match<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                    imageId => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            string fileName;

            string formattedFileExtension = GetExtensionWithDotFromExtensionOrFileName(imageUpsertRequest.FileExtension)!;

            formattedFileExtension = formattedFileExtension.ToLower();

            if (imageUpsertRequest.CustomFileNameWithoutExtension == null)
            {

                fileName = imageUpsertRequest.ExistingImageId.Value.ToString() + formattedFileExtension;
            }
            else
            {
                fileName = imageUpsertRequest.CustomFileNameWithoutExtension + formattedFileExtension;
            }

            if (fileName.Length > GroupPromotionImageFileDataConstraints.FileNameMaxLength)
            {
                ValidationFailure fileNameTooLongError = new(
                    nameof(ServiceGroupPromotionImageUpsertRequest.FileExtension),
                    "File name is too long");

                return new ValidationResult([fileNameTooLongError]);
            }

            if (existingImageFile is null)
            {
                GroupPromotionImageFileDataCreateRequest imageFileCreateRequest = new()
                {
                    PromotionId = promotionId,
                    FileName = fileName,
                    ImageId = imageUpsertRequest.ExistingImageId!.Value,
                };

                OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> imageFileCreateResult
                    = await _groupPromotionImageFileService.InsertAsync(imageFileCreateRequest);

                if (!imageFileCreateResult.IsT0)
                {
                    return imageFileCreateResult.Match<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                        imageFileId => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                        validationResult => validationResult,
                        imageFileAlreadyExistsResult => imageFileAlreadyExistsResult,
                        unexpectedFailureResult => unexpectedFailureResult);
                }

                continue;
            }

            GroupPromotionImageFileUpdateRequest changeFileRequest = new()
            {
                Id = existingImageFile.Id,
                FileName = fileName,
                Image = imageUpsertRequest.Image,
            };

            OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult> changeFileResult
                = await _groupPromotionImageFileService.ChangeFileAsync(changeFileRequest);

            if (!changeFileResult.IsT0)
            {
                return changeFileResult.Match<OneOf<Success, NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                    imageFileId => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                    notFound => notFound,
                    validationResult => validationResult,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult =>
                    {
                        return new ImageFileAlreadyExistsResult()
                        {
                            ExistingImageId = imageUpsertRequest.ExistingImageId!.Value,
                        };
                    });
            }
        }

        return new Success();
    }

    private async Task<OneOf<Tuple<int, int>, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertGroupPromotionImageAsync(
        int promotionId,
        byte[] image,
        string contentType,
        string fileExtension,
        string? customFileNameWithoutExtension)
    {
        int contentTypeSlashIndex = contentType.IndexOf('/');

        string contentTypeWithDotForExtension = contentType.Insert(contentTypeSlashIndex + 1, ".");

        GroupPromotionImageCreateRequest imageCreateRequest = new()
        {
            PromotionId = promotionId,
            Image = image,
            ContentType = contentTypeWithDotForExtension,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
            = await _groupPromotionImageCrudService.InsertAsync(imageCreateRequest);

        if (!imageInsertResult.IsT0)
        {
            return imageInsertResult.Match<OneOf<Tuple<int, int>, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>>(
                imageId => throw new InvalidOperationException("Cannot be here because of the preceding if statement"),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        int imageId = imageInsertResult.AsT0;

        string fileName;

        string formattedFileExtension = GetExtensionWithDotFromExtensionOrFileName(fileExtension)!;

        formattedFileExtension = formattedFileExtension.ToLower();

        if (customFileNameWithoutExtension == null)
        {
            fileName = imageId.ToString() + fileExtension;
        }
        else
        {
            fileName = customFileNameWithoutExtension + formattedFileExtension;
        }

        if (fileName.Length > GroupPromotionImageFileDataConstraints.FileNameMaxLength)
        {
            ValidationFailure fileNameTooLongError = new(
                nameof(ServiceGroupPromotionImageUpsertRequest.FileExtension),
                "File name is too long");

            return new ValidationResult([fileNameTooLongError]);
        }

        GroupPromotionImageFileDataCreateRequest imageFileCreateRequest = new()
        {
            PromotionId = promotionId,
            FileName = fileName,
            ImageId = imageId,
        };

        OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> imageFileCreateResult
            = await _groupPromotionImageFileService.InsertAsync(imageFileCreateRequest);

        return imageFileCreateResult.Match<OneOf<Tuple<int, int>, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult>>(
            imageFileId => new Tuple<int, int>(imageId, imageFileId),
            validationResult => validationResult,
            imageFileAlreadyExistsResult => imageFileAlreadyExistsResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }
}