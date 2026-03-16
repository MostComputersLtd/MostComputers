
using System.Text;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.GroupPromotionCarousel;

public sealed class DefaultModel
{
    internal sealed class GroupPromotionImageDisplayData
    {
        public required int Id { get; set; }
        public required int PromotionGroupId { get; set; }
        public required string FileName { get; set; }
    }

    public required string CarouselId { get; set; }

    public required List<object> Items { get; set; } = new();

    public required string ItemTemplatePartialPath { get; set; }
    public string? ItemIndicatorTemplatePartialPath { get; set; } = null;

    public Dictionary<string, string>? AdditionalAttributes { get; set; }
    public Dictionary<string, string>? AdditionalViewportAttributes { get; set; }
    public Dictionary<string, string>? AdditionalIndicatorsListContainerAttributes { get; set; }
    public Dictionary<string, string>? AdditionalIndicatorsListAttributes { get; set; }

    public int DisplayedItemsCount { get; set; } = 1;
    public int ItemHopsPerMove { get; set; } = 1;
    public int TransitionMs { get; set; } = 600;
    public int GoToTransitionMs { get; set; } = 600;
    public bool DisplayArrowButtons { get; set; } = false;
    public bool AllowAutoSlide { get; set; } = false;
    public int AutoSlideIntervalMs { get; set; } = 2500;

    public string ConvertAdditionalDataDictToString(Dictionary<string, string>? additionalDataDict)
    {
        if (additionalDataDict == null || additionalDataDict.Count == 0) return string.Empty;

        StringBuilder stringBuilder = new();

        foreach (KeyValuePair<string, string> kvp in additionalDataDict)
        {
            stringBuilder.Append($"{kvp.Key}=\"{kvp.Value}\";");
        }

        return stringBuilder.ToString();
    }

    public string? GetAttributeFromDictAsStringAndRemoveFromOriginal(Dictionary<string, string>? additionalDataDict, string attributeName)
    {
        if (additionalDataDict == null || additionalDataDict.Count == 0) return string.Empty;

        string? attributeKey = null;
        string? attributeAsString = null;

        foreach (KeyValuePair<string, string> kvp in additionalDataDict)
        {
            if (attributeName == kvp.Key)
            {
                attributeKey = kvp.Key;
                attributeAsString = $"{kvp.Key}=\"{kvp.Value}\";";

                break;
            }
        }

        if (attributeKey != null)
        {
            additionalDataDict.Remove(attributeName);
        }

        return attributeAsString;
    }

    public string? GetAttributeValueFromDictAsStringAndRemoveFromOriginal(Dictionary<string, string>? additionalDataDict, string attributeName)
    {
        if (additionalDataDict == null || additionalDataDict.Count == 0) return string.Empty;

        string? attributeKey = null;
        string? attributeAsString = null;

        foreach (KeyValuePair<string, string> kvp in additionalDataDict)
        {
            if (attributeName == kvp.Key)
            {
                attributeKey = kvp.Key;
                attributeAsString = kvp.Value;

                break;
            }
        }

        if (attributeKey != null)
        {
            additionalDataDict.Remove(attributeName);
        }

        return attributeAsString;
    }
}