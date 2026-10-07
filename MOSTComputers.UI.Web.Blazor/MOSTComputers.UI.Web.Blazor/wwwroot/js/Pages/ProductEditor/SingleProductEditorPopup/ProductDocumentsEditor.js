import * as common from "./Common.js";
import * as dataStore from "./DataStore.js";
import * as editorChanges from "./EditorChanges.js";

const productDocumentItemName = "productDocumentItem";

const productDocumentDescriptionName = "productDocumentDescription"
const productDocumentShowPopupButtonName = "productDocumentShowPopupButton";
const productDocumentSaveUrlButtonName = "productDocumentSaveUrlButton";
const productDocumentRemoveButtonName = "productDocumentRemoveButton"

const productDocumentShowPopupButtonProductIdAttribute = "data-product-id";
const productDocumentShowPopupButtonDocumentIdAttribute = "data-document-id";
const productDocumentSaveUrlButtonDocumentIdAttribute = "data-document-id";

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
    const documentId = e.currentTarget.getAttribute(productDocumentShowPopupButtonDocumentIdAttribute);

    if (!documentId || isNaN(parseInt(documentId))) {
        return;
    }

    await showProductDocumentPopup(productId, documentId);
}

async function showProductDocumentPopup(productId, documentId) {

    const url = `api/components/productEditor/single/productDocumentPopup/${productId}/${documentId}`;

    const response = await fetch(url, {
        method: "GET",
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
