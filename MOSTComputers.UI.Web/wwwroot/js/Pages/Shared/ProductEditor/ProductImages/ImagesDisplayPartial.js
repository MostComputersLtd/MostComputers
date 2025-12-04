const htmlDataDisplayCurrentImageIndexAttributeName = "data-html-data-display-current-image-index";

function displayImageHtmlData(htmlDataViewId, htmlDataInputId, rawHtmlDataDisplayId)
{
    if (htmlDataViewId == null
        || htmlDataInputId == null) return;

    const htmlDataView = document.getElementById(htmlDataViewId);
    const htmlDataInput = document.getElementById(htmlDataInputId);
    const rawHtmlDataDisplay = document.getElementById(rawHtmlDataDisplayId);

    if (htmlDataView == null
        || htmlDataInput == null) return;

    rawHtmlDataDisplay.style.display = "none";

    htmlDataView.innerHTML = htmlDataInput.value;

    const elementImageIndexAsString = getLastPartInSeparatedStringInElementId(htmlDataInputId, "-");

    htmlDataView.setAttribute(htmlDataDisplayCurrentImageIndexAttributeName, elementImageIndexAsString);

    htmlDataView.style.display = "";
}

function toggleHtmlDataAndRawHtmlData(
    htmlDataInputPrefix, htmlDataDisplayId, rawHtmlDataInputPrefix, rawHtmlDataDisplayTextAreaId, rawHtmlDataDisplayId)
{
    const htmlDataDisplay = document.getElementById(htmlDataDisplayId);
    const rawHtmlDataDisplayInput = document.getElementById(rawHtmlDataDisplayTextAreaId);
    const rawHtmlDataDisplay = document.getElementById(rawHtmlDataDisplayId);

    var currentImageIndex = htmlDataDisplay.getAttribute(htmlDataDisplayCurrentImageIndexAttributeName);

    if (currentImageIndex == null
        || currentImageIndex === "")
    {
        currentImageIndex = rawHtmlDataDisplayInput.getAttribute(htmlDataDisplayCurrentImageIndexAttributeName);
    }

    const htmlDataInputId = htmlDataInputPrefix + currentImageIndex;
    const rawHtmlDataInputId = rawHtmlDataInputPrefix + currentImageIndex;

    const rawHtmlDataInput = document.getElementById(rawHtmlDataInputId);

    if (htmlDataDisplay.style.display === "none")
    {
        displayImageHtmlData(htmlDataDisplayId, htmlDataInputId, rawHtmlDataDisplayId);

        return;
    }

    htmlDataDisplay.style.display = "none";

    rawHtmlDataDisplayInput.textContent = rawHtmlDataInput.value;

    const elementImageIndexAsString = getLastPartInSeparatedStringInElementId(rawHtmlDataInput, "-");

    rawHtmlDataDisplayInput.setAttribute(htmlDataDisplayCurrentImageIndexAttributeName, elementImageIndexAsString);

    rawHtmlDataDisplay.style.display = "";
}

function getLastPartInSeparatedStringInElementId(elementId, separator)
{
    if (typeof elementId !== "string") return null;

    const stringParts = elementId.split(separator);

    return stringParts[stringParts.length - 1];
}