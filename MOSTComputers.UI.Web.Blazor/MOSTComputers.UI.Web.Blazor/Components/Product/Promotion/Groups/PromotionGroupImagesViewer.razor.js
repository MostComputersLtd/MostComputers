export function focusElementById(elementId) {
    const element = document.getElementById(elementId);

    if (element) {
        element.focus();
    }
}