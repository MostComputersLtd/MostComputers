import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js"
import * as antiforgeryUtils from "../../Common/AntiforgeryUtils.js"
import * as singlePromotionFileEditorPopup from "./SinglePromotionFileEditorPopup.js"

const promotionFileEditorValidOnInputId = "promotionFileEditorValidOnInput";
const promotionFileEditorSearchInputId = "promotionFileEditorSearchInput";
const promotionFileEditorSearchButtonId = "promotionFileEditorSearchButton";
const promotionFileEditorAddButtonId = "promotionFileEditorAddButton";
const promotionFileEditorContainerId = "promotionFileEditorContainer";
const promotionFileSingleEditorPopupContainerId = "promotionFileSingleEditorPopupContainer";

const promotionFileEditorTableBodyId = "promotionFileEditorTableBody";

function getPromotionFileItemId(promotionFileId) {
    return `promotionFileItem-${promotionFileId}`;
}

const promotionFileEditorItemName = "promotionFileItem";
const promotionFileEditorItemEditButtonName = "promotionFileItemEditButton";
const promotionFileEditorItemDeleteButtonName = "promotionFileItemDeleteButton";

const promotionFileItemPromotionFileIdAttribute = "data-promotion-file-id";

const singlePromotionFileEditorSaveButtonSaveText = "Save";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

function initEvents() {

    const promotionFileEditorSearchButton = document.getElementById(promotionFileEditorSearchButtonId);
    const promotionFileEditorAddButton = document.getElementById(promotionFileEditorAddButtonId);

    promotionFileEditorSearchButton.addEventListener("click", searchPromotionFilesAsync)
    promotionFileEditorAddButton.addEventListener("click", onSinglePromotionFileEditorAddButtonClicked);
}

function initPromotionFilesTableEvents() {

    const promotionFileEditorTableBody = document.getElementById(promotionFileEditorTableBodyId);

    const promotionFileItems = promotionFileEditorTableBody.querySelectorAll(`[name='${promotionFileEditorItemName}']`);

    for (const promotionFileItem of promotionFileItems) {

        initPromotionFilesTableItemEvents(promotionFileItem);
    }
}

function initPromotionFilesTableItemEvents(promotionFileItem) {

    const editButton = promotionFileItem.querySelector(`[name='${promotionFileEditorItemEditButtonName}']`);
    const deleteButton = promotionFileItem.querySelector(`[name='${promotionFileEditorItemDeleteButtonName}']`);

    editButton.addEventListener("click", onShowSinglePromotionFileEditorPopupButtonClicked);
    deleteButton.addEventListener("click", onDeletePromotionFileButtonClicked);
}

async function searchPromotionFilesAsync() {

    const promotionFileEditorValidOnInput = document.getElementById(promotionFileEditorValidOnInputId);
    const promotionFileEditorSearchInput = document.getElementById(promotionFileEditorSearchInputId);

    let validOnDate = promotionFileEditorValidOnInput.value;

    if (!validOnDate) {

        validOnDate = null;
    }

    const body = {
        userInputString: promotionFileEditorSearchInput.value,
        validOnDate: validOnDate,
    };

    const url = "api/components/promotionFileEditor/search";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html, application/problem+json"
        },
        body: JSON.stringify(body)
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const data = await response.text();

    const promotionFileEditorContainer = document.getElementById(promotionFileEditorContainerId);

    promotionFileEditorContainer.innerHTML = data;

    initPromotionFilesTableEvents();
}

async function onSinglePromotionFileEditorAddButtonClicked() {

    const url = `api/components/promotionFileEditor/single/new`;

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "text/html, application/problem+json"
        }
    });

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const data = await response.text();

    const promotionFileSingleEditorPopupContainer = document.getElementById(promotionFileSingleEditorPopupContainerId);

    promotionFileSingleEditorPopupContainer.innerHTML = data;

    const dialog = promotionFileSingleEditorPopupContainer.querySelector("dialog");

    singlePromotionFileEditorPopup.initEvents(dialog, onSingleEditorPopupSaveButtonClicked);

    dialog.showModal();
}

async function onShowSinglePromotionFileEditorPopupButtonClicked(e) {

    const promotionFileEditorItem = e.currentTarget.closest(`[name='${promotionFileEditorItemName}']`);

    const promotionFileId = promotionFileEditorItem.getAttribute(promotionFileItemPromotionFileIdAttribute);

    const url = `api/components/promotionFileEditor/single/${promotionFileId}`;

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "text/html, application/problem+json"
        }
    });

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const data = await response.text();

    const promotionFileSingleEditorPopupContainer = document.getElementById(promotionFileSingleEditorPopupContainerId);

    promotionFileSingleEditorPopupContainer.innerHTML = data;

    const dialog = promotionFileSingleEditorPopupContainer.querySelector("dialog");

    singlePromotionFileEditorPopup.initEvents(dialog, onSingleEditorPopupSaveButtonClicked);

    dialog.showModal();
}

async function onSingleEditorPopupSaveButtonClicked() {

    const popupData = singlePromotionFileEditorPopup.getCurrentData();

    const idInPopupData = popupData.get(singlePromotionFileEditorPopup.idFormFileName);

    if (idInPopupData == null) {

        await createPromotionFileAsync(popupData);

        return;
    }

    await updatePromotionDataAsync(popupData);
}

async function createPromotionFileAsync(popupData) {

    const url = "api/components/promotionFileEditor/create";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Accept" : "text/html, application/problem+json",
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
        body: popupData
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const data = await response.text();

    const promotionFileEditorTableBody = document.getElementById(promotionFileEditorTableBodyId);

    singlePromotionFileEditorPopup.changeSaveButtonText(singlePromotionFileEditorSaveButtonSaveText);

    if (!promotionFileEditorTableBody) return;

    promotionFileEditorTableBody.insertAdjacentHTML("beforeend", data);

    initPromotionFilesTableItemEvents(promotionFileEditorTableBody.lastElementChild);
}

async function updatePromotionDataAsync(popupData) {

    const url = "api/components/promotionFileEditor/update";

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Accept" : "text/html, application/problem+json",
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
        body: popupData
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        const responseProblem = await response.json();

        if (responseProblem.type === "/problems/validation") {

            singlePromotionFileEditorPopup.clearValidationMessages();
            singlePromotionFileEditorPopup.displayValidationMessages(responseProblem);
        }

        return;
    }

    const data = await response.text();

    const idInPopupData = popupData.get(singlePromotionFileEditorPopup.idFormFileName);

    const existingPromotionFileItemId = getPromotionFileItemId(idInPopupData);

    const existingPromotionFileItem = document.getElementById(existingPromotionFileItemId);

    existingPromotionFileItem.outerHTML = data;

    const newPromotionFileItem = document.getElementById(existingPromotionFileItemId);

    initPromotionFilesTableItemEvents(newPromotionFileItem);
}

async function onDeletePromotionFileButtonClicked(e) {

    const promotionFileEditorItem = e.currentTarget.closest(`[name='${promotionFileEditorItemName}']`);

    const promotionFileId = promotionFileEditorItem.getAttribute(promotionFileItemPromotionFileIdAttribute);

    const url = `api/components/promotionFileEditor/delete/${promotionFileId}`;

    const response = await fetch(url, {
        method: "DELETE",
        headers: {
            "Accept" : "text/html, application/problem+json",
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    promotionFileEditorItem.remove();
}
