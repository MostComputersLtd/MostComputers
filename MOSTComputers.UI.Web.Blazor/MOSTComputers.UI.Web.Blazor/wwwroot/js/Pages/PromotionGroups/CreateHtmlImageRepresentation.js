import * as common from "./Common.js";

export const copyImageRepresentationStart = "|/imageStart/%|";
export const copyImageRepresentationEnd = "|/imageEnd/%|";

export const copyImageIndexPrefix = "index=";
export const copyImageIdPrefix = "id=";

export const copyImageDataSeparator = ",";

export function copyPromotionImageForHtml(imageElementId) {

    const imageElement = document.getElementById(imageElementId);

    const imageIndexAsString = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAsString);

    const imageIdAsString = imageElement.getAttribute(common.promotionEditorImageIdAttribute);

    const imageId = parseInt(imageIdAsString);

    const imageRepresentation = createImageRepresentationForCopy(imageIndex, imageId);

    navigator.clipboard.writeText(imageRepresentation);
}

function createImageRepresentationForCopy(imageIndex, imageId) {
   
    let output = copyImageRepresentationStart;

    output += copyImageIndexPrefix + imageIndex + copyImageDataSeparator;

    output += copyImageIdPrefix + imageId;

    output += copyImageRepresentationEnd;

    return output;
}

export function readImageRepresentationData(imageRepresentation) {

    let imageDataStartIndex = imageRepresentation.indexOf(copyImageRepresentationStart) + copyImageRepresentationStart.length;

    let imageDataEndIndex = imageRepresentation.indexOf(copyImageRepresentationEnd);

    let imageData = imageRepresentation.substring(imageDataStartIndex, imageDataEndIndex);

    const imageDataFields = imageData.split(copyImageDataSeparator);

    let imageIndex;
    let imageId;

    for (const imageDataField of imageDataFields) {

        if (imageDataField.startsWith(copyImageIndexPrefix)) {

            const imageIndexStartIndex = copyImageIndexPrefix.length;

            const imageIndexAsString = imageDataField.substring(imageIndexStartIndex);

            imageIndex = parseInt(imageIndexAsString);
        }
        else if (imageDataField.startsWith(copyImageIdPrefix)) {

            const imageIdStartIndex = copyImageIdPrefix.length;

            const imageIdAsString = imageDataField.substring(imageIdStartIndex);

            imageId = parseInt(imageIdAsString);
        }
    }

    return {
        imageIndex: imageIndex,
        imageId: imageId
    }
}

export function createImageVisibleRepresentation(imageIndex, imageId) {

    let imageText;

    const imageDisplayIndex = imageIndex + 1;

    if (!imageId) {

        imageText = `Image ${imageDisplayIndex}`;
    }
    else {
        imageText = `Image ${imageDisplayIndex} (ID: ${imageId})`;
    }

    const imageSpan = document.createElement('span');

    imageSpan.setAttribute("name", common.htmlContentImageSpanName);
    imageSpan.setAttribute("contenteditable", "false");
    imageSpan.setAttribute(common.htmlContentImageSpanImageIndexAttribute, imageIndex);

    if (imageId) {

        imageSpan.setAttribute(common.htmlContentImageSpanImageIdAttribute, imageId);
    }

    imageSpan.innerText = imageText;

    imageSpan.classList.add(common.htmlContentImageSpanClass);

    imageSpan.addEventListener('click', onImageRepresentationClick);

    return imageSpan;
}

function onImageRepresentationClick(e) {
    e.target.remove();
}
