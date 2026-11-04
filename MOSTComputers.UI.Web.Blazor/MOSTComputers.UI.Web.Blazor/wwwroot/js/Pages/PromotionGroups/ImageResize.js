const previewImageId = "imageResizePopupPreviewImage";
const widthInputId = "imageResizePopupWidthInput";
const heightInputId = "imageResizePopupHeightInput";
const maintainAspectRatioCheckboxId = "imageResizePopupMaintainAspectRatioCheckbox";
const confirmButtonId = "imageResizePopupConfirmButton";

const previewImageOriginalFileNameAttribute = "data-original-file-name";
const previewImageOriginalContentTypeAttribute = "data-original-content-type";
const widthInputOriginalWidthAttribute = "data-original-width";
const heightInputOriginalHeightAttribute = "data-original-height";

let originalImageData;
let previewImageObjectUrl = null;

let resizeTimeout;

export async function getImageResizePopupAsync(
    imageElement,
    imageIdentifierHeaderText,
    preferredResizeWidth,
    preferredResizeHeight,
    originalFileName,
    originalContentType,
    marginOptions) { 

    const data = {
        imageIdentifierText: imageIdentifierHeaderText,
        imageUrl: imageElement.src,
        currentWidth: imageElement.naturalWidth,
        currentHeight: imageElement.naturalHeight,
        preferredResizeWidth: preferredResizeWidth,
        preferredResizeHeight: preferredResizeHeight,
        originalFileName: originalFileName,
        originalContentType: originalContentType,
        marginOptions: marginOptions
    };

    const url = "api/components/promotionGroups/imagesResizePopup";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html, application/problem+json"
        },
        body: JSON.stringify(data)
    });

    return response;
}

export async function initEventsAndInitialDataAsync(confirmButtonCallback) {

    const previewImage = document.getElementById(previewImageId);
    const widthInput = document.getElementById(widthInputId);
    const heightInput = document.getElementById(heightInputId);
    const confirmButton = document.getElementById(confirmButtonId);

    if (!previewImage.complete) {

        await new Promise((resolve, reject) => {
            previewImage.addEventListener("load", resolve, { once: true });
            previewImage.addEventListener("error", reject, { once: true });
        });
    }

    originalImageData = await createImageBitmap(previewImage);

    widthInput.addEventListener("input", resizeWhenWidthChanges);
    heightInput.addEventListener("input", resizeWhenHeightChanges);

    confirmButton.addEventListener("click", confirmButtonCallback);

    let width = parseFloat(widthInput.value);
    let height = parseFloat(heightInput.value);

    if (isNaN(width) || isNaN(height)) return;

    const originalWidthAttribute = widthInput.getAttribute(widthInputOriginalWidthAttribute);
    const originalHeightAttribute = heightInput.getAttribute(heightInputOriginalHeightAttribute);

    const preferredWidthExists = widthInput.value != originalWidthAttribute;
    const preferredHeightExists = heightInput.value != originalHeightAttribute;

    const originalWidth = parseFloat(originalWidthAttribute);
    const originalHeight = parseFloat(originalHeightAttribute);

    if (!preferredWidthExists && !preferredHeightExists) return;

    if (preferredWidthExists && !preferredHeightExists) {

        height = getOtherInputSizeByAspectRatio(width, originalWidth, originalHeight);

        heightInput.value = Math.round(height);
    }
    else if (preferredHeightExists && !preferredWidthExists) {

        width = getOtherInputSizeByAspectRatio(height, originalHeight, originalWidth);

        widthInput.value = Math.round(width);
    }

    await resizePreviewImage(width, height);
}

export async function getResizedImageAsync() {

    const previewImage = document.getElementById(previewImageId);
    const widthInput = document.getElementById(widthInputId);
    const heightInput = document.getElementById(heightInputId);

    const originalFileName = previewImage.getAttribute(previewImageOriginalFileNameAttribute);
    const originalFileType = previewImage.getAttribute(previewImageOriginalContentTypeAttribute);

    const width = parseFloat(widthInput.value);
    const height = parseFloat(heightInput.value);

    if (isNaN(width)) {

        alert("Invalid image width");
    }
    else if (isNaN(height)) {

        alert("Invalid image height");
    }

    const resizedImageBlob
        = await resizeImage(originalImageData, width, height, originalFileType);

    return {
        originalFileName: originalFileName,
        originalFileType: originalFileType,
        blob: resizedImageBlob,
    };
}

function resizeWhenWidthChanges(e) {

    const widthInput = e.currentTarget;

    const newWidth = parseFloat(widthInput.value);

    if (isNaN(newWidth)) {
        return;
    }

    const heightInput = document.getElementById(heightInputId);
    const maintainAspectRatioCheckbox = document.getElementById(maintainAspectRatioCheckboxId);

    let newHeight;

    if (maintainAspectRatioCheckbox.checked) {
        
        const originalWidthAttribute = widthInput.getAttribute(widthInputOriginalWidthAttribute);
        const originalHeightAttribute = heightInput.getAttribute(heightInputOriginalHeightAttribute);

        newHeight = getOtherInputSizeByAspectRatio(
            newWidth, parseFloat(originalWidthAttribute), parseFloat(originalHeightAttribute));

        heightInput.value = Math.round(newHeight);
    }
    else {
        newHeight = parseFloat(heightInput.value);

        if (isNaN(newHeight)) {
            return;
        }
    }

    schedulePreviewImageResize(newWidth, newHeight);
}

function resizeWhenHeightChanges(e) {

    const heightInput = e.currentTarget;

    const newHeight = parseFloat(heightInput.value);

    if (isNaN(newHeight)) {
        return;
    }

    const widthInput = document.getElementById(widthInputId);
    const maintainAspectRatioCheckbox = document.getElementById(maintainAspectRatioCheckboxId);

    let newWidth;

    if (maintainAspectRatioCheckbox.checked) {

        const originalWidthAttribute = widthInput.getAttribute(widthInputOriginalWidthAttribute);
        const originalHeightAttribute = heightInput.getAttribute(heightInputOriginalHeightAttribute);

        newWidth = getOtherInputSizeByAspectRatio(
            newHeight, parseFloat(originalHeightAttribute), parseFloat(originalWidthAttribute));

        widthInput.value = Math.round(newWidth);
    }
    else {
        newWidth = parseFloat(widthInput.value);

        if (isNaN(newWidth)) {
            return;
        }
    }

    schedulePreviewImageResize(newWidth, newHeight);
}

function getOtherInputSizeByAspectRatio(resizeInputValue, resizeInputOriginalSize, otherResizeInputOriginalSize) {

    const originalReverseAspectRatio = otherResizeInputOriginalSize / resizeInputOriginalSize;

    const newOtherInputSize = originalReverseAspectRatio * resizeInputValue;

    return newOtherInputSize;
}

function schedulePreviewImageResize(width, height) {

    clearTimeout(resizeTimeout);

    resizeTimeout = setTimeout(async () => resizePreviewImage(width, height), 400);
}

async function resizePreviewImage(width, height) {

    const previewImage = document.getElementById(previewImageId);

    const originalFileType = previewImage.getAttribute(previewImageOriginalContentTypeAttribute);

    const blob = await resizeImage(originalImageData, width, height, originalFileType, false);

    if (previewImageObjectUrl) {

        URL.revokeObjectURL(previewImageObjectUrl);
    }

    previewImageObjectUrl = URL.createObjectURL(blob);

    previewImage.src = previewImageObjectUrl;
}

async function resizeImage(
    imageData,
    targetWidth,
    targetHeight,
    originalFileType) {

    const fileTypeToUse = originalFileType ? originalFileType : "image/png";

    const canvas = document.createElement("canvas");

    canvas.width = targetWidth;
    canvas.height = targetHeight;

    const context = canvas.getContext("2d");

    context.drawImage(imageData, 0, 0, targetWidth, targetHeight);

    const blob = await new Promise(
        resolve => canvas.toBlob(resolve, fileTypeToUse));

    canvas.width = 0;
    canvas.height = 0;

    return blob;
}
