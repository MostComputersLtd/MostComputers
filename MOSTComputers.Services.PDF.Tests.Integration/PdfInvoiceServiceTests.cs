using MOSTComputers.Services.PDF.Models;
using Spire.Pdf;
using MOSTComputers.Services.PDF.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.PDF.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public class PdfInvoiceServiceTests
{
    public PdfInvoiceServiceTests(IPdfInvoiceService pdfInvoiceService)
    {
        _pdfInvoiceService = pdfInvoiceService;
    }

    private readonly IPdfInvoiceService _pdfInvoiceService;

    [Fact]
    public void CreateInvoicePdf_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFileExists()
    {
        InvoiceData invoiceData = GetValidInvoiceData();

        string pdfTestFileName = $"{Guid.NewGuid()}.html";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfInvoiceTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        PdfDocument resultingDocument = _pdfInvoiceService.CreateInvoicePdf(invoiceData, pdfInvoiceTestFileFullPath);
    }

    private static InvoiceData GetValidInvoiceData()
    {
        return new()
        {
            InvoiceId = "0003343545",
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(15),
            Location = "Sofia",
            FirmInHeaderName = "MOST COMPUTERS",
            FirmInHeaderAddress = "bul. sdlkjflkdsjf No. 1212",
            InvoiceOriginText = "Копие",

            RecipientData = new()
            {
                FirmName = "АРАНГО ЕООД",
                FirmAddress = "КОСТЕНЕЦ Ж.К. ОЛИМПИАДА БЛ.9 ВХ. Б. ЕТ. 3 А",
                MRPersonFullName = "ПЕТЪР ИЛИЕВ ИЛИЕВ",
                VatId = "BG201424563",
                FirmOrPersonId = "201424563",
                BankId = null,
                Iban = null,
            },
            SupplierData = new()
            {
                FirmName = "МОСТ КОМПЮТЪРС",
                FirmAddress = "бул. Шипченски проход, бл. 240, вх. Г",
                MRPersonFullName = "Георги Петров",
                VatId = "BG201343443",
                FirmOrPersonId = "201343443",
                BankId = "ОББ АД AIC: UBBSBGSA",
                Iban = "BG23UBBS89247238478234",
            },
            Purchases = new()
            {
                new() { ProductName = "NOTEBOOK LENOVO IP1 15 / 82VG24423S", Quantity = 2, PricePerUnit = 222.49, UnitOfMeasurement = "брой", Currency = "лв" },
                new() { ProductName = "NOTEBOOK IP2 7 / 82VGFF423S", Quantity = 2, PricePerUnit = 934.99, UnitOfMeasurement = "брой", Currency = "лв" },
                new() { ProductName = "TABLET LENOVO SP1 19 / 82VNSFG423", Quantity = 2, PricePerUnit = 459.57, UnitOfMeasurement = "брой", Currency = "лв" },
                new() { ProductName = "MACBOOK 8 / 72VG2F4GV5", Quantity = 1, PricePerUnit = 2324.79, UnitOfMeasurement = "брой", Currency = "лв" },
            },

            VatPercentageFraction = 0.2F,
            RecipientFullName = "ИЛИЯН ПЕТРОВ ГЕОРГИЕВ",
            TypeOfPayment = "ПЛ. НАРЕЖДАНЕ",
            AuthorFullName = "СТАНИМИР ПЕТРОВ ПЕТРОВ",
        };
    }
}