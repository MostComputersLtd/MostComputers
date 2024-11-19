function toggleViews(element1Id, element2Id)
{
    var element1 = document.getElementById(element1Id);
    var element2 = document.getElementById(element2Id);

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
    var element = document.getElementById(elementId);

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

async function AwaitWithCallbacks(promise, onBeforeAwaitCallback = null, onAfterAwaitCallback = null)
{
    if (Object.prototype.toString.call(promise) !== '[object Promise]'
        || typeof promise.then !== "function")
    {
        return promise;
    }

    if (onBeforeAwaitCallback != null)
    {
        onBeforeAwaitCallback();
    }

    var result = await promise;

    if (onAfterAwaitCallback != null)
    {
        onAfterAwaitCallback();
    }

    return result;
}