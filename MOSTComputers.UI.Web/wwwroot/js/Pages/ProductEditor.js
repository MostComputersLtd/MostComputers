const productEditorItemProductIdAttributeName = "data-for-image-save-product-id";

const productEditorItemIndexAttributeName = "data-for-image-save-item-index";

const selectedProductEditorItemAttributeName = "data-selected-for-image-save";
const selectedProductToSaveImagesForClassName = "selected-for-image-save";

const lastClickedProductToSaveImagesForAttributeName = "data-last-clicked-on-for-image-save";

const saveAllButtonAttributeName = "data-for-image-save-save-all-button";

const productEditorProductDataTableContainerId = "productDataTableContainer";

const searchOptionValueElementIds =
{
    productSearchInputId: "images_comparer_compare_product_id_input",
    categoryIdSelectId: "images_comparer_category_to_compare_select",
    productStatusSelectId: "images_comparer_product_status_select",
    productNewStatusSelectId: "images_comparer_product_new_status_select",
    promotionStatusSelectId: "images_comparer_promotion_status_select"
};

const productEditorSearchOptionsStorageKey = "productEditor_searchOptions";

document.addEventListener("DOMContentLoaded", () =>
{
    const cachedSearchOptions = getCachedSearchOptions();

    if (!cachedSearchOptions) return;

    setInputToValueIfNotNull(searchOptionValueElementIds.productSearchInputId, cachedSearchOptions.UserInputString)
    setInputToValueIfNotNull(searchOptionValueElementIds.categoryIdSelectId, cachedSearchOptions.CategoryId)
    setInputToValueIfNotNull(searchOptionValueElementIds.productStatusSelectId, cachedSearchOptions.ProductStatus)
    setInputToValueIfNotNull(searchOptionValueElementIds.productNewStatusSelectId, cachedSearchOptions.ProductNewStatuses?.join(','))
    setInputToValueIfNotNull(searchOptionValueElementIds.promotionStatusSelectId, cachedSearchOptions.PromotionSearchOptions)

    getAllSearchedProductsAndDisplayChangesAsync(productEditorProductDataTableContainerId);
});

function getCachedSearchOptions()
{
    return getComplexObjectFromStorage(window.sessionStorage, productEditorSearchOptionsStorageKey);
}

function updateCachedSearchOptionsWithCurrentData()
{
    const searchOptions = getSearchOptionsFromValueInputs();

    if (!searchOptions) return;

    setComplexObjectToStorage(window.sessionStorage, productEditorSearchOptionsStorageKey, searchOptions);
}

function onChangeProductNewStatusesSelect()
{
    const productNewStatusSelect = document.getElementById(searchOptionValueElementIds.productNewStatusSelectId);

    const productNewStatusSelectValue = getIntOrNullFromString(productNewStatusSelect.value);

    if (productNewStatusSelectValue !== 6
        && productNewStatusSelectValue !== 7)
    {
        updateCachedSearchOptionsWithCurrentData();

        return;
    }

    const productSearchInput = document.getElementById(searchOptionValueElementIds.productSearchInputId);

    productSearchInput.value = "";

    const categoryIdSelect = document.getElementById(searchOptionValueElementIds.categoryIdSelectId);

    categoryIdSelect.value = "";

    const productStatusSelect = document.getElementById(searchOptionValueElementIds.productStatusSelectId);

    productStatusSelect.value = "";

    const promotionStatusSelect = document.getElementById(searchOptionValueElementIds.promotionStatusSelectId);

    promotionStatusSelect.value = "";
    
    updateCachedSearchOptionsWithCurrentData();
}

function getElementIndexOfProductRowElement(productRowElementId)
{
    if (!productRowElementId) return null;

    const productRowElement = document.getElementById(productRowElementId);

    if (!productRowElement) return null;

    const elementIndexAsString = productRowElement.getAttribute(productEditorItemIndexAttributeName);

    return getIntOrNullFromString(elementIndexAsString);
}

async function getProductPropertiesEditorPopupData(
    event,
    productId,
    productPropertiesEditorModalId,
    productPropertiesEditorModalDialogId,
    productPropertiesEditorModalContentId,
    relatedProductDataElementId = null)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=GetProductPropertiesEditorPartial"
        + "&productId=" + productId;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(productPropertiesEditorModalId, productPropertiesEditorModalDialogId, productPropertiesEditorModalContentId, responseData);
}

async function getSearchStringPartsPopupData(
    event,
    productId,
    searchStringPartsModalId,
    searchStringPartsModalDialogId,
    searchStringPartsModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }
    
    const url = "/ProductEditor" + "?handler=GetSearchStringPartsEditorPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(searchStringPartsModalId, searchStringPartsModalDialogId, searchStringPartsModalContentId, responseData);
}

async function getXmlViewPopupData(
    event,
    productId,
    xmlViewDisplayModalId,
    xmlViewDisplayModalDialogId,
    xmlViewDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetXmlViewPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(xmlViewDisplayModalId, xmlViewDisplayModalDialogId, xmlViewDisplayModalContentId, responseData);
}

async function openOldXmlWindow(
    event,
    productId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetOldXmlData"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const blob = new Blob([responseData], { type: 'application/xml' });

    const objectUrl = URL.createObjectURL(blob);

    const newWindow = window.open(objectUrl, '_blank');
    newWindow.onload = function ()
    {
        URL.revokeObjectURL(objectUrl);
    };
}

async function getPropertiesDisplayPopupData(
    event,
    productId,
    propertiesDisplayModalId,
    propertiesDisplayModalDialogId,
    propertiesDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetPropertiesDisplayPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(propertiesDisplayModalId, propertiesDisplayModalDialogId, propertiesDisplayModalContentId, responseData);
}

async function getOldXmlPropertiesDisplayPopupData(
    event,
    productId,
    oldXmlPropertiesDisplayModalId,
    oldXmlPropertiesDisplayModalDialogId,
    oldXmlPropertiesDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetOldXmlPropertiesDisplayPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(oldXmlPropertiesDisplayModalId, oldXmlPropertiesDisplayModalDialogId, oldXmlPropertiesDisplayModalContentId, responseData);
}

async function getImageDisplayPopupData(event,
    productId,
    isFirstImageOnly,
    imagesDisplayModalId,
    imagesDisplayModalDialogId,
    imagesDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetImagesDisplayPartial"
        + "&productId=" + productId + "&firstImageOnly=" + isFirstImageOnly + "&isOriginalData=false";

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(imagesDisplayModalId, imagesDisplayModalDialogId, imagesDisplayModalContentId, responseData);
}

async function getOriginalImageDisplayPopupData(event,
    productId,
    isFirstImageOnly,
    imagesDisplayModalId,
    imagesDisplayModalDialogId,
    imagesDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetImagesDisplayPartial"
        + "&productId=" + productId + "&firstImageOnly=" + isFirstImageOnly + "&isOriginalData=true";

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(imagesDisplayModalId, imagesDisplayModalDialogId, imagesDisplayModalContentId, responseData);
}

async function getImageFileDisplayPopupData(event,
    productId,
    imageFilesDisplayModalId,
    imageFilesDisplayModalDialogId,
    imageFilesDisplayModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetImageFilesDisplayPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(imageFilesDisplayModalId, imageFilesDisplayModalDialogId, imageFilesDisplayModalContentId, responseData);
}

async function getImageFileNameInfoDisplayPopupData(event,
    productId,
    imageFileNameInfosModalId,
    imageFileNameInfosModalDialogId,
    imageFileNameInfosModalContentId)
{
    if (isNaN(parseInt(productId))) return;

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetImageFileNameInfosDisplayPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(imageFileNameInfosModalId, imageFileNameInfosModalDialogId, imageFileNameInfosModalContentId, responseData);
}

async function getPromotionViewPopupData(event,
    productId,
    promotionId,
    promotionViewModalId,
    promotionViewModalDialogId,
    promotionViewModalContentId)
{
    if (isNaN(parseInt(productId))
        || isNaN(parseInt(promotionId)))
    {
        return;
    }

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetPromotionViewPartial"
        + "&productId=" + productId
        + "&promotionId=" + promotionId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(promotionViewModalId, promotionViewModalDialogId, promotionViewModalContentId, responseData);
}

async function getInfoPromotionViewPopupData(event,
    productId,
    infoPromotionViewModalId,
    infoPromotionViewModalDialogId,
    infoPromotionViewModalContentId)
{
    if (isNaN(parseInt(productId)))
    {
        return;
    }

    if (event)
    {
        event.stopPropagation();
    }

    const url = "/ProductEditor" + "?handler=GetInfoPromotionViewPartial"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(infoPromotionViewModalId, infoPromotionViewModalDialogId, infoPromotionViewModalContentId, responseData);
}

async function getPromotionProductFileEditorPopupData(event,
    productId,
    promotionProductFileEditorModalId,
    promotionProductFileEditorModalDialogId,
    promotionProductFileEditorModalContentId,
    relatedProductDataElementId = null)
{
    if (isNaN(parseInt(productId)))
    {
        return;
    }

    if (event)
    {
        event.stopPropagation();
    }

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=GetPromotionProductFileEditorPartial"
        + "&productId=" + productId;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(promotionProductFileEditorModalId, promotionProductFileEditorModalDialogId, promotionProductFileEditorModalContentId, responseData);
}

async function getPromotionProductFileSingleEditorPopupData(
    event,
    productId,
    promotionProductFileSingleEditorModalId,
    promotionProductFileSingleEditorModalDialogId,
    promotionProductFileSingleEditorModalContentId,
    promotionProductFileInfoId = null,
    relatedProductDataElementId = null)
{
    if (isNaN(parseInt(productId)))
    {
        return;
    }
    
    if (event)
    {
        event.stopPropagation();
    }

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    var url = "/ProductEditor" + "?handler=GetPromotionProductFileSingleEditorPartial"
        + "&productId=" + productId
        + "&promotionProductFileInfoId=" + promotionProductFileInfoId;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(
        promotionProductFileSingleEditorModalId,
        promotionProductFileSingleEditorModalDialogId,
        promotionProductFileSingleEditorModalContentId,
        responseData);
}

async function getAllSearchedProductsAndDisplayChangesAsync(
    productEditorDataTablePartialContainerId,
    productEditorDataTableBodyId = null,
    getImageDataInCategoryButtonId = null,
    loaderElementId = null)
{
    const selectedElements = getAllSelectedProductImagesElements();

    if (selectedElements.length > 0)
    {
        displaySelectedProductsInSearch(selectedElements, productEditorDataTableBodyId);

        return;
    }

    const searchOptions = getSearchOptionsFromValueInputs();

    const promise = getAllSearchedProductsAsync(searchOptions);

    const response = await awaitWithCallbacks(promise,
        function () { toggleViews(getImageDataInCategoryButtonId, loaderElementId); },
        function () { toggleViews(getImageDataInCategoryButtonId, loaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const productEditorDataTablePartialContainer = document.getElementById(productEditorDataTablePartialContainerId);

    productEditorDataTablePartialContainer.innerHTML = responseData;
}

function displaySelectedProductsInSearch(selectedElements, productEditorDataTableBodyId = null)
{
    if (!productEditorDataTableBodyId) return;

    const productEditorDataTableBody = document.getElementById(productEditorDataTableBodyId);

    const productSearchInput = document.getElementById(searchOptionValueElementIds.productSearchInputId);

    const productEditorDataTableRows = [...productEditorDataTableBody.querySelectorAll(`[${productEditorItemIndexAttributeName}]`)];

    productEditorDataTableRows.forEach(row =>
    {
        row.remove();
    });

    const productIds = [];

    selectedElements.forEach(selectedElement =>
    {
        productEditorDataTableBody.appendChild(selectedElement);

        const productIdAsString = selectedElement.getAttribute(productEditorItemProductIdAttributeName);

        const productId = getIntOrNullFromString(productIdAsString);

        if (productId)
        {
            productIds.push(productId);
        }
    });

    const productIdsAsInputString = productIds.join(',');

    productSearchInput.value = productIdsAsInputString;
}

async function getAllSearchedProductsAsync(searchOptions)
{
    const url = "/ProductEditor" + "?handler=GetAllSearchedProducts";

    return await fetch(url,
    {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(searchOptions)
    });
}

function getSearchOptionsFromValueInputs()
{
    const productSearchInput = document.getElementById(searchOptionValueElementIds.productSearchInputId);
    const categoryIdSelect = document.getElementById(searchOptionValueElementIds.categoryIdSelectId);
    const productStatusSelect = document.getElementById(searchOptionValueElementIds.productStatusSelectId);
    const productNewStatusesSelect = document.getElementById(searchOptionValueElementIds.productNewStatusSelectId);
    const promotionStatusSelect = document.getElementById(searchOptionValueElementIds.promotionStatusSelectId);

    const searchOptions = getSearchOptionsAsObject(
        productSearchInput.value.trim(),
        categoryIdSelect.value,
        productStatusSelect.value,
        productNewStatusesSelect.value,
        promotionStatusSelect.value);

    return searchOptions;
}

function getSearchOptionsAsObject(
    productSearchData = null,
    categoryId = null,
    productStatus = null,
    productNewStatusesAsString = null,
    promotionStatus = null)
{
    const categoryIdInOptions = getIntOrNullFromString(categoryId);

    const productStatusInOptions = getIntOrNullFromString(productStatus);

    const productNewStatusesInOptions = getIntOrNullFromString(productNewStatusesAsString);

    const promotionStatusInOptions = getIntOrNullFromString(promotionStatus);

    return {
        UserInputString: productSearchData,
        CategoryId: categoryIdInOptions,
        ProductStatusSearchOptions: productStatusInOptions,
        ProductNewStatusSearchOptions: productNewStatusesInOptions,
        PromotionSearchOptions: promotionStatusInOptions
    }
}

async function saveAllMatchedXmlPropertiesAndImagesForProducts(
    productEditorDataTablePartialContainerId,
    shouldGetAllIfNoneSelected = false)
{
    const selectedProductIds = getSelectedProductIdsOrAllIds(shouldGetAllIfNoneSelected);

    const searchOptions = getSearchOptionsFromValueInputs();

    const data =
    {
        ProductIds: selectedProductIds,
        SearchOptions: searchOptions
    }
   
    const url = "/ProductEditor" + "?handler=SaveAllMatchedXmlPropertiesAndImagesForProducts";

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(data)
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const productEditorDataTablePartialContainer = document.getElementById(productEditorDataTablePartialContainerId);

    productEditorDataTablePartialContainer.innerHTML = responseData;
}

async function saveAllMatchedXmlPropertiesAndImages(productId, productEditorDataTablePartialContainerId)
{
    if (isNaN(parseInt(productId)))
    {
        return;
    }

    const url = "/ProductEditor" + "?handler=SaveAllMatchedXmlPropertiesAndImages"
        + "&productId=" + productId;

    const searchOptions = getSearchOptionsFromValueInputs();

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(searchOptions),
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const productEditorDataTablePartialContainer = document.getElementById(productEditorDataTablePartialContainerId);

    productEditorDataTablePartialContainer.innerHTML = responseData;
}

async function saveXmlPropertyForProductToLocalProperties(productId, propertyRelationId, productEditorDataTablePartialContainerId)
{
    if (isNaN(parseInt(productId))
        || isNaN(parseInt(propertyRelationId)))
    {
        return;
    }
    
    var url = "/ProductEditor" + "?handler=SaveXmlPropertyForProductToLocalProperties"
        + "&productId=" + productId
        + "&relationshipId=" + propertyRelationId;

    const searchOptions = getSearchOptionsFromValueInputs();

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(searchOptions),
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const productEditorDataTablePartialContainer = document.getElementById(productEditorDataTablePartialContainerId);

    productEditorDataTablePartialContainer.innerHTML = responseData;
}

function getSelectedProductIdsOrAllIds(shouldGetAllIfNoneSelected = false)
{
    var selectedProductElements = [...document.querySelectorAll('[' + selectedProductEditorItemAttributeName + '="true"]')];

    if (shouldGetAllIfNoneSelected && selectedProductElements.length <= 0)
    {
        selectedProductElements = [...document.querySelectorAll('[' + productEditorItemProductIdAttributeName + ']')]
    }

    const selectedProductIds = [];

    if (selectedProductElements.length <= 0) return selectedProductIds;

    selectedProductElements.forEach(function (selectedProductElement)
    {
        const selectedProductId = selectedProductElement.getAttribute(productEditorItemProductIdAttributeName);

        selectedProductIds.push(parseInt(selectedProductId));
    });

    return selectedProductIds;
}

function onClickProductRowSelectCheckbox(
    event,
    productDataElementId,
    productDataCheckboxElementId,
    productDataElementIdPrefix,
    productDataCheckboxElementIdPrefix)
{
    const selectedElement = document.getElementById(productDataElementId);

    const selectedProductElements = getAllSelectedProductImagesElements();

    const lastClickedProduct = document.querySelector(`[${lastClickedProductToSaveImagesForAttributeName}]`);

    const lastOrClosestElement = (lastClickedProduct != null) ? lastClickedProduct : findClosestSelectedImageElement(selectedElement, selectedProductElements);

    if (!lastOrClosestElement)
    {
        toggleElementSelectionToSaveImages(selectedElement.id, productDataCheckboxElementId);

        return;
    }

    const elementIndex = getImageSaveElementIndex(selectedElement);

    const otherElementIndex = getImageSaveElementIndex(lastOrClosestElement);

    if (event.ctrlKey)
    {
        selectOrDeselectElementsBetweenTwoIndexes(
            elementIndex,
            otherElementIndex,
            false,
            productDataElementIdPrefix,
            productDataCheckboxElementIdPrefix);

        if (lastOrClosestElement)
        {
            lastOrClosestElement.removeAttribute(lastClickedProductToSaveImagesForAttributeName);
        }

        selectedElement.setAttribute(lastClickedProductToSaveImagesForAttributeName, "true");

        return;
    }

    if (!event.shiftKey
        || isProductImagesElementSelected(selectedElement)
        || selectedProductElements.length <= 0)
    {
        selectOrDeselectElementsBetweenTwoIndexes(
            0,
            getHighestProductItemIndex(),
            false,
            productDataElementIdPrefix,
            productDataCheckboxElementIdPrefix);

        if (lastOrClosestElement)
        {
            lastOrClosestElement.removeAttribute(lastClickedProductToSaveImagesForAttributeName);
        }

        toggleElementSelectionToSaveImages(productDataElementId, productDataCheckboxElementId);

        selectedElement.setAttribute(lastClickedProductToSaveImagesForAttributeName, "true");

        return;
    }

    selectOrDeselectElementsBetweenTwoIndexes(
        elementIndex,
        otherElementIndex,
        true,
        productDataElementIdPrefix,
        productDataCheckboxElementIdPrefix);

    if (lastOrClosestElement)
    {
        lastOrClosestElement.removeAttribute(lastClickedProductToSaveImagesForAttributeName);
    }

    selectedElement.setAttribute(lastClickedProductToSaveImagesForAttributeName, "true");
}

function selectOrDeselectElementsBetweenTwoIndexes(
    elementIndex,
    otherElementIndex,
    shouldElementsBeSelected,
    productDataElementIdPrefix,
    productDataCheckboxElementIdPrefix)
{
    const smallerIndex = Math.min(elementIndex, otherElementIndex);
    const largerIndex = Math.max(elementIndex, otherElementIndex);

    for (var i = smallerIndex; i <= largerIndex; i++)
    {
        const elementToSelect = document.getElementById(productDataElementIdPrefix + i.toString());

        const elementCheckboxId = productDataCheckboxElementIdPrefix + i.toString();

        if (elementToSelect == null)
        {
            continue;
        }

        setElementSelectionToSaveImages(shouldElementsBeSelected, elementToSelect.id, elementCheckboxId);
    }
}

function findClosestSelectedImageElement(elementToSearchAround, selectedProductElements)
{
    const elementIndex = getImageSaveElementIndex(elementToSearchAround);

    var closestElement;
    var currentMinDistance = null;

    for (var i = 0; i < selectedProductElements.length; i++)
    {
        const currentElement = selectedProductElements[i];

        if (currentElement == elementToSearchAround) continue;

        const currentElementIndex = getImageSaveElementIndex(currentElement);

        const distanceBetweenCurrentAndSelectedElements = Math.abs(elementIndex - currentElementIndex);

        if (currentMinDistance == null
            || currentMinDistance > distanceBetweenCurrentAndSelectedElements)
        {
            currentMinDistance = distanceBetweenCurrentAndSelectedElements;
            closestElement = currentElement;
        }
    }

    return closestElement;
}

function getHighestProductItemIndex()
{
    const selectedElements = getAllSelectedProductImagesElements();

    var output = -1;

    selectedElements.forEach(element =>
    {
        const index = getImageSaveElementIndex(element)

        if (index > output)
        {
            output = index;
        }
    });

    return output;
}

function toggleElementSelectionToSaveImages(elementId, elementCheckboxId = null)
{
    const element = document.getElementById(elementId);

    const shouldElementBeSelected = !isProductImagesElementSelected(element);

    setElementSelectionToSaveImages(shouldElementBeSelected, elementId, elementCheckboxId);
}

function setElementSelectionToSaveImages(shouldElementBeSelected, elementId, elementCheckboxId = null)
{
    const element = document.getElementById(elementId);

    const elementCheckbox = (elementCheckboxId != null) ? document.getElementById(elementCheckboxId) : null;

    if (!shouldElementBeSelected)
    {
        element.removeAttribute(selectedProductEditorItemAttributeName);
        element.classList.remove(selectedProductToSaveImagesForClassName);

        if (elementCheckbox)
        {
            elementCheckbox.checked = false;
        }

        return;
    }

    element.setAttribute(selectedProductEditorItemAttributeName, "true");
    element.classList.add(selectedProductToSaveImagesForClassName);

    if (elementCheckbox)
    {
        elementCheckbox.checked = true;
    }
}

function isProductImagesElementSelected(productImagesElement)
{
    return productImagesElement.getAttribute(selectedProductEditorItemAttributeName) == "true";
}

function getAllSelectedProductImagesElements()
{
    return [...document.querySelectorAll('[' + selectedProductEditorItemAttributeName + '="true"]')];
}

function getImageSaveElementIndex(element)
{
    return element.getAttribute(productEditorItemIndexAttributeName);
}