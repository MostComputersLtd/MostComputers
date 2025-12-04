using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml;

internal sealed class WarrantyCardToXmlService : IWarrantyCardToXmlService
{
    public WarrantyCardToXmlService(
        IWarrantyCardRepository warrantyCardRepository,
        IWarrantyCardXmlService warrantyCardXmlService)
    {
        _warrantyCardRepository = warrantyCardRepository;
        _warrantyCardXmlService = warrantyCardXmlService;
    }

    private readonly IWarrantyCardRepository _warrantyCardRepository;
    private readonly IWarrantyCardXmlService _warrantyCardXmlService;

    public async Task GetXmlForWarrantyCardsAsync(Stream outputStream, WarrantyCardSearchRequest warrantyCardSearchRequest)
    {
        List<WarrantyCard> warrantyCards = await _warrantyCardRepository.GetAllMatchingAsync(warrantyCardSearchRequest);

        await GetXmlForWarrantyCardsAsync(outputStream, warrantyCards);
    }

    public async Task GetXmlForWarrantyCardsAsync(Stream outputStream, IEnumerable<WarrantyCard> warrantyCards)
    {
        WarrantyCardXmlFullData warrantyCardXmlFullData = new()
        {
            WarrantyCards = new(),
        };

        foreach (WarrantyCard warrantyCard in warrantyCards)
        {
            XmlWarrantyCard xmlWarrantyCard = MapXmlWarrantyCard(warrantyCard);

            warrantyCardXmlFullData.WarrantyCards.Add(xmlWarrantyCard);
        }

        await _warrantyCardXmlService.TrySerializeXmlAsync(outputStream, warrantyCardXmlFullData);
    }


    private static XmlWarrantyCard MapXmlWarrantyCard(WarrantyCard warrantyCard)
    {
        return new()
        {
            ExportId = warrantyCard.ExportId,
            ExportDate = warrantyCard.ExportDate,
            ExportUserId = warrantyCard.ExportUserId,
            ExportUser = warrantyCard.ExportUser,
            OrderId = warrantyCard.OrderId,
            CustomerBID = warrantyCard.CustomerBID,
            CustomerName = warrantyCard.CustomerName,
            WarrantyCardDate = warrantyCard.WarrantyCardDate,
            WarrantyCardTerm = warrantyCard.WarrantyCardTerm,
            WarrantyCardItems = warrantyCard.WarrantyCardItems is not null ? MapXmlWarrantyCardItems(warrantyCard.WarrantyCardItems) : null,
        };
    }

    private static List<XmlWarrantyCardItem> MapXmlWarrantyCardItems(IEnumerable<WarrantyCardItem> warrantyCardItems)
    {
        List<XmlWarrantyCardItem> xmlWarrantyCardItems = new();

        foreach (WarrantyCardItem warrantyCardItem in warrantyCardItems)
        {
            XmlWarrantyCardItem xmlWarrantyCardItem = MapXmlWarrantyCardItem(warrantyCardItem);

            xmlWarrantyCardItems.Add(xmlWarrantyCardItem);
        }

        return xmlWarrantyCardItems;
    }

    private static XmlWarrantyCardItem MapXmlWarrantyCardItem(WarrantyCardItem warrantyCardItem)
    {
        return new()
        {
            ExportedItemId = warrantyCardItem.ExportedItemId,
            ExportId = warrantyCardItem.ExportId,
            OrderId = warrantyCardItem.OrderId,
            ProductId = warrantyCardItem.ProductId,
            ProductName = warrantyCardItem.ProductName,
            PriceInLeva = warrantyCardItem.PriceInLeva,
            Quantity = warrantyCardItem.Quantity,
            SerialNumber = warrantyCardItem.SerialNumber,
            WarrantyCardItemTermInMonths = warrantyCardItem.WarrantyCardItemTermInMonths,
            DisplayOrder = warrantyCardItem.DisplayOrder,
        };
    }
}