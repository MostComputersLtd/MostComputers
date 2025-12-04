const promotionFileItemAttributeName = "data-promotion-file-id";

const selectedPromotionFileItemAttributeName = "data-promotion-file-selected-id";

const selectedPromotionFileItemClassName = "promotion-product-file-single-editor-selected-promotion-file";

const selectedPromotionFileItemDefaultValue = "";

const pageLocationInUrl = "/ProductEditor";

const promotionProductFileSingleEditorDataNames =
{
    id: "Id",
    productId: "ProductId",
    active: "Active",
    validFrom: "ValidFrom",
    validTo: "ValidTo",
    shouldAddToImages: "ShouldAddToImagesAll",
    promotionFileInfoId: "PromotionFileInfoId",
};

function selectItemInPromotionFileList(
    tableElementId,
    itemElementId,
    itemActive = null,
    itemValidFrom = null,
    itemValidTo = null,
    activeInputElementId = null,
    validFromInputElementId = null,
    validToInputElementId = null)
{
    const tableElement = document.getElementById(tableElementId);
    const itemElement = document.getElementById(itemElementId);

    const previousSelectedElement = document.querySelector("." + selectedPromotionFileItemClassName);

    if (previousSelectedElement != null
        && previousSelectedElement == itemElement)
    {
        tableElement.setAttribute(selectedPromotionFileItemAttributeName, selectedPromotionFileItemDefaultValue);

        previousSelectedElement.classList.remove(selectedPromotionFileItemClassName);

        return;
    }

    const promotionFileIndexAsString = itemElement.getAttribute(promotionFileItemAttributeName);

    const promotionFileIndex = parseInt(promotionFileIndexAsString);

    if (promotionFileIndex == null
        || isNaN(promotionFileIndex))
    {
        return;
    }

    tableElement.setAttribute(selectedPromotionFileItemAttributeName, promotionFileIndex);

    if (previousSelectedElement != null)
    {
        previousSelectedElement.classList.remove(selectedPromotionFileItemClassName);
    }

    itemElement.classList.add(selectedPromotionFileItemClassName);

    setCheckboxCheckedState(activeInputElementId, itemActive);
    setInputToValueIfNotNull(validFromInputElementId, itemValidFrom);
    setInputToValueIfNotNull(validToInputElementId, itemValidTo);
}

function setInputToValueIfNotNull(inputElementOrElementId, value)
{
    if (value == null || inputElementOrElementId == null) return false;

    var inputElement;

    if (inputElementOrElementId instanceof Element)
    {
        inputElement = inputElementOrElementId;
    }
    else
    {
        inputElement = document.getElementById(inputElementOrElementId);
    }

    if (inputElement == null) return false;

    inputElement.value = value;

    return true;
}

function setCheckboxCheckedState(checkboxInputElementId, value)
{
    if (value == null || checkboxInputElementId == null) return false;

    const checkboxInputElement = document.getElementById(checkboxInputElementId);

    if (checkboxInputElement == null) return false;

    checkboxInputElement.checked = value;

    return true;
}

async function searchPromotionFilesAsync(
    promotionProductFilesSearchInput,
    promotionProductFilesValidOnInput,
    promotionFilesTableContainerElementId,
    selectedPromotionFileId)
{
    const userSearchInputElement = document.getElementById(promotionProductFilesSearchInput);
    const validOnInputElement = document.getElementById(promotionProductFilesValidOnInput);

    const userSearchInputData = (userSearchInputElement.value != null) ? userSearchInputElement.value.trim() : null;

    const validOn = isNaN(Date.parse(validOnInputElement.value)) ? null : validOnInputElement.value;

    const searchOptions = getSearchOptionsDataAsObject(userSearchInputData, validOn);

    const url = pageLocationInUrl + "?handler=GetPromotionFilesSelectableTablePartial"
        + "&selectedPromotionProductFileInfoId=" + selectedPromotionFileId;

    const response = await fetch(url,
        {
            method: 'POST',
            headers:
            {
                'Content-Type': "application/json",
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(searchOptions)
        });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const promotionFilesTableContainerElement = document.getElementById(promotionFilesTableContainerElementId);

    if (promotionFilesTableContainerElement != null)
    {
        promotionFilesTableContainerElement.innerHTML = responseData;
    }
}

function getSearchOptionsDataAsObject(userSearchInputData = null, validOn = null)
{
    return {
        UserInputString: userSearchInputData,
        ValidOnDate: validOn,
    }
}

async function addNewPromotionProductFileAndDisplayChanges(
    productId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    productImageIdInputElementId,
    promotionFileInfoListElementId,
    promotionProductFilesTableContainerElementId,
    promotionProductFileSingleEditorModalContentElementId,
    addNewFileButtonElementId = null,
    addNewFileButtonLoaderElementId = null,
    promotionProductFileSingleEditorModalElementId = null,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const addNewFilePromise = addNewPromotionProductFile(
        productId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        productImageIdInputElementId,
        promotionFileInfoListElementId,
        productDataElementIndex);

    const response = await awaitWithCallbacks(addNewFilePromise,
        function () { toggleViews(addNewFileButtonElementId, addNewFileButtonLoaderElementId); },
        function () { toggleViews(addNewFileButtonElementId, addNewFileButtonLoaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    const promotionProductFileSingleEditorModalContentElement = document.getElementById(promotionProductFileSingleEditorModalContentElementId);

    promotionProductFileSingleEditorModalContentElement.innerHTML = responseData.promotionProductFileInfoSingleEditorPartialAsString;

    if (promotionProductFilesTableContainerElementId != null)
    {
        const promotionProductFilesTableContainerElement = document.getElementById(promotionProductFilesTableContainerElementId);

        promotionProductFilesTableContainerElement.innerHTML = responseData.promotionProductFileInfoTablePartialAsString;
    }

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

    closeModal(promotionProductFileSingleEditorModalElementId);
}

async function addNewPromotionProductFile(
    productId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    shouldAddToImagesInputElementId,
    promotionFileInfoListElementId,
    productDataElementIndex = null)
{
    const promotionFileInfoSingleEditorData = getPromotionProductFileSingleEditorInsertData(
        productId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        shouldAddToImagesInputElementId,
        promotionFileInfoListElementId);

    promotionFileInfoSingleEditorData.append("productDataElementIndex", productDataElementIndex);

    const url = pageLocationInUrl + "?handler=AddNewPromotionFileToProduct";

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: promotionFileInfoSingleEditorData
    });
}

async function updatePromotionProductFileAndDisplayChanges(
    promotionFileId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    productImageIdInputElementId,
    promotionFileInfoListElementId,
    promotionProductFilesTableContainerElementId = null,
    updateFileFileButtonElementId = null,
    updateFileButtonLoaderElementId = null,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const updateFilePromise = updatePromotionProductFile(
        promotionFileId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        productImageIdInputElementId,
        promotionFileInfoListElementId,
        productDataElementIndex);

    const response = await awaitWithCallbacks(updateFilePromise,
        function () { toggleViews(updateFileFileButtonElementId, updateFileButtonLoaderElementId); },
        function () { toggleViews(updateFileFileButtonElementId, updateFileButtonLoaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    if (promotionProductFilesTableContainerElementId != null)
    {
        const promotionProductFilesTableContainerElement = document.getElementById(promotionProductFilesTableContainerElementId);

        promotionProductFilesTableContainerElement.innerHTML = responseData.promotionProductFileInfoEditorPartialAsString;
    }

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

async function updatePromotionProductFile(
    promotionProductFileId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    shouldAddToImagesInputElementId,
    promotionFileInfoListElementId,
    productDataElementIndex = null)
{
    const promotionFileInfoSingleEditorData = getPromotionProductFileSingleEditorUpdateData(
        promotionProductFileId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        shouldAddToImagesInputElementId,
        promotionFileInfoListElementId);

    promotionFileInfoSingleEditorData.append("productDataElementIndex", productDataElementIndex);
   
    const url = pageLocationInUrl + "?handler=UpdatePromotionProductFile";

    return fetch(url,
    {
        method: 'PUT',
        headers:
        {
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: promotionFileInfoSingleEditorData
    });
}

async function deletePromotionProductFile(
    promotionProductFileId,
    promotionProductFilesTableContainerElementId = null,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const url = pageLocationInUrl + "?handler=DeletePromotionProductFile"
        + "&promotionProductFileId=" + promotionProductFileId;

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url,
    {
        method: 'DELETE',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200 || promotionProductFilesTableContainerElementId == null) return;

    const responseData = await response.json();

    if (promotionProductFilesTableContainerElementId != null)
    {
        const promotionProductFilesTableContainerElement = document.getElementById(promotionProductFilesTableContainerElementId);

        promotionProductFilesTableContainerElement.innerHTML = responseData.promotionProductFileInfoEditorPartialAsString;
    }

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

function getPromotionProductFileSingleEditorInsertData(
    productId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    shouldAddToImagesInputElementId,
    promotionFileInfoListElementId)
{
    const promotionFileInfoSingleEditorData = getImageSingleEditorInsertAndUpdateCommonData(
        "promotionProductFileInsertData",
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        shouldAddToImagesInputElementId,
        promotionFileInfoListElementId);

    promotionFileInfoSingleEditorData.append(`promotionProductFileInsertData.${promotionProductFileSingleEditorDataNames.productId}`, productId);

    return promotionFileInfoSingleEditorData;
}

function getPromotionProductFileSingleEditorUpdateData(
    id,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    shouldAddToImagesInputElementId,
    promotionFileInfoListElementId)
{
    const promotionFileInfoSingleEditorData = getImageSingleEditorInsertAndUpdateCommonData(
        "promotionProductFileUpdateData",
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        shouldAddToImagesInputElementId,
        promotionFileInfoListElementId);

    promotionFileInfoSingleEditorData.append(`promotionProductFileUpdateData.${promotionProductFileSingleEditorDataNames.id}`, id);

    return promotionFileInfoSingleEditorData;
}

function getImageSingleEditorInsertAndUpdateCommonData(
    formDataModelName,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    shouldAddToImagesInputElementId,
    promotionFileInfoListElementId = null)
{
    const activeInputElement = document.getElementById(activeInputElementId);
    const validFromInputElement = document.getElementById(validFromInputElementId);
    const validToInputElement = document.getElementById(validToInputElementId);
    const shouldAddToImagesInputElement = document.getElementById(shouldAddToImagesInputElementId);
    const promotionFileInfoListElement = document.getElementById(promotionFileInfoListElementId);

    const active = activeInputElement.checked;

    const validFromAsDate = Date.parse(validFromInputElement.value);

    const validFrom = isNaN(validFromAsDate) ? null : validFromInputElement.value;

    const validToAsDate = Date.parse(validToInputElement.value);

    const validTo = isNaN(validToAsDate) ? null : validToInputElement.value;

    const shouldAddToImages = shouldAddToImagesInputElement.checked;

    const formData = new FormData();

    formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.active}`, active);
    formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.validFrom}`, validFrom);
    formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.validTo}`, validTo);
    formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.shouldAddToImages}`, shouldAddToImages);

    if (promotionFileInfoListElement != null)
    {
        const promotionFileInfoIdAsString = promotionFileInfoListElement.getAttribute(selectedPromotionFileItemAttributeName);

        const promotionFileInfoId = getIntOrNullFromString(promotionFileInfoIdAsString);

        formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.promotionFileInfoId}`, promotionFileInfoId);
    }

    return formData;
}