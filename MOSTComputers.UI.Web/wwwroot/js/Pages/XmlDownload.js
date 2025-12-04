const searchOptionValueElementIds =
{
    productSearchInputId: "xmlDownloadProductIdInput",
    categoryIdSelectId: "xmlDownloadCategorySelect",
    productStatusSelectId: "xmlDownloadProductStatusSelect",
    productNewStatusSelectId: "xmlDownloadProductNewStatusSelect",
    promotionStatusSelectId: "xmlDownloadPromotionStatusSelect"
};

async function openXmlFromOptionsInNewWindow(
    buttonElementId = null,
    loaderElementId = null)
{
    const searchOptions = getSearchOptionsFromValueInputs();

    const promise = getXmlOfAllSearchedProductsAsync(searchOptions);

    const response = await awaitWithCallbacks(promise,
        function () { toggleViews(buttonElementId, loaderElementId); },
        function () { toggleViews(buttonElementId, loaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openXmlDataInNewWindow(responseData);
}

async function getXmlOfAllSearchedProductsAsync(searchOptions)
{
    const url = "/XmlDownload" + "?handler=GetXmlDataForSearchedProducts";

    return await fetch(url,
    {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(searchOptions)
    });
}

function getSearchOptionsFromValueInputs()
{
    const productSearchInput = document.getElementById(searchOptionValueElementIds.productSearchInputId);
    const categoryIdSelect = document.getElementById(searchOptionValueElementIds.categoryIdSelectId);
    const productStatusSelect = document.getElementById(searchOptionValueElementIds.productStatusSelectId);
    const productNewStatusesSelect = document.getElementById(searchOptionValueElementIds.productNewStatusSelectId);
    const promotionStatusSelect = document.getElementById(searchOptionValueElementIds.promotionStatusSelectId);

    const searchOptions = getProductSearchRequestAsObject(
        productSearchInput.value.trim(),
        categoryIdSelect.value,
        productStatusSelect.value,
        productNewStatusesSelect.value,
        promotionStatusSelect.value);

    return searchOptions;
}