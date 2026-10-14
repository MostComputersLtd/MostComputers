import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as common from "./Common.js"
import * as dataStore from "./DataStore.js"
import * as propertiesEditor from "./PropertiesEditor.js"
import * as productDocumentsEditor from "./ProductDocumentsEditor.js"
import * as productImagesEditor from "./ProductImagesEditor.js"

const singleEditorProductIdHiddenInputId = "singleEditorProductIdHiddenInput";

const singleEditorShowProductDataPopupButtonId = "singleEditorShowProductDataPopupButton";
const singleEditorShowProductLegacyDisplayPopupButtonId = "singleEditorShowProductLegacyDisplayPopupButton";
const singleEditorShowSearchStringPopupButtonId = "singleEditorShowSearchStringPopupButton";

const singleEditorProductDataPopupContainerId = "singleEditorProductDataPopupContainer";
const singleEditorProductLegacyDisplayPopupContainerId = "singleEditorProductLegacyDisplayPopupContainer";
const singleEditorPromotionProductFileEditorPopupContainerId = "singleEditorPromotionProductFileEditorPopupContainer";
const singleEditorSearchStringPopupContainerId = "singleEditorSearchStringPopupContainer";

const singleEditorProductNewStatusSelectId = "singleEditorProductNewStatusSelect";
const singleEditorAddImageFileButtonId = "singleEditorAddImageFileButton";
const singleEditorAddImageFileInputId = "singleEditorAddImageFileInput";
const singleEditorAddDocumentFileButtonId = "singleEditorAddDocumentFileButton";
const singleEditorAddDocumentFileInputId = "singleEditorAddDocumentFileInput";
const singleEditorAddImageFromClipboardFileButtonId = "singleEditorAddImageFromClipboardFileButton";

export function initEvents(dialogElement) {

    const singleEditorProductNewStatusSelect = document.getElementById(singleEditorProductNewStatusSelectId);

    singleEditorProductNewStatusSelect.addEventListener("change", onProductStatusSelectChangedAsync);

    const singleEditorShowProductDataPopupButton = document.getElementById(singleEditorShowProductDataPopupButtonId);

    singleEditorShowProductDataPopupButton.addEventListener("click", showProductDataPopupAsync)

    const singleEditorShowProductLegacyDisplayPopupButton = document.getElementById(singleEditorShowProductLegacyDisplayPopupButtonId);

    singleEditorShowProductLegacyDisplayPopupButton.addEventListener("click", showProductLegacyDisplayPopupAsync)

    const singleEditorShowSearchStringPopupButton = document.getElementById(singleEditorShowSearchStringPopupButtonId);

    singleEditorShowSearchStringPopupButton.addEventListener("click", showProductSearchStringPopupAsync);

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

    const success = await productDocumentsEditor.addFileToProductDocumentsAsync(file);

    if (!success) {
    }
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

async function showProductDataPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/productDataPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productProperties: dataStoreData.productProperties,
        productLinks: dataStoreData.productLinks,
        productImages: dataStoreData.productImages
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const singleEditorProductDataPopupContainer = document.getElementById(singleEditorProductDataPopupContainerId);

    singleEditorProductDataPopupContainer.innerHTML = data;

    const dialog = singleEditorProductDataPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductLegacyDisplayPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/productLegacyDisplayPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productProperties: dataStoreData.productProperties,
        productLinks: dataStoreData.productLinks,
        productImages: dataStoreData.productImages
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const singleEditorProductLegacyDisplayPopupContainer = document.getElementById(singleEditorProductLegacyDisplayPopupContainerId);

    singleEditorProductLegacyDisplayPopupContainer.innerHTML = data;

    const dialog = singleEditorProductLegacyDisplayPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showPromotionProductFileEditorPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/promotionProductFile/showPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productProperties: dataStoreData.productProperties,
        productLinks: dataStoreData.productLinks,
        productImages: dataStoreData.productImages
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const singleEditorPromotionProductFileEditorPopupContainer = document.getElementById(singleEditorPromotionProductFileEditorPopupContainerId);

    singleEditorPromotionProductFileEditorPopupContainer.innerHTML = data;

    const dialog = singleEditorPromotionProductFileEditorPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductSearchStringPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const singleEditorSearchStringPopupContainer = document.getElementById(singleEditorSearchStringPopupContainerId);

    singleEditorSearchStringPopupContainer.innerHTML = data;

    const dialog = singleEditorSearchStringPopupContainer.querySelector("dialog");

    dialog.showModal();
}
