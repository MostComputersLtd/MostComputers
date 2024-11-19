namespace MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;

public sealed class FailedPropertyNameOfProductMultiCreateRequest
{
    public int ProductId { get; set; }
    public required HashSet<string> PropertyNames { get; init; }
}