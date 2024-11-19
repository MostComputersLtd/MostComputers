namespace MOSTComputers.Models.Product.Models.Changes.Local;

public sealed class LocalChangeData
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
    public DateTime TimeStamp { get; set; }
}