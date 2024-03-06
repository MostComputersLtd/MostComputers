function showSearchStringOriginDataSmallPopup(productId, index, searchStringPart)
{
    if (productId === null
        || productId === undefined
        || (isNaN(productId) && isNaN(parseInt(productId)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    showSearchStringOriginDataSmallPopupCommon(
        "searchStringPartOrigin_li#" + productId + "#" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList#" + productId + "#" + index,
        searchStringPart,
        "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel#" + productId + "#" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel#" + productId + "#" + index);
}

function removeSearchStringOriginDataSmallPopup(productId, index, searchStringPart)
{
    if (productId === null
        || productId === undefined
        || (isNaN(productId) && isNaN(parseInt(productId)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    searchStringPart = decodeHtmlString(searchStringPart);

    removeSearchStringOriginDataSmallPopupCommon(
        "searchStringPartOrigin_li#" + productId + "#" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList#" + productId + "#" + index,
        searchStringPart,
        "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel#" + productId + "#" + index,
        "searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel#" + productId + "#" + index);
}

function addCopiedPartToTotalCopyAndMakeElementVisible(part, elementId = null, makeElementVisible = false)
{
    addCopiedPartToTotalCopy(part, elementId);

    if (elementId === null
        || elementId === undefined
        || makeElementVisible === false) return;

    var element = document.getElementById(elementId);

    element.style.visibility = "visible";
}