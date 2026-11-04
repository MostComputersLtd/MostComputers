const productDataDialogCarouselId = "productDataCarousel";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onStartOfPage);
} else {
    onStartOfPage();
}

document.addEventListener("visibilitychange", handleVisibilityChange);
window.addEventListener("resize", handleResize);

function onStartOfPage() {
    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    if (!productDataDialogCarousel) return;

    startAutoSlide(true, productDataDialogCarouselId);

    resizeHtmlContentInCarousel(productDataDialogCarousel);
}

function handleVisibilityChange() {

    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    if (!productDataDialogCarousel) return;

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
