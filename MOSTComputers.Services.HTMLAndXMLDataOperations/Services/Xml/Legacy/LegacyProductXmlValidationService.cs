using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;

using static MOSTComputers.Services.HTMLAndXMLDataOperations.Utils.XmlContentValidationUtils;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy;
internal sealed class LegacyProductXmlValidationService : ILegacyProductXmlValidationService
{
    public bool IsValidXmlObjectData(LegacyXmlObjectData xmlObjectData)
    {
        foreach (LegacyXmlProduct xmlProduct in xmlObjectData.Products)
        {
            if (!IsValidXmlProduct(xmlProduct))
            {
                return false;
            }
        }

        if (xmlObjectData.ExchangeRates is not null)
        {
            if (!IsValidXmlExchangeRates(xmlObjectData.ExchangeRates))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsValidXmlProduct(LegacyXmlProduct xmlProduct)
    {
        if (!IsNullOrValidXmlString(xmlProduct.Code)
            || !IsNullOrValidXmlString(xmlProduct.Name)
            || !IsNullOrValidXmlString(xmlProduct.StatusString)
            || !IsNullOrValidXmlString(xmlProduct.GeneralDescription)
            || !IsNullOrValidXmlString(xmlProduct.CurrencyCode)
            || !IsNullOrValidXmlString(xmlProduct.MainPictureUrl)
            || !IsNullOrValidXmlString(xmlProduct.SearchString)
            || !IsNullOrValidXmlString(xmlProduct.PartNumbers)
            || !IsNullOrValidXmlString(xmlProduct.VendorUrl))
        {
            return false;
        }

        if (xmlProduct.Category is not null)
        {
            if (!IsValidXmlCategory(xmlProduct.Category))
            {
                return false;
            }
        }

        if (xmlProduct.SubCategory is not null)
        {
            if (!IsValidXmlSubCategory(xmlProduct.SubCategory))
            {
                return false;
            }
        }

        if (xmlProduct.Manufacturer is not null)
        {
            if (!IsValidXmlManufacturer(xmlProduct.Manufacturer))
            {
                return false;
            }
        }

        if (xmlProduct.SearchStringParts is not null)
        {
            foreach (LegacySearchStringPartInfo searchStringPart in xmlProduct.SearchStringParts)
            {
                if (!IsValidXmlSearchStringPart(searchStringPart))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Images is not null)
        {
            foreach (LegacyXmlProductImage image in xmlProduct.Images)
            {
                if (!IsValidXmlProductImage(image))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Properties is not null)
        {
            foreach (LegacyXmlProductProperty property in xmlProduct.Properties)
            {
                if (!IsValidXmlProductProperty(property))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Promotions is not null)
        {
            foreach (LegacyXmlPromotion promotion in xmlProduct.Promotions)
            {
                if (!IsNullOrValidXmlString(promotion.Type)
                    || !IsNullOrValidXmlString(promotion.ValidFromString)
                    || !IsNullOrValidXmlString(promotion.ValidToString))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Downloads is not null)
        {
            foreach (LegacyXmlFile download in xmlProduct.Downloads)
            {
                if (!IsNullOrValidXmlString(download.Url))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsValidXmlCategory(LegacyXmlCategory xmlCategory)
    {
        return IsNullOrValidXmlString(xmlCategory.Name);
    }

    public bool IsValidXmlSubCategory(LegacyXmlSubCategory xmlSubCategory)
    {
        return IsNullOrValidXmlString(xmlSubCategory.IdAsString)
            && IsNullOrValidXmlString(xmlSubCategory.Name);
    }

    public bool IsValidXmlManufacturer(LegacyXmlManufacturer xmlManufacturer)
    {
        return IsNullOrValidXmlString(xmlManufacturer.IdAsString)
            && IsNullOrValidXmlString(xmlManufacturer.Name);
    }

    public bool IsValidXmlSearchStringPart(LegacySearchStringPartInfo xmlSearchStringPart)
    {
        return IsNullOrValidXmlString(xmlSearchStringPart.Name)
            && IsNullOrValidXmlString(xmlSearchStringPart.Description);
    }

    public bool IsValidXmlProductImage(LegacyXmlProductImage xmlProductImage)
    {
        return IsNullOrValidXmlString(xmlProductImage.PictureUrl)
            && IsNullOrValidXmlString(xmlProductImage.ThumbnailUrl);
    }

    public bool IsValidXmlProductProperty(LegacyXmlProductProperty xmlProductProperty)
    {
        return IsNullOrValidXmlString(xmlProductProperty.Name)
            && IsNullOrValidXmlString(xmlProductProperty.Value)
            && IsNullOrValidXmlString(xmlProductProperty.Order);
    }

    public bool IsValidXmlPromotion(LegacyXmlPromotion xmlPromotion)
    {
        return IsNullOrValidXmlString(xmlPromotion.Type)
            && IsNullOrValidXmlString(xmlPromotion.ValidFromString)
            && IsNullOrValidXmlString(xmlPromotion.ValidToString);
    }

    public bool IsValidXmlFile(LegacyXmlFile xmlFile)
    {
        return IsNullOrValidXmlString(xmlFile.Url);
    }

    public bool IsValidXmlExchangeRates(LegacyXmlExchangeRatesEurAndUsdPerBgn xmlExchangeRates)
    {
        if (!IsNullOrValidXmlString(xmlExchangeRates.ValidToString))
        {
            return false;
        }

        return true;
    }
}