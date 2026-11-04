import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as promotionFilePopup from "./PromotionFilePopup.js"
import * as imageUtils from "../../../Common/ImageUtils.js"
import * as urlUtils from "../../../Common/UrlUtils.js"

const productImageListId = "singleEditorProductImageList";

const productImageItemName = "productImageItem";

const productImageButtonName = "productImageButton";
const productImageDeleteButtonName = "productImageDeleteButton";
const imageFileDisplayOrderTextName = "imageFileDisplayOrderText";
const imageFileActiveCheckboxName = "imageFileActiveCheckbox";
const productImageItemDataName = "productImageItemData";

const productImageItemTypeAttribute = "data-image-type";

const productImageItemRelatedPromotionFileEditorIdAttribute = "data-promotion-file-editor-id";
const productImageItemImageIdAttribute = "data-image-id";
const productImageItemImageFileIdAttribute = "data-image-file-id";

const productImageButtonImageRouteAttribute = "data-image-url";

const fileFormFieldName = "file";
const filePreviewUrlFormFieldName = "previewUrl";
const fileIndexFormFieldName = "index";
const productImageFileWidthFormFieldName = "width";
const productImageFileHeightFormFieldName = "height";

const uploadedFiles = new Map();

export function initEvents(dialogElement) {

    const productImageItems = dialogElement.querySelectorAll(`[name='${productImageItemName}']`);

    for (const productImageItem of productImageItems) {

        initProductImageItemEvents(productImageItem);
    }
}

function initProductImageItemEvents(productImageItem) {

    productImageItem.addEventListener("dragstart", onProductImageItemDragStart);
    productImageItem.addEventListener("dragover", onProductImageItemDragOver);
    productImageItem.addEventListener("drop", onProductImageItemDrop);

    const productImageButton = productImageItem.querySelector(`[name='${productImageButtonName}']`);

    productImageButton.addEventListener("click", onProductImageButtonClicked);

    const imageFileActiveCheckbox = productImageItem.querySelector(`[name='${imageFileActiveCheckboxName}']`);

    if (imageFileActiveCheckbox) {

        imageFileActiveCheckbox.addEventListener("change", onProductImageFileActiveCheckboxChecked);
    }

    const productImageDeleteButton = productImageItem.querySelector(`[name='${productImageDeleteButtonName}']`);

    productImageDeleteButton.addEventListener("click", onProductImageDeleteButtonClicked);
}

export function getProductImageItems() {

    const productImageList = document.getElementById(productImageListId);

    return [...productImageList.querySelectorAll(`[name='${productImageItemName}']`)];
}

export function getProductImageItemImageUrl(item) {

    const imageButton = item.querySelector(`[name='${productImageButtonName}']`);

    return imageButton.getAttribute(productImageButtonImageRouteAttribute);
}

export function getProductImagesRequestData() {

    const productImageList = document.getElementById(productImageListId);

    const items = [...productImageList.querySelectorAll(`[name='${productImageItemName}']`)];

    const itemDatas = [];

    for (const item of items) {

        const data = getProductImageItemRequestData(item);

        itemDatas.push(data);
    }

    return itemDatas;
}

export function getProductImageItemRequestData(item) {

    const itemType = item.getAttribute(productImageItemTypeAttribute);

    if (itemType == "promotionProductFile") {

        const promotionFileEditorId = item.getAttribute(productImageItemRelatedPromotionFileEditorIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        return {
            $type: itemType,
            promotionFileEditorId: promotionFileEditorId,
            active: fileActiveCheckbox.checked
        };
    }
    else if (itemType == "image") {

        const imageId = item.getAttribute(productImageItemImageIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        const saveFileRequest = null;

        if (fileActiveCheckbox) {

            saveFileRequest = {
                active: fileActiveCheckbox.checked
            };
        }

        return {
            $type: itemType,
            id: imageId,
            saveFileRequest: saveFileRequest
        };
    }
    else if (itemType == "imageFile") {

        const imageFileId = item.getAttribute(productImageItemImageFileIdAttribute);

        const fileActiveCheckbox = item.querySelector(`[name='${imageFileActiveCheckboxName}']`);

        return {
            $type: itemType,
            id: imageFileId,
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

    const url = "api/components/productEditor/single/image/add";

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

    const url = "api/components/productEditor/single/image/add/promotionFile";

    const productImageItems = getProductImageItems();

    const newImageItemRequest = {
        editorId: promotionProductFileEditorId,
        index: productImageItems.length,
        promotionFileId: promotionFileId,
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

    const productImageList = document.getElementById(productImageListId);

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

    const jsonDataScriptElement = productImageItem.querySelector(`[name='${productImageItemDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductImageItemEvents(productImageItem);

    return jsonData;
}

export async function changePromotionProductFileImageItemAsync(promotionProductFileEditorId, promotionFileId) {

    const url = "api/components/productEditor/single/image/add/promotionFile";

    const productImageItems = getProductImageItems();

    const existingRelatedImageItem = productImageItems.find(x => {

        const imageTypeAttribute = x.getAttribute(productImageItemTypeAttribute);

        if (imageTypeAttribute != "promotionProductFile") {

            return false;
        }

        const promotionProductFileEditorIdAttribute = x.getAttribute(productImageItemRelatedPromotionFileEditorIdAttribute);

        const promotionProductFileEditorId = parseInt(promotionProductFileEditorIdAttribute);

        return promotionProductFileEditorId == promotionProductFileEditorId;
    });

    if (existingRelatedImageItem == null) {

        await addNewPromotionProductFileToImagesAsync(promotionProductFileEditorId, promotionFileId);

        return;
    }

    const newImageItemRequest = {
        editorId: promotionProductFileEditorId,
        index: productImageItems.length,
        promotionFileId: promotionFileId,
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

    const jsonDataScriptElement = existingRelatedImageItem.querySelector(`[name='${productImageItemDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductImageItemEvents(existingRelatedImageItem);

    const imageFileActiveCheckbox = existingRelatedImageItem.querySelector(`[name='${imageFileActiveCheckboxName}']`);

    const success = dataStore.updateProductImageAndSave(jsonData.editorId, image =>
    {
        image.imageFileData.active = imageFileActiveCheckbox.checked;
    });

    editorChanges.setImageChanged(jsonData.editorId, editorChanges.ItemChangeState.Updated);

    return success;
}

let draggedImageItem = null;
let draggedItemStartIndex = null;
let lastDragPositionChangeTime = null;

const dragPositionChangeInterval = 50;

function onProductImageItemDragStart(e) {

    const productImageItem = e.currentTarget;

    draggedImageItem = productImageItem;
    draggedItemStartIndex = [...productImageItem.parentElement.children].indexOf(item);
}

function onProductImageItemDragOver(e) {

    const draggedOverItem = e.currentTarget;

    e.preventDefault();

    if (draggedImageItem === null ||
        draggedImageItem === draggedOverItem ||
        (lastDragPositionChangeTime !== null &&
            Date.now() - lastDragPositionChangeTime < dragPositionChangeInterval)) {
        return;
    }

    const container = draggedImageItem.parentElement;

    const productImageItems = getProductImageItems();

    const draggedItemIndex = productImageItems.indexOf(draggedImageItem);
    const draggedOverItemIndex = productImageItems.indexOf(draggedOverItem);

    if (draggedItemIndex === draggedOverItemIndex) {
        return;
    }

    container.removeChild(draggedImageItem);

    if (draggedOverItemIndex > draggedItemIndex) {
        container.insertBefore(draggedImageItem, draggedOverItem.nextSibling);
    }
    else {
        container.insertBefore(draggedImageItem, draggedOverItem);
    }

    lastDragPositionChangeTime = Date.now();
}

function onProductImageItemDrop() {

    const container = draggedImageItem.parentElement;
    const draggedItemIndex = [...container.children].indexOf(draggedImageItem);

    const smallerDragIndex = Math.min(draggedItemIndex, draggedItemStartIndex);
    const largerDragIndex = Math.max(draggedItemIndex, draggedItemStartIndex);

    let movedItemsIndexChange = 1;

    if (draggedItemStartIndex < draggedItemIndex) {
        movedItemsIndexChange = -1;
    }

    dataStore.moveProductImageAndSave(draggedImageItem.id, draggedItemIndex);

    for (let i = smallerDragIndex; i <= largerDragIndex; i++) {

        const productImageItem = container.children[i];

        editorChanges.setImageChanged(productImageItem.id, editorChanges.ItemChangeState.Updated);

        const imageFileDisplayOrderText = productImageItem.querySelector(`[name='${imageFileDisplayOrderTextName}']`);

        if (!imageFileDisplayOrderText) continue;

        if (productImageItem == draggedImageItem) {

            imageFileDisplayOrderText.textContent = `${draggedItemIndex}: `;
        }
        else {

            const newDisplayOrder = i + movedItemsIndexChange;

            imageFileDisplayOrderText.textContent = `${newDisplayOrder}: `;
        }
    }

    draggedImageItem = null;
    lastDragPositionChangeTime = null;
    draggedItemStartIndex = null;
}

function onProductImageButtonClicked(e) {

    const imageUrl = e.currentTarget.getAttribute(productImageButtonImageRouteAttribute);

    if (!imageUrl) return;

    urlUtils.openDataUrlInNewWindow(imageUrl);
}

function onProductImageFileActiveCheckboxChecked(e) {

    const fileActiveCheckbox = e.currentTarget;

    const productImageItem = fileActiveCheckbox.closest(`[name='${productImageItemName}']`);

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

function onProductImageDeleteButtonClicked(e) {

    const productImageItem = e.currentTarget.closest(`[name='${productImageItemName}']`);

    removeProductImage(productImageItem);
}

export function clearObjectUrls() {

    const productImageItems = getProductImageItems();

    for (const productImageItem of productImageItems) {

        const itemType = productImageItem.getAttribute(productImageItemTypeAttribute);

        if (itemType !== "uploadedFile") continue;

        const imageUrl = getProductImageItemImageUrl(productImageItem);

        URL.revokeObjectURL(imageUrl);
    }
}

export function removeProductImage(productImageItem) {

    const editorId = productImageItem.id; 

    const productImage = loadImageData(editorId);

    if (productImage["$type"] === "uploadedFile") {

        URL.revokeObjectURL(productImage.uploadedFileUrl);
    }
    else if (productImage["$type"] === "promotionProductFile") {

        const promotionFileEditorId = productImage.relatedPromotionFileEditorId;

        promotionFilePopup.onEditorItemRelatedImageDeleted(promotionFileEditorId);
    }

    productImageItem.remove();

    dataStore.removeImageAndSave(editorId);

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Deleted);
}
