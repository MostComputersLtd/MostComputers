import * as validationMessages from "../../../Validation/ValidationMessages.js"
import * as validationVisualElements from "../../../Validation/ValidationVisualElements.js"
import * as dataStore from "./DataStore.js";
import * as editorChanges from "./EditorChanges.js";

const listId = "singleEditorProductDocumentList";

const productDocumentPopupContainerId = "singleEditorProductDocumentPopupContainer";

const itemName = "productDocumentItem";

const descriptionName = "productDocumentDescription"
const showPopupButtonName = "productDocumentShowPopupButton";
const saveUrlButtonName = "productDocumentSaveUrlButton";
const removeButtonName = "productDocumentRemoveButton"
const jsonDataName = "productDocumentItemData";

const validationMessagesListName = "productDocumentValidationMessagesList";

const showPopupButtonProductIdAttribute = "data-product-id";
const showPopupButtonBrowserFileUrlAttribute = "data-browser-file-url";
const saveUrlButtonDocumentIdAttribute = "data-document-id";
const saveUrlButtonDocumentUrlAttribute = "data-document-url";

const fileFormFieldName = "previewUrl";
const filePreviewUrlFormFieldName = "previewUrl";
const fileProductIdFormFieldName = "productId";

const endIndexCharacter = ']';
const documentRequestsStartString = 'DocumentRequests[';

const uploadedFiles = new Map();

export function initEvents(dialogElement) {

    const productDocumentItems = dialogElement.querySelectorAll(`[name='${itemName}']`);

    for (const productDocumentItem of productDocumentItems) {

        initProductDocumentItemEvents(productDocumentItem);
    }
}

function initProductDocumentItemEvents(productDocumentItem) {

    const productDocumentDescription = productDocumentItem.querySelector(`[name='${descriptionName}']`);
    const productDocumentShowPopupButton = productDocumentItem.querySelector(`[name='${showPopupButtonName}']`);
    const productDocumentSaveUrlButton = productDocumentItem.querySelector(`[name='${saveUrlButtonName}']`);
    const productDocumentRemoveButton = productDocumentItem.querySelector(`[name='${removeButtonName}']`);

    productDocumentDescription.addEventListener("change", onProductDocumentDescriptionChanged);
    productDocumentShowPopupButton.addEventListener("click", onShowPopupButtonClicked);

    if (productDocumentSaveUrlButton) {

        productDocumentSaveUrlButton.addEventListener("click", onSaveUrlButtonClickAsync);
    }

    productDocumentRemoveButton.addEventListener("click", onRemoveButtonClicked);
}

function getProductDocumentItems() {

    const productDocumentList = document.getElementById(listId);

    return [...productDocumentList.querySelectorAll(`[name='${itemName}']`)];
}

export function getProductDocumentRequests() {

    const productDocumentList = document.getElementById(listId);

    const productDocumentItems = productDocumentList.querySelectorAll(`[name='${itemName}']`);

    const itemDatas = [];

    for (const productDocumentItem of productDocumentItems) {

        const data = getProductDocumentItemRequest(productDocumentItem);

        itemDatas.push(data);
    }

    return itemDatas;
}

function getProductDocumentItemRequest(item) {

    const productDocumentDescription = item.querySelector(`[name='${descriptionName}']`);

    const productDocumentSaveUrlButton = item.querySelector(`[name='${saveUrlButtonName}']`);

    const response = {
        description: productDocumentDescription.value,
    }

    if (productDocumentSaveUrlButton) {

        const id = productDocumentSaveUrlButton.getAttribute(saveUrlButtonDocumentIdAttribute);

        response.id = parseInt(id);

        return response;
    }

    response.editorId = item.id,
    response.file = uploadedFiles.get(item.id);

    return response;
}

export function clearData() {

    const productDocumentList = document.getElementById(listId);

    const productDocumentItems = productDocumentList.querySelectorAll(`[name='${itemName}']`);

    for (const productDocumentItem of productDocumentItems) {

        if (uploadedFiles.has(productDocumentItem.id)) {

            const productDocumentShowPopupButton = productDocumentItem.querySelector(`[name='${showPopupButtonName}']`);

            const browserFileUrl = productDocumentShowPopupButton.getAttribute(showPopupButtonBrowserFileUrlAttribute);

            URL.revokeObjectURL(browserFileUrl);
        }
    }

    uploadedFiles.clear();
}

export async function addFileToProductDocumentsAsync(blob, productId) {

    if (blob.size === 0) {

        return false;
    }

    if (blob.type !== "application/pdf") {

        return false;
    }

    const previewUrl = URL.createObjectURL(blob);

    const formData = new FormData();

    formData.append(fileFormFieldName, blob);
    formData.append(filePreviewUrlFormFieldName, previewUrl);
    formData.append(fileProductIdFormFieldName, productId);

    const url = "api/components/productEditor/single/productDocument/add";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Accept": "application/json"
        },
        body: formData
    });

    if (!response.ok) {
        return false;
    }

    const data = await response.text();

    const success = addNewProductDocumentFromElementHtml(data, blob);

    return success;
}

function addNewProductDocumentFromElementHtml(productDocumentHtml, blob) {

    const productDocumentList = document.getElementById(listId);

    const productDocumentItems = productDocumentList.children;

    let productDocumentItem;

    if (productDocumentItems.length === 0) {

        productDocumentList.insertAdjacentHTML("afterbegin", productDocumentHtml);

        productDocumentItem = productDocumentList.firstElementChild;
    }
    else {

        const lastItem = productDocumentItems[productDocumentItems.length - 1];

        lastItem.insertAdjacentHTML("afterend", productDocumentHtml);

        productDocumentItem = lastItem.nextElementSibling;
    }

    const jsonDataScriptElement = productDocumentItem.querySelector(`[name='${jsonDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductDocumentItemEvents(productDocumentItem);

    const success = dataStore.addProductDocumentAndSave(jsonData.editorId, jsonData);

    uploadedFiles.set(jsonData.editorId, blob);

    editorChanges.setDocumentChanged(jsonData.editorId, editorChanges.ItemChangeState.Added);

    return success;
}

export function displayValidationMessages(validationErrors) {

    const productDocumentList = document.getElementById(listId);

    const items = [...productDocumentList.querySelectorAll(`[name='${itemName}']`)];

    for (let i = 0; i < items.length; i++) {

        const item = items[i];

        const validationMessagesList = item.querySelector(`[name='${validationMessagesListName}']`);

        validationVisualElements.clearValidationMessages(validationMessagesList);
    }

    const documentValidationMessages = new Map();

    for (const [name, _] of Object.entries(validationErrors)) {

        if (!name.startsWith(documentRequestsStartString)) continue;

        const startIndex = documentRequestsStartString.length;
        const endIndex = name.indexOf(endIndexCharacter, startIndex);

        const indexAsString = name.substring(startIndex, endIndex);
        const index = parseInt(indexAsString);

        if (!documentValidationMessages.has(index)) {

            documentValidationMessages.set(index, []);
        }

        const messagesForProperty = documentValidationMessages.get(index);

        messagesForProperty.push(validationErrors[name]); 
    }

    for (const [index, validationErrorsForProperty] of documentValidationMessages) {

        const item = items[index];

        const validationMessagesList = item.querySelector(`[name='${validationMessagesListName}']`);

        validationVisualElements.displayValidationMessages(
            validationErrorsForProperty,
            validationMessagesList,
            validationMessages.getDefaultValidationMessage)
    }
}

function onProductDocumentDescriptionChanged(e) {

    const productDocumentDescription = e.currentTarget;

    const productDocumentItem = productDocumentDescription.closest(`[name='${itemName}']`);

    const editorId = productDocumentItem.id;

    dataStore.updateProductDocumentAndSave(editorId, productDocument =>
    {
        productDocument.description = productDocumentDescription.value;
    });

    editorChanges.setDocumentChanged(editorId, editorChanges.ItemChangeState.Updated);
}

async function onShowPopupButtonClicked(e) {

    const showPopupButton = e.currentTarget;

    const productId = showPopupButton.getAttribute(showPopupButtonProductIdAttribute);
    const browserFileUrl = showPopupButton.getAttribute(showPopupButtonBrowserFileUrlAttribute);

    await showProductDocumentPopup(productId, browserFileUrl);
}

async function showProductDocumentPopup(productId, browserFileUrl) {

    const url = `api/components/productEditor/single/productDocument/showPopup/${productId}`;

    const body = {
        fileUrl: browserFileUrl
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) return;

    const data = await response.text();

    const productDocumentPopupContainer = document.getElementById(productDocumentPopupContainerId);

    productDocumentPopupContainer.innerHTML = data;

    const dialog = productDocumentPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function onSaveUrlButtonClickAsync(e) {

    const documentUrl = e.currentTarget.getAttribute(saveUrlButtonDocumentUrlAttribute);

    await navigator.clipboard.writeText(documentUrl);
}

function onRemoveButtonClicked(e) {

    const productDocumentItem = e.currentTarget.closest(`[name='${itemName}']`);

    const editorId = productDocumentItem.id;

    uploadedFiles.delete(editorId);

    dataStore.removeProductDocumentAndSave(editorId);

    editorChanges.setDocumentChanged(editorId, editorChanges.ItemChangeState.Deleted);

    productDocumentItem.remove();
}
