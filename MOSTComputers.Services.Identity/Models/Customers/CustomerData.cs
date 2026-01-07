namespace MOSTComputers.Services.Identity.Models.Customers;

public sealed class CustomerData
{
    public required int Id { get; init; }
    public string? Name { get; init; }
    public string? Username { get; init; }
    public string? ContactPersonName { get; init; }
    public string? Country { get; init; }
    public string? Address { get; init; }
    public int? EmployeeId { get; init; }
}