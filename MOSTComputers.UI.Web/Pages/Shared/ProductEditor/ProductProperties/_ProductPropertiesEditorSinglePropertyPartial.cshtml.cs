using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;
public class ProductPropertiesEditorSinglePropertyPartialModel
{
    public required int ProductId { get; init; }
    public required ProductPropertyEditorPropertyData PropertyData { get; init; }
    public required int PropertyIndex { get; init; }
    public required ProductPropertyEditorPropertyGroup PropertyEditorPropertyGroup { get; init; }
    
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
    public string? NotificationBoxElementId { get; init; }
}