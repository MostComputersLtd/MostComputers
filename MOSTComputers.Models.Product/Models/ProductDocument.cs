namespace MOSTComputers.Models.Product.Models;
public sealed class ProductDocument
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public required string FileName { get; init; }
    public string? Description { get; init; }
}