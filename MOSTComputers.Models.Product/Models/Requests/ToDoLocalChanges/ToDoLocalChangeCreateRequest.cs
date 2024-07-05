using MOSTComputers.Models.Product.Models.Changes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Requests.ToDoLocalChanges;
public class ToDoLocalChangeCreateRequest
{
    public string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
    public DateTime TimeStamp { get; set; }
}