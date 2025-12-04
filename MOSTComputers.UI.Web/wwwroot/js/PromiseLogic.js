async function awaitWithCallbacks(promise, onBeforeAwaitCallback = null, onAfterAwaitCallback = null)
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

    try
    {
        var result = await promise;

        return result;
    }
    finally
    {
        if (onAfterAwaitCallback != null)
        {
            onAfterAwaitCallback();
        }
    }
}