using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class PromotionEntityMap : EntityMap<Promotion>
{
    public PromotionEntityMap()
    {
        Map(x => x.Id).ToColumn("PromotionID");
        Map(x => x.Name).ToColumn("PromotionName");
        Map(x => x.PromotionAddedDate).ToColumn("ChgDate");
        Map(x => x.Source).ToColumn("PromSource");
        Map(x => x.Type).ToColumn("PromType");
        Map(x => x.Status).ToColumn("Status");
        Map(x => x.SPOID).ToColumn("SPOID");
        Map(x => x.DiscountEUR).ToColumn("PromotionEUR");
        Map(x => x.DiscountUSD).ToColumn("PromotionUSD");
        Map(x => x.Active).ToColumn("Active");
        Map(x => x.StartDate).ToColumn("StartDate");
        Map(x => x.ExpirationDate).ToColumn("ExpDate");
        Map(x => x.MinimumQuantity).ToColumn("MinQty");
        Map(x => x.MaximumQuantity).ToColumn("MaxQty");
        Map(x => x.RequiredProductIds).Ignore();
        Map(x => x.RequiredProductIdsString).ToColumn("RequiredCSTIDs");
        Map(x => x.ExpQuantity).ToColumn("ExpQty");
        Map(x => x.SoldQuantity).ToColumn("SoldQty");
        Map(x => x.Consignation).ToColumn("Consignation");
        Map(x => x.Points).ToColumn("Points");
        Map(x => x.TimeStamp).ToColumn("Timestamp");
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.CampaignId).ToColumn("CampaignID");
        Map(x => x.RegistrationId).ToColumn("RegistrationID");
        Map(x => x.PromotionVisualizationId).ToColumn("PromotionVisualizationId");
    }
}