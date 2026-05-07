namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

public sealed class ServiceGroupPromotionImageUpsertRequest
{
	public int? ExistingImageId { get; init; }
	public required byte[] Image { get; init; }
	public required string ContentType { get; init; }
	public required string FileExtension { get; init; }
	public string? CustomFileNameWithoutExtension { get; init; }
}