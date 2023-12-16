function open_ProductSearchString_modal()
{
    var dialog = document.getElementById("ProductSearchString-modal-dialog");

    dialog.style.height = window.innerHeight;

    $("#ProductSearchString_modal").modal("show");
}

function close_ProductSearchString_modal()
{
    $("#ProductSearchString_modal").modal("toggle");
}

function getSearchStringData()
{
    var searcStringDisplayLabel = document.getElementById("__hidden_ProductSearchString_popup_searchString_label");

    if (searcStringDisplayLabel === null) return null;

    var searchStringText = searcStringDisplayLabel.textContent;

    if (searchStringText === null) return null;

    return searchStringText;
}

function colorSearchStringOriginDisplay(searchStringPartValue, color)
{
    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(searchStringPartValue);

    searchStringPartOriginDisplayToSelect.style.backgroundColor = color;
}

function colorSearchStringOriginDisplayAndGiveItABorder(searchStringPartValue, color, borderWidth, borderColor, borderRadius)
{
    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(searchStringPartValue);

    searchStringPartOriginDisplayToSelect.style.backgroundColor = color;

    if (borderWidth !== null && borderColor !== null)
    {
        searchStringPartOriginDisplayToSelect.style.border = borderWidth + " solid " + borderColor;
        searchStringPartOriginDisplayToSelect.style.borderRadius = borderRadius;
    }
}

function transportToSearchStringOriginDisplay(searchStringPartValue)
{
    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(searchStringPartValue);

    var searchStringTopDisplayList = document.getElementById("ProductSearchString_popup_searchStringPartDisplay_ul");
    var modalHeader = document.getElementById("ProductSearchString_modal-header");
    var modalFooter = document.getElementById("ProductSearchString_modal-footer");

    var searchStringPartOriginDisplayList = document.getElementById("ProductSearchString_popup_searchStringPartsOrigin_ul");

    searchStringPartOriginDisplayList.scrollTop =
        (searchStringPartOriginDisplayToSelect.offsetTop - searchStringTopDisplayList.offsetHeight - modalHeader.offsetHeight - modalFooter.offsetHeight + 28)
}

function findSearchStringPartOriginsDisplayFromSearchStringPart(searchStringPartValue)
{
    if (searchStringPartValue === null
        || searchStringPartValue === undefined) return null;

    var searchStringPartLowerDisplays = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_searchStringPartDisplay");

    for (var i = 0; i < searchStringPartLowerDisplays.length; i++)
    {
        var searchStringPartLowerDisplay = searchStringPartLowerDisplays[i];

        if (searchStringPartLowerDisplay.textContent.trim() === searchStringPartValue.trim())
        {
            var searchStringPartLowerDisplayIndex = searchStringPartLowerDisplay.id.indexOf("#") + 1;

            var indexAsString = searchStringPartLowerDisplay.id.substring(searchStringPartLowerDisplayIndex);

            var searchStringPartOriginDisplayToSelect = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_li#" + indexAsString);

            return searchStringPartOriginDisplayToSelect;
        }
    }

    return null;
}

function getIndexOfItemFromId(id)
{
    if (id === null
        || id === undefined) return null;

    var searchStringPartLowerDisplayIndex = id.indexOf("#") + 1;

    var indexAsString = id.substring(searchStringPartLowerDisplayIndex);

    return indexAsString;
}

function ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsShow(index, substringToHighlight)
{
    if (index === null
        || index === undefined) return;

    var listOfOptionsToShow = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList#" + index);

    var nameLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_nameLabel");
    var meaningLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel");

    var allLabels = [];

    console.log(nameLabels);
    console.log(meaningLabels);

    for (var i = 0; i < nameLabels.length; i++)
    {
        allLabels.push(nameLabels[i]);
    }

    for (var i = 0; i < meaningLabels.length; i++)
    {
        allLabels.push(meaningLabels[i]);
    }

    if (listOfOptionsToShow.style.visibility === "collapse")
    {
        listOfOptionsToShow.style.visibility = "visible";


        highlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight, "#FAF2B1", "customSearchStringSelectSpan");
    }
    else
    {
        listOfOptionsToShow.style.visibility = "collapse";

        removeHighlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight)
    }
}