import * as validationMessages from "../../../Common/Validation/ValidationMessages.js"
import * as validationVisualElements from "../../../Common/Validation/ValidationVisualElements.js"
import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"

const productPropertyListId = "singleEditorProductPropertyList";
const productLinkListId = "singleEditorProductLinkList";

const itemName = "propertyItem";
const valueActiveCheckboxName = "propertyValueActiveCheckbox";
const valueInputName = "propertyValueInput";

const validationMessagesListRowName = "propertyValidationMessagesListRow";
const validationMessagesListName = "propertyValidationMessagesList";

const propertyItemProductCharacteristicIdAttribute = "data-product-characteristic-id";
const propertyItemIsLinkAttribute = "data-is-link";

const validationMessagesListRowNoItemsClass = "no-items";

const endIndexCharacter = ']';
const propertyRequestsStartString = 'PropertyRequests[';

export function initEvents(dialogElement) {

    const propertyValueInputs = dialogElement.querySelectorAll(`[name='${valueInputName}']`);

    for (const propertyValueInput of propertyValueInputs) {

        propertyValueInput.addEventListener("input", onProductPropertyTextAreaInput);
        propertyValueInput.addEventListener("keydown", onProductPropertyTextAreaKeyDown);
        propertyValueInput.addEventListener("change", onProductPropertyTextAreaChanged);
    }

    const propertyValueActiveCheckboxes = dialogElement.querySelectorAll(`[name='${valueActiveCheckboxName}']`);

    for (const propertyValueActiveCheckbox of propertyValueActiveCheckboxes) {

        propertyValueActiveCheckbox.addEventListener("change", onProductPropertyActiveCheckboxChanged);
    }
}

export function resizeTextareas(dialogElement) {

    const propertyValueInputs = dialogElement.querySelectorAll(`[name='${valueInputName}']`);

    for (const propertyValueInput of propertyValueInputs) {

        resizeTextareaToShowAllText(propertyValueInput);
    }
}

export function getProductPropertyRequestData() {

    const items = getProductPropertyAndLinkItems();

    const itemDatas = [];

    for (const item of items) {

        const data = getProductPropertyItemRequestData(item);

        itemDatas.push(data);
    }

    return itemDatas;
}

export function getProductPropertyItemRequestData(item) {

    const productCharacteristicId = item.getAttribute(propertyItemProductCharacteristicIdAttribute);

    const inputElement = item.querySelector(`[name='${valueInputName}']`);
    const activeCheckbox = item.querySelector(`[name='${valueActiveCheckboxName}']`);

    return {
        productCharacteristicId: parseInt(productCharacteristicId),
        value: inputElement.value,
        active: activeCheckbox.checked
    };
}

function getProductPropertyAndLinkItems() {

    const productPropertyList = document.getElementById(productPropertyListId);
    const productLinkList = document.getElementById(productLinkListId);

    const productPropertyItemsArray = [...productPropertyList.querySelectorAll(`[name='${itemName}']`)];
    const productLinkItems = productLinkList.querySelectorAll(`[name='${itemName}']`);

    productPropertyItemsArray.push(...productLinkItems);

    return productPropertyItemsArray;
}

export function displayValidationMessages(validationErrors, propertyRequestsUsed) {

    const items = getProductPropertyAndLinkItems();

    for (let i = 0; i < items.length; i++) {

        const item = items[i];

        const validationMessagesListRow = item.querySelector(`[name='${validationMessagesListRowName}']`);
        const validationMessagesList = validationMessagesListRow.querySelector(`[name='${validationMessagesListName}']`);

        validationMessagesListRow.classList.add(validationMessagesListRowNoItemsClass);

        validationVisualElements.clearValidationMessages(validationMessagesList);
    }

    const propertyValidationMessages = new Map();

    for (const [name, validationErrorsForProperty] of Object.entries(validationErrors)) {

        if (!name.startsWith(propertyRequestsStartString)) continue;

        const startIndex = propertyRequestsStartString.length;
        const endIndex = name.indexOf(endIndexCharacter, startIndex);

        const indexAsString = name.substring(startIndex, endIndex);
        const index = parseInt(indexAsString);

        if (!propertyValidationMessages.has(index)) {

            propertyValidationMessages.set(index, []);
        }

        const messagesForProperty = propertyValidationMessages.get(index);

        messagesForProperty.push(validationErrorsForProperty); 
    }

    for (const [index, validationErrorsForProperty] of propertyValidationMessages) {

        const requestUsed = propertyRequestsUsed[index];

        const item = items.find(
            x => x.getAttribute(propertyItemProductCharacteristicIdAttribute) == requestUsed.productCharacteristicId);

        const validationMessagesListRow = item.querySelector(`[name='${validationMessagesListRowName}']`);
        const validationMessagesList = validationMessagesListRow.querySelector(`[name='${validationMessagesListName}']`);

        validationVisualElements.displayValidationMessages(
            validationErrorsForProperty,
            validationMessagesList,
            validationMessages.getDefaultValidationMessage)

        validationMessagesListRow.classList.remove(validationMessagesListRowNoItemsClass);
    }
}

function onProductPropertyTextAreaInput(e) {

    const inputElement = e.currentTarget;

    resizeTextareaToShowAllText(inputElement);
}

function resizeTextareaToShowAllText(inputElement) {

    inputElement.style.height = 'auto';
    inputElement.style.height = (inputElement.scrollHeight + 1.6) + 'px';
}

function onProductPropertyTextAreaKeyDown(e) {

    if (!e.ctrlKey || e.key !== "Enter") return;

    e.preventDefault();
    e.stopPropagation();

    const inputElement = e.currentTarget;

    const propertyItem = inputElement.closest(`[name='${itemName}']`);
    const activeCheckbox = propertyItem.querySelector(`[name='${valueActiveCheckboxName}']`);

    activeCheckbox.checked = true;

    const isLink = getPropertyIsLink(propertyItem);

    activeCheckbox.dispatchEvent(new Event("change", { bubbles: true }));

    dataStore.updatePropertyAndSave(editorId, isLink, property =>
    {
        property.isActive = true;
    });
}

function onProductPropertyTextAreaChanged(e) {

    const inputElement = e.currentTarget;

    const value = inputElement.value;

    const propertyItem = inputElement.closest(`[name='${itemName}']`);

    const editorId = propertyItem.id;

    const isLink = getPropertyIsLink(propertyItem);

    if (value !== null) {

        const propertyValueActiveCheckbox = propertyItem.querySelector(`[name='${valueActiveCheckboxName}']`);

        propertyValueActiveCheckbox.checked = true;

        dataStore.updatePropertyAndSave(editorId, isLink, property =>
        {
            property.value = value;
        });
    }

    editorChanges.setPropertyChanged(editorId, editorChanges.ItemChangeState.Updated);
}

function onProductPropertyActiveCheckboxChanged(e) {

    const propertyValueActiveCheckbox = e.currentTarget;

    const propertyItem = propertyValueActiveCheckbox.closest(`[name='${itemName}']`);

    const editorId = propertyItem.id;

    const isLink = getPropertyIsLink(propertyItem);

    const isActive = propertyValueActiveCheckbox.checked;

    dataStore.updatePropertyAndSave(editorId, isLink, property =>
    {
        property.isActive = isActive;
    });

    if (isActive) {
        editorChanges.setPropertyChanged(editorId, editorChanges.ItemChangeState.Added);
    }
    else {
        editorChanges.setPropertyChanged(editorId, editorChanges.ItemChangeState.Deleted);
    }
}

function getPropertyIsLink(item) {

    const isLink = item.getAttribute(propertyItemIsLinkAttribute);

    return isLink === "true";
}
