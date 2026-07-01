import * as common from "./Common.js";
import * as imageAndHtmlEditorCommon from "./CreateHtmlImageRepresentation.js";

const promotionEditorImagesToUpload = [];

export function getImagesToUpload()
{
    return promotionEditorImagesToUpload;
}

export function resetImageData() {

    clearAndFreeImagesToUpload();

    const imageList = document.getElementById(common.promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    for (const imageListItemElement of imageListItemElements) {

        const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);
        const imageDeleteButton = imageListItemElement.querySelector(`[name='${common.promotionImageDeleteButtonsName}']`);

        const imageElementId = imageElement.id;

        imageListItemElement.addEventListener('click', () => imageAndHtmlEditorCommon.copyPromotionImageForHtml(imageElementId));
        imageDeleteButton.addEventListener('click', removePromotionImageOnClick);
    }
}

function clearAndFreeImagesToUpload() {

    for (let i = promotionEditorImagesToUpload.length - 1; i >= 0; i--) {

        const imageObject = promotionEditorImagesToUpload[i];

        URL.revokeObjectURL(imageObject.fileObjectUrl);

        promotionEditorImagesToUpload.splice(i, 1);
    }
}

export async function addNewImageToEditor() {

    const promotionImagesInput = document.getElementById(common.promotionImagesInputId);

    promotionImagesInput.addEventListener("change", onSelectedImageToAddToPromotion, {once: true});

    promotionImagesInput.click();
}

async function onSelectedImageToAddToPromotion(e) {

    const file = e.target.files[0];

    if (!file) return;

    const fileObjectUrl = URL.createObjectURL(file);

    promotionEditorImagesToUpload.push({
        File: file,
        fileObjectUrl: fileObjectUrl
    });

    const promotionImagesList = document.getElementById(common.promotionImagesListId)

    const promotionImageHtml = await getPromotionImage(fileObjectUrl);

    promotionImagesList.insertAdjacentHTML("beforeend", promotionImageHtml);

    const imageListItemElements = [...promotionImagesList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    const lastImageListItemElement = imageListItemElements[imageListItemElements.length - 1];

    const lastImageElement = lastImageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const lastImageElementId = lastImageElement.id;

    lastImageListItemElement.addEventListener('click', () => imageAndHtmlEditorCommon.copyPromotionImageForHtml(lastImageElementId));

    const imageDeleteButton = lastImageListItemElement.querySelector(`[name='${common.promotionImageDeleteButtonsName}']`);

    imageDeleteButton.addEventListener('click', removePromotionImageOnClick);
}

async function getPromotionImage(imageUrl) {

    const promotionImageOptions = getPromotionImageOptions(imageUrl);

    const response = await fetch("api/components/promotionGroups/images", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(promotionImageOptions)
    });

    if (!response.ok) return null;

    return await response.text();
}

function getPromotionImageOptions(imageUrl) {

    const imageList = document.getElementById(common.promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    let index = 0;

    for (const imageListItemElement of imageListItemElements) {

        const elementId = imageListItemElement.id;

        const indexOfSeparator = elementId.lastIndexOf('-');

        const indexAsString = elementId.substring(indexOfSeparator + 1);

        const elementIndex = parseInt(indexAsString);

        if (elementIndex >= index) {

            index = elementIndex + 1;
        }
    }

    return {
        Index: index,
        ImageUrl: imageUrl,
    }
}

function removePromotionImageOnClick(e) {

    e.stopPropagation();

    const imageListItemElement = e.target.closest(`[name='${common.promotionImageListItemsName}']`);

    const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const imageElementUrl = imageElement.src;

    removePromotionImage(imageListItemElement.id, imageElementUrl);
}

function removePromotionImage(imageListItemElementId, imageUrl) {

    const imageListItemElement = document.getElementById(imageListItemElementId);

    const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const imageIndexAsString = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAsString);

    const imageSpansInHtmlContent = document.querySelectorAll(`[name='${common.htmlContentImageSpanName}']`);

    let imageSpanInHtmlContent;

    for (const span of imageSpansInHtmlContent) {

        const imageIndexAttribute = span.getAttribute(common.htmlContentImageSpanImageIndexAttribute);

        if (imageIndexAttribute === imageIndexAsString) {
            imageSpanInHtmlContent = span;

            break;
        }
    }

    if (imageSpanInHtmlContent) {
        alert("Image is present in the HTML content, please remove all references to it.");

        return;
    }

    if (imageListItemElement) {

        imageListItemElement.remove();
    }

    for (let i = 0; i < promotionEditorImagesToUpload.length; i++) {

        const imageToUpload = promotionEditorImagesToUpload[i];

        if (imageToUpload.fileObjectUrl == imageUrl) {

            promotionEditorImagesToUpload.splice(i, 1);

            break;
        }
    }

    const imageList = document.getElementById(common.promotionImagesListId);

    const remainingImageListItemElements = [...imageList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    for (const remainingImageListItem of remainingImageListItemElements) {

        const remainingImageElement = remainingImageListItem.querySelector(`[name='${common.promotionImagesName}']`);

        const remainingImageIndexAsString = remainingImageElement.getAttribute(common.promotionEditorImageIndexAttribute);

        const remainingImageIndex = parseInt(remainingImageIndexAsString);

        if (remainingImageIndex > imageIndexAsString) {

            const newImageIndex = remainingImageIndex - 1;

            const matchingDeleteButtonId = common.getImageListItemDeleteButtonIdFromIndex(remainingImageIndex);

            const matchingDeleteButtonElement = document.getElementById(matchingDeleteButtonId);

            remainingImageElement.setAttribute(common.promotionEditorImageIndexAttribute, newImageIndex);

            const newImageListItemId = common.getImageListItemIdFromIndex(newImageIndex);

            remainingImageListItem.id = newImageListItemId;

            matchingDeleteButtonElement.id = common.getImageListItemDeleteButtonIdFromIndex(newImageIndex);
            remainingImageElement.id = common.getImageElementIdFromIndex(newImageIndex);
        }
    }

    for (const span of imageSpansInHtmlContent) {

        const imageIndexAttribute = span.getAttribute(common.htmlContentImageSpanImageIndexAttribute);
        const imageIdAttribute = span.getAttribute(common.htmlContentImageSpanImageIdAttribute);

        const currentImageIndex = parseInt(imageIndexAttribute);
        const currentImageId = parseInt(imageIdAttribute);

        if (currentImageIndex > imageIndex) {

            const newSpan = imageAndHtmlEditorCommon.createImageVisibleRepresentation(currentImageIndex - 1, currentImageId);

            span.parentNode.replaceChild(newSpan, span);
        }
    }
}