function getElementFromText(elementAsText)
{
    const template = document.createElement('template');

    template.innerHTML = elementAsText.trim();

    const element = template.content.firstElementChild;

    return element;
}