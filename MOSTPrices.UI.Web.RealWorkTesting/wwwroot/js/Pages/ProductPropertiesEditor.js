function copySearchStringPartToClipboard(data, notificationBoxId = null)
{
    navigator.clipboard.writeText(data);

    showNotificationWithText(notificationBoxId, "Copied!", "notificationBox-short-message");
}

function showXmlPopupData(productId, productXmlPopupContentId)
{
    $("#" + productXmlPopupContentId).load("/ProductPropertiesEditor/" + productId + "?handler=GetPartialViewXmlForProduct");

    open_ProductXml_modal();
}

function copyXmlTextToClipboard(notificationBoxId = null)
{
    var xmlTextAreaValue = getXmlValue();

    if (xmlTextAreaValue === null) return;

    navigator.clipboard.writeText(xmlTextAreaValue);

    showNotificationWithText(notificationBoxId, "Copied!", "notificationBox-short-message");
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

function showImagePopupData(productId, productImagePopupContentId)
{
    $("#" + productImagePopupContentId).load("/ProductPropertiesEditor/" + productId + "?handler=GetPartialViewImagesForProduct");

    open_ProductImages_modal();
}

function getImageFileData(productId, notificationBoxId = null)
{
    var imageIndex = getSelectedImageIndex(productId);

    var urlFileResult = "/ProductPropertiesEditor/" + productId + "?handler=CurrentImageFileResultSingle" + "&imageIndex=" + imageIndex;

    window.location.href = urlFileResult;

    showNotificationWithText(notificationBoxId, "Saved!", "notificationBox-short-message");
}

//function showSearchStringPopupData(productId, productSearchStringPopupContentId)
//{
//    $("#" + productSearchStringPopupContentId).load("/ProductPropertiesEditor/" + productId + "?handler=GetSearchStringPartialView", function()
//    {
//        open_ProductSearchString_modal();
//    });
//}

//function copySearchStringDataToClipboard(notificationBoxId = null)
//{
//    let searchStringData = getSearchStringData();

//    if (searchStringData === null
//    || searchStringData === undefined) return;

//    navigator.clipboard.writeText(searchStringData);

//    showNotificationWithText(notificationBoxId, "Copied!", "notificationBox-short-message");
//}

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

function addPropertyToTable(productId, propertyTableBodyId)
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
            var propertyTableBody = document.getElementById(propertyTableBodyId)

            if (propertyTableBody == null) return;

            propertyTableBody.innerHTML += result;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function PropertyWithoutCharacteristic_select_onchange(value, propertyCharacteristicDataLabelId)
{
    var labelWithIdText = document.getElementById(propertyCharacteristicDataLabelId);

    labelWithIdText.textContent = "ID: " + value;
}

function updatePropertyInTable(
    productId,
    propertyCharacteristicDataLabelId,
    propertyValueDivId,
    propertyXmlPlacementSelectId,
    propertyCharacteristicSelectId,
    hiddenPropertyIsNewCheckboxId,
    notificationBoxId = null)
{
    var url = "/ProductPropertiesEditor/" + productId + "?handler=UpdateProperty";

    var productPropCharacteristicIdInput = document.getElementById(propertyCharacteristicDataLabelId);
    var productPropValueInput = document.getElementById(propertyValueDivId);
    var productPropXmlPlacementSelect = document.getElementById(propertyXmlPlacementSelectId);
    var productPropCharacteristicSelect = document.getElementById(propertyCharacteristicSelectId);
    var productPropHiddenIsNewCheckbox = document.getElementById(hiddenPropertyIsNewCheckboxId);

    var productPropCharacteristicId;

    var productPropIsNew = productPropHiddenIsNewCheckbox.checked;

    if (productPropCharacteristicIdInput !== null)
    {
        var idText = productPropCharacteristicIdInput.innerHTML;

        var idStartIndex = idText.indexOf("ID:") + 4;

        var id = idText.substring(idStartIndex);

        productPropCharacteristicId = parseInt(id);
    }

    var productPropValue;

    if (productPropValueInput.innerHTML !== null)
    {
        productPropValue = RemoveSpanFromSelectedText(productPropValueInput.innerHTML);
    }

    var productPropXmlPlacement;

    var productXmlPlacementString = productPropXmlPlacementSelect.value;

    productPropXmlPlacement = parseInt(productXmlPlacementString);

    if (productPropCharacteristicSelect !== null
        && isNaN(productPropCharacteristicId))
    {
        var productPropWithoutCharacteristicValue = productPropCharacteristicSelect.value;

        productPropCharacteristicId = parseInt(productPropWithoutCharacteristicValue);
    }

    var data =
    {
        ProductCharacteristicId: productPropCharacteristicId,
        Value: productPropValue,
        XmlPlacement: productPropXmlPlacement,
        IsNew: productPropIsNew
    };

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
        })
        .fail(function (jqXHR, textStatus)
        {
            showNotificationWithText(notificationBoxId, "Failed to update property", "notificationBox-long-message");
        });
}

function updateAllPropertiesInTable(
    productId,
    propertyCharacteristicDataLabelName,
    propertyValueDivName,
    propertyXmlPlacementSelectName,
    propertyCharacteristicSelectName,
    hiddenPropertyIsNewCheckboxName,
    notificationBoxId = null)
{
    var url = "/ProductPropertiesEditor/" + productId + "?handler=UpdateAndInsertProperties";

    var productPropCharacteristicIdInputs = document.getElementsByName(propertyCharacteristicDataLabelName);
    var productPropValueInputs = document.getElementsByName(propertyValueDivName);
    var productPropXmlPlacementSelects = document.getElementsByName(propertyXmlPlacementSelectName);
    var productPropWithoutCharacteristicSelects = document.getElementsByName(propertyCharacteristicSelectName);
    var hiddenPropertyIsNewCheckboxes = document.getElementsByName(hiddenPropertyIsNewCheckboxName);

    var productPropCharacteristicIds = [];

    var productPropIsNew = [];

    for (var i = 0; i < productPropCharacteristicIdInputs.length; i++)
    {
        var idText = productPropCharacteristicIdInputs[i].innerHTML;

        var idStartIndex = idText.indexOf("ID:") + 4

        var id = idText.substring(idStartIndex);

        productPropCharacteristicIds.push(parseInt(id));
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

        var indexOfListIndexStart = productPropWithoutCharacteristicSelect.id.indexOf('-') + 1;

        var indexOfSelect = productPropWithoutCharacteristicSelect.id.substring(indexOfListIndexStart);
            
        var productPropWithoutCharacteristicValue = productPropWithoutCharacteristicSelect.value;

        productPropCharacteristicIds.splice(parseInt(indexOfSelect), 0, parseInt(productPropWithoutCharacteristicValue));
    }

    for (var i = 0; i < hiddenPropertyIsNewCheckboxes.length; i++)
    {
        var hiddenPropertyIsNewCheckbox = hiddenPropertyIsNewCheckboxes[i];

        productPropIsNew.push(hiddenPropertyIsNewCheckbox.checked);
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
        })
        .fail(function (jqXHR, textStatus)
        {
            showNotificationWithText(notificationBoxId, "Failed to update property", "notificationBox-long-message");
        });
}

function productPropertyDisplay_Characteristic_select_onchange(
    productId,
    characteristicSelectNewValue,
    productCharacteristicSelectName,
    currentProductCharacteristicSelectId,
    propertyIdLabelId)
{
    var currentSelectElement = document.getElementById(currentProductCharacteristicSelectId);

    var currentSelectElementPreviousValue = currentSelectElement.getAttribute("data-previous-value");

    var elementWithPreviousValue = null;

    for (var i = 0; i < currentSelectElement.options.length; i++)
    {
        var option = currentSelectElement.options[i];

        if (option.value !== currentSelectElementPreviousValue) continue;

        elementWithPreviousValue = option;

        break;
    }

    const query = '[name="' + productCharacteristicSelectName + '"]';

    var productCharacteristicSelects = [...document.querySelectorAll(query)];

    for (var i = 0; i < productCharacteristicSelects.length; i++)
    {
        var productCharacteristicSelect = productCharacteristicSelects[i];

        if (productCharacteristicSelect.id === currentProductCharacteristicSelectId)
        {
            continue;
        }

        var selectContainsLastValue = false;

        for (var k = 0; k < productCharacteristicSelect.options.length; k++)
        {
            var productCharacteristicSelectItem = productCharacteristicSelect.options[k];

            if (productCharacteristicSelectItem.value === characteristicSelectNewValue)
            {
                productCharacteristicSelect.options[k].remove();

                break;
            }

            if (productCharacteristicSelectItem.value === currentSelectElementPreviousValue
                && currentSelectElementPreviousValue != null)
            {
                selectContainsLastValue = true;
            }
        }
        
        if (!selectContainsLastValue
            && elementWithPreviousValue != null)
        {
            var newOption = document.createElement("option");

            newOption.text = elementWithPreviousValue.text;
            newOption.value = elementWithPreviousValue.value;

            productCharacteristicSelect.appendChild(newOption);
        }
    }

    updatePropertyCharacteristicId(productId, parseInt(currentSelectElementPreviousValue), characteristicSelectNewValue)
        .then(function (response)
        {
            var propertyIdLabel = document.getElementById(propertyIdLabelId);

            if (propertyIdLabel == null) return;

            propertyIdLabel.textContent = "ID: " + characteristicSelectNewValue;
        });

    currentSelectElement.setAttribute("data-previous-value", characteristicSelectNewValue);
}


function updatePropertyCharacteristicId(
    productId,
    currentId,
    characteristicSelectNewValue)
{
    var newIdInt = parseInt(characteristicSelectNewValue);

    if (typeof productId !== "number"
        || productId <= 0
        ||typeof currentId !== "number"
        || currentId <= 0
        || isNaN(newIdInt)
        || newIdInt <= 0)
    {
        return null;
    }

    return fetch("/ProductPropertiesEditor/" + productId + "?handler=ChangePropertyCharacteristicId" +
        "&currentPropertyId=" + currentId + "&newPropertyId=" + newIdInt, {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

async function togglePropertyCharacteristicAndSelect(
    productId,
    characteristicLabelId,
    characteristicSelectId,
    productCharacteristicSelectName,
    characteristicIdLabelId)
{
    var characteristicLabel = document.getElementById(characteristicLabelId);

    var characteristicSelect = document.getElementById(characteristicSelectId);

    if (characteristicLabel == null
        || characteristicSelect == null) return;

    if (characteristicSelect.style.display === "none")
    {
        var characteristicId = getCurrentId(characteristicIdLabelId)

        var characteristicSelectIds = await getAllRemainingCharacteristics(productId, characteristicId);

        if (characteristicSelectIds != null
            && characteristicSelectIds.length > 0)
        {
            characteristicSelect.innerHTML = '';

            for (var i = 0; i < characteristicSelectIds.length; i++)
            {
                var option = document.createElement('option');

                option.value = characteristicSelectIds[i].value;
                option.text = characteristicSelectIds[i].text;

                characteristicSelect.appendChild(option);
            }

            removeOldCharacteristicFromOtherSelects(characteristicId, productCharacteristicSelectName, characteristicSelectId);

            characteristicSelect.setAttribute("data-previous-value", characteristicId);
        }

        characteristicLabel.style.display = "none";

        characteristicSelect.style.display = "";

        return;
    }

    var charactersiticSelectOptionText = characteristicSelect.options[characteristicSelect.selectedIndex].text;

    characteristicLabel.innerText = charactersiticSelectOptionText;

    characteristicSelect.style.display = "none";

    characteristicLabel.style.display = "";
}

async function getAllRemainingCharacteristics(productId, characteristicId = null)
{
    const url = "/ProductPropertiesEditor/" + productId + "?handler=GetRemainingCharacteristicsForProduct" +
        "&characteristicToAddToSelectId=" + characteristicId;

    const response = await fetch(url,
        {
            method: "GET",
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
        })

    var responseJson = await response.json();

    var responseArray = [...responseJson];

    return responseArray;
}

function removeOldCharacteristicFromOtherSelects(
    characteristicToRemoveId,
    productCharacteristicSelectName,
    characteristicSelectId = null)
{
    if (characteristicToRemoveId == null) return;

    const productCharacteristicSelectQuery = '[name="' + productCharacteristicSelectName + '"]';

    var productCharacteristicSelects = [...document.querySelectorAll(productCharacteristicSelectQuery)];

    for (var i = 0; i < productCharacteristicSelects.length; i++)
    {
        var productCharacteristicSelect = productCharacteristicSelects[i];

        if (characteristicSelectId != null
            && productCharacteristicSelect.id === characteristicSelectId) continue;

        for (var k = 0; k < productCharacteristicSelect.options.length; k++)
        {
            var value = productCharacteristicSelect.options[k].value;

            if (parseInt(value) === characteristicToRemoveId)
            {
                productCharacteristicSelect.options[k].remove();

                break;
            }
        }
    }
}

function deletePropertyInTable(productId, productCharacteristicId, propertytrId, tBodyContainerId, deleteEvenOnFail)
{
    if (productCharacteristicId === null
        || isNaN(productCharacteristicId)
        || isNaN(parseInt(productCharacteristicId))
        || productCharacteristicId === undefined
        || productCharacteristicId <= 0)
    {
        if (deleteEvenOnFail)
        {
            var childProperty = document.getElementById(propertytrId);

            document.getElementById(tBodyContainerId).removeChild(childProperty);

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
            const propertyTableBody = document.getElementById(tBodyContainerId);

            var childProperty = document.getElementById(propertytrId);

            propertyTableBody.removeChild(childProperty);
        })
        .fail(function (jqXHR, textStatus)
        {
            if (deleteEvenOnFail)
            {
                const propertyTableBody = document.getElementById(tBodyContainerId);

                var childProperty = document.getElementById(propertytrId);

                propertyTableBody.removeChild(childProperty);
            }
        });
}

function getCurrentId(productIdLabelId)
{
    var characteristicIdEl = document.getElementById(productIdLabelId);

    var idText = characteristicIdEl.innerHTML;

    var idStartIndex = idText.indexOf("ID:") + 4;

    var id = idText.substring(idStartIndex);

    return parseInt(id);
}