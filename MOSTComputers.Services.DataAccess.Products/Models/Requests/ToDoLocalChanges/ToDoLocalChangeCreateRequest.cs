using MOSTComputers.Models.Product.Models.Changes;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ToDoLocalChanges;
public class ToDoLocalChangeCreateRequest
{
    public required string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationType OperationType { get; set; }
    public DateTime TimeStamp { get; set; }
}