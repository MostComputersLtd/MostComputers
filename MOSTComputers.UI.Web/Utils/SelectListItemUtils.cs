using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Models.Documents;
using MOSTComputers.UI.Web.Models.ProductEditor.DTOs;
using MOSTComputers.UI.Web.Models.ProductSearch;

using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.UI.Web.Utils;
internal static class SelectListItemUtils
{
    public static List<SelectListItem> GetSelectListItemsFromValues<T>(
        IEnumerable<T> values,
        Func<T, SelectListItem> factory)
    {
        List<SelectListItem> selectListItems = new();

        foreach (T value in values)
        {
            SelectListItem selectListItem = factory(value);

            selectListItems.Add(selectListItem);
        }

        return selectListItems;
    }

    public static List<SelectListItem> GetSelectListItemsFromValues<T>(
        IEnumerable<T> values,
        Func<T, SelectListItem> factory,
        params SelectListItemAtIndex[] additionalItemsToInsertAtIndexes)
    {
        return GetSelectListItemsFromValues(values, factory, additionalItemsToInsertAtIndexes.ToList());
    }

    public static List<SelectListItem> GetSelectListItemsFromValues<T>(
        IEnumerable<T> values,
        Func<T, SelectListItem> factory,
        List<SelectListItemAtIndex> additionalItemsToInsertAtIndexes)
    {
        List<SelectListItem> selectListItems = GetSelectListItemsFromValues(values, factory);

        if (additionalItemsToInsertAtIndexes is not null)
        {
            foreach (SelectListItemAtIndex kvp in additionalItemsToInsertAtIndexes)
            {
                int insertIndex = kvp.Index;

                if (insertIndex <= 0)
                {
                    insertIndex = 0;
                }
                else if (insertIndex >= selectListItems.Count)
                {
                    insertIndex = selectListItems.Count;
                }

                selectListItems.Insert(insertIndex, kvp.SelectListItem);
            }
        }

        return selectListItems;
    }

    public class SelectListItemAtIndex
    {
        public required int Index { get; init; }
        public required SelectListItem SelectListItem { get; init; }
    }

    public static SelectListItem GetSelectListItemFromCategory(Category category, bool selected)
    {
        SelectListItem selectListItem = new(category.Description, category.Id.ToString(), selected, false);

        return selectListItem;
    }

    public static List<SelectListItem> GetCategorySelectListItems(
        IEnumerable<Category> options, int? selectedCategoryId = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, SelectListItemFromCategory, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, SelectListItemFromCategory);

        SelectListItem SelectListItemFromCategory(Category category)
        {
            return GetSelectListItemFromCategory(category, category.Id == selectedCategoryId);
        }
    }

    public static IEnumerable<SelectListItem> GetProductStatusSelectListItems(
        List<ProductStatus> options, ProductStatus? selectedProductStatus = null, SelectListItem? defaultSelectListItem = null)
	{
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductStatus, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductStatus);

		SelectListItem GetSelectListItemFromProductStatus(ProductStatus productStatus)
		{
			return new(GetStatusStringFromStatusEnum(productStatus), ((int)productStatus).ToString(), productStatus == selectedProductStatus);
		}
	}

    public static IEnumerable<SelectListItem> GetProductSearchStatusSelectListItems(
        List<ProductStatusSearchOptions> options,
        ProductStatusSearchOptions? selected = null,
        SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductSearchStatus, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductSearchStatus);

        SelectListItem GetSelectListItemFromProductSearchStatus(ProductStatusSearchOptions option)
        {
            return new(GetSearchStatusStringFromStatusEnum(option),
                ((int)option).ToString(),
                option == selected);
        }
    }

    public static string? GetSearchStatusStringFromStatusEnum(ProductStatusSearchOptions productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductStatusSearchOptions.Unavailable => "Old",
            ProductStatusSearchOptions.Available => "Avl",
            ProductStatusSearchOptions.Call => "Call",
            ProductStatusSearchOptions.AvailableAndCall => "Avl & Call",
            _ => null
        };
    }

    public static IEnumerable<SelectListItem> GetCurrencySelectListItems(
        List<Currency> options, Currency? selected = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromCurrencyEnum, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromCurrencyEnum);

        SelectListItem GetSelectListItemFromCurrencyEnum(Currency currencyEnum)
        {
            return new(GetCurrencyStringFromCurrencyEnum(currencyEnum), ((int)currencyEnum).ToString(), currencyEnum == selected);
        }
    }

    public static string? GetCurrencyStringFromCurrencyEnum(Currency currencyEnum)
    {
        return currencyEnum switch
        {
            Currency.BGN => "lv.",
            Currency.EUR => "€",
            Currency.USD => "$",
            _ => null
        };
    }

    public static List<SelectListItem> GetProductNewSearchStatusSelectListItems(
        List<ProductNewStatusSearchOptions> options,
        ProductNewStatusSearchOptions? selected = null,
        SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewSearchStatus, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewSearchStatus);

        SelectListItem GetSelectListItemFromProductNewSearchStatus(ProductNewStatusSearchOptions option)
        {
            string? text = GetProductNewSearchStatusStringFromStatusEnum(option);
            string value = ((int)option).ToString();

            bool isSelected = option == selected;

            SelectListItem selectListItem = new(text, value, isSelected);

            return selectListItem;
        }
    }

    public static string? GetProductNewSearchStatusStringFromStatusEnum(ProductNewStatusSearchOptions productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductNewStatusSearchOptions.New => "New",
            ProductNewStatusSearchOptions.WorkInProgress => "Work",
            ProductNewStatusSearchOptions.ReadyForUse => "Ready",
            ProductNewStatusSearchOptions.Postponed1 => "Post1",
            ProductNewStatusSearchOptions.Postponed2 => "Post2",
            _ => null
        };
    }

    public static List<SelectListItem> GetProductEditorProductNewSearchStatusSelectListItems(
       List<ProductEditorProductNewStatusSearchOptions> options,
       ProductEditorProductNewStatusSearchOptions? selected = null,
       SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewSearchStatus, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewSearchStatus);

        SelectListItem GetSelectListItemFromProductNewSearchStatus(ProductEditorProductNewStatusSearchOptions option)
        {
            string? text = GetProductEditorProductNewSearchStatusStringFromStatusEnum(option);
            string value = ((int)option).ToString();

            bool isSelected = option == selected;

            SelectListItem selectListItem = new(text, value, isSelected);

            return selectListItem;
        }
    }

    public static string? GetProductEditorProductNewSearchStatusStringFromStatusEnum(ProductEditorProductNewStatusSearchOptions productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductEditorProductNewStatusSearchOptions.New => "New",
            ProductEditorProductNewStatusSearchOptions.WorkInProgress => "Work",
            ProductEditorProductNewStatusSearchOptions.ReadyForUse => "Ready",
            ProductEditorProductNewStatusSearchOptions.Postponed1 => "Post1",
            ProductEditorProductNewStatusSearchOptions.Postponed2 => "Post2",
            ProductEditorProductNewStatusSearchOptions.NewAndWorkInProgress => "New & Work",
            ProductEditorProductNewStatusSearchOptions.LastAdded => "Last Added",
            ProductEditorProductNewStatusSearchOptions.LastAddedNew => "Last Added & New",
            _ => null
        };
    }

    public static IEnumerable<SelectListItem> GetProductNewStatusSelectListItems(
        List<ProductNewStatus> options, ProductNewStatus? selected = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewStatus, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductNewStatus);

        SelectListItem GetSelectListItemFromProductNewStatus(ProductNewStatus option)
        {
            string? text = GetProductNewStatusStringFromStatusEnum(option);
            string value = ((int)option).ToString();

            bool isSelected = option == selected;

            SelectListItem selectListItem = new(text, value, isSelected);

            return selectListItem;
        }
    }

    public static string? GetProductNewStatusStringFromStatusEnum(ProductNewStatus productStatusEnum)
    {
        return productStatusEnum switch
        {
            ProductNewStatus.New => "New",
            ProductNewStatus.WorkInProgress => "Work",
            ProductNewStatus.ReadyForUse => "Ready",
            ProductNewStatus.Postponed1 => "Post1",
            ProductNewStatus.Postponed2 => "Post2",
            _ => null
        };
    }

    public static IEnumerable<SelectListItem> GetProductXmlStatusSelectListItems(ProductWorkStatuses? productStatuses)
    {
        List<SelectListItem> productXmlStatusSelectListItems = new()
        {
            new ("Not Ready", ((int)ProductXmlStatus.NotReady).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatus.NotReady),
            new ("Work In Progress", ((int)ProductXmlStatus.WorkInProgress).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatus.WorkInProgress),
            new ("Ready", ((int)ProductXmlStatus.ReadyForUse).ToString(), productStatuses?.ProductXmlStatus == ProductXmlStatus.ReadyForUse),
        };

        if (productStatuses?.ProductXmlStatus == null)
        {
            productXmlStatusSelectListItems.Add(new("-", "-1", true, true));
        }

        return productXmlStatusSelectListItems;
    }

    public static IEnumerable<SelectListItem> GetProductPromotionSearchEnumSelectListItems(
        List<PromotionSearchOptions> options, PromotionSearchOptions? selected = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromPromotionSearchOptions, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromPromotionSearchOptions);

        SelectListItem GetSelectListItemFromPromotionSearchOptions(PromotionSearchOptions option)
        {
            string? text = PromotionSearchOptionsStringFromEnum(option);
            string value = ((int)option).ToString();

            bool isSelected = option == selected;

            SelectListItem selectListItem = new(text, value, isSelected);

            return selectListItem;
        }
    }

    public static string? PromotionSearchOptionsStringFromEnum(PromotionSearchOptions promotionSearchOptions)
    {
        return promotionSearchOptions switch
        {
            PromotionSearchOptions.None => "None",
            PromotionSearchOptions.P => "PID",
            PromotionSearchOptions.R => "RID",
            PromotionSearchOptions.I => "Info",
            PromotionSearchOptions.Discount => "Discount",
            _ => null
        };
    }

    public static IEnumerable<SelectListItem> GetManufacturerSelectListItems(Product product, IEnumerable<Manufacturer> allPossibleManufacturers)
    {
        List<SelectListItem> selectListItems = allPossibleManufacturers.Select(
            manufacturer => new SelectListItem(manufacturer.RealCompanyName, manufacturer.Id.ToString()))
            .ToList();

        string? productManufacturerIdString = product.ManufacturerId.ToString();

        if (product.ManufacturerId != null)
        {
            foreach (SelectListItem selectListItem in selectListItems)
            {
                if (selectListItem.Value == productManufacturerIdString)
                {
                    selectListItem.Selected = true;

                    break;
                }
            }

            return selectListItems;
        }

        return selectListItems.Prepend(new("-- Please select a manufacturer --", "-1", true, true));
    }

    public static IEnumerable<SelectListItem> GetStatusSelectListItems(Product product)
    {
        List<SelectListItem> currencySelectListItems = new()
        {
            new ("Unavailable", ((int)ProductStatus.Unavailable).ToString(), product.Status == ProductStatus.Unavailable),
            new ("Call", ((int)ProductStatus.Call).ToString(), product.Status == ProductStatus.Call),
            new ("Available", ((int)ProductStatus.Available).ToString(), product.Status == ProductStatus.Available),
        };

        if (product.Status == null)
        {
            currencySelectListItems.Add(new("-- Please select a status for the product --", "-1", true, true));
        }

        return currencySelectListItems;
    }

    public static List<SelectListItem> GetXmlPlacementSelectItems(XMLPlacementEnum? xmlPlacement)
    {
        List<SelectListItem> output = new()
        {
            new("At the top", ((int)XMLPlacementEnum.AtTheTop).ToString(), xmlPlacement == XMLPlacementEnum.AtTheTop),
            new("At the bottom", ((int)XMLPlacementEnum.InBottomInThePropertiesList).ToString(), xmlPlacement == XMLPlacementEnum.InBottomInThePropertiesList),
            new("As a site url", ((int)XMLPlacementEnum.AsSiteUrl).ToString(), xmlPlacement == XMLPlacementEnum.AsSiteUrl),
        };

        if (xmlPlacement == null)
        {
            output.Add(new("-- Please select a status for the product --", "-1", true, true));
        }

        return output;
    }

    public static List<SelectListItem> GetCharacteristicSelectListItems(
        IEnumerable<ProductCharacteristic> options,
        int? selectedId = null,
        SelectListItem? defaultSelectListItem = null)
    {
        List<SelectListItem> output = new();

        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductCharacteristic, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductCharacteristic);

        SelectListItem GetSelectListItemFromProductCharacteristic(ProductCharacteristic option)
        {
            bool isSelected = option.Id == selectedId;

            SelectListItem selectListItem = new()
            {
                Text = option.Name,
                Value = option.Id.ToString(),
                Selected = isSelected,
            };

            return selectListItem;
        }
    }

    public static IEnumerable<SelectListItem> GetDisplayDocumentTypeSelectListItems(
        List<DisplayDocumentType> options, DisplayDocumentType? selected = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromDisplayDocumentType, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromDisplayDocumentType);

        SelectListItem GetSelectListItemFromDisplayDocumentType(DisplayDocumentType displayDocumentType)
        {
            return new(GetStringFromDisplayDocumentType(displayDocumentType), ((int)displayDocumentType).ToString(), displayDocumentType == selected);
        }
    }

    public static string? GetStringFromDisplayDocumentType(DisplayDocumentType displayDocumentType)
    {
        return displayDocumentType switch
        {
            DisplayDocumentType.Invoice => "Invoices",
            DisplayDocumentType.WarrantyCard => "Warranty Cards",
            _ => null
        };
    }

    public static List<SelectListItem> GetProductGTINCodeTypeSelectListItems(
        List<ProductGTINCodeType> options, ProductGTINCodeType? selected = null, SelectListItem? defaultSelectListItem = null)
    {
        if (defaultSelectListItem is not null)
        {
            SelectListItemAtIndex defaultSelectListItemAtIndex = new()
            {
                Index = 0,
                SelectListItem = defaultSelectListItem
            };

            return GetSelectListItemsFromValues(options, GetSelectListItemFromProductGTINCodeType, defaultSelectListItemAtIndex);
        }

        return GetSelectListItemsFromValues(options, GetSelectListItemFromProductGTINCodeType);

        SelectListItem GetSelectListItemFromProductGTINCodeType(ProductGTINCodeType displayDocumentType)
        {
            return new(GetStringFromProductGTINCodeType(displayDocumentType), displayDocumentType.Value.ToString(), displayDocumentType == selected);
        }
    }

    public static string? GetStringFromProductGTINCodeType(ProductGTINCodeType productGTINCodeType)
    {
        if (productGTINCodeType == ProductGTINCodeType.EAN8) return "EAN-8";
        else if (productGTINCodeType == ProductGTINCodeType.EAN13) return "EAN-13";
        else if (productGTINCodeType == ProductGTINCodeType.UPC) return "UPC";
        else if (productGTINCodeType == ProductGTINCodeType.JAN) return "JAN";

        return null;
    }

    public static List<SelectListItem> GetUserRolesSelectItems()
    {
        return new()
        {
            new("Admin", UserRoles.Admin.Value.ToString(), false),
            new("Xml Relation Editor", UserRoles.XmlRelationEditor.Value.ToString(), false),
            new("Product Editor", UserRoles.ProductEditor.Value.ToString(), false),
            new("Employee", UserRoles.Employee.Value.ToString(), false),
            new("User", UserRoles.User.Value.ToString(), false),
        };
    }

    public static List<SelectListItem> GetUserRolesSelectItems(List<UserRoles> userRoles)
    {
        return new()
        {
            new("Admin", UserRoles.Admin.Value.ToString(), userRoles.FirstOrDefault(userRole => userRole.Equals(UserRoles.Admin)) is not null),

            new("Xml Relation Editor", UserRoles.XmlRelationEditor.Value.ToString(),
                userRoles.FirstOrDefault(userRole => userRole.Equals(UserRoles.XmlRelationEditor)) is not null),

            new("Product Editor", UserRoles.ProductEditor.Value.ToString(),
                userRoles.FirstOrDefault(userRole => userRole.Equals(UserRoles.ProductEditor)) is not null),

            new("Employee", UserRoles.Employee.Value.ToString(), userRoles.FirstOrDefault(userRole => userRole.Equals(UserRoles.Employee)) is not null),
            new("User", UserRoles.User.Value.ToString(), userRoles.FirstOrDefault(userRole => userRole.Equals(UserRoles.User)) is not null),
        };
    }
}