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