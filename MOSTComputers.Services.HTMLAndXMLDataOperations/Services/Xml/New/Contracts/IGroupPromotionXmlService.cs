using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.PromotionGroupData;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IGroupPromotionXmlService
{
    Task TrySerializeXmlAsync(Stream outputStream, GroupPromotionsXmlFullData xmlData);
    Task<OneOf<string, InvalidXmlResult>> TrySerializeXmlAsync(GroupPromotionsXmlFullData xmlData);
}