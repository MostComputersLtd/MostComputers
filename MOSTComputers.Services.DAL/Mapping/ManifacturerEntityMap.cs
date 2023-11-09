using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

public sealed class ManifacturerEntityMap : EntityMap<Manifacturer>
{
    public ManifacturerEntityMap()
    {
        Map(x => x.Id).ToColumn("PersonalManifacturerId");
        Map(x => x.RealCompanyName).ToColumn("Name");
        Map(x => x.BGName).ToColumn("BGName");
        Map(x => x.DisplayOrder).ToColumn("ManifacturerDisplayOrder");
        Map(x => x.Active).ToColumn("Active");
    }
}