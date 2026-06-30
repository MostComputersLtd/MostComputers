const productDataDialogCarouselId = "productDataCarousel";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onStartOfPage);
}
else {
    onStartOfPage();
}

document.addEventListener("visibilitychange", handleVisibilityChange);
window.addEventListener("resize", handleResize);

function onStartOfPage() {

    startAutoSlide(true, productDataDialogCarouselId);

    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    resizeHtmlContentInCarousel(productDataDialogCarousel);
}

function handleVisibilityChange() {

    if (document.hidden) {
        stopAutoSlide(productDataDialogCarouselId);
    }
    else {
        startAutoSlide(true, productDataDialogCarouselId);
    }
}

function handleResize() {

    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    if (!productDataDialogCarousel) return;

    resizeHtmlContentInCarousel(productDataDialogCarousel);
}