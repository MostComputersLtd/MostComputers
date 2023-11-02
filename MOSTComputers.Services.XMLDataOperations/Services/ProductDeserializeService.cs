using MOSTComputers.Services.DAL;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using MOSTComputers.Services.XMLDataOperations.Mapping;
using MOSTComputers.Services.XMLDataOperations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Services;

public sealed class ProductDeserializeService
{
    public ProductDeserializeService(XmlSerializerFactory xmlSerializerFactory)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(XmlObjectData));
    }

    private readonly XmlSerializer _xmlSerializer;

    public XmlObjectData? DeserializeProductsXml(string xml)
    {
        using StringReader reader = new(xml);

        object? dataAsObject = _xmlSerializer.Deserialize(reader);

        if (dataAsObject is null) return null;

        return (XmlObjectData)dataAsObject;
    }
}