namespace MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;

public sealed class FailedPropertyNameOfProductUpdateRequest
{
    public int ProductId { get; set; }
    public required string OldPropertyName { get; set; }
    public required string NewPropertyName { get; set; }
}