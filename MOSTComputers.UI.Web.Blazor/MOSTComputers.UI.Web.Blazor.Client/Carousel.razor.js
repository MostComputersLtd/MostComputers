
export function startAutoSlide(dotNetRef, intervalMs)
{
    return setInterval(() =>
    {
        dotNetRef.invokeMethodAsync("Next");
    },
        intervalMs);
}

export function stopAutoSlide(intervalId)
{
    if (intervalId !== null)
    {
        clearInterval(intervalId);
    }
}