function showSearchStringOriginDataSmallPopup(productId, index, searchStringPart, elementIdAndNamePrefix)
{
    if (productId === null
        || productId === undefined
        || (isNaN(productId) && isNaN(parseInt(productId)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    showSearchStringOriginDataSmallPopupCommon(
        elementIdAndNamePrefix + "searchStringPartOrigin_li-" + productId + "-" + index,
        elementIdAndNamePrefix + "searchStringPartOrigin_multipleOriginsDisplayList-" + productId + "-" + index,
        searchStringPart,
        elementIdAndNamePrefix + "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + productId + "-" + index,
        elementIdAndNamePrefix + "searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + productId + "-" + index);
}

function removeSearchStringOriginDataSmallPopup(productId, index, searchStringPart, elementIdAndNamePrefix)
{
    if (productId === null
        || productId === undefined
        || (isNaN(productId) && isNaN(parseInt(productId)))
        || index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    searchStringPart = decodeHtmlString(searchStringPart);

    removeSearchStringOriginDataSmallPopupCommon(
        elementIdAndNamePrefix + "searchStringPartOrigin_li-" + productId + "-" + index,
        elementIdAndNamePrefix + "searchStringPartOrigin_multipleOriginsDisplayList-" + productId + "-" + index,
        searchStringPart,
        elementIdAndNamePrefix + "searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + productId + "-" + index,
        elementIdAndNamePrefix +"searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + productId + "-" + index);
}

function addCopiedPartToTotalCopyAndMakeElementVisible(part, elementIds = null, makeElementVisible = false)
{
    addCopiedPartToTotalCopy(part);

    if (elementIds === null
        || elementIds === undefined
        || makeElementVisible === false) return;

    for (var i = 0; i < elementIds.length; i++)
    {
        changeValueOfSearchStringValue(elementIds[i]);

        var element = document.getElementById(elementIds[i]);

        element.style.visibility = "visible";
    }

}