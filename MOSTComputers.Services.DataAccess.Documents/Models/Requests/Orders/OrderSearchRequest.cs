namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;

public sealed class OrderSearchRequest
{
    public string? SearchByNameString { get; set; }
    public int? Status { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? SearchStartDateTime { get; set; }
}
