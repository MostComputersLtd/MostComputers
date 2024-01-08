function showSearchStringOriginDataSmallPopup(index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    var searchStringPartOriginSmallPopupToShow = document.getElementById("searchStringPartOrigin_li#" + index);

    searchStringPartOriginSmallPopupToShow.style.visibility = "visible";
}

function removeSearchStringOriginDataSmallPopup(index, searchStringPart)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    searchStringPart = decodeHtmlString(searchStringPart);

    var searchStringPartOriginSmallPopupToShow = document.getElementById("searchStringPartOrigin_li#" + index);

    var searchStringPartOriginMultipleOriginList = document.getElementById("searchStringPartOrigin_multipleOriginsDisplayList#" + index);

    searchStringPartOriginSmallPopupToShow.style.visibility = "hidden";

    if (searchStringPartOriginMultipleOriginList !== null
        && searchStringPartOriginMultipleOriginList.style.visibility !== "hidden")
    {
        searchStringPartOrigin_multipleOriginsShow(index, searchStringPart);
    }
}

function displayPopupAndTransportToSearchStringOriginDisplay(productId, index)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    $("#ProductSearchString_modal").one("shown.bs.modal", function()
    {
        transportToSearchStringOriginDisplay(index);
    });

    showSearchStringPopupData(productId);
}

function highlightAllElementsTextsWhereValueIsPresent(elements, value, selectionColor, customSpanName)
{
    for (var i = 0; i < elements.length; i++)
    {
        var element = elements[i];

        element.innerHTML = element.innerHTML.replace(new RegExp(value, "g"), "<span name='" + customSpanName + "'style='line-height: normal; background-color:" + selectionColor + "'>" + value + "</span>");
    }
}

function removeHighlightAllElementsTextsWhereValueIsPresent(elements, value)
{
    for (var i = 0; i < elements.length; i++)
    {
        var element = elements[i];

        var stop = false;

        var startIndex = 0;

        while (!stop)
        {
            var startIndexOfSpan = element.innerHTML.indexOf("<span", startIndex);

            // console.log(startIndexOfSpan);

            if (startIndexOfSpan < 0
                || startIndexOfSpan === null)
            {
                stop = true;

                break;
            }

            var endIndexOfSpan = element.innerHTML.indexOf("</span>") + 7;

            var substringBetweenSpan = element.innerHTML.substring(startIndexOfSpan, endIndexOfSpan);

            var dataWithExtra = substringBetweenSpan.substring(substringBetweenSpan.indexOf(">", 7));

            console.log(dataWithExtra);

            var indexOfSpanEnd = dataWithExtra.length - 7;

            var valueInSpan = dataWithExtra.substring(1, indexOfSpanEnd);

            if (valueInSpan === value)
            {
                element.innerHTML = element.innerHTML.replace(new RegExp(substringBetweenSpan, "g"), value);

                stop = true;

                break;
            }

            startIndex = endIndexOfSpan;
        }
    }
}

function highlightStringTextsWhereValueIsPresent(valueToHighlight, substring, selectionColor, customSpanName)
{
    valueToHighlight = valueToHighlight.replace(new RegExp(substring, "g"), "<span name='" + customSpanName + "'style='line-height: normal; background-color:" + selectionColor + "'>" + substring + "</span>");

    return valueToHighlight;
}

function showXmlPopupData(productId)
{
    $("#ProductXml_popup_modal-content").load("/ProductPropertiesEditor/" + productId + "?handler=PartialViewXmlForProduct");

    open_ProductXml_modal();
}

function copyXmlTextToClipboard()
{
    var xmlTextAreaValue = getXmlValue();

    if (xmlTextAreaValue === null) return;

    navigator.clipboard.writeText(xmlTextAreaValue);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}

//document.onkeydown = (e) =>
//{
//    if (e.ctrlKey && e.key === 's')
//    {
//        let xmlTextArea = document.getElementById("Xml_textarea");

//        if (xmlTextArea != null)
//        {
//            if (xmlTextArea.value != null)
//            {
//                e.preventDefault();

//                copyXmlTextToClipboard();

//                return;
//            }
//        }

//        let productImagesPopup = document.getElementById("ProductImages_popup");

//        if (productImagesPopup === null) return;

//        e.preventDefault();

//        copyImageDataToClipboard();
//    }
//    else if (e.ctrlKey && e.key === 'a')
//    {
//        e.preventDefault();

//        var productId = getProductId();

//        getImageFileData(productId);
//    }
//}

function showImagePopupData(productId)
{
    $("#ProductImages_popup_modal-content").load("/ProductPropertiesEditor/" + productId + "?handler=PartialViewImagesForProduct");

    open_ProductImages_modal();
}

function copyImageDataToClipboard()
{
    let selectedImageData = getSelectedImageData();

    navigator.clipboard.writeText(selectedImageData);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}

function getImageFileData(productId)
{
    var imageIndex = getSelectedImageIndex(productId);

    var urlFileResult = "/ProductPropertiesEditor/" + productId + "?handler=CurrentImageFileResultSingle" + "&imageIndex=" + imageIndex;

    window.location.href = urlFileResult;

    showNotificationWithText("copiedXmlNotificationBox", "Saved!", "copy-to-xml-success-message");
}

function showSearchStringPopupData(productId)
{
    $("#ProductSearchString_popup_modal-content").load("/ProductPropertiesEditor/" + productId + "?handler=GetSearchStringPartialView", function()
    {
        open_ProductSearchString_modal();
    });

}

function copySearchStringDataToClipboard()
{
    let searchStringData = getSearchStringData();

    if (searchStringData === null
    || searchStringData === undefined) return;

    navigator.clipboard.writeText(searchStringData);

    showNotificationWithText("copiedXmlNotificationBox", "Copied!", "copy-to-xml-success-message");
}

function searchStringPartOrigin_multipleOriginsShow(index, substringToHighlight)
{
    if (index === null
        || index === undefined
        || (isNaN(index) && isNaN(parseInt(index)))) return;

    substringToHighlight = decodeHtmlString(substringToHighlight);

    var listOfOptionsToShow = document.getElementById("searchStringPartOrigin_multipleOriginsDisplayList#" + index);

    var nameLabels = document.getElementsByName("searchStringPartOrigin_multipleOriginsDisplayList_nameLabel#" + index);
    var meaningLabels = document.getElementsByName("searchStringPartOrigin_multipleOriginsDisplayList_meaningLabel#" + index);

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
        listOfOptionsToShow.style.visibility = "visible";

        highlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight, "#FAF2B1", "customSearchStringSelectSpan");
    }
    else
    {
        listOfOptionsToShow.style.visibility = "hidden";

        removeHighlightAllElementsTextsWhereValueIsPresent(allLabels, substringToHighlight)
    }
}
   
function RemoveOldSelectedText(valueTextArea)
{
    var oldSelectedSpanStartIndex = valueTextArea.innerHTML.indexOf("<span");

    if (oldSelectedSpanStartIndex !== -1)
    {
        var oldSelectedSpanEndIndex = valueTextArea.innerHTML.indexOf("</span>") + 7;

        var oldSelectedValueSpan = valueTextArea.innerHTML.substring(oldSelectedSpanStartIndex, oldSelectedSpanEndIndex);

        var oldSelectedValueStartIndex = valueTextArea.innerHTML.indexOf('>', oldSelectedSpanStartIndex) + 1;
        var oldSelectedValueEndIndex = valueTextArea.innerHTML.indexOf('<', oldSelectedSpanStartIndex + 5);

        var oldSelectedValue = valueTextArea.innerHTML.substring(oldSelectedValueStartIndex, oldSelectedValueEndIndex);

        valueTextArea.innerHTML = valueTextArea.innerHTML.replace(new RegExp(oldSelectedValueSpan, "g"), oldSelectedValue);
    }
}

function RemoveSpanFromSelectedText(text)
{
    var oldSelectedSpanStartIndex = text.indexOf("<span");

    if (oldSelectedSpanStartIndex !== -1)
    {
        var oldSelectedSpanEndIndex = text.indexOf("</span>") + 7;

        var oldSelectedValueSpan = text.substring(oldSelectedSpanStartIndex, oldSelectedSpanEndIndex);

        var oldSelectedValueStartIndex = text.indexOf('>', oldSelectedSpanStartIndex) + 1;
        var oldSelectedValueEndIndex = text.indexOf('<', oldSelectedSpanStartIndex + 5);

        var oldSelectedValue = text.substring(oldSelectedValueStartIndex, oldSelectedValueEndIndex);

        text = text.replace(new RegExp(oldSelectedValueSpan, "g"), oldSelectedValue);
    }

    return text;
}

function addPropertyToTable(productId)
{
    var url = "/ProductPropertiesEditor/" + productId + "?handler=AddNewItem";

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            console.log(result);

            document.getElementById("propertyDisplay_table_tbody").innerHTML += result;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function PropertyWithoutCharacteristic_select_onchange(value, index)
{
    var labelWithIdText = document.getElementById("productPropertyDisplay_CharacteristicId_Data_label#" + index);

    labelWithIdText.textContent = "ID: " + value;
}

function updatePropertyInTable(productId, index)
{
    var url = "/ProductPropertiesEditor/" + productId + "?handler=UpdateProperty";

    var productPropCharacteristicIdInput = document.getElementById("productPropertyDisplay_CharacteristicId_Data_label#" + index);
    var productPropValueInput = document.getElementById("productPropertyDisplay_Value_div#" + index);
    var productPropXmlPlacementSelect = document.getElementById("productPropertyDisplay_XmlPlacement_select#" + index);
    var productPropWithoutCharacteristicSelect = document.getElementById("PropertyWithoutCharacteristic_Characteristic_select#" + index);

    var productPropCharacteristicId;

    var productPropIsNew;

    if (productPropCharacteristicIdInput !== null)
    {
        var idText = productPropCharacteristicIdInput.innerHTML;

        var idStartIndex = idText.indexOf("ID:") + 4;

        var id = idText.substring(idStartIndex);

        productPropCharacteristicId = parseInt(id);
        productPropIsNew = false;
    }

    var productPropValue;

    if (productPropValueInput.innerHTML !== null)
    {
        productPropValue = RemoveSpanFromSelectedText(productPropValueInput.innerHTML);
    }

    var productPropXmlPlacement;

    var productXmlPlacementString = productPropXmlPlacementSelect.value;

    productPropXmlPlacement = parseInt(productXmlPlacementString);

    if (productPropWithoutCharacteristicSelect !== null)
    {
        var productPropWithoutCharacteristicValue = productPropWithoutCharacteristicSelect.value;

        productPropCharacteristicId = parseInt(productPropWithoutCharacteristicValue);
        productPropIsNew = true;

    }

    var data =
    {
        ProductCharacteristicId: productPropCharacteristicId,
        Value: productPropValue,
        XmlPlacement: productPropXmlPlacement,
        IsNew: productPropIsNew
    };

    // console.log(data);

    $.ajax({
        type: "PUT",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result) {
            // document.getElementById("propertyDisplay_table").innerHTML += result;
        })
        .fail(function (jqXHR, textStatus) {
        });
}

function updateAllPropertiesInTable(productId)
{
    var url = "/ProductPropertiesEditor/" + productId + "?handler=UpdateAndInsertProperties";

    var productPropCharacteristicIdInputs = document.getElementsByName("productPropertyDisplay_CharacteristicId_Data_label");
    var productPropValueInputs = document.getElementsByName("productPropertyDisplay_Value_div");
    var productPropXmlPlacementSelects = document.getElementsByName("productPropertyDisplay_XmlPlacement_select");
    var productPropWithoutCharacteristicSelects = document.getElementsByName("PropertyWithoutCharacteristic_Characteristic_select");

    var productPropCharacteristicIds = [];

    var productPropIsNew = [];

    for (var i = 0; i < productPropCharacteristicIdInputs.length; i++)
    {
        var idText = productPropCharacteristicIdInputs[i].innerHTML;

        var idStartIndex = idText.indexOf("ID:") + 4

        var id = idText.substring(idStartIndex);

        productPropCharacteristicIds.push(parseInt(id));

        productPropIsNew.push(false);
    }

    var productPropValues = [];

    for (var i = 0; i < productPropValueInputs.length; i++)
    {
        productPropValues.push(RemoveSpanFromSelectedText(productPropValueInputs[i].innerHTML));
    }

    var productPropXmlPlacements = [];

    for (var i = 0; i < productPropXmlPlacementSelects.length; i++)
    {
        var productXmlPlacementString = productPropXmlPlacementSelects[i].value;

        productPropXmlPlacements.push(parseInt(productXmlPlacementString));
    }

    console.log(productPropWithoutCharacteristicSelects);

    for (var i = 0; i < productPropWithoutCharacteristicSelects.length; i++)
    {
        var productPropWithoutCharacteristicSelect = productPropWithoutCharacteristicSelects[i]

        var indexOfListIndexStart = productPropWithoutCharacteristicSelect.id.indexOf('#') + 1;

        var indexOfSelect = productPropWithoutCharacteristicSelect.id.substring(indexOfListIndexStart);
            
        var productPropWithoutCharacteristicValue = productPropWithoutCharacteristicSelect.value;

        productPropCharacteristicIds.splice(parseInt(indexOfSelect), 0, parseInt(productPropWithoutCharacteristicValue));
        productPropIsNew.splice(parseInt(indexOfSelect), 0, true);
    }

    // console.log(productPropIsNew);

    var data = [];

    for (var i = 0; i < productPropCharacteristicIdInputs.length; i++)
    {
        var dataEntry =
        {
            ProductCharacteristicId: productPropCharacteristicIds[i],
            Value: productPropValues[i],
            XmlPlacement: productPropXmlPlacements[i],
            IsNew: productPropIsNew[i]
        };

        data.push(dataEntry);
    }

    // console.log(data);

    $.ajax({
        type: "PUT",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            // document.getElementById("propertyDisplay_table").innerHTML += result;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function deletePropertyInTable(productId, productCharacteristicId, index, deleteEvenOnFail)
{
    if (productCharacteristicId === null
        || isNaN(productCharacteristicId)
        || isNaN(parseInt(productCharacteristicId))
        || productCharacteristicId === undefined
        || productCharacteristicId <= 0)
    {
        if (deleteEvenOnFail)
        {
            var childProperty = document.getElementById("propertytr#" + index);

            document.getElementById("propertyDisplay_table_tbody").removeChild(childProperty);

            return;
        }

        return;
    }

    var url = "/ProductPropertiesEditor/" + productId + "?handler=DeleteProperty&productCharacteristicId=" + productCharacteristicId;

    // console.log(data);

    $.ajax({
        type: "DELETE",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            var childProperty = document.getElementById("propertytr#" + index);

            document.getElementById("propertyDisplay_table_tbody").removeChild(childProperty);
        })
        .fail(function (jqXHR, textStatus)
        {
            if (deleteEvenOnFail)
            {
                var childProperty = document.getElementById("propertytr#" + index);

                document.getElementById("propertyDisplay_table_tbody").removeChild(childProperty);
            }
        });
}

function getCurrentId(index)
{
    var characteristicIdEl = document.getElementById("productPropertyDisplay_CharacteristicId_Data_label#" + index);

    var idText = characteristicIdEl.innerHTML;

    var idStartIndex = idText.indexOf("ID:") + 4;

    var id = idText.substring(idStartIndex);

    console.log("getCurrentId " + parseInt(id));

    return parseInt(id);
}