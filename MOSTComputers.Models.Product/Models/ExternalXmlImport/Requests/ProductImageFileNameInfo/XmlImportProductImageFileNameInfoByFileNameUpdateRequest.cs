using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductImageFileNameInfo;

public sealed class XmlImportProductImageFileNameInfoByFileNameUpdateRequest
{
    public int ProductId { get; set; }
    public required string FileName { get; set; }
    public string? NewFileName { get; set; }
    public bool ShouldUpdateDisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool Active { get; set; }
    public int? ImagesInImagesAllForProductCount { get; set; }
    public bool IsProductFirstImageInImages { get; set; }
}