import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as common from "./Common.js"
import * as dataStore from "./DataStore.js"
import * as propertiesEditor from "./PropertiesEditor.js"
import * as productDocumentsEditor from "./ProductDocumentsEditor.js"
import * as productImagesEditor from "./ProductImagesEditor.js"

const singleEditorProductNewStatusSelectId = "singleEditorProductNewStatusSelect";
const singleEditorAddImageFileButtonId = "singleEditorAddImageFileButton";
const singleEditorAddImageFileInputId = "singleEditorAddImageFileInput";
const singleEditorAddDocumentFileButtonId = "singleEditorAddDocumentFileButton";
const singleEditorAddDocumentFileInputId = "singleEditorAddDocumentFileInput";
const singleEditorAddImageFromClipboardFileButtonId = "singleEditorAddImageFromClipboardFileButton";

export function initEvents(dialogElement) {

    const singleEditorProductNewStatusSelect = document.getElementById(singleEditorProductNewStatusSelectId);

    singleEditorProductNewStatusSelect.addEventListener("change", onProductStatusSelectChangedAsync)

    const singleEditorAddImageFileButton = document.getElementById(singleEditorAddImageFileButtonId);

    singleEditorAddImageFileButton.addEventListener("click", onAddImageFileButtonClick);

    const singleEditorAddImageFileInput = document.getElementById(singleEditorAddImageFileInputId);

    singleEditorAddImageFileInput.addEventListener("change", onAddImageFileInputChangeAsync);

    const singleEditorAddDocumentFileButton = document.getElementById(singleEditorAddDocumentFileButtonId);

    singleEditorAddDocumentFileButton.addEventListener("click", onAddDocumentFileButtonClick);

    const singleEditorAddDocumentFileInput = document.getElementById(singleEditorAddDocumentFileInputId);

    singleEditorAddDocumentFileInput.addEventListener("change", onAddDocumentFileInputChangeAsync);

    const singleEditorAddImageFromClipboardFileButton = document.getElementById(singleEditorAddImageFromClipboardFileButtonId);

    singleEditorAddImageFromClipboardFileButton.addEventListener("click", onAddImageFromClipboardButtonClickAsync);

    propertiesEditor.initEvents(dialogElement);
    productDocumentsEditor.initEvents(dialogElement);
    productImagesEditor.initEvents(dialogElement);
}

export function registerData(jsonDataObject) {

    dataStore.saveData(jsonDataObject);
}

async function onProductStatusSelectChangedAsync(e) {

    const productStatus = e.currentTarget.value;

    const url = `api/components/productEditor/single/updateProductWorkStatus/${productId}/${productStatus}`;

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Accept": "application/json"
        },
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.json();

    const productNewStatusValue = data.productNewStatus;

    e.currentTarget.value = productNewStatusValue;
}

function onAddImageFileButtonClick() {

    const singleEditorAddImageFileInput = document.getElementById(singleEditorAddImageFileInputId);

    singleEditorAddImageFileInput.click();
}

async function onAddImageFileInputChangeAsync(e) {

    const singleEditorAddImageFileInput = e.currentTarget;

    if (singleEditorAddImageFileInput.files.length === 0) {

        return;
    }

    const file = singleEditorAddImageFileInput.files.item(0);

    const success = await productImagesEditor.addFileToImagesAsync(file);

    if (!success) {
    }
}

function onAddDocumentFileButtonClick() {

    const singleEditorAddDocumentFileInput = document.getElementById(singleEditorAddDocumentFileInputId);

    singleEditorAddDocumentFileInput.click();
}

async function onAddDocumentFileInputChangeAsync(e) {

    const singleEditorAddImageFileInput = e.currentTarget;

    if (singleEditorAddImageFileInput.files.length === 0) {

        return;
    }

    const file = singleEditorAddImageFileInput.files.item(0);
}

async function onAddImageFromClipboardButtonClickAsync() {

    const items = await navigator.clipboard.read();

    let blob;

    for (const item of items) {

        const imageType = item.types.find(type => type.startsWith("image/"));

        if (imageType) {

            blob = await item.getType(imageType);

            break;
        }
    }

    if (!blob) {

        return;
    }

    const success = await productImagesEditor.addFileToImagesAsync(blob);

    if (!success) {
    }
}

async function showProductDataPopup(productId) {

    const url = `api/components/productEditor/single/productDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productDataPopupContainer = document.getElementById(common.productDataPopupContainerId);

    productDataPopupContainer.innerHTML = data;

    const dialog = productDataPopupContainer.querySelector("dialog");

    dialog.showModal();
}
