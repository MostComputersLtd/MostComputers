const localProductLastViewSessionStorageKey = 'productCompareEditor_LocalProduct_LastView';
const outsideProductLastViewSessionStorageKey = 'productCompareEditor_OutsideProduct_LastView';

const shouldFetchDataOnLoadBackSessionStorageKey = 'productCompareEditor_ShouldFetchDataOnLoadBack';

document.addEventListener("DOMContentLoaded", async function ()
{
    const shouldFetchDataOnLoadBack = sessionStorage.getItem(shouldFetchDataOnLoadBackSessionStorageKey);

    if (shouldFetchDataOnLoadBack !== "true")
    {
        sessionStorage.setItem(shouldFetchDataOnLoadBackSessionStorageKey, false);

        return;
    }
    
    const localXmlDisplayElement = document.getElementById("productCompareEditor_LocalProductEditorContainer_XmlDisplay");
    const localXmlDisplayTextElement = document.getElementById("productCompareEditor_LocalProductEditorContainer_XmlDisplay_Text");
    const localProductDisplayElement = document.getElementById("productCompareEditor_LocalProductEditorContainer_ProductDisplay");

    const outsideXmlDisplayElement = document.getElementById("productCompareEditor_OutsideProductEditorContainer_XmlDisplay");
    const outsideXmlDisplayTextElement = document.getElementById("productCompareEditor_OutsideProductEditorContainer_XmlTextArea");
    const outsideProductDisplayElement = document.getElementById("productCompareEditor_OutsideProductEditorContainer_ProductDisplay");

    var productCompareEditorLocalProductLastViewData = sessionStorage.getItem(localProductLastViewSessionStorageKey);

    var localProductCheckbox = document.getElementById("productCompareEditor_LocalProductEditorContainer_ProductOrXmlCheckBox");

    if (productCompareEditorLocalProductLastViewData === '1')
    {
        localProductCheckbox.checked = true;

        const firstProductViewResult = await getStoredProductDataForProduct(0);

        if (localXmlDisplayElement == null
            || localProductDisplayElement == null)
        {
            return;
        }

        if (firstProductViewResult != null)
        {
            localProductDisplayElement.innerHTML = firstProductViewResult;
        }

        localXmlDisplayElement.style.display = "none";

        localProductDisplayElement.style.display = "block";
    }
    else
    {
        localProductCheckbox.checked = false;

        const firstProductXml = await getProductXml(0);

        if (localXmlDisplayElement == null
            || localProductDisplayElement == null)
        {
            return;
        }

        if (firstProductXml != null)
        {
            localXmlDisplayTextElement.value = firstProductXml;
        }

        var productIdDisplayHiddenInputElement
            = document.getElementById("productCompareEditor_LocalProductEditor__productIdDisplay");

        var productIdInput = document.getElementById("productCompareEditor_LocalProductEditorContainer_ProductIdInput");

        if (productIdDisplayHiddenInputElement != null
            && productIdInput != null)
        {
            productIdInput.value = productIdDisplayHiddenInputElement.value;
        }

        localXmlDisplayElement.style.display = "block";

        localProductDisplayElement.style.display = "none";
    }

    var productCompareEditorOutsideProductLastViewData = sessionStorage.getItem(outsideProductLastViewSessionStorageKey);

    var outsideProductCheckbox = document.getElementById("productCompareEditor_OutsideProductEditorContainer_ProductOrXmlCheckBox");

    if (productCompareEditorOutsideProductLastViewData === '1')
    {
        outsideProductCheckbox.checked = true;

        const secondProductViewResult = await getStoredProductDataForProduct(1);

        if (outsideXmlDisplayElement == null
            || outsideProductDisplayElement == null)
        {
            return;
        }

        if (secondProductViewResult != null)
        {
            outsideProductDisplayElement.innerHTML = secondProductViewResult;
        }

        outsideXmlDisplayElement.style.display = "none";

        outsideProductDisplayElement.style.display = "block";
    }
    else
    {
        outsideProductCheckbox.checked = false;

        const secondProductXml = await getProductXml(1);

        if (outsideXmlDisplayElement == null
            || outsideProductDisplayElement == null)
        {
            return;
        }

        if (secondProductXml != null)
        {
            outsideXmlDisplayTextElement.value = secondProductXml;
        }

        outsideXmlDisplayElement.style.display = "block";

        outsideProductDisplayElement.style.display = "none";
    }

    sessionStorage.setItem(shouldFetchDataOnLoadBackSessionStorageKey, false);
});

window.addEventListener("beforeunload", function ()
{
    this.sessionStorage.setItem(shouldFetchDataOnLoadBackSessionStorageKey, true);
})

var firstProductDataIsSetToIdInInput = false;

function setFirstProductDataIsSetToIdInInput(value)
{
    firstProductDataIsSetToIdInInput = value;
}

function toggleVisibilityBetweenFirstXmlAndProductViews(
    xmlElementId,
    productElementId,
    xmlTextElementId,
    productIdLabelId,
    addImageInputName,
    imagesContainerId,
    productIdHiddenInputId)
{
    var xmlElement = document.getElementById(xmlElementId);
    var productElement = document.getElementById(productElementId);

    if (xmlElement.style.display === "none")
    {
        getProductXml(0)
            .then(xmlOfProduct =>
            {
                resetCharacteristicsForBothProductsCachedValues();

                var xmlTextElement = document.getElementById(xmlTextElementId);

                xmlTextElement.value = xmlOfProduct;
            });

        xmlElement.style.display = "block";

        productElement.style.display = "none";

        sessionStorage.setItem(localProductLastViewSessionStorageKey, 0);
    }
    else
    {
        var productIdAsString = document.getElementById(productIdLabelId).value;

        var productId = parseInt(productIdAsString);

        xmlElement.style.display = "none";

        productElement.style.display = "block";

        setProductDataForFirstProductFromProductId(productId, productElementId, addImageInputName, imagesContainerId, productIdHiddenInputId);
    }
}

function setProductDataForFirstProductFromProductId(productId, productElementId, addImageInputName, imagesContainerId, productIdHiddenInputId)
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
            resetCharacteristicsForBothProductsCachedValues();

            var productElement = document.getElementById(productElementId);

            productElement.innerHTML = result;

            var productIdHiddenInput = document.getElementById(productIdHiddenInputId);

            sessionStorage.setItem(localProductLastViewSessionStorageKey, 1);

            if (productIdHiddenInput != null)
            {
                firstProductDataIsSetToIdInInput = true;
            }
        })
        .fail(function (jqXHR, textStatus)
        {
        });
}

async function getStoredProductDataForProduct(productValue)
{
    var methodHandlerSuffix = getMethodHandlerSuffixFromProductValue(productValue);

    const url = "/ProductCompareEditor/" + "?handler=GetProductPartialView" + methodHandlerSuffix;

    const response = await fetch(url,
    {
        method: "GET",
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    if (response.status !== 200) return null;

    return response.text();
}

function refreshProductDataForFirstProduct(productElementId)
{
    const url = "/ProductCompareEditor/" + "?handler=GetRefreshedProductPartialViewFirst";

    $.ajax({
        type: "GET",
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
            resetCharacteristicsForBothProductsCachedValues();

            var productElement = document.getElementById(productElementId);

            productElement.innerHTML = result;

            firstProductDataIsSetToIdInInput = true;
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
                resetCharacteristicsForBothProductsCachedValues();

                var xmlTextElement = document.getElementById(xmlTextElementId);

                xmlTextElement.value = xmlOfProduct;
            });

        xmlElement.style.display = "block";

        productElement.style.display = "none";

        sessionStorage.setItem(outsideProductLastViewSessionStorageKey, 0);
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
            resetCharacteristicsForBothProductsCachedValues();

            var productElement = document.getElementById(productElementId);

            productElement.innerHTML = result;

            sessionStorage.setItem(outsideProductLastViewSessionStorageKey, 1);

            deferred.resolve(result);
        })
        .fail(function (error)
        {
            deferred.reject(error);
        });

    return deferred.promise();
}

function productValidationFormDisableSubmit(e)
{
    e.preventDefault();
}

function toggleEditorToFullScreen(
    elementId,
    imageElementId = null,
    fullScreenButtonImagePath = null,
    normalScreenButtonImagePath = null)
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
    productToSearchForValue,
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
    
    labelWithIdText.textContent = value;

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

    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=UpdatePropertyCharacteristicId" + suffixOfHandlerName
        + "&oldCharacteristicId=" + currentSelectElementPreviousValue + "&newCharacteristicId=" + parseInt(value);

    fetch(url,
    {
        method: "PUT",
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    currentSelectElement.setAttribute("data-previous-value", value);
}

function getCurrentId(characteristicIdLabelId)
{
    var characteristicIdElement = document.getElementById(characteristicIdLabelId);

    var idText = characteristicIdElement.innerHTML;

    var id = parseInt(idText);

    if (isNaN(id)) return null;

    return id;
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

function productImageDisplay_ul_li_ondragend(
    e,
    productToSearchForValue,
    listElementId,
    otherListElementId,
    imagesContainerId,
    otherImagesContainerId)
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

const localProductName = "local";
const externalProductName = "external";

async function copyOtherProductToSearchedProduct(
    productToSearchForValue,
    productElementId,
    productIdHiddenLabelId,
    otherProductIdHiddenLabelId)
{
    var otherProductIdHiddenLabel = document.getElementById(otherProductIdHiddenLabelId);

    if (otherProductIdHiddenLabel == null) return;

    const otherProductIdHiddenLabelValue = otherProductIdHiddenLabel.value;

    var productIdHiddenLabelBefore = document.getElementById(productIdHiddenLabelId);

    var productIdHiddenLabelBeforeValue = (productIdHiddenLabelBefore != null) ? productIdHiddenLabelBefore.value : null;

    const suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    if (productIdHiddenLabelBeforeValue != null
        && otherProductIdHiddenLabelValue != productIdHiddenLabelBeforeValue)
    {
        const productToCopyName = (productToSearchForValue == 0) ? externalProductName : localProductName;
        const productToCopyToName = (productToSearchForValue == 0) ? localProductName : externalProductName;

        var success = confirm("The id of the " + productToCopyName + " product: (" + otherProductIdHiddenLabelValue + ") is different than the id of the "
            + productToCopyToName + " product: (" + productIdHiddenLabelBeforeValue + "). Are you sure you want to continue.")

        if (!success) return;
    }

    const url = "/ProductCompareEditor/" + "?handler=CopyOtherProductIntoProduct" + suffixOfHandlerName;

    const result = await fetch(url,
    {
        method: "POST",
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
    
    const resultText = await result.text();

    var productElement = document.getElementById(productElementId);

    resetCharacteristicsForBothProductsCachedValues();

    if (productElement != null)
    {
        productElement.innerHTML = resultText;

        if (productToSearchForValue === 0)
        {
            firstProductDataIsSetToIdInInput = true;
        }
    }
}

async function productIdInput_onkeydown(e, productId, productOrXmlCheckBoxId, xmlTextAreaElementId, productElementId, productIdHiddenInputId)
{
    if (e.key !== "Enter") return;

    e.preventDefault();

    var productOrXmlCheckBox = document.getElementById(productOrXmlCheckBoxId);

    if (productOrXmlCheckBox == null)
    {
        return;
    }
    
    if (productOrXmlCheckBox.checked)
    {
        setProductDataForFirstProductFromProductId(productId, productElementId, null, null, productIdHiddenInputId)
    }
    else
    {
        await getProductXmlById(0, productId, xmlTextAreaElementId);
    }
}

function productIdInput_onkeyup(e, productId, productElementId, productIdHiddenInputId)
{
    if (e.key === "Enter"
        || e.key === " ") return;

    if (!firstProductDataIsSetToIdInInput) return;

    firstProductDataIsSetToIdInInput = false;

    var shouldInputBeUpdated = confirm("Should we save the changes on this product?");

    if (shouldInputBeUpdated)
    {
        saveFirstProduct(productElementId);
    }
}

async function getProductXmlById(productToSearchForValue, currentProductId, xmlTextAreaElementId)
{
    var suffixOfHandlerName = getMethodHandlerSuffixFromProductValue(productToSearchForValue);

    const url = "/ProductCompareEditor/" + "?handler=GetProductXmlById" + suffixOfHandlerName + "&productId=" + currentProductId;

    await fetch(url,
    {
        method: "POST",
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .then(async function (response)
        {
            resetCharacteristicsForBothProductsCachedValues();

            const responseText = await response.text();

            var xmlTextAreaElement = document.getElementById(xmlTextAreaElementId);

            if (xmlTextAreaElement != null)
            {
                xmlTextAreaElement.value = responseText;

                if (productToSearchForValue === 0)
                {
                    firstProductDataIsSetToIdInInput = true;
                }
            }
        })
        .catch(function (error)
        {
            return;
        })
}

function externalLinkInput_onkeydown_displayExternalLinkInput(e, value)
{
    if (e.key !== "Enter") return;

    e.preventDefault();

    var externalLinkWindow = openHalfWindow(value);

    if (externalLinkWindow == null)
    {
        showNotificationWithText(
            "copiedNotificationBox",
            "Please allow popups for this site.",
            "notificationBox-long-message",
            15000);

        return;
    }

    var topContainerDiv = document.body.querySelector("body > div");

    topContainerDiv.style.width = topContainerDiv.clientWidth + "px";
    topContainerDiv.style.maxWidth = topContainerDiv.clientWidth + "px";

    let timer = setInterval(function ()
    {
        if (externalLinkWindow.closed)
        {
            topContainerDiv.style.width = '';
            topContainerDiv.style.maxWidth = '';

            clearInterval(timer);
        }
    }, 1000);

    resetCharacteristicsForBothProductsCachedValues();
}

function openHalfWindow(url)
{
    var windowWidth = window.innerWidth / 2;

    var windowHeight = window.innerHeight;

    const windowSettings = "toolbar=yes,scrollbars=yes,resizable=yes,top=0,left=" + windowWidth + ",width=" + windowWidth + ",height=" + windowHeight;

    return window.open(url, "_blank", windowSettings);
}

async function clearSecondProductData(xmlTextAreaElementId, productElementId)
{
    const url = "/ProductCompareEditor/" + "?handler=ClearSecondProductData";

    await fetch(url,
    {
        method: "PUT",
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    })
        .then(function (response)
        {
            resetCharacteristicsForBothProductsCachedValues();

            var xmlTextAreaElement = document.getElementById(xmlTextAreaElementId);

            if (xmlTextAreaElement != null)
            {
                xmlTextAreaElement.value = null;
            }

            var productElement = document.getElementById(productElementId);

            if (productElement != null)
            {
                productElement.innerHTML = null;
            }
        })
        .catch(function (error)
        {
            return;
        });
}

window.addEventListener("keydown", function (e)
{
    if (e.key !== "F7") return;

    var modal = document.getElementById("productCompareEditor_OutsideItemCopiedCharacteristics_modal");

    if (modal == null) return;

    e.preventDefault();

    var willBeVisible = modal.getAttribute("data:willBeVisible");

    if (willBeVisible === 'true')
    {
        productCompareEditor_OutsideItemCopiedCharacteristics_hide(
            "productCompareEditor_OutsideItemCopiedCharacteristics_modal");

        modal.setAttribute("data:willBeVisible", false);

        return;
    }

    modal.setAttribute("data:willBeVisible", true);

    productCompareEditor_OutsideItemCopiedCharacteristics_show(
        "productCompareEditor_OutsideItemCopiedCharacteristics_modal",
        "productCompareEditor_OutsideItemCopiedCharacteristics_text");
});

window.addEventListener("focus", function ()
{
    productCompareEditor_OutsideItemCopiedCharacteristics_show(
        "productCompareEditor_OutsideItemCopiedCharacteristics_modal",
        "productCompareEditor_OutsideItemCopiedCharacteristics_text");
});

document.addEventListener("mouseenter", function ()
{
    if (!document.hasFocus()) return;

    productCompareEditor_OutsideItemCopiedCharacteristics_show(
        "productCompareEditor_OutsideItemCopiedCharacteristics_modal",
        "productCompareEditor_OutsideItemCopiedCharacteristics_text");
});

document.addEventListener("mouseleave", function ()
{
    productCompareEditor_OutsideItemCopiedCharacteristics_hide("productCompareEditor_OutsideItemCopiedCharacteristics_modal");
});

document.addEventListener("mousemove", function (e)
{
    productCompareEditor_OutsideItemCopiedCharacteristics_move(e, "productCompareEditor_OutsideItemCopiedCharacteristics_modal");
});

async function productCompareEditor_OutsideItemCopiedCharacteristics_show(
    outsideItemCopiedCharacteristicsModalId,
    outsideItemCopiedCharacteristicsModalTextId)
{
    var outsideItemCopiedCharacteristicsModal = document.getElementById(outsideItemCopiedCharacteristicsModalId);

    if (outsideItemCopiedCharacteristicsModal == null) return;

    var willBeVisible = outsideItemCopiedCharacteristicsModal.getAttribute("data:willBeVisible");

    if (willBeVisible !== 'true') return;

    var outsideItemCopiedCharacteristicsModalText = document.getElementById(outsideItemCopiedCharacteristicsModalTextId);

    if (outsideItemCopiedCharacteristicsModalText == null) return;

    var clipboardData = await navigator.clipboard.readText();

    if (clipboardData == null
        || clipboardData.length <= 0) return;

    var allCharacteristicsForBothProducts = await getAllCharacteristicsForBothProducts();

    if (allCharacteristicsForBothProducts == null
        || allCharacteristicsForBothProducts.length <= 0) return;

    clipboardData = changeTextToBoldCharacteristicNames(clipboardData, allCharacteristicsForBothProducts)

    outsideItemCopiedCharacteristicsModalText.innerHTML = clipboardData;

    outsideItemCopiedCharacteristicsModal.style.display = "";
}

function changeTextToBoldCharacteristicNames(textToModify, allCharacteristicsForBothProducts)
{
    for (var i = 0; i < allCharacteristicsForBothProducts.length; i++)
    {
        var characteristic = allCharacteristicsForBothProducts[i];

        if (characteristic.text == null
            || characteristic.text == "") continue;

        var indexOfCharacteristicNameInText = textToModify.toLowerCase().indexOf(characteristic.text.toLowerCase());

        if (indexOfCharacteristicNameInText <= -1) continue;

        var textOfCharacteristicInText = textToModify.substring(indexOfCharacteristicNameInText, indexOfCharacteristicNameInText + characteristic.text.length)

        textToModify = textToModify.substring(0, indexOfCharacteristicNameInText) + "<strong>" + textOfCharacteristicInText + "</strong>"
            + textToModify.substring(indexOfCharacteristicNameInText + characteristic.text.length);
    }

    return textToModify;
}

function productCompareEditor_OutsideItemCopiedCharacteristics_hide(outsideItemCopiedCharacteristicsModalId)
{
    var outsideItemCopiedCharacteristicsModal = document.getElementById(outsideItemCopiedCharacteristicsModalId);

    if (outsideItemCopiedCharacteristicsModal == null) return;

    outsideItemCopiedCharacteristicsModal.style.display = "none";
}

function productCompareEditor_OutsideItemCopiedCharacteristics_move(e, outsideItemCopiedCharacteristicsModalId)
{
    var outsideItemCopiedCharacteristicsModal = document.getElementById(outsideItemCopiedCharacteristicsModalId);

    if (outsideItemCopiedCharacteristicsModal == null) return;

    outsideItemCopiedCharacteristicsModal.style.left = (e.clientX + 20) + "px";
    outsideItemCopiedCharacteristicsModal.style.top = (e.clientY - outsideItemCopiedCharacteristicsModal.clientHeight - 20) + "px";
}