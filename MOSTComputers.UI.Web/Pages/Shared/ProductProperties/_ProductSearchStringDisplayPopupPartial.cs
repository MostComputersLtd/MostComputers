using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductProperties;

public class ProductSearchStringDisplayPopupPartialModel
{
    public required Product Product { get; set; }
    public required IEnumerable<ProductCharacteristic> CharacteristicsAndSearchStringAbbreviationsForProduct { get; set; }

    public string[]? GetSearchStringParts()
    {
        if (Product.SearchString == null) return null;

        string[] output = Product.SearchString.Split(' ');

        return output;
    }

    public Dictionary<string, SearchStringPartOriginData?>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        if (Product.SearchString == null) return null;

        Dictionary<string, SearchStringPartOriginData?> output = new();

        string[] searchStringParts = Product.SearchString
            .Trim()
            .Split(' ');

        foreach (string searchStringPart in searchStringParts)
        {
            ProductCharacteristic? productCharacteristic
             = CharacteristicsAndSearchStringAbbreviationsForProduct.FirstOrDefault(x => x.Name == searchStringPart);

            if (productCharacteristic is null)
            {
                output.Add(searchStringPart, null);

                continue;
            }

            output.Add(searchStringPart, new SearchStringPartOriginData(searchStringPart, productCharacteristic));
        }

        return output;
    }
}
