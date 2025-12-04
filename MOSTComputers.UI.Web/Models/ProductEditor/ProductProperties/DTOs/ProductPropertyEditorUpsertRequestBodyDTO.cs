using MOSTComputers.UI.Web.Models.ProductEditor.DTOs;

namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties.DTOs;

public sealed class ProductPropertyEditorUpsertRequestBodyDTO
{
    public List<ProductPropertyEditorPropertyDTO>? ProductPropertyEditorDataList { get; init; }
}