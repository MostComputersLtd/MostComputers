using MOSTComputers.Models.Product.Models.Changes;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
public sealed class ServiceToDoLocalChangeCreateRequest
{
    public required string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
}