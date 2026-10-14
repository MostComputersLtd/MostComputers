import * as common from "./Common.js";
import * as dataStore from "./DataStore.js";
import * as editorChanges from "./EditorChanges.js";

const productDocumentListId = "singleEditorProductDocumentList";

const productDocumentItemName = "productDocumentItem";

const productDocumentDescriptionName = "productDocumentDescription"
const productDocumentShowPopupButtonName = "productDocumentShowPopupButton";
const productDocumentSaveUrlButtonName = "productDocumentSaveUrlButton";
const productDocumentRemoveButtonName = "productDocumentRemoveButton"
const productDocumentDataName = "productDocumentItemData";

const productDocumentShowPopupButtonProductIdAttribute = "data-product-id";
const productDocumentShowPopupButtonBrowserFileUrlAttribute = "data-browser-file-url";
const productDocumentSaveUrlButtonDocumentIdAttribute = "data-document-id";

const fileFormFieldName = "previewUrl";
const filePreviewUrlFormFieldName = "previewUrl";
const fileIndexFormFieldName = "index";
const fileProductIdFormFieldName = "productId";

export function initEvents(dialogElement) {

    const productDocumentItems = dialogElement.querySelectorAll(`[name='${productDocumentItemName}']`);

    for (const productDocumentItem of productDocumentItems) {

        initProductDocumentItemEvents(productDocumentItem);
    }
}

function initProductDocumentItemEvents(productDocumentItem) {

    const productDocumentDescription = productDocumentItem.querySelector(`[name='${productDocumentDescriptionName}']`);

    productDocumentDescription.addEventListener("change", onProductDocumentDescriptionChanged);

    const productDocumentShowPopupButton = productDocumentItem.querySelector(`[name='${productDocumentShowPopupButtonName}']`);

    productDocumentShowPopupButton.addEventListener("click", onShowPopupButtonClicked);

    const productDocumentSaveUrlButton = productDocumentItem.querySelector(`[name='${productDocumentSaveUrlButtonName}']`);

    if (productDocumentSaveUrlButton) {

        productDocumentSaveUrlButton.addEventListener("click", onSaveUrlButtonClickAsync);
    }

    const productDocumentRemoveButton = productDocumentItem.querySelector(`[name='${productDocumentRemoveButtonName}']`);

    productDocumentRemoveButton.addEventListener("click", onRemoveButtonClicked);
}

export async function addFileToProductDocumentsAsync(blob) {

    if (blob.size === 0) {

        return false;
    }

    if (!blob.type !== "application/pdf") {

        return false;
    }

    const previewUrl = URL.createObjectURL(blob);

    const dataStoreData = dataStore.loadData();

    const formData = new FormData();

    formData.append(fileFormFieldName, blob);
    formData.append(filePreviewUrlFormFieldName, previewUrl);
    formData.append(fileIndexFormFieldName, dataStoreData.productDocuments.length);
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

    const success = addNewProductDocumentFromElementHtml(data);

    return success;
}

function addNewProductDocumentFromElementHtml(productDocumentHtml) {

    const productDocumentList = document.getElementById(productDocumentListId);

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

    const jsonDataScriptElement = productDocumentItem.querySelector(`[name='${productDocumentDataName}']`);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    jsonDataScriptElement.remove();

    initProductDocumentItemEvents(productDocumentItem);

    const success = dataStore.addProductDocumentAndSave(jsonData.editorId, jsonData, file);

    editorChanges.setDocumentChanged(jsonData.editorId, editorChanges.ItemChangeState.Added);

    return success;
}

function onProductDocumentDescriptionChanged(e) {

    const productDocumentItem = e.currentTarget.closest(`[name='${productDocumentItemName}']`);

    const editorId = productDocumentItem.id;

    dataStore.updateProductDocumentAndSave(editorId, productDocument =>
    {
        productDocument.description = e.currentTarget.value;
    });

    editorChanges.setDocumentChanged(editorId, editorChanges.ItemChangeState.Updated);
}

async function onShowPopupButtonClicked(e) {

    const productId = e.currentTarget.getAttribute(productDocumentShowPopupButtonProductIdAttribute);
    const browserFileUrl = e.currentTarget.getAttribute(productDocumentShowPopupButtonBrowserFileUrlAttribute);

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
            "Accept": "application/json"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) return;

    const data = await response.text();

    const productDocumentPopupContainer = document.getElementById(common.productDocumentPopupContainerId);

    productDocumentPopupContainer.innerHTML = data;

    const dialog = productDocumentPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function onSaveUrlButtonClickAsync(e) {

    const documentId = e.currentTarget.getAttribute(productDocumentSaveUrlButtonDocumentIdAttribute);

    const documentUrl = `${location.origin}/api/documents/product/${documentId}`;

    await navigator.clipboard.writeText(documentUrl);
}

function onRemoveButtonClicked(e) {

    const productDocumentItem = e.currentTarget.target.closest(`[name='${productDocumentItemName}']`);

    const editorId = productDocumentItem.id;

    dataStore.removeProductDocumentAndSave(editorId);

    editorChanges.setDocumentChanged(editorId, editorChanges.ItemChangeState.Deleted);

    productDocumentItem.remove();
}
