namespace MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;

public sealed class FailedPropertyNameOfProductCreateRequest
{
    public int ProductId { get; set; }
    public required string PropertyName { get; set; }
}