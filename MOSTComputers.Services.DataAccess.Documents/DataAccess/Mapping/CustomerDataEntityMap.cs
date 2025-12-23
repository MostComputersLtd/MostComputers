using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.CustomerDataView;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;
internal sealed class CustomerDataEntityMap : EntityMap<CustomerData>
{
    public CustomerDataEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.Name).ToColumn(NameColumn);
        Map(x => x.ContactPersonName).ToColumn(ContactPersonNameColumn);
        Map(x => x.Country).ToColumn(CountryColumn);
        Map(x => x.Address).ToColumn(AddressColumn);
        Map(x => x.EmployeeId).ToColumn(EmployeeIdColumn);
    }
}