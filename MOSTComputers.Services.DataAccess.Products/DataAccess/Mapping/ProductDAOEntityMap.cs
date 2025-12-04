using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.Product;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class ProductDAOEntityMap : EntityMap<ProductDAO>
{
    public ProductDAOEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumnName);
        Map(x => x.Name).ToColumn(NameColumnName);
        Map(x => x.AdditionalWarrantyPrice).ToColumn(AdditionalWarrantyPriceColumnName);
        Map(x => x.AdditionalWarrantyTermMonths).ToColumn(AdditionalWarrantyTermMonthsColumnName);
        Map(x => x.StandardWarrantyPrice).ToColumn(StandardWarrantyPriceColumnName);
        Map(x => x.StandardWarrantyTermMonths).ToColumn(StandardWarrantyTermMonthsColumnName);
        Map(x => x.DisplayOrder).ToColumn(DisplayOrderColumnName);
        Map(x => x.Status).ToColumn(StatusColumnName);
        Map(x => x.PlShow).ToColumn(PlShowColumnName);
        Map(x => x.Price).ToColumn(Price2ColumnName);
        Map(x => x.Currency).ToColumn(CurrencyIdColumnName);
        Map(x => x.RowGuid).ToColumn(RowGuidColumnName);
        Map(x => x.PromotionPid).ToColumn(PromotionPidColumnName);
        Map(x => x.PromotionRid).ToColumn(PromotionRidColumnName);
        Map(x => x.PromotionPictureId).ToColumn(PromotionPictureIdColumnName);
        Map(x => x.PromotionExpireDate).ToColumn(PromotionExpireDateColumnName);
        Map(x => x.AlertPictureId).ToColumn(AlertPictureIdColumnName);
        Map(x => x.AlertExpireDate).ToColumn(AlertExpireDateColumnName);
        Map(x => x.PriceListDescription).ToColumn(PriceListDescriptionColumnName);
        Map(x => x.PartNumber1).ToColumn(PartNumber1ColumnName);
        Map(x => x.PartNumber2).ToColumn(PartNumber2ColumnName);
        Map(x => x.SearchString).ToColumn(SearchStringColumnName);
        Map(x => x.CategoryId).ToColumn(CategoryIdColumnName);
        Map(x => x.ManufacturerId).ToColumn(ManufacturerIdColumnName);
        Map(x => x.SubCategoryId).ToColumn(SubcategoryIdColumnName);
    }
}