import * as common from "./Common.js";
import * as imageEditor from "./ImageEditor.js";
import * as htmlEditor from "./HtmlEditor.js";
import * as relatedProducts from "./RelatedProducts.js";
import * as promotionGroupEditor from "./PromotionGroupEditor.js";
import * as promotionEditorRelatedProducts from "./PromotionEditorRelatedProducts.js";

const serverImageRepresentationStart = "|/imageStart/%|";
const serverImageRepresentationEnd = "|/imageEnd/%|";

const serverImageIndexPrefix = "index=";

export async function openPromotionEditorForPromotion(id = null) {

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

    const promotionEditPanelContainer = document.getElementById(common.promotionEditPanelContainerId);

    promotionEditPanelContainer.innerHTML = promotionEditorHtml;

    const savePromotionButton = document.getElementById(common.savePromotionButtonId);

    savePromotionButton.addEventListener("click", savePromotion);

    const editPromotionGroupButton = document.getElementById(common.editPromotionGroupButtonId);

    let promotionGroupId = null;

    if (editPromotionGroupButton) {

        const promotionGroupIdAttribute = editPromotionGroupButton.getAttribute(common.promotionEditorPromotionGroupIdAttribute);

        promotionGroupId = common.getIntegerOrNullFromString(promotionGroupIdAttribute);

        editPromotionGroupButton.addEventListener("click", () => promotionGroupEditor.openPromotionGroupEditorPopup(promotionGroupId));
    }

    const resetPromotionGroupEditorButton = document.getElementById(common.resetPromotionGroupEditorButtonId);

    resetPromotionGroupEditorButton.addEventListener("click", () => openPromotionEditorForPromotion(id));

    imageEditor.resetImageData();

    const promotionImagesAddFileButton = document.getElementById(common.promotionImagesAddFileButtonId);

    promotionImagesAddFileButton.addEventListener("click", imageEditor.addNewImageToEditor);

    const promotionHtml = document.getElementById(common.promotionHtmlId);

    promotionHtml.addEventListener("input", () => htmlEditor.expandTextAreaToNeededHeight(common.promotionHtmlId));

    htmlEditor.expandTextAreaToNeededHeight(common.promotionHtmlId);

    htmlEditor.addEventsToHtmlContent();

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = [...promotionEditRelatedProductsTable.querySelectorAll("tr")];

    for (const relatedProductElement of relatedProductElements) {

        promotionEditorRelatedProducts.attachEvents(relatedProductElement);
    }

    relatedProducts.setExistingRelatedProductIdsFromCurrentData();

    const relatedProductsPopupOpenButton = document.getElementById(common.relatedProductsPopupOpenButtonId);

    relatedProductsPopupOpenButton.addEventListener("click", () => relatedProducts.openAddRelatedProductsToPromotionPopup(promotionGroupId))
}

export async function savePromotion() {

    const promotionEditPanel = document.getElementById(common.promotionEditPanelId);

    const editorCurrentId = promotionEditPanel.getAttribute(common.promotionEditorPromotionIdAttribute);

    const savePromotionButton = document.getElementById(common.savePromotionButtonId);

    savePromotionButton.classList.add(common.loadingClass);

    try {
        if (editorCurrentId === "") {

            await createPromotion();

            return;
        }

        await updatePromotion();
    }
    finally {
        savePromotionButton.classList.remove(common.loadingClass);
    }
}

async function createPromotion() {

    const createRequest = getPromotionCreateRequestFromCurrentData();

    if (createRequest == null) return;

    const response = await fetch("api/components/promotionGroups/create", {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.getElementById(common.antiforgeryTokenInputId).value
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
    const editor = document.getElementById(common.promotionEditPanelId);

    if (!editor) return null;

    const promotionName = document.getElementById(common.promotionNameId);
    const promotionHtmlContent = document.getElementById(common.promotionHtmlId);
    const promotionStartDate = document.getElementById(common.promotionStartDateId);
    const promotionExpirationDate = document.getElementById(common.promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(common.promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(common.promotionGroupSelectId);
    const promotionDisabled = document.getElementById(common.promotionDisabledId);
    const promotionRestricted = document.getElementById(common.promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(common.promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(common.promotionDefaultPriorityId);

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = [...promotionEditRelatedProductsTable.querySelectorAll("tr")];

    const formData = new FormData();

    formData.append("Name", promotionName.value);

    const groupId = common.getIntegerOrNullFromString(promotionGroupSelect.value);

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

    const displayOrder = common.getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {

        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = common.getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {

        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    const promotionEditorImagesToUpload = imageEditor.getImagesToUpload();

    for (let i = 0; i < promotionEditorImagesToUpload.length; i++)
    {
        const imageObject = promotionEditorImagesToUpload[i];

        const blob = imageObject.File;

        formData.append(`PromotionImageCreateRequests[${i}]`, blob);
    }
 
    for (let i = 0; i < relatedProductElements.length; i++) {

        const relatedProductElement = relatedProductElements[i];

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        formData.append(`RelatedProductIds[${i}]`, relatedProductId);
    }

    return formData;
}

async function updatePromotion() {

    const editor = document.getElementById(common.promotionEditPanelId);

    if (!editor) return;

    const promotionIdAsString = editor.getAttribute(common.promotionEditorPromotionIdAttribute);

    const promotionId = common.getIntegerOrNullFromString(promotionIdAsString);

    if (promotionId == null || isNaN(promotionId)) return;

    const updateRequest = getPromotionUpdateRequestFromCurrentData(promotionId);

    const response = await fetch("api/components/promotionGroups/update", {
        method: "PUT",
        headers: {
            "RequestVerificationToken": document.getElementById(common.antiforgeryTokenInputId).value
        },
        body: updateRequest
    });

    if (!response.ok) return;

    await openPromotionEditorForPromotion(promotionId);
}

function getPromotionUpdateRequestFromCurrentData(promotionId)
{
    const promotionName = document.getElementById(common.promotionNameId);
    const promotionHtmlContent = document.getElementById(common.promotionHtmlId);
    const promotionStartDate = document.getElementById(common.promotionStartDateId);
    const promotionExpirationDate = document.getElementById(common.promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(common.promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(common.promotionGroupSelectId);
    const promotionDisabled = document.getElementById(common.promotionDisabledId);
    const promotionRestricted = document.getElementById(common.promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(common.promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(common.promotionDefaultPriorityId);

    const imageList = document.getElementById(common.promotionImagesListId);

    const imageElements = [...imageList.querySelectorAll(`[name='${common.promotionImagesName}']`)];

    const promotionEditRelatedProductsTable = document.getElementById(common.promotionEditRelatedProductsTableId);

    const relatedProductElements = [...promotionEditRelatedProductsTable.querySelectorAll("tr")];

    const formData = new FormData();

    formData.append("Id", promotionId);
    formData.append("Name", promotionName.value);

    const groupId = common.getIntegerOrNullFromString(promotionGroupSelect.value);

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

    const displayOrder = common.getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {
        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = common.getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {
        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    for (let i = 0; i < imageElements.length; i++) {

        const imageElement = imageElements[i];

        const existingImageId = imageElement.getAttribute(common.promotionEditorImageIdAttribute);
        const imageIndex = imageElement.getAttribute(common.promotionEditorImageIndexAttribute);

        if (existingImageId == null || existingImageId == '') {

            const promotionEditorImagesToUpload = imageEditor.getImagesToUpload();

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

    for (let i = 0; i < relatedProductElements.length; i++) {

        const relatedProductElement = relatedProductElements[i];

        const relatedProductIdAsString = relatedProductElement.getAttribute(common.promotionRelatedProductIdAttribute);

        const relatedProductId = common.getIntegerOrNullFromString(relatedProductIdAsString);

        formData.append(`RelatedProductIds[${i}]`, relatedProductId);
    }

    return formData;
}

function replaceImageElementsWithTextualRepresentationsInHtmlContent(htmlContentEditor) {

    const htmlContentEditorClone = htmlContentEditor.cloneNode(true);

    const imageReferencesInHtmlContent = [...htmlContentEditorClone.querySelectorAll(`[name='${common.htmlContentImageSpanName}']`)];

    for (const imageReferenceElement of imageReferencesInHtmlContent) {

        const imageIndex = imageReferenceElement.getAttribute(common.htmlContentImageSpanImageIndexAttribute);

        const textualRepresentationForServer = createImageRepresentationForServer(imageIndex);

        const textNodeWithRepresentation = document.createTextNode(textualRepresentationForServer);

        imageReferenceElement.replaceWith(textNodeWithRepresentation);
    }

    return htmlContentEditorClone.textContent;
}

function createImageRepresentationForServer(imageIndex) {

    let output = serverImageRepresentationStart;

    output += serverImageIndexPrefix + imageIndex;

    output += serverImageRepresentationEnd;

    return output;
}