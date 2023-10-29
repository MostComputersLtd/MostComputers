using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Mapping;

internal sealed class ProductEntityMap : EntityMap<Product>
{
    public ProductEntityMap()
    {
        Map(x => x.Id).ToColumn("CSTID");
        Map(x => x.SearchString).ToColumn("CFGSUBTYPE");
        Map(x => x.AddWrr).ToColumn("ADDWRR");
        Map(x => x.AddWrrTerm).ToColumn("ADDWRRTERM");
        Map(x => x.AddWrrDef).ToColumn("ADDWRRDEF");
        Map(x => x.DefWrrTerm).ToColumn("DEFWRRTERM");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.Status).ToColumn("OLD");
        Map(x => x.PlShow).ToColumn("PLSHOW");
        Map(x => x.Price1).ToColumn("PRICE1");
        Map(x => x.Price2).ToColumn("PRICE2");
        Map(x => x.Price3).ToColumn("PRICE3");
        Map(x => x.Currency).ToColumn("CurrencyId");
        Map(x => x.RowGuid).ToColumn("rowguid");
        Map(x => x.PromPid).ToColumn("PromPID");
        Map(x => x.PromRid).ToColumn("PromRID");
        Map(x => x.PromPictureId).ToColumn("PromPictureID");
        Map(x => x.PromExpDate).ToColumn("PromExpDate");
        Map(x => x.AlertPictureId).ToColumn("AlertPictureID");
        Map(x => x.AlertExpDate).ToColumn("AlertExpDate");
        Map(x => x.PriceListDescription).ToColumn("PriceListDescription");
        Map(x => x.SplModel1).ToColumn("SPLMODEL");
        Map(x => x.SplModel2).ToColumn("SPLMODEL1");
        Map(x => x.SplModel3).ToColumn("SPLMODEL2");
        Map(x => x.CategoryID).ToColumn("TID");
        Map(x => x.ManifacturerId).ToColumn("MfrID");
        Map(x => x.SubCategoryId).ToColumn("SubcategoryID");
    }
}