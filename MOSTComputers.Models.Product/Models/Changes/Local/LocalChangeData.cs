using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.Changes.Local;

public sealed class LocalChangeData
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public int TableElementId { get; set; }
    public ChangeOperationTypeEnum OperationType { get; set; }
    public DateTime TimeStamp { get; set; }
}