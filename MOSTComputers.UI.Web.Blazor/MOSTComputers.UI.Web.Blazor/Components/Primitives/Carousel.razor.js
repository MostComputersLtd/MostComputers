export async function init(carouselId, dotNetObjectReference) {

    const carousel = document.getElementById(carouselId);

    carousel._dotNetObjectReference = dotNetObjectReference;

    document.addEventListener("visibilitychange", handleVisibilityChange);

    function handleVisibilityChange() {

        if (carousel == null) return;

        if (document.hidden) {

            carousel._dotNetObjectReference.invokeMethodAsync("StopAutoPlayAsync");
        }
        else {

            carousel._dotNetObjectReference.invokeMethodAsync("StartAutoPlayAsync");
        }
    }
}

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