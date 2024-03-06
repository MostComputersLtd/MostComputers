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

function addPropertyToTable(productToSearchForValue, tBodyContainerId, trItemsName, elementIdPrefix)
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

    var url = "/ProductCompareEditor/" + "?handler=AddNewBlancPropertyToProduct" + suffixOfHandlerName + "&indexOfItem=" + (maxRowNumber + 1) + "&elementIdPrefix=" + elementIdPrefix;

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
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function updatePropertyInTable(
    productToSearchForValue,
    productPropCharacteristicIdInputId,
    productPropValueInputId,
    productPropXmlPlacementSelectId,
    productPropWithoutCharacteristicSelectId,
    nameOfPropToReplace = null)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    var url = "/ProductCompareEditor/" + "?handler=UpdatePropertyForProduct" + suffixOfHandlerName;

    var productPropCharacteristicIdInput = document.getElementById(productPropCharacteristicIdInputId);
    var productPropValueInput = document.getElementById(productPropValueInputId);
    var productPropXmlPlacementSelect = document.getElementById(productPropXmlPlacementSelectId);
    var productPropWithoutCharacteristicSelect = document.getElementById(productPropWithoutCharacteristicSelectId);

    var productPropCharacteristicId;

    var productPropIsNew;

    if (productPropCharacteristicIdInput !== null)
    {
        var idText = productPropCharacteristicIdInput.innerHTML;

        var idStartIndex = idText.indexOf("ID:") + 4;

        var id = idText.substring(idStartIndex);

        productPropCharacteristicId = parseInt(id);

        if (productPropCharacteristicId != null
            && !isNaN(productPropCharacteristicId))
        {
            productPropIsNew = false;
        }
    }

    var productPropValue = productPropValueInput.innerText;

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
        IsNew: productPropIsNew,
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
        .done(function (result) {
        })
        .fail(function (jqXHR, textStatus) {
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
    const url = "/ProductCompareEditor/" + "?handler=SaveFirstProduct";

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
            if (result != null
                && typeof result === "string"
                && result.length > 0)
            {
                document.getElementById(productElementId).innerHTML = result;
            }
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}