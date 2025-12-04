using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductImages;
public class ImageFilesDisplayPartialModel
{
    public required ModalData ModalData { get; set; }
    public string? NotificationBoxId { get; set; }

    public required int ProductId { get; init; }
    public string? ProductName { get; init; }
    public List<ProductImageFileData>? ImageFileNames { get; init; }
}