using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductEntityMap : EntityMap<Product>
{
    public ProductEntityMap()
    {
        Map(x => x.Id).ToColumn("CSTID");
        Map(x => x.Name).ToColumn("CFGSUBTYPE");
        Map(x => x.AdditionalWarrantyPrice).ToColumn("ADDWRR");
        Map(x => x.AdditionalWarrantyTermMonths).ToColumn("ADDWRRTERM");
        Map(x => x.StandardWarrantyPrice).ToColumn("ADDWRRDEF");
        Map(x => x.StandardWarrantyTermMonths).ToColumn("DEFWRRTERM");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.Status).ToColumn("OLD");
        Map(x => x.PlShow).ToColumn("PLSHOW");
        Map(x => x.Price).ToColumn("PRICE2");
        Map(x => x.Currency).ToColumn("CurrencyId");
        Map(x => x.RowGuid).ToColumn("rowguid");
        Map(x => x.Promotionid).ToColumn("PromPID");
        Map(x => x.PromRid).ToColumn("PromRID");
        Map(x => x.PromotionPictureId).ToColumn("PromPictureID");
        Map(x => x.PromotionExpireDate).ToColumn("PromExpDate");
        Map(x => x.AlertPictureId).ToColumn("AlertPictureID");
        Map(x => x.AlertExpireDate).ToColumn("AlertExpDate");
        Map(x => x.PriceListDescription).ToColumn("PriceListDescription");
        Map(x => x.PartNumber1).ToColumn("SPLMODEL");
        Map(x => x.PartNumber2).ToColumn("SPLMODEL1");
        Map(x => x.SearchString).ToColumn("SPLMODEL2");
        Map(x => x.CategoryID).ToColumn("TID");
        Map(x => x.ManifacturerId).ToColumn("MfrID");
        Map(x => x.SubCategoryId).ToColumn("SubcategoryID");
    }
}