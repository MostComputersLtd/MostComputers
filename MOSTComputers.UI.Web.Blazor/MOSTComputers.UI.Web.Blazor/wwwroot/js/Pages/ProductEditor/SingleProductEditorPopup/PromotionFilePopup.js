import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as popup from "../../../Primitives/Popup.js";
import * as dataStore from "./DataStore.js";
import * as editorChanges from "./EditorChanges.js";
import * as singlePromotionFilePopup from "./SinglePromotionFilePopup.js";
import * as productImagesEditor from "./ProductImagesEditor.js";

const addButtonId = "promotionProductFileEditorAddButton";

const itemTableId = "promotionProductFileEditorTable";

const singleEditorPopupContainerId = "singlePromotionProductEditorPopupContainer";

const editorItemName = "promotionProductFileEditorItem";

const editorItemEditButtonName = "promotionProductFileEditorEditButton"
const editorItemDeleteButtonName = "promotionProductFileEditorDeleteButton";

const readonlyEditorItemEditButtonPromotionProductFileIdAttribute = "data-promotion-product-file-id";

const singleEditorPopupSaveButtonAddText = "Add";
const singleEditorPopupSaveButtonUpdateText = "Save";

const promotionProductFiles = [];

export function initEventsForReadonlyInstance(dialogElement) {

    const editorItems = dialogElement.querySelectorAll(`[name='${editorItemName}']`);

    for (const editorItem of editorItems) {

        const editorButton = editorItem.querySelector(`[name='${editorItemEditButtonName}']`);

        editorButton.addEventListener("click", onReadonlyEditorItemEditButtonClicked);
    }

    popup.attachCloseEvents(dialogElement);
}

export function initEvents(dialogElement, onAddCallback, onUpdateCallback, onDeleteCallback) {

    const addButton = document.getElementById(addButtonId);

    addButton.addEventListener("click", onAddPromotionProductFileButtonClicked);

    const editorItems = dialogElement.querySelectorAll(`[name='${editorItemName}']`);

    for (const editorItem of editorItems) {

        initPromotionProductFileItemEvents(editorItem);
    }

    dialogElement.__onAddCallback = onAddCallback;
    dialogElement.__onUpdateCallback = onUpdateCallback;
    dialogElement.__onDeleteCallback = onDeleteCallback;

    popup.attachCloseEvents(dialogElement);
}

function initPromotionProductFileItemEvents(editorItem) {

    const editorButton = editorItem.querySelector(`[name='${editorItemEditButtonName}']`);
    const deleteButton = editorItem.querySelector(`[name='${editorItemDeleteButtonName}']`);

    editorButton.addEventListener("click", onEditorItemEditButtonClicked);
    deleteButton.addEventListener("click", onEditorItemDeleteButtonClicked);
}

export function initData(data) {

    for (const jsonItem of data) {

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

    const request = {
        existingEditorId: null,
        id: null,
        promotionFileId: null,
        validFrom: null,
        validTo: null,
        active: true,
        saveButtonText: singleEditorPopupSaveButtonAddText,
        isInImage: false,
    };

    showSinglePromotionProductFileEditorPopup(request);
}

async function onReadonlyEditorItemEditButtonClicked(e) {

    const promotionProductFileId = e.currentTarget.getAttribute(readonlyEditorItemEditButtonPromotionProductFileIdAttribute);

    showReadonlySinglePromotionProductFileEditorPopup(promotionProductFileId);
}

async function onEditorItemEditButtonClicked(e) {

    const editorItem = e.currentTarget.closest(`[name='${editorItemName}']`);

    const editorId = editorItem.id;

    const promotionData = promotionProductFiles.find(x => x.editorId == editorId);

    const relatedImageItem = productImagesEditor.getProductImageItemRelatedToPromotionFile(editorId);

    const request = {
        existingEditorId: editorId,
        id: promotionData.id,
        promotionFileId: promotionData.promotionFileId,
        validFrom: promotionData.validFrom,
        validTo: promotionData.validTo,
        active: promotionData.active,
        saveButtonText: singleEditorPopupSaveButtonUpdateText,
        isInImage: relatedImageItem != null,
    };

    await showSinglePromotionProductFileEditorPopup(request);
}

async function showReadonlySinglePromotionProductFileEditorPopup(promotionProductFileId) {

    const url = `api/components/productEditor/singlePromotionFilePopup/${promotionProductFileId}`;

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "text/html, application/problem+json"
        }
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.text();

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

    const data = await response.text();

    const singleEditorPopupContainer = document.getElementById(singleEditorPopupContainerId);

    singleEditorPopupContainer.innerHTML = data;

    const dialog = singleEditorPopupContainer.querySelector("dialog");

    singlePromotionFilePopup.initEvents(onSingleEditorPopupSaveButtonClicked);

    const initFileResponse = singlePromotionFilePopup.initializePromotionFile();

    if (initFileResponse === fetchWithAuthRedirect.AuthRedirected) return;

    dialog.showModal();
}

async function onSingleEditorPopupSaveButtonClicked() {

    const popupData = singlePromotionFilePopup.getPopupData();

    if (popupData.selectedFileId == null) return;

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

        itemTableBody.insertAdjacentHTML("beforeend", newItemElementData);

        initPromotionProductFileItemEvents(itemTableBody.lastElementChild);

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

        singlePromotionFilePopup.changeSaveButtonText(singleEditorPopupSaveButtonUpdateText);

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

    existingEditorItem.outerHTML = existingItemElementData;

    const newEditorItem = document.getElementById(editorId);

    initPromotionProductFileItemEvents(newEditorItem);

    existingPromotionProductFile.promotionFileId = popupData.selectedFileId;
    existingPromotionProductFile.active = popupData.active;
    existingPromotionProductFile.validFrom = popupData.validFrom;
    existingPromotionProductFile.validTo = popupData.validTo;

    dataStore.updatePromotionProductFileAndSave(editorId, promotionProductFile => {
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

    const promotionProductFileIndex = promotionProductFiles.findIndex(x => x.editorId == editorId);

    promotionProductFiles.splice(promotionProductFileIndex, 1);

    dataStore.removePromotionProductFileAndSave(editorId);

    editorChanges.setPromotionProductFileChanged(editorId, editorChanges.ItemChangeState.Deleted);

    const editorItem = document.getElementById(editorId);

    if (editorItem) {

        editorItem.remove();
    }
}
