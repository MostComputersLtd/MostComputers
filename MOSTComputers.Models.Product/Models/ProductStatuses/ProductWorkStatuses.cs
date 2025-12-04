namespace MOSTComputers.Models.Product.Models.ProductStatuses;

public sealed class ProductWorkStatuses
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public ProductNewStatus ProductNewStatus { get; init; }
    public ProductXmlStatus ProductXmlStatus { get; init; }
    public bool ReadyForImageInsert { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string CreateUserName { get; init; }
    public DateTime CreateDate { get; init; }
    public string LastUpdateUserName { get; init; }
    public DateTime LastUpdateDate { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}