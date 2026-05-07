public sealed class ServicePromotionGroupUpdateRequest
{
	public required int Id { get; set; }
    public string? Name { get; set; }
	public string Header { get; set; } = string.Empty;
    public PromotionGroupLogo? Logo { get; set; }
	public int? DisplayOrder { get; set; }
	public required bool IsDefault { get; set; }
	public required bool ShowEmptyForLogged { get; set; }
	public required bool ShowEmptyForNonLogged { get; set; }
}