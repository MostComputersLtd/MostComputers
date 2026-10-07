import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js";
import * as productEditorImageFilePopup from "./ProductEditorImageFilePopup.js";
import * as singleProductEditorPopup from "./SingleProductEditorPopup/SingleProductPopupEditor.js";

const productXmlPopupContainerId = "productXmlPopupContainerId";
const productPromotionPopupContainerId = "productPromotionPopupContainer";
const productInfoPromotionPopupContainerId = "productInfoPromotionPopupContainer";
const productImagesPopupContainerId = "productImagesPopupContainer";
const productImageFilesPopupContainerId = "productImageFilesPopupContainer";
const productPropertiesPopupContainerId = "productPropertiesPopupContainer";
const productSearchStringPopupContainerId = "productSearchStringPopupContainer";
const singleProductEditorPopupContainerId = "singleProductEditorPopupContainer";

const singleProductEditorPopupJsonDataScriptId = "singleEditorData";

const imageFileButtonName = "imageFileButton";

const imageFileButtonImageRouteAttribute = "data-image-route";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

function initEvents() {
}

async function showProductXmlPopup(productId) {
    
    const url = `api/components/productEditor/productXmlDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productXmlPopupContainer = document.getElementById(productXmlPopupContainerId);

    productXmlPopupContainer.innerHTML = data;

    const dialog = productXmlPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductPromotionPopup(productId, promotionId) {
    
    const url = `api/components/productEditor/productPromotionPopup/${productId}/${promotionId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productPromotionPopupContainer = document.getElementById(productPromotionPopupContainerId);

    productPromotionPopupContainer.innerHTML = data;

    const dialog = productPromotionPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductInfoPromotionPopup(productId) {
    
    const url = `api/components/productEditor/productInfoPromotionPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productInfoPromotionPopupContainer = document.getElementById(productInfoPromotionPopupContainerId);

    productInfoPromotionPopupContainer.innerHTML = data;

    const dialog = productInfoPromotionPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductImagesPopup(productId) {
    
    const url = `api/components/productEditor/productImagesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productImagesPopupContainer = document.getElementById(productImagesPopupContainerId);

    productImagesPopupContainer.innerHTML = data;

    const dialog = productImagesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductImageFilesPopup(productId) {
    
    const url = `api/components/productEditor/productImageFilesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productImageFilesPopupContainer = document.getElementById(productImageFilesPopupContainerId);

    productImageFilesPopupContainer.innerHTML = data;

    const imageFileButtons = [...productImageFilesPopupContainer.querySelectorAll($`[name='${imageFileButtonName}']`)];

    for (const imageFileButton of imageFileButtons) {

        imageFileButton.addEventListener("click", productEditorImageFilePopup.imageDataButtonOpenRoute);
    }

    const dialog = productImageFilesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductPropertiesPopup(productId) {
    
    const url = `api/components/productEditor/productPropertiesPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productPropertiesPopupContainer = document.getElementById(productPropertiesPopupContainerId);

    productPropertiesPopupContainer.innerHTML = data;

    const dialog = productPropertiesPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductSearchStringPopup(productId) {
    
    const url = `api/components/productEditor/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const productSearchStringPopupContainer = document.getElementById(productSearchStringPopupContainerId);

    productSearchStringPopupContainer.innerHTML = data;

    const dialog = productSearchStringPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showSingleProductEditorPopup(productId) {

    const url = `api/components/productEditor/singleProductEditorPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const singleProductEditorPopupContainer = document.getElementById(singleProductEditorPopupContainerId);

    singleProductEditorPopupContainer.innerHTML = data;

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    singleProductEditorPopup.registerData(jsonData);

    jsonDataScriptElement.remove();

    const dialog = singleProductEditorPopupContainer.querySelector("dialog");

    singleProductEditorPopup.initEvents(dialog);

    dialog.showModal();
}
