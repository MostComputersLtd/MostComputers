import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js";
import * as common from "./Common.js";
import * as createHtmlImageRepresentation from "./CreateHtmlImageRepresentation.js";
import * as imageResize from "./ImageResize.js";

const promotionEditorImagesToUpload = [];
const promotionEditorImagesToResize = [];

export function getImagesToUpload() {

    return promotionEditorImagesToUpload;
}

export function getImagesToResize() {

    return promotionEditorImagesToResize;
}

export function resetImageData() {

    clearAndFreeImagesToUpload();
    clearAndFreeImagesToResize();

    const imageList = document.getElementById(common.promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    for (const imageListItemElement of imageListItemElements) {

        const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);
        const imageResizeButton = imageListItemElement.querySelector(`[name='${common.promotionImageResizeButtonsName}']`);
        const imageDeleteButton = imageListItemElement.querySelector(`[name='${common.promotionImageDeleteButtonsName}']`);

        const imageElementId = imageElement.id;

        imageListItemElement.addEventListener('click', () => createHtmlImageRepresentation.copyPromotionImageForHtml(imageElementId));
        imageResizeButton.addEventListener("click", showImageResizePopupOnClick);
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

function clearAndFreeImagesToResize() {

    for (let i = promotionEditorImagesToResize.length - 1; i >= 0; i--) {

        const imageObject = promotionEditorImagesToResize[i];

        URL.revokeObjectURL(imageObject.fileObjectUrl);

        promotionEditorImagesToResize.splice(i, 1);
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

    const promotionImagesList = document.getElementById(common.promotionImagesListId)

    const promotionImageHtml = await getPromotionImage(fileObjectUrl);

    promotionImagesList.insertAdjacentHTML("beforeend", promotionImageHtml);

    const imageListItemElements = [...promotionImagesList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    const lastImageListItemElement = imageListItemElements[imageListItemElements.length - 1]; 

    const imageListItemIndex = getIndexFromId(lastImageListItemElement.id);

    promotionEditorImagesToUpload.push({
        file: file,
        fileObjectUrl: fileObjectUrl,
        imageIndex: imageListItemIndex,
    });

    const lastImageElement = lastImageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const lastImageElementId = lastImageElement.id;

    lastImageListItemElement.addEventListener('click', () => createHtmlImageRepresentation.copyPromotionImageForHtml(lastImageElementId));

    const imageResizeButton = lastImageListItemElement.querySelector(`[name='${common.promotionImageResizeButtonsName}']`);
    const imageDeleteButton = lastImageListItemElement.querySelector(`[name='${common.promotionImageDeleteButtonsName}']`);

    imageResizeButton.addEventListener("click", showImageResizePopupOnClick);
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

    if (response == null) return null;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return null;

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

async function showImageResizePopupOnClick(e) {

    const imageListItemElement = e.currentTarget.closest(`[name='${common.promotionImageListItemsName}']`);

    const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const imageIndexAsString = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAsString);

    const imageIdAsString = imageElement.getAttribute(common.promotionEditorImageIdAttribute);

    const imageId = parseInt(imageIdAsString);

    let headerImageDataText = `Image ${imageIndex}`;

    if (!isNaN(imageId)) {

        headerImageDataText += ` (ID: ${imageId})`;
    }

    const originalFileName = imageElement.getAttribute(common.promotionEditorImageFileNameAttribute);
    const originalContentType = imageElement.getAttribute(common.promotionEditorImageContentTypeAttribute);

    const imageResizePopupResponse = await imageResize.getImageResizePopupAsync(
        imageElement,
        headerImageDataText,
        1150,
        null,
        originalFileName,
        originalContentType,
        {
            left: "55vw",
            right: "10vw",
            top: "10vh",
            bottom: "10vh"
        });

    if (imageResizePopupResponse == null) return null;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(imageResizePopupResponse);

    if (!didNotRedirect || !imageResizePopupResponse.ok) return null;

    const imageResizePopupHtml = await imageResizePopupResponse.text();

    const imageResizePopupContainer = document.getElementById(common.imageResizePopupContainerId);

    imageResizePopupContainer.innerHTML = imageResizePopupHtml;

    imageResize.initEventsAndInitialDataAsync(() => onImageResizeConfirmButtonClicked(imageElement));

    const dialog = imageResizePopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function onImageResizeConfirmButtonClicked(imageElement) {

    const resizedImageData = await imageResize.getResizedImageAsync();

    const originalFileName = resizedImageData.originalFileName;
    const originalFileType = resizedImageData.originalFileType;
    const resizedImageBlob = resizedImageData.blob;

    const fileForResizedImage = new File([resizedImageBlob], originalFileName, {
        type: originalFileType
    });

    const fileBlobUrl = URL.createObjectURL(resizedImageBlob);

    imageElement.src = fileBlobUrl;

    const imageIndexAttribute = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAttribute);

    const existingImageIdAttribute = imageElement.getAttribute(common.promotionEditorImageIdAttribute);

    if (existingImageIdAttribute == null || existingImageIdAttribute == '') {

        const imageToUploadEntry = promotionEditorImagesToUpload.find(x => x.imageIndex == imageIndex);

        imageToUploadEntry.file = fileForResizedImage;

        return;
    }

    const imageToResizeEntry = promotionEditorImagesToResize.find(x => x.imageIndex == imageIndex);

    const existingImageId = parseInt(existingImageIdAttribute);

    if (imageToResizeEntry == null) {

        promotionEditorImagesToResize.push({

            file: fileForResizedImage,
            imageIndex: imageIndex,
            imageId: existingImageId,
        });
    }
    else {

        imageToResizeEntry.file = fileForResizedImage;
    }
}

async function removePromotionImageOnClick(e) {

    e.stopPropagation();

    const imageListItemElement = e.target.closest(`[name='${common.promotionImageListItemsName}']`);

    const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

    const imageElementUrl = imageElement.src;

    await removePromotionImage(imageListItemElement.id, imageElementUrl);
}

async function removePromotionImage(imageListItemElementId, imageUrl) {

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

        if (imageToUpload.imageIndex == imageIndex) {

            promotionEditorImagesToUpload.splice(i, 1);
        }
        else if (imageToUpload.imageIndex > imageIndex) {

            imageToUpload.imageIndex--;
        }
    }

    for (let i = 0; i < promotionEditorImagesToResize.length; i++) {

        const imageToResize = promotionEditorImagesToResize[i];

        if (imageToResize.imageIndex == imageIndex) {

            promotionEditorImagesToResize.splice(i, 1);
        }
        else if (imageToResize.imageIndex > imageIndex) {

            imageToResize.imageIndex--;
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

            const newSpan = createHtmlImageRepresentation.createImageVisibleRepresentation(currentImageIndex - 1, currentImageId);

            span.parentNode.replaceChild(newSpan, span);
        }
    }

    const currentClipboardText = await navigator.clipboard.readText();

    if (currentClipboardText != null
        && currentClipboardText.startsWith(createHtmlImageRepresentation.copyImageRepresentationStart)
        && currentClipboardText.endsWith(createHtmlImageRepresentation.copyImageRepresentationEnd)) {

        const imageData = createHtmlImageRepresentation.readImageRepresentationData(currentClipboardText);

        if (imageData.imageIndex == imageIndex) {

            navigator.clipboard.writeText("");
        }
    }
}

function getIndexFromId(elementId) {

    const indexOfSeparator = elementId.lastIndexOf('-');

    const indexAsString = elementId.substring(indexOfSeparator + 1);

    return parseInt(indexAsString);
}
