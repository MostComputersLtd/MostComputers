const searchOptionsKey = "Products.SearchOptions";

const productSearchInputId = "product_table_search_input";
const categoryIdSelectId = "product_table_category_select";
const productStatusSelectId = "product_table_product_status_select";
const productNewStatusSelectId = "product_table_product_new_status_select";
const promotionStatusSelectId = "product_table_promotion_status_select";

$(document).ready(function ()
{
    const cachedSearchOptions = getComplexObjectFromStorage(sessionStorage, searchOptionsKey);

    if (!cachedSearchOptions) return;

    changeSearchOptionElementValuesToMatchData(cachedSearchOptions);
});

function changeSearchOptionElementValuesToMatchData(searchOptions)
{
    setInputValueIfNotNull(productSearchInputId, searchOptions.UserInputString);
    setInputValueIfNotNull(categoryIdSelectId, searchOptions.CategoryId);
    setInputValueIfNotNull(productStatusSelectId, searchOptions.ProductStatus);
    setInputValueIfNotNull(productNewStatusSelectId, searchOptions.ProductNewStatus);
    setInputValueIfNotNull(promotionStatusSelectId, searchOptions.PromotionSearchInfo);
}

async function searchProductsAndToggleSearchButtonAsync(
    productSearchInputId,
    categoryIdSelectId,
    productStatusSelectId,
    productNewStatusSelectId,
    promotionStatusSelectId,
    productsTablePartialContainerId,
    buttonElementId,
    loaderElementId)
{
    const promise = searchProductsAsync(
        productSearchInputId,
        categoryIdSelectId,
        productStatusSelectId,
        productNewStatusSelectId,
        promotionStatusSelectId,
        productsTablePartialContainerId);

    await awaitWithCallbacks(promise,
        function () { toggleViews(buttonElementId, loaderElementId); },
        function () { toggleViews(buttonElementId, loaderElementId); });
}

async function searchProductsAsync(
    productSearchInputId,
    categoryIdSelectId,
    productStatusSelectId,
    productNewStatusSelectId,
    promotionStatusSelectId,
    productsTablePartialContainerId)
{
    const productSearchInput = document.getElementById(productSearchInputId);
    const categoryIdSelect = document.getElementById(categoryIdSelectId);
    const productStatusSelect = document.getElementById(productStatusSelectId);
    const productNewStatusSelect = document.getElementById(productNewStatusSelectId);
    const promotionStatusSelect = document.getElementById(promotionStatusSelectId);

    const url = "/Index" + "?handler=GetSearchedProducts";

    const searchOptions = getProductSearchRequestAsObject(
        productSearchInput.value.trim(),
        categoryIdSelect.value,
        productStatusSelect.value,
        productNewStatusSelect.value,
        promotionStatusSelect.value);

    const response = await fetch(url,
        {
            method: 'POST',
            headers:
            {
                'Content-Type': 'application/json',
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(searchOptions)
        });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const productsTablePartialContainer = document.getElementById(productsTablePartialContainerId);

    productsTablePartialContainer.innerHTML = responseData;

    setComplexObjectToStorage(sessionStorage, searchOptionsKey, searchOptions);
}