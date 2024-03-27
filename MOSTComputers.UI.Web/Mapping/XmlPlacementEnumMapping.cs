using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Mapping;

internal static class XmlPlacementEnumMapping
{
    private readonly static List<SelectListItem> _defaultItemsForXmlPlacement = new()
    {
        GetSelectListItemFromXmlPlacementEnum(XMLPlacementEnum.AtTheTop),
        GetSelectListItemFromXmlPlacementEnum(XMLPlacementEnum.InBottomInThePropertiesList),
        GetSelectListItemFromXmlPlacementEnum(XMLPlacementEnum.AsSiteUrl),
    };

    public static string GetStringFromXmlPlacementEnum(XMLPlacementEnum placementEnum)
    {
        return placementEnum switch
        {
            XMLPlacementEnum.AtTheTop => "At the top",
            XMLPlacementEnum.InBottomInThePropertiesList => "At the bottom",
            XMLPlacementEnum.AsSiteUrl => "As a site url",
            _ => throw new NotImplementedException()
        };
    }

    public static XMLPlacementEnum? GetXmlPlacementEnumFromString(string placementEnumString)
    {
        return placementEnumString switch
        {
            "At the top" => XMLPlacementEnum.AtTheTop,
            "At the bottom" => XMLPlacementEnum.InBottomInThePropertiesList,
            "As a site url" => XMLPlacementEnum.AsSiteUrl,
            _ => null
        };
    }

    public static IEnumerable<SelectListItem> GetSelectListItemsFromXmlPlacementEnumWithOneSelected(XMLPlacementEnum? placementEnum)
    {
        List<SelectListItem> output = CloneDefaultSelectItems();

        if (placementEnum is null) return output;

        foreach (SelectListItem selectListItem in output)
        {
            int selectListItemValue = int.Parse(selectListItem.Value);

            if (selectListItemValue == (int)placementEnum)
            {
                selectListItem.Selected = true;

                return output;
            }
        }

        return output;
    }

    public static IEnumerable<SelectListItem> GetSelectListItemsFromXmlPlacementEnumWithOneSelected(uint? xmlPlacementAsInt)
    {
        List<SelectListItem> output = CloneDefaultSelectItems();

        if (xmlPlacementAsInt is null) return output;

        foreach (SelectListItem selectListItem in output)
        {
            int selectListItemValue = int.Parse(selectListItem.Value);

            if (selectListItemValue == xmlPlacementAsInt)
            {
                selectListItem.Selected = true;

                return output;
            }
        }

        return output;
    }

    private static SelectListItem GetSelectListItemFromXmlPlacementEnum(XMLPlacementEnum placementEnum)
    {
        return new()
        {
            Text = GetStringFromXmlPlacementEnum(placementEnum),
            Value = ((int)placementEnum).ToString(),
            Selected = false
        };
    }

    private static List<SelectListItem> CloneDefaultSelectItems()
    {
        List<SelectListItem> output = new();

        foreach (SelectListItem defaultItem in _defaultItemsForXmlPlacement)
        {
            SelectListItem selectListItem = new()
            {
                Text = defaultItem.Text,
                Value = defaultItem.Value,
                Selected = defaultItem.Selected,
                Disabled = defaultItem.Disabled,
                Group = defaultItem.Group,
            };

            output.Add(selectListItem);
        }

        return output;
    }
}