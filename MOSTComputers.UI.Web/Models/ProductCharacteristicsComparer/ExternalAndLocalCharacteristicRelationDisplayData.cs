using System.Drawing;

namespace MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;
public sealed class ExternalAndLocalCharacteristicRelationDisplayData
{
    private LocalCharacteristicDisplayData? _matchingLocalCharacteristics;
    public LocalCharacteristicDisplayData? MatchingLocalCharacteristic => _matchingLocalCharacteristics;

    private List<ExternalXmlPropertyDisplayData>? _matchingExternalProperties;
    public IReadOnlyList<ExternalXmlPropertyDisplayData>? MatchingExternalProperties => _matchingExternalProperties;

    public void AddCharacteristic(LocalCharacteristicDisplayData productCharacteristic)
    {
        _matchingLocalCharacteristics ??= productCharacteristic;
    }

    public void AddProperties(IEnumerable<ExternalXmlPropertyDisplayData> externalXmlProperties)
    {
        _matchingExternalProperties ??= new List<ExternalXmlPropertyDisplayData>();

        foreach (ExternalXmlPropertyDisplayData externalXmlPropertyToAdd in externalXmlProperties)
        {
            bool hasADuplicate = false;

            for (int i = 0; i < _matchingExternalProperties.Count; i++)
            {
                ExternalXmlPropertyDisplayData externalXmlProperty = _matchingExternalProperties[i];

                if (IsValueEqual(externalXmlProperty, externalXmlPropertyToAdd))
                {
                    hasADuplicate = true;

                    break;
                }
            }

            if (hasADuplicate) continue;

            _matchingExternalProperties.Add(externalXmlPropertyToAdd);
        }
    }

    public void AddProperties(params ExternalXmlPropertyDisplayData[] externalXmlProperties)
    {
        _matchingExternalProperties ??= new List<ExternalXmlPropertyDisplayData>();

        foreach (ExternalXmlPropertyDisplayData externalXmlPropertyToAdd in externalXmlProperties)
        {
            bool hasADuplicate = false;

            for (int i = 0; i < _matchingExternalProperties.Count; i++)
            {
                ExternalXmlPropertyDisplayData externalXmlProperty = _matchingExternalProperties[i];

                if (IsValueEqual(externalXmlProperty, externalXmlPropertyToAdd))
                {
                    hasADuplicate = true;

                    break;
                }
            }

            if (hasADuplicate) continue;

            _matchingExternalProperties.Add(externalXmlPropertyToAdd);
        }
    }

    public bool RemoveCharacteristic()
    {
        _matchingLocalCharacteristics = null;

        return true;
    }

    public bool RemoveProperty(ExternalXmlPropertyDisplayData externalXmlProperty)
    {
        if (_matchingExternalProperties is null) return false;

        int indexOfExternalXmlProperty = _matchingExternalProperties.IndexOf(externalXmlProperty);

        if (indexOfExternalXmlProperty == -1) return false;

        _matchingExternalProperties.RemoveAt(indexOfExternalXmlProperty);

        return true;
    }

    public bool RemovePropertyAt(int indexOfExternalXmlProperty)
    {
        if (_matchingExternalProperties is null
            || indexOfExternalXmlProperty < 0
            || indexOfExternalXmlProperty >= _matchingExternalProperties.Count) return false;

        _matchingExternalProperties.RemoveAt(indexOfExternalXmlProperty);

        return true;
    }

    internal static bool IsValueEqual(LocalCharacteristicDisplayData localCharacteristicDisplayData1, LocalCharacteristicDisplayData localCharacteristicDisplayData2)
    {
        return localCharacteristicDisplayData1.Id == localCharacteristicDisplayData2.Id
            && localCharacteristicDisplayData1.CategoryId == localCharacteristicDisplayData2.CategoryId
            && localCharacteristicDisplayData1.Name == localCharacteristicDisplayData2.Name
            && localCharacteristicDisplayData1.Meaning == localCharacteristicDisplayData2.Meaning
            && localCharacteristicDisplayData1.DisplayOrder == localCharacteristicDisplayData2.DisplayOrder;
    }

    internal static bool IsValueEqual(ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData1, ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData2)
    {
        return externalXmlPropertyDisplayData1.CategoryId == externalXmlPropertyDisplayData2.CategoryId
            && externalXmlPropertyDisplayData1.Name == externalXmlPropertyDisplayData2.Name
            && externalXmlPropertyDisplayData1.Order == externalXmlPropertyDisplayData2.Order;
    }
}