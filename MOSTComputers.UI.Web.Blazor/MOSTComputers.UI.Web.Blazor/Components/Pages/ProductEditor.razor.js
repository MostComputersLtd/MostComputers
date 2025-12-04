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