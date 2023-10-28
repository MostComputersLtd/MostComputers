using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;

namespace MOSTComputers.Services.DAL.Mapping;

public class ManifacturerEntityMap : EntityMap<Manifacturer>
{
    public ManifacturerEntityMap()
    {
        Map(x => x.Id).ToColumn("MfrID");
        Map(x => x.RealCompanyName).ToColumn("Name");
        Map(x => x.BGName).ToColumn("BGName");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.Active).ToColumn("Active");
    }
}