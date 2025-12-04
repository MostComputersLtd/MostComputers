function toggleViews(element1Id, element2Id)
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