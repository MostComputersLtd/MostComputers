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

    public Dictionary<string, List<SearchStringPartOriginData>?>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        if (Product.SearchString == null) return null;

        Dictionary<string, List<SearchStringPartOriginData>?> output = new();

        string[] searchStringParts = Product.SearchString
            .Trim()
            .Split(' ');

        foreach (string searchStringPart in searchStringParts)
        {
            List<ProductCharacteristic>? productCharacteristics
             = GetCharacteristicsForSearchStringPart(searchStringPart);

            if (productCharacteristics is null)
            {
                output.Add(searchStringPart, null);

                continue;
            }

            IEnumerable<SearchStringPartOriginData> productOriginDataFromRelatedCharacteristics
                = productCharacteristics.Select(characteristic => new SearchStringPartOriginData(searchStringPart, characteristic));

            output.Add(searchStringPart, productOriginDataFromRelatedCharacteristics.ToList());
        }

        return output;
    }

    public List<ProductCharacteristic>? GetCharacteristicsForSearchStringPart(string searchStringPart)
    {
        ProductCharacteristic? output = CharacteristicsAndSearchStringAbbreviationsForProduct
             .FirstOrDefault(x => x.Name == searchStringPart);

        if (output is not null) return new() { output };

        output = CharacteristicsAndSearchStringAbbreviationsForProduct
            .FirstOrDefault(x => string.Equals(x.Name, searchStringPart, StringComparison.OrdinalIgnoreCase));

        if (output is not null) return new() { output };

        IEnumerable<ProductCharacteristic> characteristicsContainingInput = CharacteristicsAndSearchStringAbbreviationsForProduct
            .Where(x => x.Name?.Contains(searchStringPart) ?? false);

        if (characteristicsContainingInput.Any())
        {
            return characteristicsContainingInput.ToList();
        }

        return null;
    }
}
