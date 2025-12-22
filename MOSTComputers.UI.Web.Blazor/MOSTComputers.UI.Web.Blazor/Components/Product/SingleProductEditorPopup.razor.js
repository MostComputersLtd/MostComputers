export function forceFileInputClick(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    fileInputElement.click();
}

export function openDataUrlInNewWindow(url)
{
    window.open(url, '_blank');
}

export async function changeFileInputDataToImageFromClipboardAndForceChangeEvent(fileInputElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    if (!fileInputElement) return;

    const success = await changeFileInputDataToImageFromClipboard(fileInputElement);

    if (!success) return;

    const changeEvent = new Event('change');

    fileInputElement.dispatchEvent(changeEvent);
}

async function changeFileInputDataToImageFromClipboard(fileInputElement)
{
    const clipboardItems = await navigator.clipboard.read();

    const file = await getFirstImageFileFromClipboard(clipboardItems);

    if (file == null) return false;

    changeFileInputData(fileInputElement, file);

    return true;
}

function changeFileInputData(fileInputElement, file)
{
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

            const fileFromBlob = new File([blob], "unusedFileNameOfImageFile." + newImageExtension, { type: blob.type, lastModified: Date.now() });

            file = fileFromBlob;

            break;
        }

        if (file != null) break;
    }

    return file;
}

export async function createUrlFromStreamData(dotNetStreamRef, contentType)
{
    const arrayBuffer = await dotNetStreamRef.arrayBuffer();

    const blob = new Blob([arrayBuffer], { type: contentType });

    const url = URL.createObjectURL(blob);

    return url;
}

export async function displayImageFromFileInput(fileInputElementId, imageElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    const imageElement = document.getElementById(imageElementId);

    if (!fileInputElement || fileInputElement.files.length == 0 || !imageElement) return null;

    const url = URL.createObjectURL(fileInputElement.files[0]);

    imageElement.addEventListener('load', () =>
    {
        URL.revokeObjectURL(url);
    }, { once: true });

    imageElement.src = url;

    return url;
}

export function revokePreviewUrls(urls)
{
    if (!Array.isArray(urls)) return;

    for (const url of urls)
    {
        revokePreviewUrl(url);
    }
}

export function revokePreviewUrl(url)
{
    URL.revokeObjectURL(url);
}

export function resizeTextareasToColumnText(textAreasCommonName, minRows = null)
{
    const textAreas = [...document.getElementsByName(textAreasCommonName)];

    for (const textArea of textAreas)
    {
        const lineHeight = parseInt(window.getComputedStyle(textArea).lineHeight, 10);

        if (minRows != null)
        {
            const minHeight = lineHeight * minRows;

            textArea.style.height = 'auto';
            textArea.style.height = Math.max(textArea.scrollHeight + 1, minHeight) + 'px';

            continue;
        }

        textArea.style.height = 'auto';
        textArea.style.height = (textArea.scrollHeight + 1) + 'px';
    }
}

const imageListItemInteractableWithDragAndDropAttributeName = "data-drag-and-drop-interactable-image-list-item";

const draggedImageListItemAttributeName = "data-dragged-image-list-item";

var currentIndex = null;

export function onDragStartImageListItem(imageListItemElementId)
{
    const imageListItemElement = document.getElementById(imageListItemElementId);

    imageListItemElement.setAttribute(draggedImageListItemAttributeName, "true");
}

export async function onDragOverImageList(clientX, clientY, imageListElementId, dotNetObjectRef)
{
    const imageListElement = document.getElementById(imageListElementId);

    const draggedElement = imageListElement.querySelector(`[${draggedImageListItemAttributeName}]`);

    if (draggedElement == null) return;

    const notDraggedListItemsQuery = `[${imageListItemInteractableWithDragAndDropAttributeName}='true']:not([${draggedImageListItemAttributeName}='true'])`;

    const allImageListItemElementsThatArentDragged = [...imageListElement.querySelectorAll(notDraggedListItemsQuery)];

    const closestElement = getClosestElement(allImageListItemElementsThatArentDragged, clientX, clientY);

    if (closestElement == null) return;
    
    const closestElementRect = closestElement.getBoundingClientRect();

    const edgeToCenterPointDistanceX = closestElementRect.width / 2;

    const closestElementCenterPoint = getElementCenterPointOnScreen(closestElement);

    const differenceX = clientX - closestElementCenterPoint.x;

    const distanceX = Math.abs(differenceX);

    if (distanceX < edgeToCenterPointDistanceX)
    {
        const draggedElementIndex = getListItemIndex(imageListElement, draggedElement);
        const closestElementIndex = getListItemIndex(imageListElement, closestElement);

        if (differenceX > 0)
        {
            //imageListElement.insertBefore(draggedElement, closestElement);

            if (currentIndex == closestElementIndex) return;
            
            currentIndex = closestElementIndex;

            await dotNetObjectRef.invokeMethodAsync("OnImageItemDragPositionChange", draggedElementIndex, currentIndex);

            return;
        }

         if (currentIndex == closestElementIndex + 1) return;
        
        currentIndex = closestElementIndex + 1;

        await dotNetObjectRef.invokeMethodAsync("OnImageItemDragPositionChange", draggedElementIndex, currentIndex);

        //if (!closestElement.nextElementSibling)
        //{
            //imageListElement.appendChild(draggedElement);


        //    return;
        //}

        //imageListElement.insertBefore(draggedElement, closestElement.nextSibling);

        return;
    }

    currentIndex = null;
}

export function onDragEndImageListItem(imageListElementId)
{
    currentIndex = null;

    const imageListElement = document.getElementById(imageListElementId);

    const draggedElement = document.querySelector(`[${draggedImageListItemAttributeName}='true']`);

    draggedElement.removeAttribute(draggedImageListItemAttributeName);

    return getListItemIndex(imageListElement, draggedElement);
}

function getListItemIndex(imageListElement, imageListItemElement)
{
    const allImageItemsInList = Array.from(imageListElement.children);

    return allImageItemsInList.indexOf(imageListItemElement);
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