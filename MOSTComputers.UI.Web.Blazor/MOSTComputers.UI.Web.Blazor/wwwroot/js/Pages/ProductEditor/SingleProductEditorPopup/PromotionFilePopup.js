import * as dataStore from "./DataStore.js";
import * as singlePromotionFilePopup from "./SinglePromotionFilePopup.js";

const addButtonId = "promotionProductEditorAddButton";

const singleEditorPopupContainerId = "singlePromotionProductEditorPopupContainer";

const editorItemName = "promotionProductFileEditorItem";

const editorItemEditButtonName = "promotionProductFileEditorEditButton"
const editorItemDeleteButtonName = "promotionProductFileEditorDeleteButton";

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

async function onAddPromotionProductFileButtonClicked() {

    const saveButtonText = "Add";

    const request = {
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

async function onEditorItemEditButtonClicked(e) {

    const saveButtonText = "Save";

    const editorItem = e.currentTarget.closest(`[name='${editorItemName}']`);

    const editorId = editorItem.id;

    const promotionData = dataStore.loadPromotionProductFileData(editorId);

    const dataStoreData = dataStore.loadData();

    const relatedImage = dataStoreData.productImages.find(x => x.relatedPromotionFileEditorId == editorId);

    const request = {
        id: promotionData.id,
        promotionFileId: promotionData.fileInfoId,
        validFrom: promotionData.validFrom,
        validTo: promotionData.validTo,
        active: promotionData.active,
        saveButtonText: saveButtonText,
        isInImage: relatedImage != null,
    };

    showSinglePromotionProductFileEditorPopup(request);
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

    singlePromotionFilePopup.initEvents();
    singlePromotionFilePopup.initializePromotionFile();

    dialog.showModal();
}

async function onSingleEditorPopupSaveButtonClicked() {

    const popupData = singlePromotionFilePopup.getPopupData();

    const newItemData = {
        editorId: popupData.editorId,
        active: popupData.active,
        validFrom: popupData.validFrom,
        validTo: popupData.validTo,
        shouldAddToImages: popupData.shouldAddToImages,
        selectedFileId: popupData.selectedFileId,
    };
}

function onEditorItemDeleteButtonClicked(e) {

    const editorItem = e.currentTarget.closest(`[name='${editorItemName}']`);

    const editorId = editorItem.id;

    dataStore.removePromotionProductFileAndSave(editorId);

    const editorPopup = editorItem.closest("dialog");

    editorItem.remove();

    editorPopup.__onDeleteCallback(editorId);
}
