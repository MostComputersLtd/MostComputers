namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

public sealed class ServiceGroupPromotionContentUpdateRequest
{
    public required int Id { get; init; }
    public string? Name { get; init; }
	public int? GroupId { get; init; }
	public string? HtmlContent { get; init; }
	public DateTime? StartDate { get; init; }
	public DateTime? ExpirationDate { get; init; }
	public int? DisplayOrder { get; init; }
	public bool? Disabled { get; init; }
	public bool? Restricted { get; init; }
	public bool? MemberOfDefaultGroup { get; init; }
	public int? DefaultGroupPriority { get; init; }

	public List<ServiceGroupPromotionImageUpsertRequest>? ImageRequests { get; init; }
}