using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils;

internal static class ProductSelectListItemUtils
{
    public static IEnumerable<SelectListItem> GetCategorySelectListItems(Product product, IEnumerable<Category> allPossibleCategories)
    {
        List<SelectListItem> selectListItems = allPossibleCategories.Select(
            category => new SelectListItem(category.Description, category.Id.ToString()))
            .ToList();

        string? productCategoryIdString = product.CategoryId.ToString();

        if (product.CategoryId != null)
        {
            foreach (SelectListItem selectListItem in selectListItems)
            {
                if (selectListItem.Value == productCategoryIdString)
                {
                    selectListItem.Selected = true;

                    break;
                }
            }

            return selectListItems;
        }

        return selectListItems.Prepend(new("-- Please select a category --", "-1", true, true));
    }

    public static IEnumerable<SelectListItem> GetCategorySelectListItems(ProductDisplayData productData, IEnumerable<Category> allPossibleCategories)
    {
        List<SelectListItem> selectListItems = allPossibleCategories.Select(
            category => new SelectListItem(category.Description, category.Id.ToString()))
            .ToList();

        string? productCategoryIdString = productData.CategoryId.ToString();

        if (productData.CategoryId != null)
        {
            foreach (SelectListItem selectListItem in selectListItems)
            {
                if (selectListItem.Value == productCategoryIdString)
                {
                    selectListItem.Selected = true;

                    break;
                }
            }

            return selectListItems;
        }

        return selectListItems.Prepend(new("-- Please select a category --", "-1", true, true));
    }

    public static IEnumerable<SelectListItem> GetCurrencySelectListItems(Product product)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("lv.", ((int)CurrencyEnum.BGN).ToString(), product.Currency == CurrencyEnum.BGN),
            new ("€", ((int)CurrencyEnum.EUR).ToString(), product.Currency == CurrencyEnum.EUR),
            new ("$", ((int)CurrencyEnum.USD).ToString(), product.Currency == CurrencyEnum.USD),
        };

        if (product.Currency == null)
        {
            currencySelectListItems.Add(new("-- Please select a currency --", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static IEnumerable<SelectListItem> GetCurrencySelectListItems(ProductDisplayData productData)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("lv.", ((int)CurrencyEnum.BGN).ToString(), productData.Currency == CurrencyEnum.BGN),
            new ("€", ((int)CurrencyEnum.EUR).ToString(), productData.Currency == CurrencyEnum.EUR),
            new ("$", ((int)CurrencyEnum.USD).ToString(), productData.Currency == CurrencyEnum.USD),
        };

        if (productData.Currency == null)
        {
            currencySelectListItems.Add(new("-- Please select a currency --", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static IEnumerable<SelectListItem> GetProductNewStatusSelectListItems(ProductWorkStatuses? productStatuses)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("New", ((int)ProductNewStatusEnum.New).ToString(), productStatuses?.ProductNewStatus == ProductNewStatusEnum.New),
            new ("Work In Progress", ((int)ProductNewStatusEnum.WorkInProgress).ToString(), productStatuses?.ProductNewStatus == ProductNewStatusEnum.WorkInProgress),
            new ("Ready", ((int)ProductNewStatusEnum.ReadyForUse).ToString(), productStatuses?.ProductNewStatus == ProductNewStatusEnum.ReadyForUse),
        };

        if (productStatuses?.ProductNewStatus == null)
        {
            currencySelectListItems.Add(new("-", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static IEnumerable<SelectListItem> GetProductNewStatusSelectListItems(ProductDisplayData productData)
    {
        List<SelectListItem> productNewStatusSelectListItems = new()
        {
            new ("New", ((int)ProductNewStatusEnum.New).ToString(), productData?.ProductNewStatus == ProductNewStatusEnum.New),
            new ("Work In Progress", ((int)ProductNewStatusEnum.WorkInProgress).ToString(), productData?.ProductNewStatus == ProductNewStatusEnum.WorkInProgress),
            new ("Ready", ((int)ProductNewStatusEnum.ReadyForUse).ToString(), productData?.ProductNewStatus == ProductNewStatusEnum.ReadyForUse),
        };

        if (productData?.ProductNewStatus == null)
        {
            productNewStatusSelectListItems.Add(new("-", "-1", true, true));
        }

        return productNewStatusSelectListItems;
    }

    public static IEnumerable<SelectListItem> GetProductXmlStatusSelectListItems(ProductWorkStatuses? productStatuses)
    {
        List<SelectListItem> productXmlStatusSelectListItems = new()
        {
            new ("Not Ready", ((int)ProductXmlStatusEnum.NotReady).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatusEnum.NotReady),
            new ("Work In Progress", ((int)ProductXmlStatusEnum.WorkInProgress).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatusEnum.WorkInProgress),
            new ("Ready", ((int)ProductXmlStatusEnum.ReadyForUse).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatusEnum.ReadyForUse),
        };

        if (productStatuses?.ProductXmlStatus == null)
        {
            productXmlStatusSelectListItems.Add(new("-", "-1", true, true));
        }

        return productXmlStatusSelectListItems;
    }

    public static IEnumerable<SelectListItem> GetProductXmlStatusSelectListItems(ProductDisplayData productData)
    {
        List<SelectListItem> productXmlStatusSelectListItems = new()
        {
            new ("Not Ready", ((int)ProductXmlStatusEnum.NotReady).ToString(), productData?.ProductXmlStatus == ProductXmlStatusEnum.NotReady),
            new ("Work In Progress", ((int)ProductXmlStatusEnum.WorkInProgress).ToString(), productData?.ProductXmlStatus == ProductXmlStatusEnum.WorkInProgress),
            new ("Ready", ((int)ProductXmlStatusEnum.ReadyForUse).ToString(), productData?.ProductXmlStatus == ProductXmlStatusEnum.ReadyForUse),
        };

        if (productData?.ProductXmlStatus == null)
        {
            productXmlStatusSelectListItems.Add(new("-", "-1", true, true));
        }

        return productXmlStatusSelectListItems;
    }

    public static IEnumerable<SelectListItem> GetManifacturerSelectListItems(Product product, IEnumerable<Manifacturer> allPossibleManifacturers)
    {
        List<SelectListItem> selectListItems = allPossibleManifacturers.Select(
            category => new SelectListItem(category.RealCompanyName, category.Id.ToString()))
            .ToList();

        string? productManifacturerIdString = product.ManifacturerId.ToString();

        if (product.ManifacturerId != null)
        {
            foreach (SelectListItem selectListItem in selectListItems)
            {
                if (selectListItem.Value == productManifacturerIdString)
                {
                    selectListItem.Selected = true;

                    break;
                }
            }

            return selectListItems;
        }

        return selectListItems.Prepend(new("-- Please select a manifacturer --", "-1", true, true));
    }

    public static IEnumerable<SelectListItem> GetManifacturerSelectListItems(ProductDisplayData productData,
        IEnumerable<Manifacturer> allPossibleManifacturers)
    {
        List<SelectListItem> selectListItems = allPossibleManifacturers.Select(
            category => new SelectListItem(category.RealCompanyName, category.Id.ToString()))
            .ToList();

        string? productManifacturerIdString = productData.ManifacturerId.ToString();

        if (productData.ManifacturerId != null)
        {
            foreach (SelectListItem selectListItem in selectListItems)
            {
                if (selectListItem.Value == productManifacturerIdString)
                {
                    selectListItem.Selected = true;

                    break;
                }
            }

            return selectListItems;
        }

        return selectListItems.Prepend(new("-- Please select a manifacturer --", "-1", true, true));
    }

    public static IEnumerable<SelectListItem> GetStatusSelectListItems(Product product)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("Unavailable", ((int)ProductStatusEnum.Unavailable).ToString(), product.Status == ProductStatusEnum.Unavailable),
            new ("Call", ((int)ProductStatusEnum.Call).ToString(), product.Status == ProductStatusEnum.Call),
            new ("Available", ((int)ProductStatusEnum.Available).ToString(), product.Status == ProductStatusEnum.Available),
        };

        if (product.Status == null)
        {
            currencySelectListItems.Add(new("-- Please select a status for the product --", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static IEnumerable<SelectListItem> GetStatusSelectListItems(ProductDisplayData productData)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("Unavailable", ((int)ProductStatusEnum.Unavailable).ToString(), productData.Status == ProductStatusEnum.Unavailable),
            new ("Call", ((int)ProductStatusEnum.Call).ToString(), productData.Status == ProductStatusEnum.Call),
            new ("Available", ((int)ProductStatusEnum.Available).ToString(), productData.Status == ProductStatusEnum.Available),
        };

        if (productData.Status == null)
        {
            currencySelectListItems.Add(new("-- Please select a status for the product --", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static List<SelectListItem> GetXmlPlacementSelectItems(ProductProperty productProperty)
    {
        return new()
        {
            new("At the top", ((int)XMLPlacementEnum.AtTheTop).ToString(), productProperty.XmlPlacement == XMLPlacementEnum.AtTheTop),
            new("At the bottom", ((int)XMLPlacementEnum.InBottomInThePropertiesList).ToString(), productProperty.XmlPlacement == XMLPlacementEnum.InBottomInThePropertiesList),
            new("As a site url", ((int)XMLPlacementEnum.AsSiteUrl).ToString(), productProperty.XmlPlacement == XMLPlacementEnum.AsSiteUrl),
        };
    }

    public static IEnumerable<SelectListItem> GetCharacteristicSelectListItems(IEnumerable<ProductCharacteristic> characteristicsForProductCategory)
    {
        return characteristicsForProductCategory
            .Select(productCharacteristic =>
            new SelectListItem()
            {
                Text = productCharacteristic.Name,
                Value = productCharacteristic.Id.ToString(),
            });
    }
}