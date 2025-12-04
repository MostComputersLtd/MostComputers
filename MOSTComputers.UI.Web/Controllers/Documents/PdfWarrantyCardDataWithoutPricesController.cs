using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.UI.Web.Controllers.Documents;

[ApiController]
[Route(ControllerRoute)]
public sealed class PdfWarrantyCardDataWithoutPricesController : ControllerBase
{
    public PdfWarrantyCardDataWithoutPricesController(
        IPdfWarrantyCardDataService pdfWarrantyCardDataService,
        IPdfWarrantyCardWithoutPricesFileGeneratorService pdfWarrantyCardFileGeneratorService)
    {
        _pdfWarrantyCardDataService = pdfWarrantyCardDataService;
        _pdfWarrantyCardWithoutPricesFileGeneratorService = pdfWarrantyCardFileGeneratorService;
    }

    internal const string ControllerRoute = ControllerCommonElements.ControllerRoutePrefix + "documents/" + "warrantyCardData";

    private readonly IPdfWarrantyCardDataService _pdfWarrantyCardDataService;
    private readonly IPdfWarrantyCardWithoutPricesFileGeneratorService _pdfWarrantyCardWithoutPricesFileGeneratorService;

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetWarrantyCardPdfFromExportIdAsync(int orderId)
    {
        WarrantyCardWithoutPricesData? warrantyCardDataWithoutPrices
            = await _pdfWarrantyCardDataService.GetWarrantyCardDataWithoutPricesByOrderIdAsync(orderId);

        if (warrantyCardDataWithoutPrices is null)
        {
            return new NotFoundObjectResult($"Warranty card with id {orderId} was not found.");
        }

        byte[] fileData = await _pdfWarrantyCardWithoutPricesFileGeneratorService.CreateWarrantyCardPdfAsync(warrantyCardDataWithoutPrices);

        string contentType = "application/pdf";

        return new FileContentResult(fileData, contentType);
    }
}