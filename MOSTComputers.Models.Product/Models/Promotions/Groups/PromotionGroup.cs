namespace MOSTComputers.Models.Product.Models.Promotions.Groups;
public sealed class PromotionGroup
{
	public int Id { get; init; }
    public string? Name  { get; init; }
	public string? Header { get; init; }
	public byte[]? LogoImage { get; init; }
	public string? LogoContentType { get; init; }
	public int? DisplayOrder { get; init; }
	public bool IsDefault { get; init; }
	public bool ShowEmptyForLogged { get; init; }
	public bool ShowEmptyForNonLogged { get; init; }
}