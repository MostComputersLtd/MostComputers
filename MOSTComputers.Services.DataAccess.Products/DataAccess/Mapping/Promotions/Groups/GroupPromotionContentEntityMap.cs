using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions.Groups;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionContentsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
internal sealed class GroupPromotionContentEntityMap : EntityMap<GroupPromotionContent>
{
    public GroupPromotionContentEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.GroupId).ToColumn(GroupIdColumnName);
        Map(x => x.HtmlContent).ToColumn(HtmlContentColumnName);
        Map(x => x.StartDate).ToColumn(StartDateColumnName);
        Map(x => x.ExpirationDate).ToColumn(ExpireDateColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.DateModified).ToColumn(DateModifiedColumnName);
        Map(x => x.Disabled).ToColumn(DisabledColumnName);
        Map(x => x.Restricted).ToColumn(RestrictedColumnName);
        Map(x => x.MemberOfDefaultGroup).ToColumn(MemberOfDefaultGroupColumnName);
        Map(x => x.DefaultGroupPriority).ToColumn(DefaultGroupPriorityColumnName);
    }
}