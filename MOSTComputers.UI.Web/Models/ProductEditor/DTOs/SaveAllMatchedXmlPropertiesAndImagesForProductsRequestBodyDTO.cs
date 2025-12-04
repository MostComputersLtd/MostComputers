namespace MOSTComputers.UI.Web.Models.ProductEditor.DTOs;

public sealed class SaveAllMatchedXmlPropertiesAndImagesForProductsRequestBodyDTO
{
    public required IEnumerable<int> ProductIds { get; init; }
    public ProductEditorSearchOptions? SearchOptions { get; init; }
}