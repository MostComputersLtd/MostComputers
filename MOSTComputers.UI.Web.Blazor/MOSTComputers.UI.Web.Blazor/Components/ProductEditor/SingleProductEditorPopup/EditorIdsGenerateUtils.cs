namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;

internal static class EditorIdsGenerateUtils
{
    internal static string GeneratePropertyId(int characteristicId, bool isLink)
    {
        if (isLink)
        {
            return $"link-{characteristicId}";
        }

        return $"property-{characteristicId}";
    }

    internal static string GenerateImageId(int? existingImageId, int? existingFileInfoId, int? existingRelatedPromotionFileId)
    {
        if (existingImageId != null)
        {
            return $"from-image-{existingImageId.Value}";
        }
        else if (existingFileInfoId != null)
        {
            return $"from-image-file-{existingFileInfoId.Value}";
        }
        else if (existingRelatedPromotionFileId != null)
        {
            return $"from-promotion-file-{existingRelatedPromotionFileId.Value}";
        }

        return $"from-random-value-{Guid.NewGuid()}";
    }
    
    internal static string GeneratePromotionFileId(int promotionFileId)
    {
        return $"promotion-file-{promotionFileId}";
    }

    internal static string GenerateDocumentId(int productDocumentId)
    {
        return $"product-document-{productDocumentId}";
    }
}
