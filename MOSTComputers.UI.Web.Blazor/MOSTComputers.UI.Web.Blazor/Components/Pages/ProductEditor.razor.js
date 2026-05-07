const pageRootDivId = "productEditorPage";
const searchInputId = "userInput";
const searchButtonId = "productEditorSearchButton";

export function registerKeyDownEvent(dotNetReference) {
   
    attachKeyDownEvent(dotNetReference);

    Blazor.addEventListener("enhancedload", removeKeyDownEvent);
    window.addEventListener("beforeunload", removeKeyDownEvent);
}

function attachKeyDownEvent(dotNetReference) {
    const root = document.getElementById(pageRootDivId);

    if (!root || root._listenerAttached) return;

    window.addEventListener("keydown", MakeEnterStartSearch);

    root._listenerAttached = true;
    root._dotNetReference = dotNetReference;

    console.log("Attached");
}

function removeKeyDownEvent() {
    const root = document.getElementById(pageRootDivId);

    window.removeEventListener("keydown", MakeEnterStartSearch);

    if (root) {
        root._listenerAttached = false;
        root._dotNetReference = null;
    }

    console.log("Removed");
}

function MakeEnterStartSearch(e) {
    if (e.key !== 'Enter') return;

    console.log("EVENT");

    const pageRootElement = document.getElementById(pageRootDivId);
    const searchButton = document.getElementById(searchButtonId);

    if (!pageRootElement || !searchButton) return;

    const el = document.activeElement;

    if (!el) return;

    const elementTag = el.tagName.toLowerCase();
    const elementType = (el.getAttribute('type') || '').toLowerCase();

    const nativeInteractiveInputTypes = ['button', 'submit', 'checkbox', 'radio'];

    const isNativeInteractiveElement =
        elementTag === 'button' ||
        elementTag === 'textarea' ||
        elementTag === 'select' ||
        elementTag === 'a' ||
        (elementTag === 'input' && nativeInteractiveInputTypes.includes(elementType));

    const isContentEditable = el.isContentEditable;
    const isRoleButton = el.getAttribute('role') === 'button';

    if (isNativeInteractiveElement || isContentEditable || isRoleButton) {
        return;
    }

    const isThereAnyOpenDialog = Array.from(document.querySelectorAll('dialog')).some(
        dialogElement => dialogElement.open
    );

    if (isThereAnyOpenDialog) {
        return;
    }

    e.preventDefault();

    console.log("Prevented default");

    searchButton.focus();

    const dotNetReference = pageRootElement._dotNetReference;

    dotNetReference.invokeMethodAsync("SearchProductsFromJs");
}

function toggleElementDisplay(elementId)
{
    const element = document.getElementById(elementId);

    if (element.style.display === "none")
    {
        element.style.display = "";
    }
    else
    {
        element.style.display = "none";
    }
}

function toggleElementsDisplay(elementIds)
{
    if (!Array.isArray(elementIds))
    {
        throw new Error("elementIds must be an array");
    }

    for (var i = 0; i < elementIds.length; i++)
    {
        toggleElementDisplay(elementIds[i]);
    }
}

export function alternateElementsDisplay(element1Id, element2Id)
{
    const element1 = document.getElementById(element1Id);
    const element2 = document.getElementById(element2Id);

    if (element1 == null
        || element2 == null)
    {
        return false;
    }

    if (element2.style.display === "none")
    {
        element1.style.display = "none";
        element2.style.display = "";
    }
    else
    {
        element2.style.display = "none";
        element1.style.display = "";
    }

    return true;
}

export function openDataUrlInNewWindow(url)
{
    window.open(url, '_blank');
}

export function openFileDataInNewWindow(fileData, contentType, newWindowTitle)
{
    const blob = new Blob([fileData], { type: contentType });

    const url = URL.createObjectURL(blob);

    const newWindow = window.open(url, '_blank');

    newWindow.onload = () => URL.revokeObjectURL(url);

    if (newWindow)
    {
        newWindow.document.title = newWindowTitle;
    }
}
