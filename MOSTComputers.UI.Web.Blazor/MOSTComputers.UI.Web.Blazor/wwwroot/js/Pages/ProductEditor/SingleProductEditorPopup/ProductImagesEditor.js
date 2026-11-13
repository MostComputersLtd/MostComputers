import * as validationMessages from "../../../Validation/ValidationMessages.js"
import * as validationVisualElements from "../../../Validation/ValidationVisualElements.js"
import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as promotionFilePopup from "./PromotionFilePopup.js"
import * as imageUtils from "../../../Common/ImageUtils.js"
import * as urlUtils from "../../../Common/UrlUtils.js"

const listId = "singleEditorProductImageList";

const itemName = "productImageItem";

const imageButtonName = "productImageButton";
const deleteButtonName = "productImageDeleteButton";
const imageFileDisplayOrderTextName = "imageFileDisplayOrderText";
const imageFileActiveCheckboxName = "imageFileActiveCheckbox";
const itemJsonDataName = "productImageItemData";

const validationMessagesListName = "productImageValidationMessagesList";

const itemTypeAttribute = "data-image-type";

const itemRelatedPromotionFileEditorIdAttribute = "data-promotion-file-editor-id";
const itemImageIdAttribute = "data-image-id";
const itemImageFileIdAttribute = "data-image-file-id";

const imageButtonImageUrlAttribute = "data-image-url";

const fileFormFieldName = "file";
const filePreviewUrlFormFieldName = "previewUrl";
const fileIndexFormFieldName = "index";
const productImageFileWidthFormFieldName = "width";
const productImageFileHeightFormFieldName = "height";

const endIndexCharacter = ']';
const imageRequestsStartString = 'ImageRequests[';

const dragAndDropType = "application/x-mostcomputers-product-editor-single-image-reorder";

const uploadedFiles = new Map();

export function initEvents(dialogElement) {

    const productImageList = document.getElementById(listId);

    productImageList.addEventListener("dragenter", onProductImageListDragEnter);
    productImageList.addEventListener("dragover", onProductImageListDragOver);
    productImageList.addEventListener("drop", onProductImageListDrop);

    const productImageItems = dialogElement.querySelectorAll(`[name='${itemName}']`);

    for (const productImageItem of productImageItems) {

        initProductImageItemEvents(productImageItem);
    }
}

function initProductImageItemEvents(productImageItem) {

    productImageItem.addEventListener("dragstart", onProductImageItemDragStart);
    productImageItem.addEventListener("dragend", onProductImageItemDragEnd);

    const productImageButton = productImageItem.querySelector(`[name='${imageButtonName}']`);

    productImageButton.addEventListener("click", onProductImageButtonClicked);

    const imageFileActiveCheckbox = productImageItem.querySelector(`[name='${imageFileActiveCheckboxName}']`);

    if (imageFileActiveCheckbox) {

        imageFileActiveCheckbox.addEventListener("change", onProductImageFileActiveCheckboxChecked);
    }

    const productImageDeleteButton = productImageItem.querySelector(`[name='${deleteButtonName}']`);

    productImageDeleteButton.addEventListener("click", onProductImageDeleteButtonClicked);
}

export function getProductImageItems() {

    const productImageList = document.getElementById(listId);

    return [...productImageList.querySelectorAll(`[name='${itemName}']`)];
}

export function getProductImageItemImageUrl(item) {

    const imageButton = item.querySelector(`[name='${imageButtonName}']`);

    return imageButton.getAttribute(imageButtonImageUrlAttribute);
}

export function getProductImagesRequestData() {

    const productImageList = document.getElementById(listId);

    const items = [...productImageList.querySelectorAll(`[name='${itemName}']`)];

    const itemDatas = [];

    for (const item of items) {

        const data = getProductImageItemRequestData(item);

        itemDatas.push(data);
    }

    return itemDatas;
}

export function getProductImageItemRequestData(item) {

    const itemType = item.getAttribute(itemTypeAttribute);

    if (itemType == "promotionProductFile") {

        const promotionFileEditorId = item.getAttribute(itemRelatedPromotionFileEditorIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        return {
            $type: itemType,
            promotionFileEditorId: promotionFileEditorId,
            active: fileActiveCheckbox.checked
        };
    }
    else if (itemType == "image") {

        const imageId = item.getAttribute(itemImageIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        let saveFileRequest = null;

        if (fileActiveCheckbox) {

            saveFileRequest = {
                active: fileActiveCheckbox.checked
            };
        }

        return {
            $type: itemType,
            id: parseInt(imageId),
            saveFileRequest: saveFileRequest
        };
    }
    else if (itemType == "imageFile") {

        const imageFileId = item.getAttribute(itemImageFileIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        return {
            $type: itemType,
            id: parseInt(imageFileId),
            active: fileActiveCheckbox.checked,
        };
    }
    else {

        const file = uploadedFiles.get(item.id);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        return {
            $type: itemType,
            editorId: item.id,
            uploadedFile: file,
            active: fileActiveCheckbox.checked
        };
    }
}

export async function addFileToImagesAsync(blob) {

    if (blob.size === 0) {

        return false;
    }

    if (!blob.type.startsWith('image/')) {

        return false;
    }

    const previewUrl = URL.createObjectURL(blob);

    const productImageItems = getProductImageItems();

    const imageDimensions = await imageUtils.getImageFileDimensions(blob);

    const formData = new FormData();

    formData.append(fileFormFieldName, blob);
    formData.append(filePreviewUrlFormFieldName, previewUrl);
    formData.append(fileIndexFormFieldName, productImageItems.length);
    formData.append(productImageFileWidthFormFieldName, imageDimensions.width);
    formData.append(productImageFileHeightFormFieldName, imageDimensions.height);

    const url = "api/components/productEditor/single/productImage/add";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Accept": "text/html"
        },
        body: formData
    });

    if (!response.ok) {
        return false;
    }

    const data = await response.text();

    const jsonData = addNewImageFromElementHtml(data);

    const success = dataStore.addProductImageAndSave(jsonData.editorId, jsonData);

    uploadedFiles.set(jsonData.editorId, blob);

    editorChanges.setImageChanged(jsonData.editorId, editorChanges.ItemChangeState.Added);

    return success;
}

export async function addNewPromotionProductFileToImagesAsync(promotionProductFileEditorId, promotionFileId) {

    const url = "api/components/productEditor/single/productImage/promotionFile";

    const productImageItems = getProductImageItems();

    const newImageItemRequest = {
        existingEditorId: null,
        relatedPromotionFileEditorId: promotionProductFileEditorId,
        index: productImageItems.length,
        promotionFileId: promotionFileId,
        active: true,
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(newImageItemRequest)
    });

    if (!response.ok) {
        return false;
    }

    const data = await response.text();

    const jsonData = addNewImageFromElementHtml(data);

    const success = dataStore.addProductImageAndSave(jsonData.editorId, jsonData);

    editorChanges.setImageChanged(jsonData.editorId, editorChanges.ItemChangeState.Added);

    return success;
}

function addNewImageFromElementHtml(productImageHtml) {

    const productImageList = document.getElementById(listId);

    const productImageItems = getProductImageItems();

    let productImageItem;

    if (productImageItems.length === 0) {

        productImageList.insertAdjacentHTML("afterbegin", productImageHtml);

        productImageItem = productImageList.firstElementChild;
    }
    else {

        const lastItem = productImageItems[productImageItems.length - 1];

        lastItem.insertAdjacentHTML("afterend", productImageHtml);

        productImageItem = lastItem.nextElementSibling;
    }

    const jsonDataScriptElement = productImageItem.querySelector(`[name='${itemJsonDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductImageItemEvents(productImageItem);

    return jsonData;
}

export async function changePromotionProductFileImageItemAsync(promotionProductFileEditorId, promotionFileId, active) {

    const url = "api/components/productEditor/single/productImage/promotionFile";

    const productImageItems = getProductImageItems();

    const existingRelatedImageItem = getProductImageItemRelatedToPromotionFile(promotionProductFileEditorId);

    if (existingRelatedImageItem == null) {

        await addNewPromotionProductFileToImagesAsync(promotionProductFileEditorId, promotionFileId);

        return;
    }

    const imageItemIndex = productImageItems.indexOf(existingRelatedImageItem);

    const editorId = existingRelatedImageItem.id;

    const newImageItemRequest = {
        existingEditorId: editorId,
        relatedPromotionFileEditorId: promotionProductFileEditorId,
        index: imageItemIndex,
        promotionFileId: promotionFileId,
        active: active,
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(newImageItemRequest)
    });

    if (!response.ok) {
        return false;
    }

    const data = await response.text();

    existingRelatedImageItem.outerHTML = data;

    const newRelatedImageItem = document.getElementById(editorId);

    const jsonDataScriptElement = newRelatedImageItem.querySelector(`[name='${itemJsonDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductImageItemEvents(newRelatedImageItem);

    const success = dataStore.updateProductImageAndSave(editorId, image =>
    {
        image.imageFileData.active = jsonData.imageFileData.active;
    });

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Updated);

    return success;
}

export function getProductImageItemRelatedToPromotionFile(promotionFileEditorId) {

    const productImageList = document.getElementById(listId);

    const items = [...productImageList.querySelectorAll(`[name='${itemName}']`)];

    for (let i = 0; i < items.length; i++) {

        const item = items[i];

        const itemType = item.getAttribute(itemTypeAttribute);

        if (itemType !== "promotionProductFile") continue;

        const promotionFileEditorIdAttribute = item.getAttribute(itemRelatedPromotionFileEditorIdAttribute);

        if (promotionFileEditorIdAttribute === promotionFileEditorId) return item;
    }

    return null;
}

export function displayValidationMessages(validationErrors) {

    const productImageList = document.getElementById(listId);

    const items = [...productImageList.querySelectorAll(`[name='${itemName}']`)];

    for (let i = 0; i < items.length; i++) {

        const item = items[i];

        const validationMessagesList = item.querySelector(`[name='${validationMessagesListName}']`);

        validationVisualElements.clearValidationMessages(validationMessagesList);
    }

    const imageValidationMessages = new Map();

    for (const [name, _] of Object.entries(validationErrors)) {

        if (!name.startsWith(imageRequestsStartString)) continue;

        const startIndex = imageRequestsStartString.length;
        const endIndex = name.indexOf(endIndexCharacter, startIndex);

        const indexAsString = name.substring(startIndex, endIndex);
        const index = parseInt(indexAsString);

        if (!imageValidationMessages.has(index)) {

            imageValidationMessages.set(index, []);
        }

        const messagesForProperty = imageValidationMessages.get(index);

        messagesForProperty.push(validationErrors[name]); 
    }

    for (const [index, validationErrorsForProperty] of imageValidationMessages) {

        const item = items[index];

        const validationMessagesList = item.querySelector(`[name='${validationMessagesListName}']`);

        validationVisualElements.displayValidationMessages(
            validationErrorsForProperty,
            validationMessagesList,
            validationMessages.getDefaultValidationMessage)
    }
}

let draggedImageItem = null;
let draggedItemStartIndex = null;
let lastDragPositionChangeTime = null;

const dragPositionChangeInterval = 50;

function onProductImageItemDragStart(e) {

    const productImageItem = e.currentTarget;

    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData(dragAndDropType, "true");

    draggedImageItem = productImageItem;
    draggedItemStartIndex = [...productImageItem.parentElement.children].indexOf(productImageItem);
}

function onProductImageItemDragEnd(e) {

    const dragWasCancelled = e.dataTransfer.dropEffect === "none";

    if (!dragWasCancelled) return;

    const productImageList = draggedImageItem.parentElement;

    const productImageItems = [...productImageList.children];

    const elementAtDragStartIndex = productImageItems[draggedItemStartIndex];

    const draggedItemIndex = productImageItems.indexOf(draggedImageItem);

    if (draggedItemStartIndex < draggedItemIndex) {

        productImageList.insertBefore(draggedImageItem, elementAtDragStartIndex);
    }
    else if (draggedItemStartIndex > draggedItemIndex) {

        productImageList.insertBefore(draggedImageItem, elementAtDragStartIndex.nextSibling);
    }

    draggedImageItem = null;
    lastDragPositionChangeTime = null;
    draggedItemStartIndex = null;
}

function onProductImageListDragEnter(e) {

    const isImageDragAndDrop = hasImageDragAndDropTypeItem(e);

    if (isImageDragAndDrop) {
        e.preventDefault();
    }
}

function onProductImageListDragOver(e) {

    const isImageDragAndDrop = hasImageDragAndDropTypeItem(e);

    if (!isImageDragAndDrop) {

        e.dataTransfer.dropEffect = 'none';
        return;
    }

    e.dataTransfer.dropEffect = 'move';
    e.preventDefault();

    const draggedOverItem = e.target.closest(`[name='${itemName}']`);

    if (!draggedOverItem) return;

    if (draggedImageItem === null ||
        draggedImageItem === draggedOverItem ||
        (lastDragPositionChangeTime !== null &&
            Date.now() - lastDragPositionChangeTime < dragPositionChangeInterval)) {
        return;
    }

    const productImageList = e.currentTarget;

    const productImageItems = getProductImageItems();

    const draggedItemIndex = productImageItems.indexOf(draggedImageItem);
    const draggedOverItemIndex = productImageItems.indexOf(draggedOverItem);

    if (draggedItemIndex === draggedOverItemIndex) {
        return;
    }

    if (draggedOverItemIndex > draggedItemIndex) {
        productImageList.insertBefore(draggedImageItem, draggedOverItem.nextSibling);
    }
    else {
        productImageList.insertBefore(draggedImageItem, draggedOverItem);
    }

    lastDragPositionChangeTime = Date.now();
}

function onProductImageListDrop(e) {

    const productImageList = draggedImageItem.parentElement;
    const draggedItemIndex = [...productImageList.children].indexOf(draggedImageItem);

    const smallerDragIndex = Math.min(draggedItemIndex, draggedItemStartIndex);
    const largerDragIndex = Math.max(draggedItemIndex, draggedItemStartIndex);

    dataStore.moveProductImageAndSave(draggedImageItem.id, draggedItemIndex);

    for (let i = smallerDragIndex; i <= largerDragIndex; i++) {

        const productImageItem = productImageList.children[i];

        editorChanges.setImageChanged(productImageItem.id, editorChanges.ItemChangeState.Updated);

        const imageFileDisplayOrderText = productImageItem.querySelector(`[name='${imageFileDisplayOrderTextName}']`);

        if (!imageFileDisplayOrderText) continue;

        if (productImageItem == draggedImageItem) {

            imageFileDisplayOrderText.textContent = `${draggedItemIndex + 1}: `;
        }
        else {

            const newDisplayOrder = i + 1;

            imageFileDisplayOrderText.textContent = `${newDisplayOrder}: `;
        }
    }

    e.preventDefault();

    draggedImageItem = null;
    lastDragPositionChangeTime = null;
    draggedItemStartIndex = null;
}

function hasImageDragAndDropTypeItem(e) {

    const dataTransferItems = e.dataTransfer.items;

    for (let i = 0; i < dataTransferItems.length; i++) {

        const dataTransferItem = dataTransferItems[i];

        if (dataTransferItem.kind === "string"
            && dataTransferItem.type === dragAndDropType) {
            return true;
        }
    }

    return false;
}

function onProductImageButtonClicked(e) {

    const imageUrl = e.currentTarget.getAttribute(imageButtonImageUrlAttribute);

    if (!imageUrl) return;

    urlUtils.openDataUrlInNewWindow(imageUrl);
}

function onProductImageFileActiveCheckboxChecked(e) {

    const fileActiveCheckbox = e.currentTarget;

    const productImageItem = fileActiveCheckbox.closest(`[name='${itemName}']`);

    const editorId = productImageItem.id; 

    dataStore.updateProductImageAndSave(editorId, image =>
    {
        switch (image["$type"]) {
            case "promotionProductFile":
                image.imageFileData.active = fileActiveCheckbox.checked;

                break;

            case "image":
                image.relatedImageFile.active = fileActiveCheckbox.checked;

                break;

            case "imageFile":
                image.active = fileActiveCheckbox.checked;

                break;

            case "uploadedImage":
                image.imageFileData.active = fileActiveCheckbox.checked;

                break;

            default:
                break;
        }
    });

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Updated);
}

export function clearData() {

    const productImageItems = getProductImageItems();

    for (const productImageItem of productImageItems) {

        const itemType = productImageItem.getAttribute(itemTypeAttribute);

        if (itemType !== "uploadedImage") continue;

        const imageUrl = getProductImageItemImageUrl(productImageItem);

        URL.revokeObjectURL(imageUrl);
    }

    uploadedFiles.clear();
}

function onProductImageDeleteButtonClicked(e) {

    const productImageItem = e.currentTarget.closest(`[name='${itemName}']`);

    const itemType = productImageItem.getAttribute(itemTypeAttribute);

    if (itemType === "promotionProductFile") {

        const promotionFileEditorId = productImageItem.getAttribute(itemRelatedPromotionFileEditorIdAttribute);

        promotionFilePopup.onEditorItemRelatedImageDeleted(promotionFileEditorId);
    }

    removeProductImage(productImageItem);
}

export function removeProductImage(productImageItem) {

    const editorId = productImageItem.id; 

    const itemType = productImageItem.getAttribute(itemTypeAttribute);

    if (itemType === "uploadedImage") {

        const imageUrl = getProductImageItemImageUrl(productImageItem);

        uploadedFiles.delete(editorId);

        URL.revokeObjectURL(imageUrl);
    } 

    const productImageItems = getProductImageItems();

    const removedItemIndex = productImageItems.indexOf(productImageItem);

    productImageItems.splice(removedItemIndex, 1);
    productImageItem.remove();

    for (let i = removedItemIndex; i < productImageItems.length; i++) {

        const item = productImageItems[i];

        const imageFileDisplayOrderText = item.querySelector(`[name='${imageFileDisplayOrderTextName}']`);

        if (!imageFileDisplayOrderText) continue;

        const newDisplayOrder = i + 1;

        imageFileDisplayOrderText.textContent = `${newDisplayOrder}: `;
    }

    dataStore.removeImageAndSave(editorId);

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Deleted);
}
