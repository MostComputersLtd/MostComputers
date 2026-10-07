import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"

const propertyItemName = "propertyItem";
const propertyValueActiveCheckboxName = "propertyValueActiveCheckbox";
const propertyValueInputName = "propertyValueInput";

const propertyItemIsLinkAttribute = "data-is-link";

export function initEvents(dialogElement) {

    const propertyValueInputs = dialogElement.querySelectorAll(`[name='${propertyValueInputName}']`);

    for (const propertyValueInput of propertyValueInputs) {

        propertyValueInput.addEventListener("input", onProductPropertyTextAreaInput);
        propertyValueInput.addEventListener("change", onProductPropertyTextAreaChanged);
    }

    const propertyValueActiveCheckboxes = dialogElement.querySelectorAll(`[name='${propertyValueActiveCheckboxName}']`);

    for (const propertyValueActiveCheckbox of propertyValueActiveCheckboxes) {

        propertyValueActiveCheckbox.addEventListener("change", onProductPropertyActiveCheckboxChanged);
    }
}

function onProductPropertyTextAreaInput(e) {

    const inputElement = e.currentTarget;

    inputElement.style.height = 'auto';
    inputElement.style.height = (inputElement.scrollHeight + 1.6) + 'px';
}

function onProductPropertyTextAreaChanged(e) {

    const value = e.currentTarget.value;

    const propertyItem = e.currentTarget.closest(`[name='${propertyItemName}']`);

    const editorId = propertyItem.id;

    const isLink = propertyItem.getAttribute(propertyItemIsLinkAttribute);

    if (value !== null) {

        dataStore.updatePropertyAndSave(editorId, isLink, property =>
        {
            property.isActive = true;
            property.value = e.target.value;
        });

        setPropertyCheckboxActive(editorId, true);
    }

    editorChanges.setPropertyChanged(editorId, editorChanges.ItemChangeState.Updated);
}

function onProductPropertyActiveCheckboxChanged(e) {

    const propertyValueActiveCheckbox = e.currentTarget;

    const propertyItem = propertyValueActiveCheckbox.closest(`[name='${propertyItemName}']`);

    const editorId = propertyItem.id;

    const isLink = propertyItem.getAttribute(propertyItemIsLinkAttribute);

    const isActive = propertyValueActiveCheckbox.value;

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

function setPropertyCheckboxActive(editorId, isActive) { 

    const propertyElement = document.getElementById(editorId);

    const propertyValueActiveCheckbox = propertyElement.querySelector(`[name='${propertyValueActiveCheckboxName}']`);

    propertyValueActiveCheckbox.value = isActive;
}
