namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

public sealed class PromotionGroupUpdateRequest
{
	public required int Id { get; set; }
    public string? Name { get; set; }
	public string Header { get; set; } = string.Empty;
	public byte[]? LogoImage { get; set; }
	public string? LogoContentType { get; set; }
	public int? DisplayOrder { get; set; }
	public required bool IsDefault { get; set; }
	public required bool ShowEmptyForLogged { get; set; }
	public required bool ShowEmptyForNonLogged { get; set; }
}