namespace MOSTComputers.Services.Identity.Models.Customers;

public sealed class CustomerLoginData
{
    public required int Id { get; init; }
    public string? Username { get; init; }
}