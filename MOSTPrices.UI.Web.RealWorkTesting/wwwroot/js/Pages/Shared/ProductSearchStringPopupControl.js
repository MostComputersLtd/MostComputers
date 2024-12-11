function open_ProductSearchString_modal(modalId, modalDialogId)
{
    openModal(modalId, modalDialogId);

    var idsOfMultipleOriginsLists = getMultipleOriginsListIds();

    window.addEventListener("resize", function ()
    {
        for (var i = 0; i < idsOfMultipleOriginsLists.length; i++)
        {
            placeFixedListInTheCorrectPosition(idsOfMultipleOriginsLists[i]);
        }
    });

    document.getElementById("ProductSearchString_popup_searchStringPartsOrigin_ul").onscroll = function ()
    {
        for (var i = 0; i < idsOfMultipleOriginsLists.length; i++)
        {
            searchStringPartOrigin_multipleOriginsRemove(idsOfMultipleOriginsLists[i]);
        }
    };
}

function getMultipleOriginsListIds()
{
    var multipleOriginsLists = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList");

    var output = [];

    for (var i = 0; i < multipleOriginsLists.length; i++)
    {
        var multipleOriginList = multipleOriginsLists[i];

        var indexFromId = getIndexOfItemFromId(multipleOriginList.id);

        output.push(indexFromId);
    }

    return output;
}

function getSearchStringData()
{
    var searcStringDisplayLabel = document.getElementById("__hidden_ProductSearchString_popup_searchString_label");

    if (searcStringDisplayLabel === null) return null;

    var searchStringText = searcStringDisplayLabel.textContent;

    if (searchStringText === null) return null;

    return searchStringText;
}

//function colorSearchStringOriginDisplay(index, color)
//{
//    if (index === null #
//        || index === undefined
//        || (isNaN(index) && isNaN(parseInt(index)))) return null;

//    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(index);

//    searchStringPartOriginDisplayToSelect.style.backgroundColor = color;
//}

function colorSearchStringOriginDisplayAndGiveItABorder(index, color, borderWidth, borderColor, borderRadius)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return null;

    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(index);

    searchStringPartOriginDisplayToSelect.style.backgroundColor = color;

    if (borderWidth !== null && borderColor !== null)
    {
        searchStringPartOriginDisplayToSelect.style.border = borderWidth + " solid " + borderColor;
        searchStringPartOriginDisplayToSelect.style.borderRadius = borderRadius;
    }
}

function transportToSearchStringOriginDisplay(index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return null;

    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(index);

    var searchStringTopDisplayList = document.getElementById("ProductSearchString_popup_searchStringPartDisplay_ul");
    var modalHeader = document.getElementById("ProductSearchString_modal_header");
    var modalFooter = document.getElementById("ProductSearchString_modal_footer");

    var searchStringPartOriginDisplayList = document.getElementById("ProductSearchString_popup_searchStringPartsOrigin_ul");

    searchStringPartOriginDisplayList.scrollTop =
        (searchStringPartOriginDisplayToSelect.offsetTop - searchStringTopDisplayList.offsetHeight - modalHeader.offsetHeight - modalFooter.offsetHeight + 28)
}

function findSearchStringPartOriginsDisplayFromSearchStringPart(index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return null;

    var searchStringPartLowerDisplays = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_li-" + index);

    return searchStringPartLowerDisplays;
}

function getIndexOfItemFromId(id)
{
    if (id === null
        || id === undefined) return null;

    var searchStringPartLowerDisplayIndex = id.indexOf("-") + 1;

    var indexAsString = id.substring(searchStringPartLowerDisplayIndex);

    return parseInt(indexAsString);
}

function ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsShow(index, substringToHighlight)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    var listOfOptionsToShow = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList-" + index);

    var nameLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + index);
    var meaningLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + index);

    var allLabels = [];

    for (var i = 0; i < nameLabels.length; i++)
    {
        allLabels.push(nameLabels[i]);
    }

    for (var i = 0; i < meaningLabels.length; i++)
    {
        allLabels.push(meaningLabels[i]);
    }

    if (listOfOptionsToShow.style.visibility === "hidden")
    {
        placeFixedListInTheCorrectPosition(index);

        listOfOptionsToShow.style.visibility = "visible";

        highlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight, "#FAF2B1", "customSearchStringSelectSpan");
    }
    else
    {
        listOfOptionsToShow.style.visibility = "hidden";

        removeHighlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight)
    }
}

function searchStringPartOrigin_multipleOriginsRemove(index, substringToHighlight)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    var listOfOptionsToShow = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList-" + index);

    if (listOfOptionsToShow.visibility == "hidden") return;

    var nameLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_nameLabel-" + index);
    var meaningLabels = document.getElementsByName("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel-" + index);

    var allLabels = [];

    for (var i = 0; i < nameLabels.length; i++)
    {
        allLabels.push(nameLabels[i]);
    }

    for (var i = 0; i < meaningLabels.length; i++)
    {
        allLabels.push(meaningLabels[i]);
    }

    listOfOptionsToShow.style.visibility = "hidden";

    removeHighlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight)
}

function placeFixedListInTheCorrectPosition(index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    var listOfOptionsToShow = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayList-" + index);
    var buttonToShowOptions = document.getElementById("ProductSearchStringPopup_searchStringPartOrigin_multipleOriginsDisplayButton-" + index);

    var popupDialog = document.getElementById("ProductSearchString_popup_searchStringPartsOrigin_ul");

    var rectOfDialog = popupDialog.getBoundingClientRect();
    var rectOfButton = buttonToShowOptions.getBoundingClientRect();

    listOfOptionsToShow.style.maxWidth = popupDialog.clientWidth;
    listOfOptionsToShow.style.height = listOfOptionsToShow.offsetHeight;

    listOfOptionsToShow.style.top = (rectOfButton.top + buttonToShowOptions.offsetHeight) + "px";
    listOfOptionsToShow.style.left = rectOfButton.left + "px";

    var rectOfList = listOfOptionsToShow.getBoundingClientRect();

    var listRight = window.innerWidth - rectOfList.right;
    var dialogRight = window.innerWidth - rectOfDialog.right;

    var listBottom = window.innerHeight - rectOfList.bottom;

    if (listRight < 0)
    {
        listOfOptionsToShow.style.right = dialogRight + "px";
        listOfOptionsToShow.style.left = "";
    }

    if (rectOfList.left < 0)
    {
        listOfOptionsToShow.style.left = 0 + "px";
        listOfOptionsToShow.style.right = "";
    }

    if (listBottom < 0)
    {
        listOfOptionsToShow.style.bottom = (rectOfButton.top - listOfOptionsToShow.clientHeight - buttonToShowOptions.offsetHeight) + "px";
        listOfOptionsToShow.style.top = "";
    }

    if (rectOfList.top < 0)
    {
        listOfOptionsToShow.style.bottom = "";
    }
}