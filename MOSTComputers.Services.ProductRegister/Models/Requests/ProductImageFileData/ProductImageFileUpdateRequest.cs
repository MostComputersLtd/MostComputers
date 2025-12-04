using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;

public sealed class ProductImageFileUpdateRequest
{
    public required int Id { get; set; }
    public bool? Active { get; set; }
    public int? NewDisplayOrder { get; set; }
    public OneOf<int?, No> UpdateImageIdRequest { get; set; } = new No();
    public ProductImageFileUpdateFileDataRequest? UpdateFileDataRequest { get; set; }
    public required string UpdateUserName { get; init; }
}