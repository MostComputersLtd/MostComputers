using MOSTComputers.Services.PDF.Services;
using OneOf;
using OneOf.Types;
using Spire.Pdf;
using Spire.Pdf.Fields;

namespace MOSTComputers.Services.PDF.Tests.Integration;

public class PdfBasicOperationsServiceTests
{
    public const string TEMP_TestPDFFilePath = "C:/Users/MostDeveloper/Downloads/TEST_PDF.pdf";

    public PdfBasicOperationsServiceTests()
    {
    }

    private readonly PdfBasicOperationsService _pdfBasicOperationsService = new();

    [Fact]
    public void ModifyPdf_ShouldSucceed_WhenFilePathAndFieldNameAreValid()
    {
        PdfDocument pdfDocument = new();

        pdfDocument.LoadFromFile(TEMP_TestPDFFilePath);

        OneOf<PdfTextBoxField, NotFound> getFieldResult = _pdfBasicOperationsService.GetFieldIfExists<PdfTextBoxField>(pdfDocument, "text_1qwzf");

        Assert.True(getFieldResult.Match(
            field => true,
            notFound => false));

        OneOf<Success, NotFound> modifyPdfResult = _pdfBasicOperationsService.ModifyPdfField(TEMP_TestPDFFilePath, getFieldResult.AsT0, "NEW Value");

        Assert.True(modifyPdfResult.Match(
            success => true,
            notFound => false));
    }
}