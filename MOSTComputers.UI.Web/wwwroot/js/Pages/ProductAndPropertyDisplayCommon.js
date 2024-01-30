function showSearchStringOriginDataSmallPopupCommon(elementId, listOfMultiplePossibilitiesId, searchStringPart, nameLabelName, meaningLabelName)
{
    var searchStringPartOriginSmallPopupToShow = document.getElementById(elementId);

    if (searchStringPartOriginSmallPopupToShow.style.visibility === "visible"
        || searchStringPartOriginSmallPopupToShow.style.visibility === '') return;

    var listOfOptionsToShow = document.getElementById(listOfMultiplePossibilitiesId);

    if (listOfOptionsToShow !== null
        && listOfOptionsToShow !== undefined)
    {
        searchStringPartOrigin_multipleOriginsHighlight(nameLabelName, meaningLabelName, searchStringPart);
    }

    searchStringPartOriginSmallPopupToShow.style.visibility = "visible";
}

function removeSearchStringOriginDataSmallPopupCommon(elementId, listOfMultiplePossibilitiesId, searchStringPart, nameLabelName, meaningLabelName)
{
    searchStringPart = decodeHtmlString(searchStringPart);

    var searchStringPartOriginSmallPopupToShow = document.getElementById(elementId);

    var searchStringPartOriginMultipleOriginList = document.getElementById(listOfMultiplePossibilitiesId);

    searchStringPartOriginSmallPopupToShow.style.visibility = "hidden";

    if (searchStringPartOriginMultipleOriginList !== null)
    {
        searchStringPartOrigin_multipleOriginsRemoveHighlight(nameLabelName, meaningLabelName, searchStringPart);
    }
}

function searchStringPartOrigin_multipleOriginsHighlight(nameLabelName, meaningLabelName, substringToHighlight, spanName)
{
    substringToHighlight = decodeHtmlString(substringToHighlight);

    var nameLabels = document.getElementsByName(nameLabelName);
    var meaningLabels = document.getElementsByName(meaningLabelName);

    var allLabels = [];

    for (var i = 0; i < nameLabels.length; i++)
    {
        allLabels.push(nameLabels[i]);
    }

    for (var i = 0; i < meaningLabels.length; i++)
    {
        allLabels.push(meaningLabels[i]);
    }

    highlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight, "#FAF2B1", spanName);
}

function searchStringPartOrigin_multipleOriginsRemoveHighlight(nameLabelName, meaningLabelName, substringToHighlight)
{
    substringToHighlight = decodeHtmlString(substringToHighlight);

    var nameLabels = document.getElementsByName(nameLabelName);
    var meaningLabels = document.getElementsByName(meaningLabelName);

    var allLabels = [];

    for (var i = 0; i < nameLabels.length; i++)
    {
        allLabels.push(nameLabels[i]);
    }

    for (var i = 0; i < meaningLabels.length; i++)
    {
        allLabels.push(meaningLabels[i]);
    }

    removeHighlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight)
}