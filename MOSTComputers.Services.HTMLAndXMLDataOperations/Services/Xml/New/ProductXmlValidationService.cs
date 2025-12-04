using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using static MOSTComputers.Services.HTMLAndXMLDataOperations.Utils.XmlContentValidationUtils;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New;

internal sealed class ProductXmlValidationService : IProductXmlValidationService
{
    public bool IsValidXmlObjectData(ProductsXmlFullData xmlObjectData)
    {
        foreach (XmlProduct xmlProduct in xmlObjectData.Products)
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

    public bool IsValidXmlProduct(XmlProduct xmlProduct)
    {
        if (!IsNullOrValidXmlString(xmlProduct.Name)
            || !IsNullOrValidXmlString(xmlProduct.StatusString)
            || !IsNullOrValidXmlString(xmlProduct.GeneralDescription)
            || !IsNullOrValidXmlString(xmlProduct.CurrencyCode)
            || !IsNullOrValidXmlString(xmlProduct.SearchString)
            || !IsNullOrValidXmlString(xmlProduct.PartNumber1)
            || !IsNullOrValidXmlString(xmlProduct.PartNumber2)
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
            foreach (XmlSearchStringPartInfo searchStringPart in xmlProduct.SearchStringParts)
            {
                if (!IsValidXmlSearchStringPart(searchStringPart))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Images is not null)
        {
            foreach (XmlProductImage image in xmlProduct.Images)
            {
                if (!IsValidXmlProductImage(image))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Properties is not null)
        {
            foreach (XmlProductProperty property in xmlProduct.Properties)
            {
                if (!IsValidXmlProductProperty(property))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.Promotions is not null)
        {
            foreach (XmlPromotion promotion in xmlProduct.Promotions)
            {
                if (!IsNullOrValidXmlString(promotion.Type)
                    || !IsNullOrValidXmlString(promotion.ValidFromString)
                    || !IsNullOrValidXmlString(promotion.ValidToString))
                {
                    return false;
                }
            }
        }

        if (xmlProduct.GroupPromotion is not null)
        {
            XmlGroupPromotion groupPromotion = xmlProduct.GroupPromotion;

            if (!IsNullOrValidXmlString(groupPromotion.VendorName)
                || !IsNullOrValidXmlString(groupPromotion.GroupPromotionsUrl))
            {
                return false;
            }
        }

        if (xmlProduct.Downloads is not null)
        {
            foreach (XmlFile download in xmlProduct.Downloads)
            {
                if (!IsNullOrValidXmlString(download.Url))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsValidXmlCategory(XmlCategory xmlCategory)
    {
        return IsNullOrValidXmlString(xmlCategory.Name);
    }

    public bool IsValidXmlSubCategory(XmlSubCategory xmlSubCategory)
    {
        return IsNullOrValidXmlString(xmlSubCategory.IdAsString)
            && IsNullOrValidXmlString(xmlSubCategory.Name);
    }

    public bool IsValidXmlManufacturer(XmlManufacturer xmlManufacturer)
    {
        return IsNullOrValidXmlString(xmlManufacturer.IdAsString)
            && IsNullOrValidXmlString(xmlManufacturer.Name);
    }

    public bool IsValidXmlSearchStringPart(XmlSearchStringPartInfo xmlSearchStringPart)
    {
        return IsNullOrValidXmlString(xmlSearchStringPart.Name)
            && IsNullOrValidXmlString(xmlSearchStringPart.Description);
    }

    public bool IsValidXmlProductImage(XmlProductImage xmlProductImage)
    {
        return IsNullOrValidXmlString(xmlProductImage.PictureUrl);
    }

    public bool IsValidXmlProductProperty(XmlProductProperty xmlProductProperty)
    {
        return IsNullOrValidXmlString(xmlProductProperty.Name)
            && IsNullOrValidXmlString(xmlProductProperty.Value)
            && IsNullOrValidXmlString(xmlProductProperty.Order);
    }

    public bool IsValidXmlPromotion(XmlPromotion xmlPromotion)
    {
        return IsNullOrValidXmlString(xmlPromotion.Type)
            && IsNullOrValidXmlString(xmlPromotion.ValidFromString)
            && IsNullOrValidXmlString(xmlPromotion.ValidToString);
    }

    public bool IsValidXmlFile(XmlFile xmlFile)
    {
        return IsNullOrValidXmlString(xmlFile.Url);
    }

    public bool IsValidXmlExchangeRates(XmlExchangeRates xmlExchangeRates)
    {
        if (!IsNullOrValidXmlString(xmlExchangeRates.ValidToString))
        {
            return false;
        }

        return true;
    }
}