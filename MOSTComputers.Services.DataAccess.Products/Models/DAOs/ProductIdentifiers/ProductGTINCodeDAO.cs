namespace MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductIdentifiers;
internal sealed class ProductGTINCodeDAO
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public int ProductId { get; init; }
    public int CodeType { get; init; }
    public string CodeTypeAsText { get; init; }
    public string Value { get; init; }

    public string CreateUserName { get; init; }
    public DateTime CreateDate { get; init; }
    public string LastUpdateUserName { get; init; }
    public DateTime LastUpdateDate { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}