import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as dataStore from "./DataStore.js";
import * as editorChanges from "./EditorChanges.js";
import * as singlePromotionFilePopup from "./SinglePromotionFilePopup.js";
import * as productImagesEditor from "./ProductImagesEditor.js";

const addButtonId = "promotionProductFileEditorAddButton";

const itemTableId = "promotionProductFileEditorItemTable";

const singleEditorPopupContainerId = "singlePromotionProductEditorPopupContainer";

const editorItemName = "promotionProductFileEditorItem";

const editorItemEditButtonName = "promotionProductFileEditorEditButton"
const editorItemDeleteButtonName = "promotionProductFileEditorDeleteButton";

const readonlyEditorItemEditButtonPromotionProductFileIdAttribute = "data-promotion-product-file-id";

const promotionProductFiles = [];

export function initEventsForReadonlyInstance(dialogElement) {

    const editorItems = dialogElement.querySelectorAll(`[name='${editorItemName}']`);

    for (const editorItem of editorItems) {

        const editorButton = editorItem.querySelector(`[name='${editorItemEditButtonName}']`);

        editorButton.addEventListener("click", onReadonlyEditorItemEditButtonClicked);
    }
}

export function initEvents(dialogElement, onAddCallback, onUpdateCallback, onDeleteCallback) {

    const addButton = document.getElementById(addButtonId);

    addButton.addEventListener("click", onAddPromotionProductFileButtonClicked);

    const editorItems = dialogElement.querySelectorAll(`[name='${editorItemName}']`);

    for (const editorItem of editorItems) {

        const editorButton = editorItem.querySelector(`[name='${editorItemEditButtonName}']`);

        editorButton.addEventListener("click", onEditorItemEditButtonClicked);

        const deleteButton = editorItem.querySelector(`[name='${editorItemDeleteButtonName}']`);

        deleteButton.addEventListener("click", onEditorItemDeleteButtonClicked);
    }

    dialogElement.__onAddCallback = onAddCallback;
    dialogElement.__onUpdateCallback = onUpdateCallback;
    dialogElement.__onDeleteCallback = onDeleteCallback;
}

export function initData(data) {

    for (const jsonItem in data) {

        promotionProductFiles.push(jsonItem);
    }
}

export function clearInitializedData() {

    promotionProductFiles.length = 0;
}

export function getPromotionProductFileRequests() {

    return promotionProductFiles;
}

async function getEditorItem(request) {

    const url = "api/components/productEditor/single/promotionFile/item";

    return await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(request)
    });
}

async function onAddPromotionProductFileButtonClicked() {

    const saveButtonText = "Add";

    const request = {
        existingEditorId: null,
        id: null,
        promotionFileId: null,
        validFrom: null,
        validTo: null,
        active: null,
        saveButtonText: saveButtonText,
        isInImage: false,
    };

    showSinglePromotionProductFileEditorPopup(request);
}

async function onReadonlyEditorItemEditButtonClicked(e) {

    const promotionProductFileId = e.currentTarget.getAttribute(readonlyEditorItemEditButtonPromotionProductFileIdAttribute);

    showReadonlySinglePromotionProductFileEditorPopup(promotionProductFileId);
}

async function onEditorItemEditButtonClicked(e) {

    const saveButtonText = "Save";

    const editorItem = e.currentTarget.closest(`[name='${editorItemName}']`);

    const editorId = editorItem.id;

    const promotionData = promotionProductFiles.find(x => x.editorId == editorId);

    const productImages = productImagesEditor.getProductImagesRequestData();

    const relatedImage = productImages.find(x =>
    {
        const itemType = x["$type"];

        return itemType == "promotionProductFile" && x.promotionFileEditorId == editorId;
    });

    const request = {
        existingEditorId: editorId,
        id: promotionData.id,
        promotionFileId: promotionData.promotionFileId,
        validFrom: promotionData.validFrom,
        validTo: promotionData.validTo,
        active: promotionData.active,
        saveButtonText: saveButtonText,
        isInImage: relatedImage != null,
    };

    await showSinglePromotionProductFileEditorPopup(request);
}

async function showReadonlySinglePromotionProductFileEditorPopup(promotionProductFileId) {

    const url = `api/components/productEditor/singlePromotionFilePopup/${promotionProductFileId}`;

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html, application/problem+json"
        },
        body: JSON.stringify(request)
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = response.text();

    const singleEditorPopupContainer = document.getElementById(singleEditorPopupContainerId);

    singleEditorPopupContainer.innerHTML = data;

    const dialog = singleEditorPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showSinglePromotionProductFileEditorPopup(request) {

    const url = "api/components/productEditor/single/promotionFile/single/showPopup";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(request)
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = response.text();

    const singleEditorPopupContainer = document.getElementById(singleEditorPopupContainerId);

    singleEditorPopupContainer.innerHTML = data;

    const dialog = singleEditorPopupContainer.querySelector("dialog");

    singlePromotionFilePopup.initEvents(onSingleEditorPopupSaveButtonClicked);
    singlePromotionFilePopup.initializePromotionFile();

    dialog.showModal();
}

async function onSingleEditorPopupSaveButtonClicked() {

    const popupData = singlePromotionFilePopup.getPopupData();

    const editorId = popupData.editorId;

    const existingPromotionProductFile = promotionProductFiles.find(x => x.editorId == editorId); 

    const newItemCallbackData = {
        editorId: editorId,
        shouldAddToImages: popupData.shouldAddToImages,
        selectedFileId: popupData.selectedFileId,
        validFrom: popupData.validFrom,
        validTo: popupData.validTo, 
        active: popupData.active,
    };

    const itemTable = document.getElementById(itemTableId);

    const itemTableBody = itemTable.querySelector("tbody");

    const editorPopup = itemTable.closest("dialog");

    if (existingPromotionProductFile == null) {

        const newItemRequest = {
            existingEditorId: editorId,
            id: null,
            promotionFileId: popupData.selectedFileId,
            active: popupData.active,
            validFrom: popupData.validFrom,
            validTo: popupData.validTo,
        };

        const newEditorData = {
            editorId: editorId,
            id: null,
            promotionFileId: popupData.selectedFileId,
            active: popupData.active,
            validFrom: popupData.validFrom,
            validTo: popupData.validTo,
        };

        const newItemResponse = await getEditorItem(newItemRequest);

        const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(newItemResponse);

        if (!didNotRedirect || !newItemResponse.ok) return;

        const newItemElementData = await newItemResponse.text();

        itemTableBody.appendChild(newItemElementData);

        promotionProductFiles.push(newEditorData);

        const newRecoveryData = {
            editorId: editorId,
            id: null,
            promotionFileId: popupData.selectedFileId,
            validFrom: popupData.validFrom,
            validTo: popupData.validTo, 
            active: popupData.active,
        };

        dataStore.addPromotionProductFileAndSave(editorId, newRecoveryData);

        editorChanges.setPromotionProductFileChanged(editorId, editorChanges.ItemChangeState.Added);

        editorPopup.__onAddCallback(newItemCallbackData);

        return;
    }

    const itemRequest = {
        existingEditorId: editorId,
        id: existingPromotionProductFile.id,
        promotionFileId: popupData.selectedFileId,
        active: popupData.active,
        validFrom: popupData.validFrom,
        validTo: popupData.validTo,
    };

    const itemResponse = await getEditorItem(itemRequest);

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(itemResponse);

    if (!didNotRedirect || !itemResponse.ok) return;

    const existingItemElementData = await itemResponse.text();

    const existingEditorItem = document.getElementById(editorId);

    itemTableBody.replaceChild(existingItemElementData, existingEditorItem);

    existingPromotionProductFile.promotionFileId = popupData.selectedFileId;
    existingPromotionProductFile.active = popupData.active;
    existingPromotionProductFile.validFrom = popupData.validFrom;
    existingPromotionProductFile.validTo = popupData.validTo;

    dataStore.updatePromotionProductFileAndSave(editorId, promotionProductFile =>
    {
        promotionProductFile.promotionFileId = popupData.selectedFileId;
        promotionProductFile.active = popupData.active;
        promotionProductFile.validFrom = popupData.validFrom;
        promotionProductFile.validTo = popupData.validTo;
    });

    editorChanges.setPromotionProductFileChanged(editorId, editorChanges.ItemChangeState.Updated);

    editorPopup.__onUpdateCallback(newItemCallbackData);
}

function onEditorItemDeleteButtonClicked(e) {

    const editorItem = e.currentTarget.closest(`[name='${editorItemName}']`);

    const editorId = editorItem.id;

    const promotionProductFileIndex = promotionProductFiles.findIndex(x => x.editorId == editorId); 

    promotionProductFiles.splice(promotionProductFileIndex, 1);

    dataStore.removePromotionProductFileAndSave(editorId);

    editorChanges.setPromotionProductFileChanged(editorId, editorChanges.ItemChangeState.Deleted);

    const editorPopup = editorItem.closest("dialog");

    editorItem.remove();

    editorPopup.__onDeleteCallback(editorId);
}

export function onEditorItemRelatedImageDeleted(editorId) {

    const editorItem = document.getElementById(editorId);

    const promotionProductFileIndex = promotionProductFiles.findIndex(x => x.editorId == editorId); 

    promotionProductFiles.splice(promotionProductFileIndex, 1);

    dataStore.removePromotionProductFileAndSave(editorId);

    editorChanges.setPromotionProductFileChanged(editorId, editorChanges.ItemChangeState.Deleted);

    editorItem.remove();
}
