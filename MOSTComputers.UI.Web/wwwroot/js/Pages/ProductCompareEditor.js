function toggleVisibilityBetweenFirstXmlAndProductViews(
    xmlElementId,
    productElementId,
    xmlTextElementId,
    productIdLabelId,
    addImageInputName,
    imagesContainerId)
{
    var xmlElement = document.getElementById(xmlElementId);
    var productElement = document.getElementById(productElementId);

    if (xmlElement.style.display === "none")
    {
        getProductXml(0)
            .then(xmlOfProduct =>
            {
                var xmlTextElement = document.getElementById(xmlTextElementId);

                xmlTextElement.value = xmlOfProduct;
            });

        xmlElement.style.display = "block";

        productElement.style.display = "none";
    }
    else
    {
        var productIdAsString = document.getElementById(productIdLabelId).value;

        var productId = parseInt(productIdAsString);

        xmlElement.style.display = "none";

        productElement.style.display = "block";

        setProductDataForFirstProductFromProductId(productId, productElementId, addImageInputName, imagesContainerId);
    }
}

function setProductDataForFirstProductFromProductId(productId, productElementId, addImageInputName, imagesContainerId)
{
    if (productId == null
        || isNaN(productId)
        || isNaN(parseInt(productId))
        || productId < 0) return;

    const url = "/ProductCompareEditor/" + "?handler=GetProductDataByIdFirst" + "&productId=" + productId;

    $.ajax({
        type: "POST",
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
            var productElement = document.getElementById(productElementId);

            productElement.innerHTML = result;
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

function toggleVisibilityBetweenSecondXmlAndProductViews(
    xmlElementId,
    productElementId,
    xmlTextElementId,
    addImageInputName,
    imagesContainerId,
    productIdDisplayElementId,
    productIdInputOfFirstProductId)
{
    var xmlElement = document.getElementById(xmlElementId);
    var productElement = document.getElementById(productElementId);

    if (xmlElement.style.display === "none")
    {
        getProductXml(1)
            .then(xmlOfProduct =>
            {
                var xmlTextElement = document.getElementById(xmlTextElementId);

                xmlTextElement.value = xmlOfProduct;
            });

        xmlElement.style.display = "block";

        productElement.style.display = "none";
    }
    else
    {
        var xmlData = document.getElementById(xmlTextElementId).value;

        xmlElement.style.display = "none";

        productElement.style.display = "block";

        $.when(setProductDataForSecondProductFromXml(xmlData, productElementId, addImageInputName, imagesContainerId))
            .done(function ()
            {
                var productIdDisplayElement = document.getElementById(productIdDisplayElementId);

                var productIdAsString = productIdDisplayElement.value;

                var productId = parseInt(productIdAsString);

                if (isNaN(productId)) return;

                var productIdInputOfFirstProduct = document.getElementById(productIdInputOfFirstProductId);

                if (productIdInputOfFirstProduct.value == null
                    || productIdInputOfFirstProduct.value.length === 0)

                productIdInputOfFirstProduct.value = productId;
            });
    }
}

function setProductDataForSecondProductFromXml(xmlData, productElementId, addImageInputName, imagesContainerId)
{
    const deferred = $.Deferred();

    const url = "/ProductCompareEditor/" + "?handler=GetProductDataFromXmlSecond";

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: JSON.stringify(xmlData),
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .done(function (result)
        {
            var productElement = document.getElementById(productElementId);

            productElement.innerHTML = result;

            deferred.resolve(result);
        })
        .fail(function (error)
        {
            deferred.reject(error);
        });

    return deferred.promise();
}

function toggleEditorToFullScreen(elementId, imageElementId = null, fullScreenButtonImagePath = null, normalScreenButtonImagePath = null)
{
    var element = document.getElementById(elementId);

    if (element != null)
    {
        if (imageElementId != null
            && element.classList.contains("fullScreen")
            && fullScreenButtonImagePath != null)
        {
            var imageElement = document.getElementById(imageElementId);

            imageElement.src = fullScreenButtonImagePath;
        }
        else if (imageElementId != null
            && normalScreenButtonImagePath != null)
        {
            var imageElement = document.getElementById(imageElementId);

            imageElement.src = normalScreenButtonImagePath;
        }

        element.classList.toggle("fullScreen");
        element.classList.toggle("fullScreenProductEditor");
    }
}

function copySearchStringSearchDataToClipboard(searchStringSearchData)
{
    navigator.clipboard.writeText(searchStringSearchData);

    showNotificationWithText("copiedNotificationBox", "Copied!", "copy-to-xml-success-message")
}

function getImageDisplayOrder(target, imageListId)
{
    const productImageDisplayList = document.getElementById(imageListId);

    const childrenArray = Array.from(productImageDisplayList.children);

    var newIndexOfDraggedItem = childrenArray.indexOf(target);

    return newIndexOfDraggedItem + 1;
}

var indexOfDraggedItem;

var isDraggedItemInOtherList = false;

function productImageDisplay_ul_li_ondragstart(e, listElementId)
{
    indexOfDraggedItem = getImageDisplayOrder(e.target, listElementId);

    setTimeout(() =>
    {
        e.target.classList.add("draggedImageLi");
    }, 0);
}

function productImageDisplay_ul_ondragover(e, listElementId, addButtonLiId)
{
    const productImageDisplayList = document.getElementById(listElementId);

    const childrenOfListThatArentDragged = [...productImageDisplayList.querySelectorAll("li")];

    var itemThatIsDragged = productImageDisplayList.querySelector(".draggedImageLi");

    var potentialSiblings = childrenOfListThatArentDragged.filter(child =>
    {
        return e.clientY <= child.getBoundingClientRect().bottom
            && e.clientY >= child.getBoundingClientRect().top;
    });

    potentialSiblings = potentialSiblings.sort((x, y) =>
    {
        x.getBoundingClientRect().left - y.getBoundingClientRect().left;
    });

    var potentialSiblingIds = [];

    for (var i = 0; i < potentialSiblings.length; i++)
    {
        potentialSiblingIds.push(potentialSiblings[i].id);
    }

    var nextSibling = potentialSiblings.find(child =>
    {
        return e.clientX <= child.getBoundingClientRect().left + (child.clientWidth / 2);
    });

    if (nextSibling == itemThatIsDragged)
    {
        return;
    }

    if (nextSibling != undefined)
    {
        if (itemThatIsDragged == null)
        {
            itemThatIsDragged = document.querySelector(".draggedImageLi");

            if (itemThatIsDragged != null)
            {
                itemThatIsDragged = itemThatIsDragged.cloneNode(true);

                isDraggedItemInOtherList = true;
            }
        }

        if (nextSibling.id === addButtonLiId)
        {
            productImageDisplayList.insertBefore(itemThatIsDragged, nextSibling);

            return;
        }

        var itemBeforeDraggedItem = itemThatIsDragged.previousElementSibling;
        var itemAfterNextSibling = nextSibling.nextElementSibling;

        var draggedItemRect = itemThatIsDragged.getBoundingClientRect();
        var nextSiblingRect = nextSibling.getBoundingClientRect();
        
        var isDraggedItemTheFirstItemOnTheRow = true;
        var isNextSiblingTheLastItemOnTheRow = true;

        if (itemBeforeDraggedItem !== null
            && itemBeforeDraggedItem !== undefined)
        {
            isDraggedItemTheFirstItemOnTheRow = draggedItemRect.top - itemBeforeDraggedItem.getBoundingClientRect().top >= nextSibling.clientHeight / 2;
        }

        if (itemAfterNextSibling !== null
            && itemAfterNextSibling !== undefined)
        {
            isNextSiblingTheLastItemOnTheRow = itemAfterNextSibling.getBoundingClientRect().top - nextSiblingRect.top >= nextSibling.clientHeight / 2;
        }

        var isDraggedItemOnSameRow = itemThatIsDragged.getBoundingClientRect().top - nextSiblingRect.top <= nextSibling.clientHeight / 2;

        if ((isNextSiblingTheLastItemOnTheRow
            && isDraggedItemOnSameRow
            && isSiblingAfter(itemThatIsDragged, nextSibling))
            || (isDraggedItemTheFirstItemOnTheRow && isDraggedItemOnSameRow))
        {
            productImageDisplayList.insertBefore(itemThatIsDragged, nextSibling.nextSibling);

            return;
        }

        productImageDisplayList.insertBefore(itemThatIsDragged, nextSibling);
    }
    else if (isDraggedItemInOtherList)
    {
        productImageDisplayList.removeChild(itemThatIsDragged);

        isDraggedItemInOtherList = false;
    }
}

function isSiblingAfter(currentElement, elementToSearch)
{
    while (currentElement)
    {
        currentElement = currentElement.nextElementSibling;

        if (currentElement === elementToSearch)
        {
            return true;
        }
    }

    return false;
}

function PropertyWithoutCharacteristicLocal_select_onchange(
    value,
    productIdLabelId,
    productCharacteristicSelectName,
    currentProductCharacteristicSelectId)
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

    var labelWithIdText = document.getElementById(productIdLabelId);
    
    labelWithIdText.textContent = "ID: " + value;

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

            if (productCharacteristicSelectItem.value === value)
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

    currentSelectElement.setAttribute("data-previous-value", value);
}

function getCurrentId(characteristicIdLabelId)
{
    var characteristicIdEl = document.getElementById(characteristicIdLabelId);

    var idText = characteristicIdEl.innerHTML;

    var idStartIndex = idText.indexOf("ID:") + 4;

    var id = idText.substring(idStartIndex);

    return parseInt(id);
}

function getMethodHandlerSuffixFromProductValue(productValue)
{
    if (productValue == null
        || isNaN(productValue)
        || isNaN(parseInt(productValue))) return null;

    if (productValue === 0)
    {
        return "First";
    }
    else if (productValue === 1)
    {
        return "Second";
    }

    return null;
}

function toggleProductToSearchForValue(productToSearchForValue)
{
    if (productToSearchForValue == null) return null;

    if (productToSearchForValue === 0)
    {
        return 1;
    }
    else if (productToSearchForValue === 1)
    {
        return 0;
    }
}

function dataURLToBlob(dataURL)
{
    var parts = dataURL.split(';base64,');

    var contentType = parts[0].split(":")[1];

    var byteCharacters = atob(parts[1]);

    var byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += 512)
    {
        let slice = byteCharacters.slice(offset, offset + 512);

        let byteNumbers = new Array(slice.length);

        for (let i = 0; i < slice.length; i++)
        {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        let byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
    }

    return new Blob(byteArrays, { type: contentType });
}

function productImageDisplay_ul_li_ondragend(e, productToSearchForValue, listElementId, otherListElementId, imagesContainerId, otherImagesContainerId)
{
    setTimeout(() =>
    {
        e.target.classList.remove("draggedImageLi");
    }, 0);

    if (isDraggedItemInOtherList)
    {
        isDraggedItemInOtherList = false;

        productToSearchForValue = toggleProductToSearchForValue(productToSearchForValue);

        const childImageOfElement = e.target.querySelector("img");

        var fileBlob = dataURLToBlob(childImageOfElement.src);

        var otherListElement = document.getElementById(otherListElementId);

        var newIndexOfDraggedItem = getImageDisplayOrder(otherListElement.querySelector(".draggedImageLi"), otherListElementId);

        addNewImageFromData(productToSearchForValue, fileBlob, otherImagesContainerId, newIndexOfDraggedItem);

        return;
    }

    var newIndexOfDraggedItem = getImageDisplayOrder(e.target, listElementId);

    if (indexOfDraggedItem === newIndexOfDraggedItem) return;

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=UpdateImageOrderForProduct" + suffixOfHandlerName + "&oldDisplayOrder=" + indexOfDraggedItem + "&newDisplayOrder=" + newIndexOfDraggedItem;
    
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
            isDraggedItemInOtherList = false;

            var imagesContainer = document.getElementById(imagesContainerId);

            if (imagesContainer != null)
            {
                imagesContainer.innerHTML = result;
            }
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}