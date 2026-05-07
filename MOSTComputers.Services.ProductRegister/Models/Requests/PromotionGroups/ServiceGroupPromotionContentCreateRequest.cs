namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

public sealed class ServiceGroupPromotionContentCreateRequest
{
    public string? Name { get; set; }
	public int? GroupId { get; set; }
	public string? HtmlContent { get; set; }
	public DateTime? StartDate { get; set; }
	public DateTime? ExpirationDate { get; set; }
	public int? DisplayOrder { get; set; }
	public bool? Disabled { get; set; }
	public bool? Restricted { get; set; }
	public bool? MemberOfDefaultGroup { get; set; }
	public int? DefaultGroupPriority { get; set; }
	public List<ServiceGroupPromotionImageCreateRequest>? PromotionImageCreateRequests { get; set; }
}
