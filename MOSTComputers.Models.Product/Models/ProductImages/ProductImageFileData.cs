namespace MOSTComputers.Models.Product.Models.ProductImages;

public sealed class ProductImageFileData
{
    public int ProductId { get; init; }
    public int Id { get; init; }
    public int? ImageId { get; init; }
    public string? FileName { get; init; }
    public int DisplayOrder { get; init; }
    public bool Active { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string CreateUserName { get; init; }
    public DateTime CreateDate { get; init; }
    public string LastUpdateUserName { get; init; }
    public DateTime LastUpdateDate { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}