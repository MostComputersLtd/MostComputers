const propertyCharacteristicIdSelectCurrentValueAttributeName = "data-current-value";

const elementGroupAttributeName = "data-element-group";

const propertyGroupAttributeName = "data-property-group";
const propertyIndexAttributeName = "data-property-index";
const propertyElementIdsPrefixAttributeName = "data-property-prefix";

const propertyEditorItemValueElementIdsWithoutPrefix =
{
    propertyCharacteristicIdSelectName: "productPropertyDisplay_Characteristic_select",
    propertyValueDivName: "productPropertyDisplay_Value_div",
    propertyXmlPlacementSelectName: "productPropertyDisplay_XmlPlacement_select"
};

async function addPropertyToTableAndDisplayChanges(
    productId,
    productCharacteristicType,
    propertyGroup,
    propertyTableBodyId,
    relatedProductDataElementId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const response = await addPropertyToTable(productId, productCharacteristicType, propertyGroup, productDataElementIndex);

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200)
    {
        if (notificationBoxId != null)
        {
            showNotificationWithText(notificationBoxId, "Failed to add property", "notificationBox-long-message");
        }

        return;
    }

    const responseData = await response.text();

    const propertyElementPartial = getSinglePropertyPartialFromText(responseData);

    const propertyTableBody = document.getElementById(propertyTableBodyId)

    if (propertyTableBody == null) return;

    propertyTableBody.append(propertyElementPartial);
}

async function addPropertyToTable(
    productId,
    productCharacteristicType,
    propertyGroup,
    productDataElementIndex = null)
{
    const allPropertiesData = getAllPropertiesDataInGroup(propertyGroup);

    var url = "/ProductEditor" + "?handler=AddNewProperty"
        + "&productId=" + productId
        + "&productCharacteristicType=" + productCharacteristicType
        + "&propertyEditorPropertyGroupValue=" + propertyGroup;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': "application/json; charset=utf-8",
            'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(allPropertiesData)
    });

    return response;
}

async function upsertPropertyInTable(
    productId,
    propertyIndex,
    propertyGroup,
    productPropertiesEditorTableElementId = null,
    productNewStatusCheckboxesIds = null,
    relatedProductDataElementId = null,
    notificationBoxId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=UpsertProperty"
        + "&productId=" + productId
        + "&propertyIndex=" + propertyIndex
        + "&propertyEditorPropertyGroupValue=" + propertyGroup;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const allPropertiesData = getAllPropertiesDataInGroup(propertyGroup);

    const data =
    {
        ProductPropertyEditorDataList: allPropertiesData,
    };

    const response = await fetch(url,
    {
        method: "PUT",
        headers: {
            'Content-Type': "application/json; charset=utf-8",
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(data)
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200)
    {
        if (notificationBoxId != null)
        {
            showNotificationWithText(notificationBoxId, "Failed to update property", "notificationBox-long-message");
        }

        return;
    }

    const responseData = await response.json();

    const propertyPartial = getSinglePropertyPartialFromText(responseData.propertyPartialAsString);

    upsertPropertyElementInTable(propertyIndex, propertyGroup, propertyPartial, productPropertiesEditorTableElementId);

    displayProductNewStatus(responseData.productNewStatus, productNewStatusCheckboxesIds);

    if (relatedProductDataElementId)
    {
        const relatedProductDataElement = document.getElementById(relatedProductDataElementId);

        const productDataTableRowPartial = getElementFromText(responseData.productDataTableRowPartialString);

        relatedProductDataElement.parentNode.replaceChild(productDataTableRowPartial, relatedProductDataElement);
    }
}

async function upsertAllPropertiesInTable(
    productId,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null,
    notificationBoxId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=UpsertAllProductProperties"
        + "&productId=" + productId;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const propertyElements = [...document.querySelectorAll(`[${propertyGroupAttributeName}]`)];

    const allPropertiesData = getAllPropertiesDataByPropertyElements(propertyElements);

    const searchOptions = getSearchOptionsFromValueInputs();

    const data =
    {
        ProductPropertyEditorDataList: allPropertiesData,
    };

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            'Content-Type': "application/json; charset=utf-8",
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(data)
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200)
    {
        if (notificationBoxId != null)
        {
            showNotificationWithText(notificationBoxId, "Failed to update properties", "notificationBox-long-message");
        }

        return;
    }

    const responseData = await response.json();

    if (productPropertiesEditorContainerElementId)
    {
        const productPropertiesEditorContainerElement = document.getElementById(productPropertiesEditorContainerElementId);

        productPropertiesEditorContainerElement.innerHTML = responseData.productPropertiesEditorPartialAsString;
    }

    if (relatedProductDataElementId)
    {
        const relatedProductDataElement = document.getElementById(relatedProductDataElementId);

        const productDataTableRowPartial = getElementFromText(responseData.productDataTableRowPartialString);

        relatedProductDataElement.parentNode.replaceChild(productDataTableRowPartial, relatedProductDataElement);
    }
}

async function productPropertyDisplay_Characteristic_select_onchange(
    productId,
    propertyGroup,
    propertyIndex,
    currentProductCharacteristicSelectId,
    characteristicSelectNewValue,
    productCharacteristicSelectName,
    productPropertiesEditorTableElementId = null,
    productNewStatusCheckboxesIds = null,
    relatedProductDataElementId = null,
    notificationBoxId = null)
{
    const currentSelectElement = document.getElementById(currentProductCharacteristicSelectId);

    const currentSelectElementPreviousValue = currentSelectElement.getAttribute(propertyCharacteristicIdSelectCurrentValueAttributeName);

    if (!currentSelectElementPreviousValue)
    {
        currentSelectElement.value = characteristicSelectNewValue;

        currentSelectElement.setAttribute(propertyCharacteristicIdSelectCurrentValueAttributeName, characteristicSelectNewValue);

        updateAllCharacteristicSelectElementOptions(
            productCharacteristicSelectName,
            currentSelectElementPreviousValue,
            characteristicSelectNewValue,
            currentSelectElement);

        return;
    }

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);
   
    const updatePropertyCharacteristicIdResponse = await updatePropertyCharacteristicId(
        productId,
        propertyIndex,
        propertyGroup,
        parseInt(currentSelectElementPreviousValue),
        productDataElementIndex);
    
    redirectIfResponseIsRedirected(updatePropertyCharacteristicIdResponse);

    if (updatePropertyCharacteristicIdResponse == null
        || updatePropertyCharacteristicIdResponse.status !== 200)
    {
        if (notificationBoxId != null)
        {
            showNotificationWithText(notificationBoxId, "Failed to switch characteristic", "notificationBox-long-message");
        }

        currentSelectElement.value = currentSelectElementPreviousValue;

        return;
    }

    const responseData = await updatePropertyCharacteristicIdResponse.json();

    updateAllCharacteristicSelectElementOptions(
        productCharacteristicSelectName,
        currentSelectElementPreviousValue,
        characteristicSelectNewValue,
        currentSelectElement);

    const propertyPartial = getSinglePropertyPartialFromText(responseData.propertyPartialAsString);

    upsertPropertyElementInTable(propertyIndex, propertyGroup, propertyPartial, productPropertiesEditorTableElementId);

    displayProductNewStatus(responseData.productNewStatus, productNewStatusCheckboxesIds);

    if (relatedProductDataElementId)
    {
        const relatedProductDataElement = document.getElementById(relatedProductDataElementId);

        const productDataTableRowPartial = getElementFromText(responseData.productDataTableRowPartialString);

        relatedProductDataElement.parentNode.replaceChild(productDataTableRowPartial, relatedProductDataElement);
    }
}

function updateAllCharacteristicSelectElementOptions(
    productCharacteristicSelectName,
    currentSelectElementPreviousValue = null,
    currentSelectElementNewValue = null,
    currentSelectElement = null)
{
    const elementWithPreviousValue = getSelectOptionElementWithValue(currentSelectElement, currentSelectElementPreviousValue);

    const productCharacteristicSelects = [...document.querySelectorAll('[name="' + productCharacteristicSelectName + '"]')];

    for (var i = 0; i < productCharacteristicSelects.length; i++)
    {
        const productCharacteristicSelect = productCharacteristicSelects[i];

        if (productCharacteristicSelect === currentSelectElement) continue;

        var selectContainsLastValue = false;

        for (var k = 0; k < productCharacteristicSelect.options.length; k++)
        {
            const productCharacteristicSelectItem = productCharacteristicSelect.options[k];

            if (productCharacteristicSelectItem.value === currentSelectElementNewValue)
            {
                productCharacteristicSelectItem.remove();

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
            const newOption = createOptionElement(elementWithPreviousValue.text, elementWithPreviousValue.value);

            productCharacteristicSelect.appendChild(newOption);
        }
    }
}

function createOptionElement(text, value)
{
    const newOption = document.createElement("option");

    newOption.text = text;
    newOption.value = value;

    return newOption;
}

function getSelectOptionElementWithValue(selectElement, value)
{
    if (!selectElement) return null;

    for (var i = 0; i < selectElement.options.length; i++)
    {
        const option = selectElement.options[i];

        if (option.value === value) return option;
    }

    return null;
}

async function updatePropertyCharacteristicId(
    productId,
    propertyIndex,
    propertyGroup,
    oldCharacteristicId,
    productDataElementIndex = null)
{
    if (typeof productId !== "number"
        || productId <= 0
        ||typeof propertyIndex !== "number"
        || propertyIndex < 0
        || isNaN(oldCharacteristicId)
        || oldCharacteristicId <= 0)
    {
        return null;
    }

    var url = "/ProductEditor" + "?handler=ChangePropertyCharacteristicId"
        + "&productId=" + productId
        + "&propertyIndex=" + propertyIndex
        + "&oldCharacteristicId=" + oldCharacteristicId
        + "&propertyEditorPropertyGroupValue=" + propertyGroup;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const allPropertiesData = getAllPropertiesDataInGroup(propertyGroup);

    const data =
    {
        ProductPropertyEditorDataList: allPropertiesData,
    };

    return await fetch(url, {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val(),
        },
        body: JSON.stringify(data)
    });
}

async function deletePropertyInTable(
    productId,
    productCharacteristicId,
    propertyGroup,
    propertyIndex,
    productCharacteristicSelectName,
    productNewStatusCheckboxesIds = null,
    relatedProductDataElementId = null,
    notificationBoxId = null)
{
    if (productCharacteristicId == null
        || isNaN(parseInt(productCharacteristicId)))
    {
        const existingProductPropertyElement = getProductPropertyElementByGroupAndIndex(propertyIndex, propertyGroup);

        if (existingProductPropertyElement)
        {
            existingProductPropertyElement.remove();
        }

        updateAllCharacteristicSelectElementOptions(productCharacteristicSelectName, productCharacteristicId, null, null);

        return;
    }

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=DeleteProperty"
        + "&productId=" + productId
        + "&productCharacteristicId=" + productCharacteristicId;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url, {
        method: "DELETE",
        headers: {
            'Content-Type': "application/json",
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200)
    {
        if (notificationBoxId != null)
        {
            showNotificationWithText(notificationBoxId, "Failed to switch property", "notificationBox-long-message");
        }

        return;
    }

    const responseData = await response.json();

    const existingProductPropertyElement = getProductPropertyElementByGroupAndIndex(propertyIndex, propertyGroup);

    if (existingProductPropertyElement)
    {
        existingProductPropertyElement.remove();
    }

    updateAllCharacteristicSelectElementOptions(productCharacteristicSelectName, productCharacteristicId, null, null);

    displayProductNewStatus(responseData.productNewStatus, productNewStatusCheckboxesIds);

    if (relatedProductDataElementId)
    {
        const relatedProductDataElement = document.getElementById(relatedProductDataElementId);

        const productDataTableRowPartial = getElementFromText(responseData.productDataTableRowPartialString);

        relatedProductDataElement.parentNode.replaceChild(productDataTableRowPartial, relatedProductDataElement);
    }
}

function getAllPropertiesDataInGroup(propertyGroup)
{
    const propertyElements = [...document.querySelectorAll(`[${propertyGroupAttributeName}="${propertyGroup}"]`)];

    return getAllPropertiesDataByPropertyElements(propertyElements);
}

function getAllPropertiesDataByPropertyElements(propertyElements)
{
    const data = [];

    for (var i = 0; i < propertyElements.length; i++)
    {
        const propertyElement = propertyElements[i];

        const propertyGroupAsString = propertyElement.getAttribute(propertyGroupAttributeName);
        const propertyGroup = getIntOrNullFromString(propertyGroupAsString);

        const propertyIndexAsString = propertyElement.getAttribute(propertyIndexAttributeName);
        const propertyIndex = getIntOrNullFromString(propertyIndexAsString);

        const propertyElementIdsPrefix = propertyElement.getAttribute(propertyElementIdsPrefixAttributeName);

        const propertyCharacteristicIdSelectName = propertyElementIdsPrefix + propertyEditorItemValueElementIdsWithoutPrefix.propertyCharacteristicIdSelectName;
        const propertyValueDivName = propertyElementIdsPrefix + propertyEditorItemValueElementIdsWithoutPrefix.propertyValueDivName;
        const propertyXmlPlacementSelectName = propertyElementIdsPrefix + propertyEditorItemValueElementIdsWithoutPrefix.propertyXmlPlacementSelectName;

        const propertyCharacteristicIdSelect = propertyElement.querySelector(`[${elementGroupAttributeName}=${propertyCharacteristicIdSelectName}]`);
        const productPropValueInput = propertyElement.querySelector(`[${elementGroupAttributeName}=${propertyValueDivName}]`);
        const productPropXmlPlacementSelect = propertyElement.querySelector(`[${elementGroupAttributeName}=${propertyXmlPlacementSelectName}]`);

        const propertyCharacteristicIdAsString = propertyCharacteristicIdSelect.value;

        const propertyCharacteristicId = getIntOrNullFromString(propertyCharacteristicIdAsString);

        const propertyValue = productPropValueInput.innerText.trim();

        const productXmlPlacementString = productPropXmlPlacementSelect?.value;

        const productXmlPlacement = getIntOrNullFromString(productXmlPlacementString);

        const dataEntry =
        {
            PropertyIndex: propertyIndex,
            PropertyGroup: propertyGroup,
            ProductCharacteristicId: propertyCharacteristicId,
            Value: propertyValue,
            XmlPlacement: productXmlPlacement,
        };

        data.push(dataEntry);
    }

   return data;
}

function getProductPropertyElementByGroupAndIndex(propertyIndex, propertyGroup)
{
    return document.querySelector(`[${propertyIndexAttributeName}="${propertyIndex}"][${propertyGroupAttributeName}="${propertyGroup}"]`);
}

function getSinglePropertyPartialFromText(singlePropertyPartialAsText)
{
    return getElementFromText("<table>" + singlePropertyPartialAsText + "</table>")
        .children[0]
        .children[0];
}

function displayProductNewStatus(productNewStatus, productNewStatusCheckboxesIds)
{
    if (productNewStatusCheckboxesIds)
    {
        productNewStatusCheckboxesIds.forEach(productNewStatusCheckboxId =>
        {
            const productNewStatusCheckbox = document.getElementById(productNewStatusCheckboxId);

            productNewStatusCheckbox.value = productNewStatus;
        });
    }
}

function upsertPropertyElementInTable(index, group, propertyPartial, productPropertiesEditorTableElementId)
{
    const existingProductPropertyElement = getProductPropertyElementByGroupAndIndex(index, group);

    if (existingProductPropertyElement)
    {
        existingProductPropertyElement.replaceWith(propertyPartial);
    }
    else if (productPropertiesEditorTableElementId)
    {
        const productPropertiesEditorTableElement = document.getElementById(productPropertiesEditorTableElementId);

        productPropertiesEditorTableElement.append(propertyPartial);
    }
}

async function updateProductNewStatusFromSelect(
    productId,
    productStatusSelectElementId,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const productStatusSelectElement = document.getElementById(productStatusSelectElementId);

    const productStatus = productStatusSelectElement.value;

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const response = await updateProductNewStatus(productId, productStatus, productDataElementIndex);

    redirectIfResponseIsRedirected(response);

    if (response.status != 200) return;

     const responseData = await response.json();

    if (productPropertiesEditorContainerElementId)
    {
        const productPropertiesEditorContainerElement = document.getElementById(productPropertiesEditorContainerElementId);

        productPropertiesEditorContainerElement.innerHTML = responseData.productPropertiesEditorPartialAsString;
    }

    if (relatedProductDataElementId)
    {
        const relatedProductDataElement = document.getElementById(relatedProductDataElementId);

        const productDataTableRowPartial = getElementFromText(responseData.productDataTableRowPartialString);

        relatedProductDataElement.parentNode.replaceChild(productDataTableRowPartial, relatedProductDataElement);
    }
}

async function updateProductNewStatus(productId, productNewStatus, productDataElementIndex)
{
    var url = "/ProductEditor" + "?handler=UpdateProductNewStatus"
        + "&productId=" + productId
        + "&productNewStatus=" + productNewStatus;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }
    

    return await fetch(url, {
        method: "PUT",
        headers: {
            'Content-Type': "application/json",
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}