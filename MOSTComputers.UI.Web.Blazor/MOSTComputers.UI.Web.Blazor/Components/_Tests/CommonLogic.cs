using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;
using OneOf;
using SixLabors.ImageSharp;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Components._Tests;

public static class CommonLogic
{
    public sealed class ProductTestMetrics
    {
        private readonly List<int> _processedProductIds = new();
        private readonly List<int> _processedImagesIds = new();
        private readonly List<int> _processedImagesAllIds = new();
        private readonly List<Tuple<int, List<int>>> _processedProductPropertyData = new();
        private readonly List<LegacyHtmlProductProperty> _processedHtmlProperties = new();

        public IReadOnlyList<int> ProcessedProductIds => _processedProductIds;
        public IReadOnlyList<int> ProcessedImagesIds => _processedImagesIds;
        public IReadOnlyList<int> ProcessedImagesAllIds => _processedImagesAllIds;
        public IReadOnlyList<Tuple<int, List<int>>> ProcessedProductPropertyData => _processedProductPropertyData;
        public IReadOnlyList<LegacyHtmlProductProperty> ProcessedHtmlProperties => _processedHtmlProperties;

        public void AddProcessedProductId(int id)
        {
            if (!_processedProductIds.Contains(id))
            {
                _processedProductIds.Add(id);
            }
        }

        public void RemoveProcessedProductId(int id)
        {
            _processedProductIds.Remove(id);
        }

        public void AddProcessedImagesId(int id)
        {
            if (!_processedImagesIds.Contains(id))
            {
                _processedImagesIds.Add(id);
            }
        }

        public void RemoveProcessedImagesId(int id)
        {
            _processedImagesIds.Remove(id);
        }

        public void AddProcessedImagesAllId(int id)
        {
            if (!_processedImagesAllIds.Contains(id))
            {
                _processedImagesAllIds.Add(id);
            }
        }

        public void RemoveProcessedImagesAllId(int id)
        {
            _processedImagesAllIds.Remove(id);
        }

        public void AddProcessedProductPropertyForProduct(int productId, int characteristicId)
        {
            Tuple<int, List<int>>? existingEntry = _processedProductPropertyData
                .FirstOrDefault(x => x.Item1 == productId);

            if (existingEntry is not null
                && !existingEntry.Item2.Contains(characteristicId))
            {
                existingEntry.Item2.Add(characteristicId);

                return;
            }

            _processedProductPropertyData.Add(new(productId, new List<int> { characteristicId }));
        }

        public void RemoveProcessedProductPropertyForProduct(int productId, int characteristicId)
        {
            Tuple<int, List<int>>? existingEntry = _processedProductPropertyData
                .FirstOrDefault(x => x.Item1 == productId);

            if (existingEntry is null) return;

            existingEntry.Item2.Remove(characteristicId);
        }

        public void RemoveAllProcessedProductPropertiesForProduct(int productId)
        {
            int existingEntryIndex = _processedProductPropertyData
                .FindIndex(x => x.Item1 == productId);

            if (existingEntryIndex < 0) return;

            _processedProductPropertyData.RemoveAt(existingEntryIndex);
        }

        public void AddProcessedHtmlProperty(LegacyHtmlProductProperty property)
        {
            if (!_processedHtmlProperties.Contains(property))
            {
                _processedHtmlProperties.Add(property);
            }
        }

        public void RemoveProcessedHtmlProperty(LegacyHtmlProductProperty property)
        {
            _processedHtmlProperties.Remove(property);
        }
    }

    public static OneOf<string, ValidationResult> GetFileExtensionFromImageContentType(string? imageContentType)
    {
        if (!IsImageContentType(imageContentType))
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "Image content type is invalid");

            return new ValidationResult([validationFailure]);
        }

        string? fileName = TEMP__GetImageFileExtensionFromImageData(imageContentType);

        if (fileName is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        string fileExtension = Path.GetExtension(fileName);

        if (fileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        return fileExtension;
    }

    public static string? TEMP__GetImageFileExtensionFromImageData(string imageContentType)
    {
        if (imageContentType == "image/jpeg") return ".jpeg";

        List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(imageContentType);

        string? extensionToUse = possibleFileExtensions.FirstOrDefault();

        return extensionToUse;
    }

    public static bool IsImageDataValid(Stream imageDataStream)
    {
        try
        {
            Image.Load(imageDataStream);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
