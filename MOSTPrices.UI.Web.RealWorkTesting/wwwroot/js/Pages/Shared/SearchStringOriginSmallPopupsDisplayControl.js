function addCopiedPartToTotalCopyAndMakeElementVisible(part, elementIds = null, makeElementVisible = false)
{
    addCopiedPartToTotalCopy(part);

    if (elementIds === null
        || elementIds === undefined
        || makeElementVisible === false) return;

    for (var i = 0; i < elementIds.length; i++)
    {
        changeValueOfSearchStringValue(elementIds[i]);

        var element = document.getElementById(elementIds[i]);

        element.style.visibility = "visible";
    }

}