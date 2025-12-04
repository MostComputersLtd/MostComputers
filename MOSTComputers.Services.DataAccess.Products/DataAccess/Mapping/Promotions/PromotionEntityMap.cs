using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.Promotions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions;
internal sealed class PromotionEntityMap : EntityMap<Promotion>
{
    public PromotionEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.ProductId).ToColumn(ProductIdColumnName);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.PromotionAddedDate).ToColumn(PromotionAddedDateColumnName);
        Map(x => x.Source).ToColumn(SourceColumnName);
        Map(x => x.Type).ToColumn(TypeColumnName);
        Map(x => x.Status).ToColumn(StatusColumnName);
        Map(x => x.SPOID).ToColumn(SPOIDColumnName);
        Map(x => x.DiscountEUR).ToColumn(DiscountEURColumnName);
        Map(x => x.DiscountUSD).ToColumn(DiscountUSDColumnName);
        Map(x => x.Active).ToColumn(ActiveColumnName);
        Map(x => x.StartDate).ToColumn(StartDateColumnName);
        Map(x => x.ExpirationDate).ToColumn(ExpirationDateColumnName);
        Map(x => x.MinimumQuantity).ToColumn(MinimumQuantityColumnName);
        Map(x => x.MaximumQuantity).ToColumn(MaximumQuantityColumnName);
        Map(x => x.RequiredProductIds).Ignore();
        Map(x => x.QuantityIncrement).ToColumn(QuantityIncrementColumnName);
        Map(x => x.RequiredProductIdsString).ToColumn(RequiredProductIdsColumnName);
        Map(x => x.ExpQuantity).ToColumn(ExpQuantityColumnName);
        Map(x => x.SoldQuantity).ToColumn(SoldQuantityColumnName);
        Map(x => x.Consignation).ToColumn(ConsignationColumnName);
        Map(x => x.Points).ToColumn(PointsColumnName);
        Map(x => x.TimeStamp).ToColumn(TimeStampColumnName);
        Map(x => x.CampaignId).ToColumn(CampaignIdColumnName);
        Map(x => x.RegistrationId).ToColumn(RegistrationIdColumnName);
        Map(x => x.PromotionVisualizationId).ToColumn(PromotionVisualizationIdColumnName);
    }
}