import * as common from "./Common.js";
import * as createHtmlImageRepresentation from "./CreateHtmlImageRepresentation.js";

export function addEventsToHtmlContent() {

    const promotionHtml = document.getElementById(common.promotionHtmlId);

    replaceHtmlContentImageUrlsWithImageRepresentation(promotionHtml);

    promotionHtml.addEventListener("paste", onHtmlContentInputPaste);

    promotionHtml.addEventListener("beforeinput", e => {
        if (e.inputType === "insertParagraph") {

            e.preventDefault();

            document.execCommand("insertLineBreak");
        }
    });
}

export function expandTextAreaToNeededHeight(textareaId) {

    const textarea = document.getElementById(textareaId);

    textarea.style.height = 'auto';

    textarea.style.height = (textarea.scrollHeight + 1.6) + 'px';
}

export function onHtmlContentInputPaste(e) {

    const text = e.clipboardData.getData("text/plain");

    e.preventDefault();

    if (!text.startsWith(createHtmlImageRepresentation.copyImageRepresentationStart)
        || !text.endsWith(createHtmlImageRepresentation.copyImageRepresentationEnd))
    {
        const textNode = document.createTextNode(text);

        insertNodeAtEditorTextCaret(textNode);

        return;
    }

    addImageRepresentationToHtmlContent(text);
}

function addImageRepresentationToHtmlContent(imageRepresentation)
{
    const imageData = createHtmlImageRepresentation.readImageRepresentationData(imageRepresentation);

    const imageSpan = createHtmlImageRepresentation.createImageVisibleRepresentation(imageData.imageIndex, imageData.imageId);

    insertNodeAtEditorTextCaret(imageSpan);
}

function replaceHtmlContentImageUrlsWithImageRepresentation(htmlContentElement) {

    const imageIdsToIndexesMap = getImageIdsAndIndexesFromCurrentEditorData();

    const legacyImageUrlFormatStart = "PromViewImage.aspx?ImageId=";

    const textNodesInElement = getTextNodesInRootElement(htmlContentElement);

    for (const textNodeInElement of textNodesInElement) {

        replaceHtmlContentImageUrls(textNodeInElement, legacyImageUrlFormatStart, imageIdsToIndexesMap);
    }
}

function getImageIdsAndIndexesFromCurrentEditorData() {

    const output = {}

    const imageList = document.getElementById(common.promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${common.promotionImageListItemsName}']`)];

    for (const imageListItemElement of imageListItemElements) {

        const imageElement = imageListItemElement.querySelector(`[name='${common.promotionImagesName}']`);

        const imageIndexAsString = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

        const imageIndex = parseInt(imageIndexAsString);
        
        const imageIdAsString = imageElement.getAttribute(common.promotionEditorImageIdAttribute);

        output[imageIdAsString] = imageIndex;
    }

    return output;
}

function replaceHtmlContentImageUrls(textNode, imageFileFormat, imageIdsToIndexesMap) {

    while (textNode) {

        const nodeText = textNode.nodeValue;

        const imageUrlStartIndex = nodeText.indexOf(imageFileFormat);

        if (imageUrlStartIndex < 0) break;

        const imageIdStartIndex = imageUrlStartIndex + imageFileFormat.length;

        let imageIdEndIndex = imageIdStartIndex;

        while (imageIdEndIndex < nodeText.length) {

            const imageIdCurrentCharacter = nodeText.substring(imageIdEndIndex, imageIdEndIndex + 1);

            if (isNaN(parseInt(imageIdCurrentCharacter))) break;

            imageIdEndIndex++;
        }

        const imageIdAsString = nodeText.substring(imageIdStartIndex, imageIdEndIndex);

        const imageId = parseInt(imageIdAsString);

        const imageIndex = imageIdsToIndexesMap[imageId];

        const afterStart = textNode.splitText(imageUrlStartIndex);

        const afterImageRepresentation = afterStart.splitText(imageIdEndIndex - imageUrlStartIndex);

        const imageRepresentationInHtmlContent = createHtmlImageRepresentation.createImageVisibleRepresentation(imageIndex, imageId);

        afterStart.parentNode.replaceChild(imageRepresentationInHtmlContent, afterStart);

        textNode = afterImageRepresentation;
    }
}

function getTextNodesInRootElement(rootElement) {

    const treeWalker = document.createTreeWalker(
        rootElement,
        NodeFilter.SHOW_TEXT,
        null
    );

    const nodes = [];

    let node;

    while (node = treeWalker.nextNode()) {

        nodes.push(node);
    }

    return nodes;
}

function insertNodeAtEditorTextCaret(node) {

    const selection = window.getSelection();

    if (!selection || selection.rangeCount === 0) {
        return false;
    }

    const range = selection.getRangeAt(0);

    range.deleteContents();

    range.insertNode(node);

    range.setStartAfter(node);
    range.collapse(true);

    selection.removeAllRanges();
    selection.addRange(range);

    return true;
}
