function scrollHorizontally(e, imageDisplayListElementId)
{
    const imageFileDisplayListElement = document.getElementById(imageDisplayListElementId);

    const delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));

    imageFileDisplayListElement.scrollLeft -= (delta * 30);

    e.preventDefault();
}