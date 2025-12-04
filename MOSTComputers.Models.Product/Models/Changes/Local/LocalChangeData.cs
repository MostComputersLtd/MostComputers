namespace MOSTComputers.Models.Product.Models.Changes.Local;

public sealed class LocalChangeData
{
    public int Id { get; init; }
    public string TableName { get; init; }
    public int TableElementId { get; init; }
    public ChangeOperationType OperationType { get; init; }
    public DateTime TimeStamp { get; init; }
}