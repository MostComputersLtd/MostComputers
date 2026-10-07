import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as urlUtils from "../../../Common/UrlUtils.js"

const productImageListId = "singleEditorProductImageList";

const productImageItemName = "productImageItem";
const promotionFileItemName = "promotionFileItem";

const productImageItemDataId = "productImageItemData";

const productImageButtonName = "productImageButton";
const productImageDeleteButtonName = "productImageDeleteButton";
const imageFileActiveCheckboxName = "imageFileActiveCheckbox";

const productImageButtonImageRouteAttribute = "data-image-url";

const fileFormFieldName = "file";
const filePreviewUrlFormFieldName = "previewUrl";
const fileIndexFormFieldName = "index";
const productImageFileWidthFormFieldName = "width";
const productImageFileHeightFormFieldName = "height";

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

export async function addFileToImagesAsync(blob) {

    if (blob.size === 0) {

        return false;
    }

    if (!blob.type.startsWith('image/')) {

        return false;
    }

    const previewUrl = URL.createObjectURL(blob);

    const dataStoreData = dataStore.loadData();

    const imageDimensions = await getImageFileDimensions(blob);

    const formData = new FormData();

    formData.append(fileFormFieldName, blob);
    formData.append(filePreviewUrlFormFieldName, previewUrl);
    formData.append(fileIndexFormFieldName, dataStoreData.productImages.length);
    formData.append(productImageFileWidthFormFieldName, imageDimensions.width);
    formData.append(productImageFileHeightFormFieldName, image.height);

    const url = "api/components/productEditor/single/addImage";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Accept": "application/json"
        },
        body: formData
    });

    if (!response.ok) {
        return false;
    }

    const data = await response.text();

    const success = addNewImageFromElementHtml(data);

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

    const jsonDataScriptElement = document.getElementById(productImageItemDataId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductImageItemEvents(productImageItem);

    const success = dataStore.addProductImageAndSave(jsonData.editorId, jsonData, file);

    editorChanges.setImageChanged(jsonData.editorId, editorChanges.ItemChangeState.Added);

    return success;
}

export function getImageFileDimensions(blob) {

    return new Promise((resolve, reject) => {

        const image = new Image();

        image.onload = () => {

            URL.revokeObjectURL(image.src);

            resolve({
                width: image.naturalWidth,
                height: image.naturalHeight
            });
        };

        image.onerror = () => {

            URL.revokeObjectURL(image.src);

            reject(new Error("Invalid image file."));
        };

        image.src = URL.createObjectURL(blob);
    });
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

    for (let i = smallerDragIndex; i < largerDragIndex; i++) {

        const productImageItem = container.children[i];

        editorChanges.setImageChanged(productImageItem.id, editorChanges.ItemChangeState.Updated);
    }

    draggedImageItem = null;
    lastDragPositionChangeTime = null;
    draggedItemStartIndex = null;
}

function getProductImageItems() {

    const productImageList = document.getElementById(productImageListId);

    return [...productImageList.querySelectorAll(`[name='${productImageItemName}']`)];
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
        image.relatedImageFile.Active = fileActiveCheckbox.checked;
    });

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Updated);
}

function onProductImageDeleteButtonClicked(e) {

    const productImageItem = e.currentTarget.closest(`[name='${productImageItemName}']`);

    const editorId = productImageItem.id; 

    const productImage = loadImageData(editorId);

    if (productImage.uploadedImageFileData?.previewUrl) {

        URL.revokeObjectURL(productImage.uploadedImageFileData?.previewUrl);
    }

    if (productImage.relatedPromotionFile) {

        const relatedPromotionFileItem = productImageItem.querySelector(`[name='${promotionFileItemName}']`);

        if (relatedPromotionFileItem) {

            const promotionFileEditorId = relatedPromotionFileItem.id;

            editorChanges.setPromotionProductFileChanged(promotionFileEditorId, editorChanges.ItemChangeState.Deleted);
        }
    }

    dataStore.removeImageAndSave(editorId);

    editorChanges.setImageChanged(editorId, editorChanges.ItemChangeState.Deleted);
}
