using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class PdfWarrantyCardDataService : IPdfWarrantyCardDataService
{
    public PdfWarrantyCardDataService(IWarrantyCardRepository warrantyCardRepository)
    {
        _warrantyCardRepository = warrantyCardRepository;
    }

    private readonly IWarrantyCardRepository _warrantyCardRepository;

    public async Task<WarrantyCardWithoutPricesData?> GetWarrantyCardDataWithoutPricesByOrderIdAsync(int warrantyCardOrderId)
    {
        if (warrantyCardOrderId < 0) return null;

        WarrantyCard? warrantyCard = await _warrantyCardRepository.GetWarrantyCardByOrderIdAsync(warrantyCardOrderId);

        if (warrantyCard == null) return null;

        return GetWarrantyCardDataWithoutPrices(warrantyCard);
    }

    public WarrantyCardWithPricesData GetWarrantyCardDataWithPrices(WarrantyCard warrantyCard)
    {
        return new()
        {
            ExportUser = warrantyCard.ExportUser,
            OrderId = warrantyCard.OrderId,
            CustomerBID = warrantyCard.CustomerBID,
            WarrantyCardDate = warrantyCard.WarrantyCardDate,
            CustomerName = warrantyCard.CustomerName,
            WarrantyCardTermInMonths = warrantyCard.WarrantyCardTerm,
            WarrantyCardItems = (warrantyCard.WarrantyCardItems is not null) ? GetWarrantyCardItemsWithPrices(warrantyCard.WarrantyCardItems) : null,
        };
    }

    public WarrantyCardWithoutPricesData GetWarrantyCardDataWithoutPrices(WarrantyCard warrantyCard)
    {
        return new()
        {
            OrderId = warrantyCard.OrderId,
            WarrantyCardDate = warrantyCard.WarrantyCardDate,
            CustomerName = warrantyCard.CustomerName,
            ExportDate = warrantyCard.ExportDate,
            WarrantyCardTermInMonths = warrantyCard.WarrantyCardTerm,
            WarrantyCardItems = (warrantyCard.WarrantyCardItems is not null) ? GetWarrantyCardItemsWithoutPrices(warrantyCard.WarrantyCardItems) : null,
        };
    }

    private List<WarrantyCardItemWithPricesData> GetWarrantyCardItemsWithPrices(IEnumerable<WarrantyCardItem> warrantyCardItems)
    {
        List<WarrantyCardItemWithPricesData> output = new();

        foreach (WarrantyCardItem warrantyCardItem in warrantyCardItems)
        {
            WarrantyCardItemWithPricesData warrantyCardItemWithoutPricesData = new()
            {
                ProductName = warrantyCardItem.ProductName,
                SerialNumber = warrantyCardItem.SerialNumber,
                WarrantyCardItemTermInMonths = warrantyCardItem.WarrantyCardItemTermInMonths,
                PriceInLeva = warrantyCardItem.PriceInLeva,
                Quantity = warrantyCardItem.Quantity,
                DisplayOrder = warrantyCardItem.DisplayOrder,
            };

            output.Add(warrantyCardItemWithoutPricesData);
        }

        return output;
    }

    private List<WarrantyCardItemWithoutPricesData> GetWarrantyCardItemsWithoutPrices(IEnumerable<WarrantyCardItem> warrantyCardItems)
    {
        List<WarrantyCardItemWithoutPricesData> output = new();

        foreach (WarrantyCardItem warrantyCardItem in warrantyCardItems)
        {
            WarrantyCardItemWithoutPricesData warrantyCardItemWithoutPricesData = new()
            {
                ProductName = warrantyCardItem.ProductName,
                SerialNumber = warrantyCardItem.SerialNumber,
                WarrantyCardItemTermInMonths = warrantyCardItem.WarrantyCardItemTermInMonths,
            };

            output.Add(warrantyCardItemWithoutPricesData);
        }

        return output;
    }
}