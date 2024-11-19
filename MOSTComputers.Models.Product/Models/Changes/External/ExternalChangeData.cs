namespace MOSTComputers.Models.Product.Models.Changes.External;

public sealed class ExternalChangeData
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
}