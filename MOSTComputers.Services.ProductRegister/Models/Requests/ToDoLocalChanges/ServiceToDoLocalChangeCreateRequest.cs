using MOSTComputers.Models.Product.Models.Changes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
public sealed class ServiceToDoLocalChangeCreateRequest
{
    public required string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
}