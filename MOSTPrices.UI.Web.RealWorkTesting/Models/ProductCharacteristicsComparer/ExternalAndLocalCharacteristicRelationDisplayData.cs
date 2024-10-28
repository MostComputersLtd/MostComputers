using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MOSTComputers.Models.Product.Models;
using System.Collections.Generic;
using System.Drawing;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

public sealed class ExternalAndLocalCharacteristicRelationDisplayData
{
    private List<LocalCharacteristicDisplayData>? _matchingLocalCharacteristics;
    public IReadOnlyList<LocalCharacteristicDisplayData>? MatchingLocalCharacteristics => _matchingLocalCharacteristics;

    private List<ExternalXmlPropertyDisplayData>? _matchingExternalProperties;
    public IReadOnlyList<ExternalXmlPropertyDisplayData>? MatchingExternalProperties => _matchingExternalProperties;

    private Dictionary<int, List<int>>? _correspondingCharacteristicAndXmlPropertyIndexes;
    public IReadOnlyDictionary<int, List<int>>? CorrespondingCharacteristicAndXmlPropertyIndexes => _correspondingCharacteristicAndXmlPropertyIndexes;

    public void AddCharacteristics(IEnumerable<LocalCharacteristicDisplayData> productCharacteristics)
    {
        _matchingLocalCharacteristics ??= new List<LocalCharacteristicDisplayData>();

        foreach (LocalCharacteristicDisplayData localCharacteristicDisplayDataToAdd in productCharacteristics)
        {
            bool hasADuplicate = false;

            for (int i = 0; i < _matchingLocalCharacteristics.Count; i++)
            {
                LocalCharacteristicDisplayData localCharacteristicDisplayData = _matchingLocalCharacteristics[i];

                if (IsValueEqual(localCharacteristicDisplayData, localCharacteristicDisplayDataToAdd))
                {
                    hasADuplicate = true;

                    break;
                }
            }

            if (hasADuplicate) continue;

            _matchingLocalCharacteristics.Add(localCharacteristicDisplayDataToAdd);
        }
    }

    public void AddCharacteristics(params LocalCharacteristicDisplayData[] productCharacteristics)
    {
        _matchingLocalCharacteristics ??= new List<LocalCharacteristicDisplayData>();

        foreach (LocalCharacteristicDisplayData localCharacteristicDisplayDataToAdd in productCharacteristics)
        {
            bool hasADuplicate = false;

            for (int i = 0; i < _matchingLocalCharacteristics.Count; i++)
            {
                LocalCharacteristicDisplayData localCharacteristicDisplayData = _matchingLocalCharacteristics[i];

                if (IsValueEqual(localCharacteristicDisplayData, localCharacteristicDisplayDataToAdd))
                {
                    hasADuplicate = true;

                    break;
                }
            }

            if (hasADuplicate) continue;

            _matchingLocalCharacteristics.Add(localCharacteristicDisplayDataToAdd);
        }
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

    public bool MatchCharacteristicAndProperty(
        LocalCharacteristicDisplayData localCharacteristicDisplayData,
        ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData,
        Color? matchedItemsBackgroundColor = null)
    {
        if (_matchingLocalCharacteristics is null
            || _matchingExternalProperties is null)
        {
            return false;
        }

        int localCharacteristicIndex = _matchingLocalCharacteristics.FindIndex(x => IsValueEqual(localCharacteristicDisplayData, x));
        int externalXmlPropertyIndex = _matchingExternalProperties.FindIndex(x => IsValueEqual(externalXmlPropertyDisplayData, x));

        if (localCharacteristicIndex == -1
            || externalXmlPropertyIndex == -1)
        {
            return false;
        }

        return MatchCharacteristicAndPropertyAt(localCharacteristicIndex, externalXmlPropertyIndex, matchedItemsBackgroundColor);
    }

    public bool MatchCharacteristicAndPropertyAt(
        int productCharacteristicIndex, int externalXmlPropertyIndex, Color? matchedItemsBackgroundColor = null)
    {
        if (_matchingLocalCharacteristics is null
            || _matchingExternalProperties is null
            || productCharacteristicIndex < 0
            || productCharacteristicIndex >= _matchingLocalCharacteristics.Count
            || externalXmlPropertyIndex < 0
            || externalXmlPropertyIndex >= _matchingExternalProperties.Count)
        {
            return false;
        }

        LocalCharacteristicDisplayData localCharacteristicDisplayData = _matchingLocalCharacteristics[productCharacteristicIndex];
        ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData = _matchingExternalProperties[externalXmlPropertyIndex];

        _correspondingCharacteristicAndXmlPropertyIndexes ??= new();

        KeyValuePair<int, List<int>>? matchingKvp = GetKeyValuePairWithMatchedProperty(externalXmlPropertyIndex);

        if (matchingKvp is not null)
        {
            KeyValuePair<int, List<int>> otherKvpWithProperty = matchingKvp.Value;

            List<int> listOfExternalXmlPropertyIndexes = _correspondingCharacteristicAndXmlPropertyIndexes[otherKvpWithProperty.Key];

            listOfExternalXmlPropertyIndexes.Remove(externalXmlPropertyIndex);

            LocalCharacteristicDisplayData oldMatchedCharacteristicDisplayData = _matchingLocalCharacteristics[otherKvpWithProperty.Key];

            bool areOtherPropsMatchedToOldCharacteristicWithColor = listOfExternalXmlPropertyIndexes
                .Any(index =>
                {
                    ExternalXmlPropertyDisplayData externalXmlProperty = _matchingExternalProperties[index];

                    return externalXmlProperty.CustomBackgroundColor == oldMatchedCharacteristicDisplayData.CustomBackgroundColor;
                });

            if (!areOtherPropsMatchedToOldCharacteristicWithColor
                && oldMatchedCharacteristicDisplayData.CustomBackgroundColor == externalXmlPropertyDisplayData.CustomBackgroundColor)
            {
                oldMatchedCharacteristicDisplayData.CustomBackgroundColor = null;
            }
        }

        if (_correspondingCharacteristicAndXmlPropertyIndexes.TryGetValue(productCharacteristicIndex, out List<int>? matchingExternalXmlProperties))
        {
            if (!matchingExternalXmlProperties.Contains(externalXmlPropertyIndex))
            {
                matchingExternalXmlProperties.Add(externalXmlPropertyIndex);
            }
        }
        else
        {
            _correspondingCharacteristicAndXmlPropertyIndexes.Add(productCharacteristicIndex, new() { externalXmlPropertyIndex });
        }

        localCharacteristicDisplayData.CustomBackgroundColor = matchedItemsBackgroundColor;
        externalXmlPropertyDisplayData.CustomBackgroundColor = matchedItemsBackgroundColor;

        return true;
    }

    public bool RemoveMatchForCharacteristicAt(int productCharacteristicIndex)
    {
        if (_matchingLocalCharacteristics is null
            || _matchingExternalProperties is null
            || productCharacteristicIndex < 0
            || productCharacteristicIndex >= _matchingLocalCharacteristics.Count
            || _correspondingCharacteristicAndXmlPropertyIndexes is null)
        {
            return false;
        }

        LocalCharacteristicDisplayData localCharacteristicDisplayData = _matchingLocalCharacteristics[productCharacteristicIndex];

        List<int> matchedExternalXmlPropertyIndexes = _correspondingCharacteristicAndXmlPropertyIndexes[productCharacteristicIndex];

        foreach (int externalXmlPropertyIndex in matchedExternalXmlPropertyIndexes)
        {
            ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData = _matchingExternalProperties[externalXmlPropertyIndex];

            if (externalXmlPropertyDisplayData.CustomBackgroundColor == localCharacteristicDisplayData.CustomBackgroundColor)
            {
                externalXmlPropertyDisplayData.CustomBackgroundColor = null;
            }
        }

        _correspondingCharacteristicAndXmlPropertyIndexes.Remove(productCharacteristicIndex);

        return true;
    }

    public bool RemoveMatchForPropertyAt(int externalXmlPropertyIndex)
    {
        if (_matchingLocalCharacteristics is null
            || _matchingExternalProperties is null
            || externalXmlPropertyIndex < 0
            || externalXmlPropertyIndex >= _matchingLocalCharacteristics.Count
            || _correspondingCharacteristicAndXmlPropertyIndexes is null)
        {
            return false;
        }

        KeyValuePair<int, List<int>>? kvp = GetKeyValuePairWithMatchedProperty(externalXmlPropertyIndex);

        if (kvp?.Value is null) return false;

        KeyValuePair<int, List<int>> matchingKvp = kvp.Value;

        LocalCharacteristicDisplayData localCharacteristicDisplayData = _matchingLocalCharacteristics[matchingKvp.Key];
        ExternalXmlPropertyDisplayData externalXmlPropertyDisplayData = _matchingExternalProperties[externalXmlPropertyIndex];

        List<int> listOfExternalXmlPropertyIndexes = _correspondingCharacteristicAndXmlPropertyIndexes[matchingKvp.Key];

        listOfExternalXmlPropertyIndexes.Remove(externalXmlPropertyIndex);

        bool areOtherPropsMatchedToOldCharacteristicWithColor = listOfExternalXmlPropertyIndexes
            .Any(index =>
            {
                ExternalXmlPropertyDisplayData externalXmlProperty = _matchingExternalProperties[index];

                return externalXmlProperty.CustomBackgroundColor == localCharacteristicDisplayData.CustomBackgroundColor;
            });

        if (!areOtherPropsMatchedToOldCharacteristicWithColor
            && externalXmlPropertyDisplayData.CustomBackgroundColor == localCharacteristicDisplayData.CustomBackgroundColor)
        {
            externalXmlPropertyDisplayData.CustomBackgroundColor = null;
        }

        return true;
    }

    public bool RemoveCharacteristic(LocalCharacteristicDisplayData characteristic)
    {
        if (_matchingLocalCharacteristics is null) return false;

        int indexOfCharacteristic = _matchingLocalCharacteristics.IndexOf(characteristic);

        if (indexOfCharacteristic == -1) return false;

        RemoveMatchForCharacteristicAt(indexOfCharacteristic);

        _matchingLocalCharacteristics.RemoveAt(indexOfCharacteristic);

        LowerAllKeysInCorrespondingIndexesDictionaryWhenCharacteristicDeleted(indexOfCharacteristic);

        return true;
    }

    public bool RemoveProperty(ExternalXmlPropertyDisplayData externalXmlProperty)
    {
        if (_matchingExternalProperties is null) return false;

        int indexOfExternalXmlProperty = _matchingExternalProperties.IndexOf(externalXmlProperty);

        if (indexOfExternalXmlProperty == -1) return false;

        RemoveMatchForPropertyAt(indexOfExternalXmlProperty);

        _matchingExternalProperties.RemoveAt(indexOfExternalXmlProperty);

        LowerAllValuesInCorrespondingIndexesDictionaryWhenPropertyDeleted(indexOfExternalXmlProperty);

        return true;
    }

    public bool RemoveCharacteristicAt(int indexOfCharacteristic)
    {
        if (_matchingLocalCharacteristics is null
            || indexOfCharacteristic < 0
            || indexOfCharacteristic >= _matchingLocalCharacteristics.Count) return false;

        RemoveMatchForCharacteristicAt(indexOfCharacteristic);

        _matchingLocalCharacteristics.RemoveAt(indexOfCharacteristic);

        LowerAllKeysInCorrespondingIndexesDictionaryWhenCharacteristicDeleted(indexOfCharacteristic);

        return true;
    }

    public bool RemovePropertyAt(int indexOfExternalXmlProperty)
    {
        if (_matchingExternalProperties is null
            || indexOfExternalXmlProperty < 0
            || indexOfExternalXmlProperty >= _matchingExternalProperties.Count) return false;

        RemoveMatchForPropertyAt(indexOfExternalXmlProperty);

        _matchingExternalProperties.RemoveAt(indexOfExternalXmlProperty);

        LowerAllValuesInCorrespondingIndexesDictionaryWhenPropertyDeleted(indexOfExternalXmlProperty);

        return true;
    }

    private bool LowerAllKeysInCorrespondingIndexesDictionaryWhenCharacteristicDeleted(int characteristicIndex)
    {
        if (_correspondingCharacteristicAndXmlPropertyIndexes is null
           || _correspondingCharacteristicAndXmlPropertyIndexes.Count <= 0) return true;

        if (characteristicIndex <= 0
            || _correspondingCharacteristicAndXmlPropertyIndexes.TryGetValue(characteristicIndex, out _)) return false;

        List<int> dictKeysToLower = new();

        foreach (KeyValuePair<int, List<int>> kvp in _correspondingCharacteristicAndXmlPropertyIndexes)
        {
            if (kvp.Key > characteristicIndex)
            {
                dictKeysToLower.Add(kvp.Key);
            }
        }

        foreach (int dictKeyToLower in dictKeysToLower.Order())
        {
            List<int> externalXmlPropertyIndexes = _correspondingCharacteristicAndXmlPropertyIndexes[dictKeyToLower];

            _correspondingCharacteristicAndXmlPropertyIndexes.Remove(dictKeyToLower);

            _correspondingCharacteristicAndXmlPropertyIndexes.Add(dictKeyToLower - 1, externalXmlPropertyIndexes);
        }

        return true;
    }

    private bool LowerAllValuesInCorrespondingIndexesDictionaryWhenPropertyDeleted(int propertyIndex)
    {
        if (_correspondingCharacteristicAndXmlPropertyIndexes is null
           || _correspondingCharacteristicAndXmlPropertyIndexes.Count <= 0) return true;

        if (propertyIndex <= 0
            || GetKeyValuePairWithMatchedProperty(propertyIndex) is not null) return false;


        foreach (KeyValuePair<int, List<int>> kvp in _correspondingCharacteristicAndXmlPropertyIndexes)
        {
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                int externalXmlPropertyIndex = kvp.Value[i];

                if (externalXmlPropertyIndex < propertyIndex) continue;

                kvp.Value[i] = externalXmlPropertyIndex - 1;
            }
        }

        return true;
    }

    private KeyValuePair<int, List<int>>? GetKeyValuePairWithMatchedProperty(int externalXmlPropertyIndex)
    {
        if (_correspondingCharacteristicAndXmlPropertyIndexes is null) return null;

        KeyValuePair<int, List<int>> defaultValue = new(-1, null!);

        KeyValuePair<int, List<int>> kvp = _correspondingCharacteristicAndXmlPropertyIndexes
            .FirstOrDefault(x => x.Value.Contains(externalXmlPropertyIndex), defaultValue);

        return (!kvp.Equals(defaultValue)) ? kvp : null;
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