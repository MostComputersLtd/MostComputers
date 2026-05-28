const promotionSearchInputId = "groupPromotionSearchInput";
const groupSelectId = "promotionGroupSelect";
const onlyActiveCheckboxId = "groupPromotionOnlyActivePromotionsCheckbox";

const searchPromotionButtonId = "searchPromotionButton";
const addPromotionButtonId = "addPromotionButton";

const promotionListContainerId = "promotionListContainer";

const promotionEditPanelContainerId = "promotionEditPanelContainer";
const promotionEditPanelId = "promotionEditPanel";

const savePromotionButtonId = "savePromotionButton";

const promotionNameId = "promotionName";
const promotionStartDateId = "promotionStartDate";
const promotionExpirationDateId = "promotionExpirationDate";
const promotionDisplayOrderId = "promotionDisplayOrder";
const promotionGroupSelectId = "promotionGroupEditorSelect";
const promotionDisabledId = "promotionDisabled";
const promotionRestrictedId = "promotionRestricted";
const promotionMemberDefaultId = "promotionMemberDefault";
const promotionDefaultPriorityId = "promotionDefaultPriority";
const promotionHtmlId = "promotionHtml";


const promotionImagesInputId = "promotionImagesInput";
const promotionImagesListId = "promotionImagesList";

const promotionEditRelatedProductsTableId = "promotionEditRelatedProductsTable";

function getImageListItemIdFromIndex(imageIndex) {
    return `promotionEditImageListItem-${imageIndex}`;
}

function getImageElementIdFromIndex(imageIndex) {
    return `promotionEditImage-${imageIndex}`;
}

function getImageListItemDeleteButtonIdFromIndex(imageIndex) {
    return `promotionEditImageDeleteButton-${imageIndex}`;
}

const promotionGroupEditorPopupContainerId = "promotionGroupEditorPopupContainer";

const promotionGroupEditorPopupId = "promotionGroupEditorPopup";

const promotionGroupNameInputId = "promotionGroupName";
const promotionGroupDisplayOrderInputId = "promotionGroupDisplayOrder";
const promotionGroupLogoImageDisplayId = "promotionGroupLogoImageDisplay";
const promotionGroupLogoChangeButtonId = "promotionGroupLogoChangeButton";
const promotionGroupLogoInputId = "promotionGroupLogoInput";
const promotionGroupEditLogoImageContainerId = "promotionGroupEditLogoImageContainer";

const addRelatedProductsPopupContainerId = "addRelatedProductsPopupContainer";

const addRelatedProductsPopupId = "addRelatedProductsPopup";
const addRelatedProductSearchResultsContainerId = "addRelatedProductSearchResultsContainer";

const antiforgeryTokenInputId = "__RequestVerificationToken";

const promotionImageListItemsName = "promotionEditImageListItem";
const promotionImagesName = "promotionEditImage";
const promotionImageDeleteButtonsName = "promotionEditImageDeleteButton";
const htmlContentImageSpanName = "groupPromotionEditorHtmlContentImageSpan";

const promotionEditorPromotionIdAttribute = "data-current-promotion-id";
const promotionEditorImageIdAttribute = "data-current-promotion-image-id";
const promotionEditorImageIndexAttribute = "data-current-promotion-image-index";

const htmlContentImageSpanImageIndexAttribute = "data-promotion-image-index-in-span"
const htmlContentImageSpanImageIdAttribute = "data-promotion-image-id-in-span"

const loadingClass = "loading";

const htmlContentImageSpanClass = "promotion-edit-html-content-image-span";

const promotionEditorImagesToUpload = [];

let promotionGroupImageToUpload = null;

async function searchGroupPromotions()
{
    const searchOptions = getGroupPromotionSearchOptionsFromCurrentData();

    const response = await fetch("api/components/promotionGroups/search", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(searchOptions)
    });

    const searchResultHtml = await response.text();

    const promotionListContainer = document.getElementById(promotionListContainerId);

    promotionListContainer.innerHTML = searchResultHtml;
}

function getGroupPromotionSearchOptionsFromCurrentData()
{
    const searchInput = document.getElementById(promotionSearchInputId);
    const groupSelect = document.getElementById(groupSelectId);
    const onlyActiveCheckbox = document.getElementById(onlyActiveCheckboxId);

    const searchInputValue = searchInput.value;
    const groupSelectValue = getIntegerOrNullFromString(groupSelect.value);
    const activeOnlyValue = onlyActiveCheckbox.checked;

    return {
        SearchData: searchInputValue,
        PromotionGroupId: groupSelectValue,
        ActiveOnly: activeOnlyValue
    };
}

async function openPromotionEditorForPromotion(id = null)
{
    let response;

    if (id == null)
    {
        response = await fetch("api/components/promotionGroups/editor/new", {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }
    else
    {
        response = await fetch(`api/components/promotionGroups/editor/${id}`, {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    const promotionEditorHtml = await response.text();

    const promotionEditPanelContainer = document.getElementById(promotionEditPanelContainerId);

    clearAndFreeImagesToUpload();

    promotionEditPanelContainer.innerHTML = promotionEditorHtml;

    expandTextAreaToNeededHeight(promotionHtmlId);

    const imageList = document.getElementById(promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

    for (const imageListItemElement of imageListItemElements) {

        const imageElement = imageListItemElement.querySelector(`[name='${promotionImagesName}']`);
        const imageDeleteButton = imageListItemElement.querySelector(`[name='${promotionImageDeleteButtonsName}']`);

        const imageElementId = imageElement.id;

        imageListItemElement.addEventListener('click', () => copyPromotionImageForHtml(imageElementId));
        imageDeleteButton.addEventListener('click', removePromotionImageOnClick);
    }

    const promotionHtml = document.getElementById(promotionHtmlId);

    replaceHtmlContentImageUrlsWithImageRepresentation(promotionHtml);

    promotionHtml.addEventListener("paste", onHtmlContentInputPaste);

    promotionHtml.addEventListener("beforeinput", e => {
        if (e.inputType === "insertParagraph") {

            e.preventDefault();

            document.execCommand("insertLineBreak");
        }
    });
}

function clearAndFreeImagesToUpload() {

    for (let i = promotionEditorImagesToUpload.length - 1; i >= 0; i--) {

        const imageObject = promotionEditorImagesToUpload[i];

        URL.revokeObjectURL(imageObject.fileObjectUrl);

        promotionEditorImagesToUpload.splice(i, 1);
    }
}

async function addNewImageToEditor() {

    const promotionImagesInput = document.getElementById(promotionImagesInputId);

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

    const promotionImagesList = document.getElementById(promotionImagesListId)

    const promotionImageHtml = await getPromotionImage(fileObjectUrl);

    promotionImagesList.insertAdjacentHTML("beforeend", promotionImageHtml);

    const imageListItemElements = [...promotionImagesList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

    const lastImageListItemElement = imageListItemElements[imageListItemElements.length - 1];

    const lastImageElement = lastImageListItemElement.querySelector(`[name='${promotionImagesName}']`);

    const lastImageElementId = lastImageElement.id;

    lastImageListItemElement.addEventListener('click', () => copyPromotionImageForHtml(lastImageElementId));

    const imageDeleteButton = lastImageListItemElement.querySelector(`[name='${promotionImageDeleteButtonsName}']`);

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

    const imageList = document.getElementById(promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

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

const copyImageRepresentationStart = "|/imageStart/%|";
const copyImageRepresentationEnd = "|/imageEnd/%|";

const copyImageIndexPrefix = "index=";
const copyImageIdPrefix = "id=";

const copyImageDataSeparator = ",";

const serverImageRepresentationStart = "|/imageStart/%|";
const serverImageRepresentationEnd = "|/imageEnd/%|";

const serverImageIndexPrefix = "index=";

function copyPromotionImageForHtml(imageElementId) {

    const imageElement = document.getElementById(imageElementId);

    const imageIndexAsString = imageElement.getAttribute(promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAsString);

    const imageIdAsString = imageElement.getAttribute(promotionEditorImageIdAttribute);

    const imageId = parseInt(imageIdAsString);

    const imageRepresentation = createImageRepresentationForCopy(imageIndex, imageId);

    navigator.clipboard.writeText(imageRepresentation);
}

function onHtmlContentInputPaste(e) {

    const text = e.clipboardData.getData("text/plain");

    e.preventDefault();

    if (!text.startsWith(copyImageRepresentationStart)
        || !text.endsWith(copyImageRepresentationEnd))
    {
        const textNode = document.createTextNode(text);

        insertNodeAtEditorTextCaret(textNode);

        return;
    }

    addImageRepresentationToHtmlContent(text);
}

function addImageRepresentationToHtmlContent(imageRepresentation)
{
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

    const imageSpan = createImageVisibleRepresentation(imageIndex, imageId);

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

    const imageList = document.getElementById(promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

    for (const imageListItemElement of imageListItemElements) {

        const imageElement = imageListItemElement.querySelector(`[name='${promotionImagesName}']`);

        const imageIndexAsString = imageElement.getAttribute(promotionEditorImageIndexAttribute);

        const imageIndex = parseInt(imageIndexAsString);
        
        const imageIdAsString = imageElement.getAttribute(promotionEditorImageIdAttribute);

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

        const imageRepresentationInHtmlContent = createImageVisibleRepresentation(imageIndex, imageId);

        afterStart.parentNode.replaceChild(imageRepresentationInHtmlContent, afterStart);

        textNode = afterImageRepresentation;
    }
}

function createImageRepresentationForCopy(imageIndex, imageId) {
   
    let output = copyImageRepresentationStart;

    output += copyImageIndexPrefix + imageIndex + copyImageDataSeparator;

    output += copyImageIdPrefix + imageId;

    output += copyImageRepresentationEnd;

    return output;
}

function createImageRepresentationForServer(imageIndex) {

    let output = serverImageRepresentationStart;

    output += serverImageIndexPrefix + imageIndex;

    output += serverImageRepresentationEnd;

    return output;
}

function createImageVisibleRepresentation(imageIndex, imageId) {

    let imageText;

    const imageDisplayIndex = imageIndex + 1;

    if (!imageId) {

        imageText = `Image ${imageDisplayIndex}`;
    }
    else {
        imageText = `Image ${imageDisplayIndex} (ID: ${imageId})`;
    }

    const imageSpan = document.createElement('span');

    imageSpan.setAttribute("name", htmlContentImageSpanName);
    imageSpan.setAttribute("contenteditable", "false");
    imageSpan.setAttribute(htmlContentImageSpanImageIndexAttribute, imageIndex);

    if (imageId) {

        imageSpan.setAttribute(htmlContentImageSpanImageIdAttribute, imageId);
    }

    imageSpan.innerText = imageText;

    imageSpan.classList.add(htmlContentImageSpanClass);

    imageSpan.addEventListener('click', onImageRepresentationClick);

    return imageSpan;
}

function onImageRepresentationClick(e) {
    e.target.remove();
}

async function savePromotion() {

    const promotionEditPanel = document.getElementById(promotionEditPanelId);

    const editorCurrentId = promotionEditPanel.getAttribute(promotionEditorPromotionIdAttribute);

    const savePromotionButton = document.getElementById(savePromotionButtonId);

    savePromotionButton.classList.add(loadingClass);

    try {
        if (editorCurrentId === "") {

            await createPromotion();

            return;
        }

        await updatePromotion();
    }
    finally {
        savePromotionButton.classList.remove(loadingClass);
    }
}

async function createPromotion() {

    const createRequest = getPromotionCreateRequestFromCurrentData();

    if (createRequest == null) return;

    const response = await fetch("api/components/promotionGroups/create", {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: createRequest
    });

    if (!response.ok) return;

    const newPromotionIdAsString = await response.text();

    const newPromotionId = parseInt(newPromotionIdAsString);

    await openPromotionEditorForPromotion(newPromotionId);
}

function getPromotionCreateRequestFromCurrentData()
{
    const editor = document.getElementById(promotionEditPanelId);

    if (!editor) return null;

    const promotionName = document.getElementById(promotionNameId);
    const promotionHtmlContent = document.getElementById(promotionHtmlId);
    const promotionStartDate = document.getElementById(promotionStartDateId);
    const promotionExpirationDate = document.getElementById(promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(promotionGroupSelectId);
    const promotionDisabled = document.getElementById(promotionDisabledId);
    const promotionRestricted = document.getElementById(promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(promotionDefaultPriorityId);

    const formData = new FormData();

    formData.append("Name", promotionName.value);

    const groupId = getIntegerOrNullFromString(promotionGroupSelect.value);

    if (groupId != null) {

        formData.append("GroupId", groupId);
    }

    const htmlContent = replaceImageElementsWithTextualRepresentationsInHtmlContent(promotionHtmlContent);

    formData.append("HtmlContent", htmlContent);

    const startDate = promotionStartDate.value;

    if (startDate != null && startDate != '') {

        formData.append("StartDate", startDate);
    }

    const expirationDate = promotionExpirationDate.value;

    if (expirationDate != null && expirationDate != '') {

        formData.append("ExpirationDate", expirationDate);
    }

    const displayOrder = getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {

        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {

        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    for (let i = 0; i < promotionEditorImagesToUpload.length; i++)
    {
        const imageObject = promotionEditorImagesToUpload[i];

        const blob = imageObject.File;

        formData.append(`PromotionImageCreateRequests[${i}]`, blob);
    }

    return formData;
}

async function updatePromotion() {

    const editor = document.getElementById(promotionEditPanelId);

    if (!editor) return;

    const promotionIdAsString = editor.getAttribute(promotionEditorPromotionIdAttribute);

    const promotionId = getIntegerOrNullFromString(promotionIdAsString);

    if (promotionId == null || isNaN(promotionId)) return;

    const updateRequest = getPromotionUpdateRequestFromCurrentData(promotionId);

    const response = await fetch("api/components/promotionGroups/update", {
        method: "PUT",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: updateRequest
    });

    if (!response.ok) return;

    await openPromotionEditorForPromotion(promotionId);
}

function getPromotionUpdateRequestFromCurrentData(promotionId)
{
    const promotionName = document.getElementById(promotionNameId);
    const promotionHtmlContent = document.getElementById(promotionHtmlId);
    const promotionStartDate = document.getElementById(promotionStartDateId);
    const promotionExpirationDate = document.getElementById(promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(promotionGroupSelectId);
    const promotionDisabled = document.getElementById(promotionDisabledId);
    const promotionRestricted = document.getElementById(promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(promotionDefaultPriorityId);

    const imageList = document.getElementById(promotionImagesListId);

    const imageElements = [...imageList.querySelectorAll(`[name='${promotionImagesName}']`)];

    const formData = new FormData();

    formData.append("Id", promotionId);
    formData.append("Name", promotionName.value);

    const groupId = getIntegerOrNullFromString(promotionGroupSelect.value);

    if (groupId != null) {
        formData.append("GroupId", groupId);
    }

    const htmlContent = replaceImageElementsWithTextualRepresentationsInHtmlContent(promotionHtmlContent);

    formData.append("HtmlContent", htmlContent);

    const startDate = promotionStartDate.value;

    if (startDate != null && startDate != '') {
        formData.append("StartDate", startDate);
    }

    const expirationDate = promotionExpirationDate.value;

    if (expirationDate != null && expirationDate != '') {
        formData.append("ExpirationDate", expirationDate);
    }

    const displayOrder = getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {
        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {
        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    for (let i = 0; i < imageElements.length; i++) {

        const imageElement = imageElements[i];

        const existingImageId = imageElement.getAttribute(promotionEditorImageIdAttribute);
        const imageIndex = imageElement.getAttribute(promotionEditorImageIndexAttribute);

        if (existingImageId == null || existingImageId == '') {

            for (let i = 0; i < promotionEditorImagesToUpload.length; i++) {

                const imageObject = promotionEditorImagesToUpload[i];

                if (imageObject.fileObjectUrl != imageElement.src) continue;

                const blob = imageObject.File;

                formData.append(`PromotionImageCreateRequests[${i}].ImageFile`, blob);
                formData.append(`PromotionImageCreateRequests[${i}].ImageIndex`, imageIndex);

                break;
            }
        }
        else {
            formData.append(`ImageIdsToKeep[${i}].Id`, existingImageId);
            formData.append(`ImageIdsToKeep[${i}].ImageIndex`, imageIndex);
        }
        
    } 

    return formData;
}

function replaceImageElementsWithTextualRepresentationsInHtmlContent(htmlContentEditor) {
    const htmlContentEditorClone = htmlContentEditor.cloneNode(true);

    const imageReferencesInHtmlContent = [...htmlContentEditorClone.querySelectorAll(`[name='${htmlContentImageSpanName}']`)];

    for (const imageReferenceElement of imageReferencesInHtmlContent) {

        const imageIndex = imageReferenceElement.getAttribute(htmlContentImageSpanImageIndexAttribute);

        const textualRepresentationForServer = createImageRepresentationForServer(imageIndex);

        const textNodeWithRepresentation = document.createTextNode(textualRepresentationForServer);

        imageReferenceElement.replaceWith(textNodeWithRepresentation);
    }

    return htmlContentEditorClone.textContent;
}

function extractTopLevelTextNodesFromHtml(html) {

    const parsedDocument = new DOMParser().parseFromString(html, "text/html");

    let extractedText = "";

    for (const childNode of parsedDocument.body.childNodes) {

        if (childNode.nodeType === Node.TEXT_NODE) {

            extractedText += childNode.textContent;
        }
    }

    return extractedText;
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

function removePromotionImageOnClick(e) {

    e.stopPropagation();

    const imageListItemElement = e.target.closest(`[name='${promotionImageListItemsName}']`);

    const imageElement = imageListItemElement.querySelector(`[name='${promotionImagesName}']`);

    const imageElementUrl = imageElement.src;

    removePromotionImage(imageListItemElement.id, imageElementUrl);
}

function removePromotionImage(imageListItemElementId, imageUrl) {

    const imageListItemElement = document.getElementById(imageListItemElementId);

    const imageElement = imageListItemElement.querySelector(`[name='${promotionImagesName}']`);

    const imageIndexAsString = imageElement.getAttribute(promotionEditorImageIndexAttribute);

    const imageIndex = parseInt(imageIndexAsString);

    const imageSpansInHtmlContent = document.querySelectorAll(`[name='${htmlContentImageSpanName}']`);

    let imageSpanInHtmlContent;

    for (const span of imageSpansInHtmlContent) {

        const imageIndexAttribute = span.getAttribute(htmlContentImageSpanImageIndexAttribute);

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

    const imageList = document.getElementById(promotionImagesListId);

    const remainingImageListItemElements = [...imageList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

    for (const remainingImageListItem of remainingImageListItemElements) {

        const remainingImageElement = remainingImageListItem.querySelector(`[name='${promotionImagesName}']`);

        const remainingImageIndexAsString = remainingImageElement.getAttribute(promotionEditorImageIndexAttribute);

        const remainingImageIndex = parseInt(remainingImageIndexAsString);

        if (remainingImageIndex > imageIndexAsString) {

            const newImageIndex = remainingImageIndex - 1;

            const matchingDeleteButtonId = getImageListItemDeleteButtonIdFromIndex(remainingImageIndex);

            remainingImageElement.setAttribute(promotionEditorImageIndexAttribute, newImageIndex);

            const newImageListItemId = getImageListItemIdFromIndex(newImageIndex);

            remainingImageListItem.id = newImageListItemId;

            matchingDeleteButtonId.id = getImageListItemDeleteButtonIdFromIndex(newImageIndex);
            remainingImageElement.id = getImageElementIdFromIndex(newImageIndex);
        }
    }

    for (const span of imageSpansInHtmlContent) {

        const imageIndexAttribute = span.getAttribute(htmlContentImageSpanImageIndexAttribute);
        const imageIdAttribute = span.getAttribute(htmlContentImageSpanImageIdAttribute);

        const currentImageIndex = parseInt(imageIndexAttribute);
        const currentImageId = parseInt(imageIdAttribute);

        if (currentImageIndex > imageIndex) {

            const newSpan = createImageVisibleRepresentation(currentImageIndex - 1, currentImageId);

            span.parentNode.replaceChild(newSpan, span);
        }
    }
}

async function openAddRelatedProductsToPromotionPopup(initialGroupId = null) {

    const response = await fetch(`api/components/promotionGroups/addRelatedProductsPopup?selectedPromotionGroupId=${productId}`,
    {
        method: "GET",
        headers: {
            "Accept": "application/html"
        }
    });

    if (!response.ok) return;

    const data = await response.text();

    const addRelatedProductsPopupContainer = document.getElementById(addRelatedProductsPopupContainerId);

    addRelatedProductsPopupContainer.innerHTML = data;

    const addRelatedProductsPopup = document.getElementById(addRelatedProductsPopupId);

    addRelatedProductsPopup.showModal();
}

async function addRelatedProductToPromotion(productId) {

    const response = await fetch(`api/components/promotionGroups/relatedProduct/${productId}`,
    {
        method: "GET",
        headers: {
            "Accept": "application/html"
        }
    });

    if (!response.ok) return;

    const data = await response.text();

    const promotionEditRelatedProductsTable = document.getElementById(promotionEditRelatedProductsTableId);

    promotionEditRelatedProductsTable.insertAdjacentHTML("beforeend", data);
}

function removePromotionEditRelatedProduct(promotionEditRelatedProductElementId) {

    const promotionEditRelatedProductElement = document.getElementById(promotionEditRelatedProductElementId);

    promotionEditRelatedProductElement.remove();
}

async function openPromotionGroupEditorPopup(id = null) {

    const promotionGroupEditorPopupHtml = await getPromotionGroupEditorPopupData(id);

    const promotionGroupEditorPopupContainer = document.getElementById(promotionGroupEditorPopupContainerId);

    promotionGroupEditorPopupContainer.innerHTML = promotionGroupEditorPopupHtml;

    const promotionGroupEditorPopup = document.getElementById(promotionGroupEditorPopupId);

    promotionGroupEditorPopup.showModal();
}

async function getPromotionGroupEditorPopupData(id = null) {

    let response;

    if (id == null) {

        response = await fetch("api/components/promotionGroups/groupEditorPopup/new", {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    else {
        response = await fetch(`api/components/promotionGroups/groupEditorPopup/${id}`, {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    return await response.text();
}

function changePromotionGroupLogo() {

    const logoImageInput = document.getElementById(promotionGroupLogoInputId);

    logoImageInput.addEventListener("change", onLogoImageInputChanged, { once: true });

    logoImageInput.click(); 
}

async function onLogoImageInputChanged(e) {

    const file = e.target.files[0];

    if (!file) return;

    const fileObjectUrl = URL.createObjectURL(file);

    const newImageElementHtml = await getPromotionGroupLogoImage(fileObjectUrl);

    if (promotionGroupImageToUpload != null) {

        URL.revokeObjectURL(promotionGroupImageToUpload.fileObjectUrl);
    }

    const promotionGroupLogoImageDisplay = document.getElementById(promotionGroupLogoImageDisplayId);

    promotionGroupLogoImageDisplay.innerHTML = newImageElementHtml;

    promotionGroupImageToUpload = {
        File: file,
        fileObjectUrl: fileObjectUrl
    };

     const promotionGroupLogoChangeButton = document.getElementById(promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Change Image";
}

async function getPromotionGroupLogoImage(imageUrl) {

    const promotionLogoGroupImageOptions = getPromotionGroupLogoImageOptions(imageUrl);

    const response = await fetch("api/components/promotionGroups/groupImages", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(promotionLogoGroupImageOptions)
    });

    if (!response.ok) return null;

    return await response.text();
}

function getPromotionGroupLogoImageOptions(imageUrl) {

    return {
        ImageUrl: imageUrl
    }
}

async function savePromotionGroup(id = null) {

    if (id == null) {
        await createPromotionGroup();

        return;
    }

    await updatePromotionGroup(id);
}

async function createPromotionGroup() {

    const createRequest = getPromotionGroupCreateRequestFromCurrentData();

    if (createRequest == null) return;

    const response = await fetch("api/components/promotionGroups/createGroup", {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: createRequest
    });

    if (!response.ok) return;

    const newPromotionIdAsString = await response.text();

    const newPromotionId = parseInt(newPromotionIdAsString);

    openPromotionGroupEditorPopup(newPromotionId);
}

async function updatePromotionGroup(id) {

    const updateRequest = getPromotionGroupUpdateRequestFromCurrentData(id);

    if (updateRequest == null) return;

    const response = await fetch("api/components/promotionGroups/updateGroup", {
        method: "PUT",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: updateRequest
    });

    if (!response.ok) return;

    await openPromotionGroupEditorPopup(id);
}

function getPromotionGroupCreateRequestFromCurrentData() {

    const promotionGroupNameInput = document.getElementById(promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(promotionGroupDisplayOrderInputId);

    const formData = new FormData();

    const promotionGroupName = promotionGroupNameInput.value;
    const promotionGroupDisplayOrder = getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);

    formData.append("Name", promotionGroupName);
    formData.append("DisplayOrder", promotionGroupDisplayOrder);

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.File;

        formData.append("LogoImage", blob);
    }

    return formData;
}

function getPromotionGroupUpdateRequestFromCurrentData(id) {

    const promotionGroupNameInput = document.getElementById(promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(promotionGroupDisplayOrderInputId);

    const formData = new FormData();

    const promotionGroupName = promotionGroupNameInput.value;
    const promotionGroupDisplayOrder = getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);

    formData.append("Id", id);
    formData.append("Name", promotionGroupName);
    formData.append("DisplayOrder", promotionGroupDisplayOrder);

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.File;

        formData.append("NewLogoImage", blob);
        formData.append("PreserveOldImage", false);
    }
    else {
        const promotionGroupEditLogoImageContainer = document.getElementById(promotionGroupEditLogoImageContainerId);

        if (!promotionGroupEditLogoImageContainer) {
            formData.append("PreserveOldImage", false);
        }
        else {
            formData.append("PreserveOldImage", true);
        }
    }

    return formData;
}

function removePromotionGroupImage() {

    const promotionGroupEditLogoImageContainer = document.getElementById(promotionGroupEditLogoImageContainerId);

    promotionGroupEditLogoImageContainer.remove();

    const promotionGroupLogoChangeButton = document.getElementById(promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Add Image";
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

function expandTextAreaToNeededHeight(textareaId)
{
    const textarea = document.getElementById(textareaId);

    textarea.style.height = 'auto';

    textarea.style.height = (textarea.scrollHeight + 1.6) + 'px';
}

function getNumberOrNull(numberValue) {
    return typeof numberValue === "number" ? numberValue : null;
}

function getIntegerOrNullFromString(stringValue) {

    if (stringValue == null || stringValue === "") return null;

    var output = null;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}
