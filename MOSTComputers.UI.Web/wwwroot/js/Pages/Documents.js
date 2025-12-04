const pageModelRoute = "/Documents";

const documentTypeSelectElementId = "documentTypeSelect";

const documentFromDateSearchInputElementId = "documentFromDateSearchInput";
const documentToDateSearchInputElementId = "documentToDateSearchInput";
const documentSearchInputElementId = "documentSearchInput";

const documentSearchButtonElementId = "documentSearchButton";
const documentSearchButtonLoaderElementId = "documentSearchButtonLoader";

const documentSearchResultsContainerElementId = "documentSearchResultsContainer";

const documentPdfDialogElementId = "documentPdfDialog";
const documentPdfDialogPdfContainerElementId = "documentPdfDialogPdfContainer";

async function getInvoicePdfAndOpenInDialog(invoiceNumber)
{
    const getInvoicePdfPartialResponse = await getInvoicePdfPartial(invoiceNumber);

    redirectIfResponseIsRedirected(getInvoicePdfPartialResponse);

    if (!getInvoicePdfPartialResponse.ok) return;

    const documentPdfDialogPdfContainer = document.getElementById(documentPdfDialogPdfContainerElementId);

    if (documentPdfDialogPdfContainer)
    {
        const pdfDisplayPartialAsString = await getInvoicePdfPartialResponse.text();

        documentPdfDialogPdfContainer.innerHTML = pdfDisplayPartialAsString;

        openDialog(documentPdfDialogElementId, true);
    }
}

async function getInvoicePdfPartial(invoiceNumber)
{
    const url = pageModelRoute + "?handler=GetInvoicePdfFromInvoiceNumber"
        + "&invoiceNumber=" + invoiceNumber;

    return await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

async function getSearchedDocumentsAndDisplayChanges()
{
    const documentTypeSelectElement = document.getElementById(documentTypeSelectElementId);

    const selectedDocumentType = documentTypeSelectElement.value;

    if (selectedDocumentType === "1")
    {
        await getSearchedInvoicesAndDisplayChanges();
    }
    else if (selectedDocumentType === "2")
    {
        await getSearchedWarrantyCardsAndDisplayChanges();
    }
}

async function getSearchedInvoicesAndDisplayChanges()
{
    const invoiceSearchOptions = getInvoiceSearchRequestFromValueInputs();

    const promise = getSearchedInvoices(invoiceSearchOptions);

    const response = await awaitWithCallbacks(promise,
        function () { toggleViews(documentSearchButtonElementId, documentSearchButtonLoaderElementId); },
        function () { toggleViews(documentSearchButtonElementId, documentSearchButtonLoaderElementId); });

    const searchResultsContainerElement = document.getElementById(documentSearchResultsContainerElementId);

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    if (searchResultsContainerElement)
    {
        const invoiceDataResultsPartialAsString = await response.text()

        searchResultsContainerElement.innerHTML = invoiceDataResultsPartialAsString;
    }
}

async function getSearchedInvoices(invoiceSearchRequest)
{
    const url = pageModelRoute + "?handler=GetSearchedInvoices";

    return await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(invoiceSearchRequest),
    });
}

async function getWarrantyCardPdfAndOpenInDialog(exportId)
{
    const getWarrantyCardPdfPartialResponse = await getWarrantyCardPdfPartial(exportId);

    redirectIfResponseIsRedirected(getWarrantyCardPdfPartialResponse);

    if (!getWarrantyCardPdfPartialResponse.ok) return;

    const documentPdfDialogPdfContainer = document.getElementById(documentPdfDialogPdfContainerElementId);

    if (documentPdfDialogPdfContainer)
    {
        const pdfDisplayPartialAsString = await getWarrantyCardPdfPartialResponse.text();

        documentPdfDialogPdfContainer.innerHTML = pdfDisplayPartialAsString;

        openDialog(documentPdfDialogElementId, true);
    }
}

async function getWarrantyCardPdfPartial(exportId)
{
    const url = pageModelRoute + "?handler=GetWarrantyCardPdfFromExportId"
        + "&warrantyCardExportId=" + exportId;

    return await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

async function getSearchedWarrantyCardsAndDisplayChanges()
{
    const warrantyCardSearchRequest = getWarrantyCardSearchRequestFromValueInputs();

    const promise = getSearchedWarrantyCards(warrantyCardSearchRequest);

    const response = await awaitWithCallbacks(promise,
        function () { toggleViews(documentSearchButtonElementId, documentSearchButtonLoaderElementId); },
        function () { toggleViews(documentSearchButtonElementId, documentSearchButtonLoaderElementId); });

    const searchResultsContainerElement = document.getElementById(documentSearchResultsContainerElementId);

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    if (searchResultsContainerElement)
    {
        const warrantyCardDataResultsPartialAsString = await response.text()

        searchResultsContainerElement.innerHTML = warrantyCardDataResultsPartialAsString;
    }
}

async function getSearchedWarrantyCards(warrantyCardSearchRequest)
{
    const url = pageModelRoute + "?handler=GetSearchedWarrantyCards";

    return await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(warrantyCardSearchRequest),
    });
}

function getInvoiceSearchRequestFromValueInputs()
{
    const searchInputElement = document.getElementById(documentSearchInputElementId);
    const fromDateInputElement = document.getElementById(documentFromDateSearchInputElementId);
    const toDateInputElement = document.getElementById(documentToDateSearchInputElementId);

    const searchValue = searchInputElement.value;

    const fromDateValue = getDateOrNullFromString(fromDateInputElement.value);
    const fromDateAsString = fromDateValue?.toISOString();

    const toDateValue = getDateOrNullFromString(toDateInputElement.value);
    const toDateAsString = toDateValue?.toISOString();

    return {
        InvoiceId: null,
        InvoiceNumber: null,
        Keyword: searchValue,
        FromDate: fromDateAsString,
        ToDate: toDateAsString
    };
}

function getWarrantyCardSearchRequestFromValueInputs()
{
    const searchInputElement = document.getElementById(documentSearchInputElementId);
    const fromDateInputElement = document.getElementById(documentFromDateSearchInputElementId);
    const toDateInputElement = document.getElementById(documentToDateSearchInputElementId);

    const searchValue = searchInputElement.value;

    const fromDateValue = getDateOrNullFromString(fromDateInputElement.value);
    const fromDateAsString = fromDateValue?.toISOString();

    const toDateValue = getDateOrNullFromString(toDateInputElement.value);
    const toDateAsString = toDateValue?.toISOString();

    return {
        Keyword: searchValue,
        FromDate: fromDateAsString,
        ToDate: toDateAsString
    };
}