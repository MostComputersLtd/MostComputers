using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoicesTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

internal sealed class InvoiceCustomerDataEntityMap : EntityMap<InvoiceCustomerData>
{
    public InvoiceCustomerDataEntityMap()
    {
        Map(x => x.CustomerName).ToColumn(CustomerNameColumn);
        Map(x => x.CustomerBID).ToColumn(CustomerBIDColumn);
    }
}