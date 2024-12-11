using OneOf;
using OneOf.Types;
using Spire.Pdf;
using Spire.Pdf.Fields;
using Spire.Pdf.Widget;

namespace MOSTComputers.Services.PDF.Services;

public sealed class PdfBasicOperationsService
{
    public OneOf<TPdfField, NotFound> GetFieldIfExists<TPdfField>(PdfDocument pdfDocument, string fieldName)
        where TPdfField : PdfField
    {
        PdfFormWidget pdfFormWidget = (PdfFormWidget)pdfDocument.Form;
        
        for (int i = 0; i < pdfFormWidget.FieldsWidget.Count; i++)
        {
            PdfField field = pdfFormWidget.FieldsWidget[i];

            if (field.Name == fieldName)
            {
                if (field is TPdfField fieldInCorrectType)
                {
                    return fieldInCorrectType;
                }
                else
                {
                    return new NotFound();
                }
            }
        }

        return new NotFound();
    }

    public OneOf<Success, NotFound> SaveDocument(PdfDocument pdfDocument, string destinationFilePath)
    {
        if (string.IsNullOrWhiteSpace(destinationFilePath)
           || !File.Exists(destinationFilePath))
        {
            return new NotFound();
        }

        pdfDocument.SaveToFile(destinationFilePath);

        return new Success();
    }

    public OneOf<Success, NotFound> ModifyPdfField(string documentFilePath, PdfTextBoxField pdfField, string newValue)
    {
        if (string.IsNullOrWhiteSpace(documentFilePath)
            || !File.Exists(documentFilePath))
        {
            return new NotFound();
        }

        pdfField.Text = newValue;

        pdfField.Page.Document.Save(documentFilePath, FileFormat.PDF);

        return new Success();
    }

    public OneOf<Success, NotFound> ModifyPdfField(string documentFilePath, PdfCheckBoxField pdfField, bool newValue)
    {
        if (string.IsNullOrWhiteSpace(documentFilePath)
            || !File.Exists(documentFilePath))
        {
            return new NotFound();
        }

        pdfField.Checked = newValue;

        pdfField.Page.Document.Save(documentFilePath, FileFormat.PDF);

        return new Success();
    }

    public OneOf<Success, NotFound> ModifyPdfField(string documentFilePath, PdfButtonField pdfField, string buttonText)
    {
        if (string.IsNullOrWhiteSpace(documentFilePath)
            || !File.Exists(documentFilePath))
        {
            return new NotFound();
        }

        pdfField.Text = buttonText;

        pdfField.Page.Document.Save(documentFilePath, FileFormat.PDF);

        return new Success();
    }

    public OneOf<Success, NotFound> SelectListItem(string documentFilePath, PdfListBoxField pdfField, int elementIndex)
    {
        if (string.IsNullOrWhiteSpace(documentFilePath)
            || !File.Exists(documentFilePath))
        {
            return new NotFound();
        }

        pdfField.SelectedIndex = elementIndex;

        pdfField.Page.Document.Save(documentFilePath, FileFormat.PDF);

        return new Success();
    }
}