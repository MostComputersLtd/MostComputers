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
        searchStringPartOriginDisplayToSelect.style.borderColor = borderColor;
        searchStringPartOriginDisplayToSelect.style.borderWidth = borderWidth;
        searchStringPartOriginDisplayToSelect.style.borderRadius = borderRadius;
    }
}

function transportToSearchStringOriginDisplay(searchStringPartValue)
{
    var searchStringPartOriginDisplayToSelect = findSearchStringPartOriginsDisplayFromSearchStringPart(searchStringPartValue);

    var searchStringPartOriginDisplayList = document.getElementById("ProductSearchString_popup_searchStringPartsOrigin_ul");

    searchStringPartOriginDisplayList.scrollTop = (searchStringPartOriginDisplayToSelect.offsetTop - searchStringPartOriginDisplayToSelect.clientHeight + searchStringPartOriginDisplayToSelect.style.marginTop);
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