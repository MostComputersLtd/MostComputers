async function getProductXml(productToSearchForValue)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=GetXmlFromProduct" + suffixOfHandlerName;

    const response = await fetch(url,
        {
            method: "GET",
            headers:
            {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
        }
    );

    if (response.status === 200)
    {
        setFirstProductDataIsSetToIdInInput(true);
    }

    return response.text();
}

function addNewImage(productToSearchForValue, fileInputElement, imagesContainerId)
{
    const deferred = $.Deferred();

    var formBody = new FormData();

    formBody.append("fileInfo", fileInputElement.files[0]);

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var url = "/ProductCompareEditor/" + "?handler=AddNewImageToProduct" + suffixOfHandlerName;

    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: formBody,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            var imagesContainer = document.getElementById(imagesContainerId);

            if (imagesContainer != null)
            {
                imagesContainer.innerHTML = result;
            }

            deferred.resolve(result);
        })
        .fail(function (error)
        {
            deferred.reject(error);
        });

    return deferred.promise();
}

function addNewImageFromData(productToSearchForValue, fileBlob, imagesContainerId, displayOrder = null)
{
    const deferred = $.Deferred();

    var fileType = fileBlob.type.split("/").pop();

    var formBody = new FormData();

    formBody.append("fileInfo", fileBlob, "newFile." + fileType);

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var url = "/ProductCompareEditor/" + "?handler=AddNewImageToProduct" + suffixOfHandlerName;

    if (displayOrder != null)
    {
        url = "/ProductCompareEditor/" + "?handler=AddNewImageAtGivenDisplayOrderToProduct" + suffixOfHandlerName + "&displayOrder=" + displayOrder;
    }
    
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: formBody,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            var imagesContainer = document.getElementById(imagesContainerId);

            if (imagesContainer != null)
            {
                imagesContainer.innerHTML = result;
            }

            deferred.resolve(result);
        })
        .fail(function (error)
        {
            deferred.reject(error);
        });

    return deferred.promise();
}

function update_productImageDisplay_ul_li_activeState(productToSearchForValue, elementId, elementCheckBoxId, displayOrder)
{
    if (displayOrder < 0) return;

    var elementCheckbox = document.getElementById(elementCheckBoxId);

    var checkboxIsChecked = elementCheckbox.checked;

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=UpdateImageActiveStatusForProduct" + suffixOfHandlerName + "&displayOrder=" + displayOrder + "&active=" + checkboxIsChecked;

    $.ajax({
        type: "PUT",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            var opacityOfLi = checkboxIsChecked ? 1 : 0.35;

            document.getElementById(elementId).style.opacity = opacityOfLi;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function delete_productImageDisplay_ul_li(
    productToSearchForValue,
    elementId,
    imageId,
    fileType,
    fileName,
    imagesContainerId)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var url = "/ProductCompareEditor/" + "?handler=DeleteImageForProduct" + suffixOfHandlerName + "&imageId=" + imageId + "&fileType=" + fileType;

    if (fileName != null
        && fileName.trim().length !== 0)
    {
        url += ("&fileName=" + fileName);
    }

    $.ajax({
        type: "DELETE",
        url: url,
        contentType: "application/json",
        data: null,
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            const productImageDisplayList = document.getElementById(imagesContainerId);

            if (result != null
                && typeof result === "string"
                && result.length > 0)
            {
                productImageDisplayList.innerHTML = result;

                return;
            }

            const productImageListItemToDelete = document.getElementById(elementId);

            productImageDisplayList.removeChild(productImageListItemToDelete)
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function addPropertyToTable(
    productToSearchForValue,
    tBodyContainerId,
    trItemsName,
    elementIdPrefix,
    characteristicIdLabelName,
    characteristicSelectName)
{
    var tBodyContainer = document.getElementById(tBodyContainerId);

    if (tBodyContainer == null) return;

    var trItems = tBodyContainer.querySelectorAll('[name="' + trItemsName + '"]');

    var maxRowNumber = 0;

    for (var i = 0; i < trItems.length; i++)
    {
        var itemId = trItems[i].id;

        var itemIdIndexOfRowNumber = itemId.indexOf('#') + 1;

        var indexOfRowNumberAsString = itemId.substring(itemIdIndexOfRowNumber);

        var indexOfRowNumber = parseInt(indexOfRowNumberAsString);

        if (maxRowNumber < indexOfRowNumber)
        {
            maxRowNumber = indexOfRowNumber;
        }
    }

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const indexOfNewItem = maxRowNumber + 1;

    var url = "/ProductCompareEditor/" + "?handler=AddNewBlancPropertyToProduct" + suffixOfHandlerName + "&indexOfItem=" + indexOfNewItem + "&elementIdPrefix=" + elementIdPrefix;

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
            tBodyContainer.insertAdjacentHTML("beforeend", result);

            var characteristicIdLabelId = characteristicIdLabelName + "#" + indexOfNewItem;

            var characteristicId = getCurrentId(characteristicIdLabelId);

            if (characteristicId == null) return;

            var characteristicSelectId = characteristicSelectName + "#" + indexOfNewItem;

            removeOldCharacteristicFromOtherSelects(characteristicId, characteristicSelectName, characteristicSelectId);

            var characteristicSelect = document.getElementById(characteristicSelectId);

            if (characteristicSelect == null) return;

            characteristicSelect.setAttribute("data-previous-value", characteristicId);

            //characteristicSelect.setAttribute("data-previous-value-inserted-to-product", characteristicId);
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function updatePropertyInTable(
    productToSearchForValue,
    productPropCharacteristicIdInputId,
    productPropValueInputId,
    productPropXmlPlacementCheckboxId,
    productPropWithoutCharacteristicSelectId = null,
    nameOfPropToReplace = null)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var url = "/ProductCompareEditor/" + "?handler=UpdatePropertyForProduct" + suffixOfHandlerName;

    var productPropCharacteristicIdInput = document.getElementById(productPropCharacteristicIdInputId);
    var productPropValueInput = document.getElementById(productPropValueInputId);
    var productPropXmlPlacementCheckbox = document.getElementById(productPropXmlPlacementCheckboxId);
    var productPropCharacteristicSelect = document.getElementById(productPropWithoutCharacteristicSelectId);

    var productPropCharacteristicId;

    if (productPropCharacteristicIdInput !== null)
    {
        productPropCharacteristicId = getCurrentId(productPropCharacteristicIdInputId);
    }

    var productPropValue = productPropValueInput.innerText;

    var productPropXmlPlacement = productPropXmlPlacementCheckbox.checked ? 2 : 0;

    if (productPropCharacteristicSelect !== null)
    {
        var productPropWithoutCharacteristicValue = productPropCharacteristicSelect.value;

        if (productPropWithoutCharacteristicValue != null
            && productPropWithoutCharacteristicValue.length !== 0)
        {
            productPropCharacteristicId = parseInt(productPropWithoutCharacteristicValue);
        }

        //var productPropCharacteristicSelectOldValue = productPropCharacteristicSelect.getAttribute("data-previous-value-inserted-to-product");

        //var productPropCharacteristicSelectOldValueInt = parseInt(productPropCharacteristicSelectOldValue);

        //if (!isNaN(productPropCharacteristicSelectOldValueInt)
        //    && productPropCharacteristicSelectOldValueInt !== productPropCharacteristicId)
        //{
        //    url += ("&productCharacteristicToRemoveId=" + productPropCharacteristicSelectOldValueInt);
        //}
    }

    var data =
    {
        ProductCharacteristicId: productPropCharacteristicId,
        Value: productPropValue,
        XmlPlacement: productPropXmlPlacement,
        IsNew: false,
        NameOfCharacteristicToReplace: nameOfPropToReplace
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
            //productPropCharacteristicSelect.setAttribute("data-previous-value-inserted-to-product", productPropCharacteristicId);
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function deletePropertyInTableByLabelId(
    productToSearchForValue,
    productCharacteristicIdLabelId,
    propertyTrId,
    tBodyContainerId,
    currentProductCharacteristicSelectId,
    productCharacteristicSelectName,
    deleteEvenOnFail,
    invalidTextRowToRemoveId = null)
{
    var tBodyContainer = document.getElementById(tBodyContainerId);

    var productCharacteristicLabel = document.getElementById(productCharacteristicIdLabelId);

    if (productCharacteristicLabel == null)
    {
        if (deleteEvenOnFail)
        {
            removePropertyDisplay(
                propertyTrId,
                tBodyContainer,
                currentProductCharacteristicSelectId,
                productCharacteristicSelectName,
                invalidTextRowToRemoveId);
        }

        return;
    }

    var productCharacteristicId = getCurrentId(productCharacteristicIdLabelId);

    deletePropertyInTable(
        productToSearchForValue,
        productCharacteristicId,
        propertyTrId,
        tBodyContainerId,
        currentProductCharacteristicSelectId,
        productCharacteristicSelectName,
        deleteEvenOnFail,
        invalidTextRowToRemoveId);
}

function deletePropertyInTable(
    productToSearchForValue,
    productCharacteristicId,
    propertyTrId,
    tBodyContainerId,
    currentProductCharacteristicSelectId,
    productCharacteristicSelectName,
    deleteEvenOnFail,
    invalidTextRowToRemoveId = null)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var tBodyContainer = document.getElementById(tBodyContainerId);

    if (productCharacteristicId === null
        || isNaN(productCharacteristicId)
        || isNaN(parseInt(productCharacteristicId))
        || productCharacteristicId === undefined
        || productCharacteristicId <= 0)
    {
        if (deleteEvenOnFail)
        {
            removePropertyDisplay(
                propertyTrId,
                tBodyContainer,
                currentProductCharacteristicSelectId,
                productCharacteristicSelectName,
                invalidTextRowToRemoveId);
        }

        return;
    }

    var url = "/ProductCompareEditor/" + "?handler=DeletePropertyForProduct" + suffixOfHandlerName + "&productCharacteristicId=" + productCharacteristicId;

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
            removePropertyDisplay(
                propertyTrId,
                tBodyContainer,
                currentProductCharacteristicSelectId,
                productCharacteristicSelectName,
                invalidTextRowToRemoveId);
        })
        .fail(function (jqXHR, textStatus)
        {
            if (deleteEvenOnFail)
            {
                removePropertyDisplay(
                    propertyTrId,
                    tBodyContainer,
                    currentProductCharacteristicSelectId,
                    productCharacteristicSelectName,
                    invalidTextRowToRemoveId);
            }
        });
}

function removePropertyDisplay(propertyTrId, tBodyContainer, currentProductCharacteristicSelectId, productCharacteristicSelectName, invalidTextRowToRemoveId)
{
    var currentProductCharacteristicSelect = document.getElementById(currentProductCharacteristicSelectId);

    if (currentProductCharacteristicSelect != null)
    {
        var dataOfCurrentSelect = currentProductCharacteristicSelect.options[currentProductCharacteristicSelect.selectedIndex];

        if (dataOfCurrentSelect != null
            && dataOfCurrentSelect.value != null)
        {
            const productCharacteristicSelectQuery = '[name="' + productCharacteristicSelectName + '"]';

            var productCharacteristicSelects = [...document.querySelectorAll(productCharacteristicSelectQuery)];

            for (var i = 0; i < productCharacteristicSelects.length; i++)
            {
                var productCharacteristicSelect = productCharacteristicSelects[i];

                if (productCharacteristicSelect.id === currentProductCharacteristicSelectId) continue;

                var selectContainsValue = false;

                for (var k = 0; k < productCharacteristicSelect.options.length; k++)
                {
                    if (productCharacteristicSelect.options[k].value === dataOfCurrentSelect)
                    {
                        selectContainsValue = true;

                        break;
                    }
                }

                if (!selectContainsValue)
                {
                    var newOption = document.createElement("option");

                    newOption.text = dataOfCurrentSelect.text;
                    newOption.value = dataOfCurrentSelect.value;

                    productCharacteristicSelect.appendChild(newOption);
                }
            }
        }
    }

    var childProperty = document.getElementById(propertyTrId);

    tBodyContainer.removeChild(childProperty);

    if (invalidTextRowToRemoveId != null)
    {
        var invalidTextRowToRemove = document.getElementById(invalidTextRowToRemoveId);

        if (invalidTextRowToRemove == null) return;

        tBodyContainer.removeChild(invalidTextRowToRemove);
    }
}

function saveFirstProduct(productElementId)
{
    var isSaveConfirmed = confirm("This operation will change the product. Are you sure you want to continue?");

    if (isSaveConfirmed == null
        || typeof isSaveConfirmed !== "boolean"
        || !isSaveConfirmed) return;

    const url = "/ProductCompareEditor/" + "?handler=SaveFirstProduct";

    $.ajax({
        type: "PUT",
        url: url,
        contentType: "application/json",
        data: null,
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            if (result != null
                && typeof result === "string"
                && result.length > 0)
            {
                var productElement = document.getElementById(productElementId);

                if (productElement != null)
                {
                    productElement.innerHTML = result;
                }
            }
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

var propertyDragOverItemWasInserted = false;
var propertyDragOverInvalidPropertyPopupWasShown = false;

var currentPropBeingDragged = null;
var propWithSameIdAsTheOneBeingDraggedInOtherProduct = null;

function propertyTr_ul_li_ondragstart(e, propertyTrId)
{
    e.dataTransfer.effectAllowed = 'copy';

    setTimeout(function ()
    {
        var propertyTr = document.getElementById(propertyTrId).cloneNode(true);

        if (propertyTr != null)
        {
            currentPropBeingDragged = propertyTr;

            propertyTr.classList.add("draggedPropertyTr");
        }
    }, 0);
}

async function propertyDisplayTable_ondragover(
    e,
    productToSearchForValue,
    productPrefix,
    propertyDisplayTableId,
    propertyDisplayTableContainerId,
    characteristicIdLabelName,
    propertyValueDivsName,
    propertyValueXmlPlacementCheckboxName,
    otherCharacteristicIdLabelName,
    otherPropertyValueDivsName,
    otherPropertyValueXmlPlacementCheckboxName)
{
    e.preventDefault();

    var propertyDisplayTable = document.getElementById(propertyDisplayTableId);

    var draggedProperty = currentPropBeingDragged;

    if (draggedProperty.id.startsWith(productPrefix)
        && !propertyDragOverItemWasInserted) return;

    var allowedPropertyIds = await getAllPropertyIds(productToSearchForValue);

    var characteristicIdLabelInDraggedProperty = draggedProperty
        .querySelector('[name="' + otherCharacteristicIdLabelName + '"]');

    if (characteristicIdLabelInDraggedProperty == null) return;

    var characteristicIdInDragged = getCurrentId(characteristicIdLabelInDraggedProperty.id);

    if (allowedPropertyIds.indexOf(characteristicIdInDragged) === -1)
    {
        if (!propertyDragOverInvalidPropertyPopupWasShown)
        {
            showNotificationWithText(
                "copiedNotificationBox",
                "Property is not valid for product.",
                "notificationBox-long-message");

            propertyDragOverInvalidPropertyPopupWasShown = true;
        }

        return;
    }

    var propertiesInDisplayTable = [...propertyDisplayTable.querySelectorAll('tr')];

    if (!propertyDragOverItemWasInserted)
    {
        var propertyValueDivInDragged = draggedProperty.querySelector('[name="' + otherPropertyValueDivsName + '"]');
        var propertyXmlPlacementCheckboxInDragged = draggedProperty.querySelector('[name="' + otherPropertyValueXmlPlacementCheckboxName + '"]');

        for (var i = 0; i < propertiesInDisplayTable.length; i++)
        {
            var property = propertiesInDisplayTable[i];

            var characteristicIdLabelInProperty = property.querySelector('[name="' + characteristicIdLabelName + '"]');

            if (characteristicIdLabelInProperty == null) continue;

            var characteristicId = getCurrentId(characteristicIdLabelInProperty.id);

            if (characteristicId == null
                || isNaN(characteristicId)
                || characteristicId !== characteristicIdInDragged) continue;

            var propertyRect = property.getBoundingClientRect();

            if (propertyRect.top <= 0
                || propertyRect.bottom >= (window.innerHeight || document.documentElement.clientHeight))
            {
                property.scrollIntoView({ block: "nearest", behavior: "smooth" });
            }

            propertyDisplayTable_ondragover_addCssClassesToPropWithMatchingIdInOtherProduct(
                property,
                propertyValueDivsName,
                propertyValueXmlPlacementCheckboxName,
                propertyValueDivInDragged,
                propertyXmlPlacementCheckboxInDragged);

            propWithSameIdAsTheOneBeingDraggedInOtherProduct = property;

            propertyDragOverItemWasInserted = true;

            return;
        }
    }

    propertyDisplayTable_ondragover_addOrMoveProperty(propertiesInDisplayTable, e, draggedProperty, propertyDisplayTable);
}

function propertyDisplayTable_ondragover_addCssClassesToPropWithMatchingIdInOtherProduct(
    property,
    propertyValueDivsName,
    propertyValueXmlPlacementCheckboxName,
    propertyValueDivInDragged,
    propertyXmlPlacementCheckboxInDragged)
{
    property.classList.add("draggedPropertyTrPropertyWithSameId");

    var propertyValueDiv = property.querySelector('[name="' + propertyValueDivsName + '"]');
    var propertyXmlPlacementCheckbox = property.querySelector('[name="' + propertyValueXmlPlacementCheckboxName + '"]');

    if (propertyValueDiv.innerText.trim() !== propertyValueDivInDragged.innerText.trim())
    {
        propertyValueDiv.classList.add("draggedPropertyTrPropertyWithSameIdDifferentValueItem");
    }

    if (propertyXmlPlacementCheckbox.checked
        !== propertyXmlPlacementCheckboxInDragged.checked)
    {
        propertyXmlPlacementCheckbox.classList.add("draggedPropertyTrPropertyWithSameIdDifferentValueItem");
    }
}

function propertyDisplayTable_ondragover_addOrMoveProperty(propertiesInDisplayTable, e, draggedProperty, propertyDisplayTable)
{
    var nextSiblingIndex = null;

    nextSiblingIndex = propertiesInDisplayTable.findIndex(item =>
    {
        var itemRect = item.getBoundingClientRect();

        return e.clientY < itemRect.top;
    });

    var nextSibling = propertiesInDisplayTable[nextSiblingIndex];

    if (!propertyDragOverItemWasInserted)
    {
        propertiesInDisplayTable.splice(nextSiblingIndex, 0, draggedProperty);

        while (propertyDisplayTable.firstChild)
        {
            propertyDisplayTable.removeChild(propertyDisplayTable.firstChild);
        }

        propertiesInDisplayTable.forEach(function (child)
        {
            propertyDisplayTable.appendChild(child);
        });

        propertyDragOverItemWasInserted = true;
    }
    else if (propertiesInDisplayTable.indexOf(draggedProperty) !== -1)
    {
        propertyDisplayTable.insertBefore(draggedProperty, nextSibling);
    }
}

async function propertyDisplayTable_ondragend(
    productToSearchForValue,
    productPrefix,
    propertyDisplayTableId,
    characteristicIdLabelName,
    propertyValueXmlPlacementCheckboxName,
    productElementId)
{
    propertyDragOverInvalidPropertyPopupWasShown = false;

    var draggedProperty = currentPropBeingDragged;

    propertyDisplayTable_dragend_handleCssClassCleanup(draggedProperty);

    if (draggedProperty.id.startsWith(productPrefix)
        && !propertyDragOverItemWasInserted) return;
    
    var propertyDisplayTable = document.getElementById(propertyDisplayTableId);

    var propertiesInDisplayTable = [...propertyDisplayTable.querySelectorAll('tr')];

    var propertyDragEndIndex = propertiesInDisplayTable.indexOf(draggedProperty);

    if (propertyDragEndIndex === -1)
    {
        if(!propertyDragOverItemWasInserted) return;

        propertyDragEndIndex = 0;
    }
    
    propertyDragOverItemWasInserted = false;

    var characteristicIdLabelInProperty = draggedProperty.querySelector('[name="' + characteristicIdLabelName + '"]');

    if (characteristicIdLabelInProperty == null)
    {
        draggedProperty.remove();

        return;
    }

    var characteristicId = getCurrentId(characteristicIdLabelInProperty.id);

    if (characteristicId == null
        || isNaN(characteristicId))
    {
        draggedProperty.remove();

        return;
    }

    var propertyValueXmlPlacementCheckbox = draggedProperty.querySelector('[name="' + propertyValueXmlPlacementCheckboxName + '"]');

    var xmlPlacementValue = propertyValueXmlPlacementCheckbox.checked ? 2 : 0;

    await addOrOverwritePropertyFromDataInOtherProduct(
        toggleProductToSearchForValue(productToSearchForValue),
        characteristicId,
        propertyDragEndIndex,
        xmlPlacementValue,
        productElementId);
}

function propertyDisplayTable_dragend_handleCssClassCleanup(draggedProperty)
{
    setTimeout(function ()
    {
        if (draggedProperty != null)
        {
            draggedProperty.classList.remove("draggedPropertyTr");
        }
    }, 0);

    if (propWithSameIdAsTheOneBeingDraggedInOtherProduct != null)
    {
        propWithSameIdAsTheOneBeingDraggedInOtherProduct.classList.remove("draggedPropertyTrPropertyWithSameId");

        propWithSameIdAsTheOneBeingDraggedInOtherProduct = null;
    }

    currentPropBeingDragged = null;

    var draggedPropertyPropsWithSameIdDifferentValueItems
        = document.querySelectorAll('.draggedPropertyTrPropertyWithSameIdDifferentValueItem');

    draggedPropertyPropsWithSameIdDifferentValueItems.forEach(
        x => x.classList.remove('draggedPropertyTrPropertyWithSameIdDifferentValueItem'));
}

async function addOrOverwritePropertyFromDataInOtherProduct(
    productToSearchForValue,
    characteristicId,
    indexToInsertAtOnAdd,
    xmlPlacementValue,
    productElementId)
{
    var productToSearchForSuffix = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=AddOrOverwritePropertyFromDataInOtherProduct" + productToSearchForSuffix
        + "&characteristicId=" + characteristicId
        + "&indexToInsertAtOnAdd=" + indexToInsertAtOnAdd
        + "&xmlPlacementValue=" + xmlPlacementValue;

    const response = await fetch(url,
    {
        method: "PUT",
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    const responseText = await response.text();

    if (responseText != null
        && typeof responseText === "string"
        && responseText.length > 0)
    {
        var productElement = document.getElementById(productElementId);

        if (productElement != null)
        {
            productElement.innerHTML = responseText;
        }
    }
}

async function togglePropertyCharacteristicAndSelect(
    productToSearchForValue,
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

        var characteristicSelectIds = await getAllRemainingCharacteristics(productToSearchForValue, characteristicId);

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

    characteristicSelect.style.display = "none";

    characteristicLabel.style.display = "";
}

var allowedPropertyIds = null;

async function getAllPropertyIds(productToSearchForValue)
{
    if (allowedPropertyIds != null) return allowedPropertyIds;

    var productToSearchForSuffix = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=GetAllowedCharacteristicIds" + productToSearchForSuffix;

    const response = await fetch(url,
    {
        method: "GET",
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })

    var responseJson = await response.json();

    if (responseJson.allowedIds != null)
    {
        var allowedIds = [...responseJson.allowedIds];

        allowedPropertyIds = allowedIds;

        return allowedIds;
    }
}

async function getAllRemainingCharacteristics(productToSearchForValue, characteristicId = null)
{
    var productToSearchForSuffix = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=GetRemainingCharacteristicsForProduct" + productToSearchForSuffix + "&characteristicToAddToSelectId=" + characteristicId;

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

var characteristicsForBothProducts = null;

function resetCharacteristicsForBothProductsCachedValues()
{
    characteristicsForBothProducts = null;
}

async function getAllCharacteristicsForBothProducts()
{
    if (characteristicsForBothProducts != null) return characteristicsForBothProducts;

    const url = "/ProductCompareEditor/" + "?handler=GetCharacteristicsForBothProducts";

    const response = await fetch(url,
    {
        method: "GET",
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        });

    var responseJson = await response.json();

    if (responseJson == null
        || responseJson.characteristics == null) return null;

    var responseArray = [...(responseJson.characteristics)];

    characteristicsForBothProducts = responseArray;

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

function productPropertyDisplay_value_div_onclick(e, propertyXmlPlacementCheckboxId, propertyValueDivId)
{
    if (e.ctrlKey || e.metaKey)
    {
        var propertyXmlPlacementCheckbox = document.getElementById(propertyXmlPlacementCheckboxId);

        if (!propertyXmlPlacementCheckbox.checked) return;

        var propertyValueDiv = document.getElementById(propertyValueDivId);

        const url = propertyValueDiv.innerText.trim();

        window.open(url, "_blank");
    }
}

function productPropertyDisplay_XmlPlacement_checkbox_onchange(propertyXmlPlacementCheckboxId, propertyValueDivId)
{
    var propertyXmlPlacementCheckbox = document.getElementById(propertyXmlPlacementCheckboxId);

    if (propertyXmlPlacementCheckbox == null) return;

    var propertyValueDiv = document.getElementById(propertyValueDivId);

    if (propertyXmlPlacementCheckbox.checked)
    {
        propertyValueDiv.classList.add('siteUrlProperty');

        return;
    }

    propertyValueDiv.classList.remove('siteUrlProperty');
}