const productDataDialogCarouselId = "productDataCarousel";

document.addEventListener("visibilitychange", handleVisibilityChange);

function handleVisibilityChange() {
    const productDataDialogCarousel = document.getElementById(productDataDialogCarouselId);

    if (productDataDialogCarousel == null) return;

    if (document.hidden) {
        stopAutoSlide(productDataDialogCarouselId);
    }
    else {
        startAutoSlide(productDataDialogCarouselId);
    }
}