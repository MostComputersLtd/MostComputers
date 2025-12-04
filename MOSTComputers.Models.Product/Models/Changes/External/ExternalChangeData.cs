namespace MOSTComputers.Models.Product.Models.Changes.External;

public sealed class ExternalChangeData
{
    public int Id { get; init; }
    public string TableName { get; init; }
    public int TableElementId { get; init; }
    public ChangeOperationType OperationType { get; init; }
}