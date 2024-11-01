using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using System.Numerics;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;

namespace MOSTComputers.Services.ProductRegister.Validation;

internal static class CommonElements
{
    private static readonly ValidationResult _successResult = new();

    internal static ValidationResult ValidateDefault<T>(IValidator<T>? validator, T model)
    {
        if (validator is null) return _successResult;

        return validator.Validate(model);
    }

    internal static ValidationResult ValidateTwoValidatorsDefault<T>(T model, IValidator<T>? validator = null, IValidator<T>? otherValidator = null)
    {
        ValidationResult validationResult = ValidateDefault(validator, model);

        if (!validationResult.IsValid) return validationResult;

        ValidationResult validationResultInternal = ValidateDefault(otherValidator, model);

        if (!validationResultInternal.IsValid) return validationResultInternal;

        return _successResult;
    }

    public static bool NullOrGreaterThanZero(int? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(decimal? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(float? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(short? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(long? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(byte? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero<T>(T? num)
        where T : INumber<T>
    {
        if (num == null) return true;
        
        return num.CompareTo(T.Zero) > 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(int? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(decimal? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(short? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(long? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(float? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(byte? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero<T>(T? num)
        where T : INumberBase<T>, IComparable<T>
    {
        return num == null || num.CompareTo(T.Zero) > 0;
    }

    public static bool NullOrGreaterThanOrEqualTo<T>(T? num, T comparisonNum)
        where T : struct, INumberBase<T>, IComparable<T>
    {
        return num == null || num.Value.CompareTo(comparisonNum) > 0;
    }

    public static bool IsNotNullEmptyOrWhiteSpace(string? str)
    {
        if (string.IsNullOrEmpty(str)) return false;

        foreach (char character in str)
        {
            if (!char.IsWhiteSpace(character)) return true;
        }

        return false;
    }

    public static bool IsNotEmptyOrWhiteSpace(string? str)
    {
        if (str is null) return true;

        if (str == string.Empty) return false;

        foreach (char character in str)
        {
            if (!char.IsWhiteSpace(character)) return true;
        }

        return false;
    }

    public static bool IsNotWhiteSpace(string? str)
    {
        if (str is null) return true;

        if (str == string.Empty) return true;

        foreach (char character in str)
        {
            if (char.IsWhiteSpace(character)) return false;
        }

        return true;
    }

    public static bool IsEmpty(string str)
    {
        return !(str == string.Empty);
    }

    public static bool IsNullOrNotEmpty(byte[]? x)
    {
        if (x is null) return true;

        return x.Length != 0;
    }

    internal static bool DoesNotHavePropertiesWithDuplicateCharacteristics(List<CurrentProductPropertyCreateRequest>? propertyCreateRequests)
    {
        if (propertyCreateRequests is null
            || propertyCreateRequests.Count <= 0) return true;

        List<int> productCharacteristicIds = new();

        foreach (CurrentProductPropertyCreateRequest propertyCreateRequest in propertyCreateRequests)
        {
            int? productCharacteristicId = propertyCreateRequest.ProductCharacteristicId;

            if (productCharacteristicId is null) continue;

            if (productCharacteristicIds.Contains(productCharacteristicId.Value))
            {
                return false;
            }

            productCharacteristicIds.Add(productCharacteristicId.Value);
        }

        return true;
    }

    internal static bool DoesNotHavePropertiesWithDuplicateCharacteristics(List<LocalProductPropertyUpsertRequest>? propertyUpsertRequests)
    {
        if (propertyUpsertRequests is null
            || propertyUpsertRequests.Count <= 0) return true;

        List<int> productCharacteristicIds = new();

        foreach (LocalProductPropertyUpsertRequest propertyUpsertRequest in propertyUpsertRequests)
        {
            int? productCharacteristicId = propertyUpsertRequest.ProductCharacteristicId;

            if (productCharacteristicId is null) continue;

            if (productCharacteristicIds.Contains(productCharacteristicId.Value))
            {
                return false;
            }

            productCharacteristicIds.Add(productCharacteristicId.Value);
        }

        return true;
    }

    internal static bool DoesNotHavePropertiesWithDuplicateCharacteristics(List<CurrentProductPropertyUpdateRequest>? propertyCreateRequests)
    {
        if (propertyCreateRequests is null
            || propertyCreateRequests.Count <= 0) return true;

        List<int> productCharacteristicIds = new();

        foreach (CurrentProductPropertyUpdateRequest propertyUpdateRequest in propertyCreateRequests)
        {
            int? productCharacteristicId = propertyUpdateRequest.ProductCharacteristicId;

            if (productCharacteristicId is null) continue;

            if (productCharacteristicIds.Contains(productCharacteristicId.Value))
            {
                return false;
            }

            productCharacteristicIds.Add(productCharacteristicId.Value);
        }

        return true;
    }

    public static bool DoesNotHaveImagesWithTheSameId(List<ImageAndImageFileNameUpsertRequest>? imageAndImageFileNameUpsertRequests)
    {
        if (imageAndImageFileNameUpsertRequests is null
            || imageAndImageFileNameUpsertRequests.Count <= 0) return true;

        List<int> imageIds = new();

        foreach (ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest in imageAndImageFileNameUpsertRequests)
        {
            int? imageId = imageAndImageFileNameUpsertRequest.ProductImageUpsertRequest?.OriginalImageId;

            if (imageId is null) continue;

            if (imageIds.Contains(imageId.Value))
            {
                return false;
            }

            imageIds.Add(imageId.Value);
        }

        return true;
    }

    public static bool DoesNotHaveImageFileUpsertRequestsWithTheSameImageId(List<ImageFileAndFileNameInfoUpsertRequest>? imageFileAndFileNameInfoUpsertRequests)
    {
        if (imageFileAndFileNameInfoUpsertRequests is null
            || imageFileAndFileNameInfoUpsertRequests.Count <= 0) return true;

        List<int> imageIds = new();

        foreach (ImageFileAndFileNameInfoUpsertRequest imageFileAndFileNameInfoUpsertRequest in imageFileAndFileNameInfoUpsertRequests)
        {
            int? imageId = imageFileAndFileNameInfoUpsertRequest?.RelatedImageId;

            if (imageId is null) continue;

            if (imageIds.Contains(imageId.Value))
            {
                return false;
            }

            imageIds.Add(imageId.Value);
        }

        return true;
    }

    public static bool DoesNotHaveImageFileNamesWithTheSameImageNumber(List<ImageAndImageFileNameUpsertRequest>? imageAndImageFileNameUpsertRequests)
    {
        if (imageAndImageFileNameUpsertRequests is null
            || imageAndImageFileNameUpsertRequests.Count <= 0) return true;

        List<int> imageFileNameImageNumbers = new();

        foreach (ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest in imageAndImageFileNameUpsertRequests)
        {
            int? originalImageNumber = imageAndImageFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest?.OriginalImageNumber;

            if (originalImageNumber is null) continue;

            if (imageFileNameImageNumbers.Contains(originalImageNumber.Value))
            {
                return false;
            }

            imageFileNameImageNumbers.Add(originalImageNumber.Value);
        }

        return true;
    }
    
    public static bool DoesNotHaveImageFileUpsertRequestsWithTheSameOldFileName(List<ImageFileAndFileNameInfoUpsertRequest>? imageFileAndFileNameInfoUpsertRequests)
    {
        if (imageFileAndFileNameInfoUpsertRequests is null
            || imageFileAndFileNameInfoUpsertRequests.Count <= 0) return true;

        List<string> imageFileNameImageNumbers = new();

        foreach (ImageFileAndFileNameInfoUpsertRequest imageFileAndFileNameInfoUpsertRequest in imageFileAndFileNameInfoUpsertRequests)
        {
            string? oldFileName = imageFileAndFileNameInfoUpsertRequest?.OldFileName;

            if (oldFileName is null) continue;

            if (imageFileNameImageNumbers.Contains(oldFileName))
            {
                return false;
            }

            imageFileNameImageNumbers.Add(oldFileName);
        }

        return true;
    }

    public static bool DoesNotHaveImageFileNamesWithTheSameDisplayOrder(List<ImageAndImageFileNameUpsertRequest>? imageAndImageFileNameUpsertRequests)
    {
        if (imageAndImageFileNameUpsertRequests is null
            || imageAndImageFileNameUpsertRequests.Count <= 0) return true;

        List<int> imageFileNameDisplayOrders = new();

        foreach (ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest in imageAndImageFileNameUpsertRequests)
        {
            int? displayOrder = imageAndImageFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest?.NewDisplayOrder;

            if (displayOrder is null) continue;

            if (imageFileNameDisplayOrders.Contains(displayOrder.Value))
            {
                return false;
            }

            imageFileNameDisplayOrders.Add(displayOrder.Value);
        }

        return true;
    }

    public static bool DoesNotHaveImageFileUpsertRequestsWithTheSameDisplayOrder(List<ImageFileAndFileNameInfoUpsertRequest>? imageFileAndFileNameInfoUpsertRequests)
    {
        if (imageFileAndFileNameInfoUpsertRequests is null
            || imageFileAndFileNameInfoUpsertRequests.Count <= 0) return true;

        List<int> imageFileNameDisplayOrders = new();

        foreach (ImageFileAndFileNameInfoUpsertRequest imageFileAndFileNameInfoUpsertRequest in imageFileAndFileNameInfoUpsertRequests)
        {
            int? displayOrder = imageFileAndFileNameInfoUpsertRequest?.DisplayOrder;

            if (displayOrder is null) continue;

            if (imageFileNameDisplayOrders.Contains(displayOrder.Value))
            {
                return false;
            }

            imageFileNameDisplayOrders.Add(displayOrder.Value);
        }

        return true;
    }

    public static bool DoesNotHaveImageFileNamesWithOutOfBoundsDisplayOrder(List<ImageAndImageFileNameUpsertRequest>? imageAndImageFileNameUpsertRequests)
    {
        if (imageAndImageFileNameUpsertRequests is null
            || imageAndImageFileNameUpsertRequests.Count <= 0) return true;

        foreach (ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest in imageAndImageFileNameUpsertRequests)
        {
            int? displayOrder = imageAndImageFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest?.NewDisplayOrder;

            if (displayOrder is not null
                && displayOrder > imageAndImageFileNameUpsertRequests.Count) return false;
        }

        return true;
    }

    internal static bool IsContentTypeValidAndSupported(string contentType)
    {
        string? fileExtension = GetImageFileExtensionFromContentType(contentType);

        if (fileExtension is null) return false;

        return true;
    }

    internal static bool FileNameLengthIsLessThanMaxLength(string fileName, int maxLength)
    {
        return fileName.Length < maxLength;
    }

    internal static bool FileNameLengthIsLessThanMaxLength(string? fileNameWithoutExtension, string contentType, int maxLength)
    {
        if (fileNameWithoutExtension is null) return true;

        string? fileExtension = GetImageFileExtensionFromContentType(contentType);

        if (fileExtension is null) return false;

        string fullFileName = $"{fileNameWithoutExtension}.{fileExtension}";

        return fullFileName.Length < maxLength;
    }

    internal static bool IsDisplayOrderValidWhenNeeded(int? displayOrder, bool shouldUpdateDisplayOrder)
    {
        if (shouldUpdateDisplayOrder)
        {
            return NullOrGreaterThanZero(displayOrder);
        }

        return true;
    }

    public static List<int> RemoveValuesSmallerThanNumber(IEnumerable<int> intList, int value)
    {
        List<int> output = new();

        foreach (int item in intList)
        {
            if (item <= value) continue;

            output.Add(item);
        }

        return output;
    }

    public static List<int> RemoveValuesSmallerThanOne(IEnumerable<int> intList)
    {
        return RemoveValuesSmallerThanNumber(intList, 0);
    }
}