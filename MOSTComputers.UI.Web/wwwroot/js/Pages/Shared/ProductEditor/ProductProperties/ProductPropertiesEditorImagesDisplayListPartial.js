const productImageUpsertDataNames =
{
    IncludeHtmlDataView: "IncludeHtmlDataView",
    ProductId: "ProductId",
    ExistingImageId: "ExistingImageId",
    File: "File",
    FileUpsertData: "FileUpsertData",
};

const productImageFileUpsertDataNames =
{
    ExistingFileInfoId: "ExistingFileInfoId",
    DisplayOrder: "DisplayOrder",
    Active: "Active",
}

const imageListItemImageIdAttributeName = "data-image-list-item-image-id";

const imageListItemInteractableWithDragAndDropAttributeName = "data-drag-and-drop-interactable-image-list-item";

const draggedImageListItemAttributeName = "data-dragged-image-list-item";

function onDragStartImageListItem(event)
{
    event.target.setAttribute(draggedImageListItemAttributeName, "true");
}

function onDragOverImageList(event, imageListElementId)
{
    event.preventDefault();

    const productImageDisplayList = document.getElementById(imageListElementId);

    const notDraggedListItemsQuery = `[${imageListItemInteractableWithDragAndDropAttributeName}='true']:not([${draggedImageListItemAttributeName}='true'])`;

    const allImageListItemElementsThatArentDragged = [...productImageDisplayList.querySelectorAll(notDraggedListItemsQuery)];

    const draggedElement = productImageDisplayList.querySelector(`[${draggedImageListItemAttributeName}='true']`);

    const closestElement = getClosestElement(allImageListItemElementsThatArentDragged, event.clientX, event.clientY);

    if (closestElement == null)
    {
        productImageDisplayList.appendChild(draggedElement);

        return;
    }
    
    const closestElementRect = closestElement.getBoundingClientRect();

    const edgeToCenterPointDistanceX = closestElementRect.width / 2;

    const closestElementCenterPoint = getElementCenterPointOnScreen(closestElement);

    const differenceX = event.clientX - closestElementCenterPoint.x;

    const distanceX = Math.abs(differenceX);

    if (distanceX < edgeToCenterPointDistanceX)
    {
        if (differenceX > 0)
        {
            productImageDisplayList.insertBefore(draggedElement, closestElement);

            return;
        }

        if (!closestElement.nextElementSibling)
        {
            productImageDisplayList.appendChild(draggedElement);

            return;
        }

        productImageDisplayList.insertBefore(draggedElement, closestElement.nextSibling);
    }
}

async function onDragEndImageListItem(
    event,
    productId,
    productPropertiesEditorContainerElementId,
    relatedProductDataElementId = null)
{
    event.preventDefault();

    const imageElementsListElement = event.target;

    const draggedElement = document.querySelector(`[${draggedImageListItemAttributeName}='true']`);

    draggedElement.removeAttribute(draggedImageListItemAttributeName);

    const productIdAsInt = getIntOrNullFromString(productId);

    const imageIdAsString = draggedElement.getAttribute(imageListItemImageIdAttributeName);

    const imageId = getIntOrNullFromString(imageIdAsString);

    if (!imageElementsListElement || !draggedElement) return;

    //return;

    const allImageItemsInList = Array.from(productImageDisplayList.children);

    const newIndexOfDraggedItem = allImageItemsInList.indexOf(draggedElement);

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const response = await updateImageDisplayOrder(productIdAsInt, imageId, newIndexOfDraggedItem + 1, productDataElementIndex);

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

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

function getClosestElement(elements, mouseX, mouseY)
{
    var closestItem = null;

    var smallestDistance = null;

    for (const element of elements)
    {
        const elementCenterPoint = getElementCenterPointOnScreen(element);

        const distanceX = Math.abs(mouseX - elementCenterPoint.x);
        const distanceY = Math.abs(mouseY - elementCenterPoint.y);

        const totalDistance = distanceX + distanceY;

        if (smallestDistance == null
            || totalDistance < smallestDistance)
        {
            smallestDistance = totalDistance;

            closestItem = element;
        }
    }

    return closestItem;
}

function getElementCenterPointOnScreen(element)
{
    const clientRect = element.getBoundingClientRect();

    const itemMiddleX = clientRect.left + clientRect.width / 2;
    const itemMiddleY = clientRect.bottom + clientRect.height / 2;

    return {
        x: itemMiddleX,
        y: itemMiddleY
    }
}

async function copyFileFromClipboard(fileInputElementId)
{
    await changeFileInputDataFromClipboard(fileInputElementId);

    const changeEvent = new Event('change');

    productPropertyEditorAddImageFileInput.dispatchEvent(changeEvent);
}

async function changeFileInputDataFromClipboard(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    if (!fileInputElement) return;

    const clipboardItems = await navigator.clipboard.read();

    const file = await getFirstImageFileFromClipboard(clipboardItems);

    if (!file) return;

    const dataTransfer = new DataTransfer()

    dataTransfer.items.add(file);

    fileInputElement.files = dataTransfer.files;
}

async function getFirstImageFileFromClipboard(clipboardItems)
{
    var file = null;

    for (const clipboardItem of clipboardItems)
    {
        for (const type of clipboardItem.types)
        {
            if (!type.startsWith('image/')) continue;

            const newImageExtension = type.split("image/")[1];

            const blob = await clipboardItem.getType(type);

            const fileFromBlob = new File([blob], "unusedFileNameOfImageFile." + newImageExtension);

            file = fileFromBlob;

            break;
        }

        if (file) break;
    }

    return file;
}

async function updateImageDisplayOrder(
    productId,
    imageId,
    newDisplayOrder,
    productDataElementIndex = null)
{
    const productIdAsInt = getIntOrNullFromString(productId);
    const imageIdAsInt = getIntOrNullFromString(imageId);
    const newDisplayOrderAsInt = getIntOrNullFromString(newDisplayOrder);
    const productDataElementIndexAsInt = getIntOrNullFromString(productDataElementIndex);

    var url = "/ProductEditor" + "?handler=UpdateImageDisplayOrder"
        + "&productId=" + productIdAsInt
        + "&imageId=" + imageIdAsInt
        + "&newDisplayOrder=" + newDisplayOrderAsInt
        + "&productDataElementIndexAsInt=" + productDataElementIndexAsInt;

    return await fetch(url,
    {
        method: 'PUT',
        headers:
        {
            'Content-Type' : "application/json",
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

async function upsertImageAndDisplayChanges(
    shouldAddImageFile,
    includeHtmlDataView,
    productId,
    existingImageId,
    fileInputElementId,
    existingFileInfoId,
    displayOrder,
    active,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    const file = fileInputElement.files[0];

    if (!file) return;

    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const response = await upsertImage(shouldAddImageFile, includeHtmlDataView,
        productId, existingImageId, file, existingFileInfoId, displayOrder, active, productDataElementIndex);

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

async function upsertImage(
    shouldAddImageFile,
    includeHtmlDataView,
    productId,
    existingImageId,
    file,
    existingFileInfoId,
    displayOrder,
    active,
    productDataElementIndex = null)
{
    var url = "/ProductEditor" + "?handler=UpsertImage";

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    var fileUpsertData = null;

    if (shouldAddImageFile)
    {
        const existingFileInfoIdAsInt = getIntOrNullFromString(existingFileInfoId);
        const displayOrderAsInt = getIntOrNullFromString(displayOrder);

        if (!(typeof active === "boolean"))
        {
            active = false;
        }

        fileUpsertData =
        {
            ExistingFileInfoId: existingFileInfoIdAsInt,
            DisplayOrder: displayOrderAsInt,
            Active: active
        }
    }

    const formData = getProductImageUpsertData(includeHtmlDataView, productId, existingImageId, file, fileUpsertData);

    return await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: formData
    });
}

async function deleteImageAndDisplayChanges(
    existingImageId,
    deleteFile,
    productPropertiesEditorContainerElementId = null,
    relatedProductDataElementId = null)
{
    const productDataElementIndex = getElementIndexOfProductRowElement(relatedProductDataElementId);

    const response = await deleteImage(existingImageId, deleteFile, productDataElementIndex)

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

async function deleteImage(existingImageId, deleteFile, productDataElementIndex = null)
{
    const existingImageIdAsInt = getIntOrNullFromString(existingImageId);

    var url = "/ProductEditor" + "?handler=DeleteImage"
        + "&imageId=" + existingImageIdAsInt
        + "&deleteFile=" + deleteFile;

    if (productDataElementIndex != null)
    {
        url += "&productDataElementIndex=" + productDataElementIndex;
    }

    return await fetch(url,
    {
        method: 'DELETE',
        headers:
        {
            'Content-Type': "application/json",
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });
}

function getProductImageUpsertData(includeHtmlDataView, productId, existingImageId, file, fileUpsertData = null)
{
    const productIdAsInt = getIntOrNullFromString(productId);
    const existingImageIdAsInt = getIntOrNullFromString(existingImageId);

    if (!(typeof active === "boolean"))
    {
        includeHtmlDataView = false;
    }

    const imageUpsertData =
    {
        IncludeHtmlDataView: includeHtmlDataView,
        ProductId: productIdAsInt,
        ExistingImageId: existingImageIdAsInt,
        File: file,
        FileUpsertData: fileUpsertData
    };

    return getFormDataFromObject(imageUpsertData, new FormData(), "imageUpsertData")
}