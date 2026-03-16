const TEMP__activeTransformStyle = "translateX(0%)";
const TEMP__leftTransformStyle = "translateX(-100%)";
const TEMP__rightTransformStyle = "translateX(100%)";

const TEMP__carouselTransitionTimeMsAttribute = "data-custom-carousel-transition-time-ms";
const TEMP__carouselGoToTransitionTimeMsAttribute = "data-custom-carousel-go-to-transition-time-ms";
const TEMP__carouselAutoSlideAllowedAttribute = "data-custom-carousel-auto-slide-allowed";
const TEMP__carouselAutoSlideIntervalIdAttribute = "data-custom-carousel-auto-slide-interval-id";
const TEMP__carouselAutoSlideIntervalTimeMsAttribute = "data-custom-carousel-auto-slide-interval-time-ms";
const TEMP__carouselDisplayedItemsAtOnceAttribute = "data-custom-carousel-items-displayed-at-once";
const TEMP__carouselItemHopsPerMoveAttribute = "data-custom-carousel-item-hops-per-move";

const TEMP__carouselItemClass = "custom-carousel-slide";
const TEMP__activeItemClass = "active-carousel-item";

const TEMP__carouselIndicatorsListClass = "custom-carousel-indicators";
const TEMP__activeIndicatorClass = "active-carousel-indicator";

function TEMP__getLeftTransformStyleWithPercent(percent) {
    return `translateX(-${percent}%)`
}

function TEMP__getRightTransformStyleWithPercent(percent) {
    return `translateX(${percent}%)`
}

function TEMP__setDefaultTransitionStyle(element, transitionTimeMs) {

    element.style.transitionProperty = "all"
    element.style.transitionTimingFunction = "ease"
    element.style.transitionDuration = `${transitionTimeMs}ms`
}

function configureCarouselAutoSlideOnLoad(carouselId) {
    const carousel = document.getElementById(carouselId);

    if (carousel == null) return;

    carousel.addEventListener("mouseenter", onCarouselMouseEnter);
    carousel.addEventListener("mouseleave", onCarouselMouseLeave);

    TEMP__startAutoSlide(carousel.id);
}

function onCarouselMouseEnter(e) {
    const carousel = e.currentTarget;

    TEMP__stopAutoSlide(carousel.id);
}

function onCarouselMouseLeave(e) {
    const carousel = e.currentTarget;

    TEMP__startAutoSlide(carousel.id);
}

function TEMP__startAutoSlide(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const isAutoSlideAllowedString = carousel.getAttribute(TEMP__carouselAutoSlideAllowedAttribute);

    const isAutoSlideAllowed = isAutoSlideAllowedString === "true";

    if (!isAutoSlideAllowed) return;

    const existingIntervalId = carousel.getAttribute(TEMP__carouselAutoSlideIntervalIdAttribute);

    if (existingIntervalId != null) return;

    const intervalMs = carousel.getAttribute(TEMP__carouselAutoSlideIntervalTimeMsAttribute);

    if (intervalMs == null) return;

    const intervalId = setInterval(() => {
        TEMP__slideInDirection(carouselId, false)
    },
    intervalMs);

    carousel.setAttribute(TEMP__carouselAutoSlideIntervalIdAttribute, intervalId);
}

function TEMP__stopAutoSlide(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const intervalId = carousel.getAttribute(TEMP__carouselAutoSlideIntervalIdAttribute);

    if (!intervalId) return;

    if (intervalId !== null) {
        clearInterval(intervalId);

        carousel.removeAttribute(TEMP__carouselAutoSlideIntervalIdAttribute);
    }
}

function TEMP__slideInDirection(carouselId, goBackwards) {

    if (goBackwards) {
        TEMP__prev(carouselId);
    }
    else {
        TEMP__next(carouselId);
    }
}

function TEMP__next(carouselId) {
    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${TEMP__carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(TEMP__carouselTransitionTimeMsAttribute);
    const displayedItemsAtOnceString = carousel.getAttribute(TEMP__carouselDisplayedItemsAtOnceAttribute);
    const itemHopsPerMoveString = carousel.getAttribute(TEMP__carouselItemHopsPerMoveAttribute);

    const transitionTimeMs = TEMP__getNumberOrDefaultFromString(transitionTimeMsString);
    const displayedItemsAtOnce = TEMP__getNumberOrDefaultFromString(displayedItemsAtOnceString, 1);
    const itemHopsPerMove = TEMP__getNumberOrDefaultFromString(itemHopsPerMoveString, 1);

    const allRelatedItems = TEMP__getItemsToMoveForward(carouselItems, itemHopsPerMove, displayedItemsAtOnce);

    allRelatedItems.forEach((item, i) => {
        const offset = i;

        item.classList.remove(TEMP__activeItemClass);

        item.style.transition = "";
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    void carousel.offsetWidth;

    allRelatedItems.forEach((item, i) => {
        const offset = i - itemHopsPerMove;

        if (offset >= 0) {
            item.classList.add(TEMP__activeItemClass);
        }

        TEMP__setDefaultTransitionStyle(item, transitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    })

    const firstActiveElement = allRelatedItems[itemHopsPerMove];

    TEMP__setActiveIndicatorForItem(carousel, displayedItemsAtOnce, carouselItems.indexOf(firstActiveElement));

    carousel.setAttribute("aria-activedescendant", firstActiveElement.id);
}

function TEMP__prev(carouselId) {
    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${TEMP__carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(TEMP__carouselTransitionTimeMsAttribute);
    const displayedItemsAtOnceString = carousel.getAttribute(TEMP__carouselDisplayedItemsAtOnceAttribute);
    const itemHopsPerMoveString = carousel.getAttribute(TEMP__carouselItemHopsPerMoveAttribute);

    const transitionTimeMs = TEMP__getNumberOrDefaultFromString(transitionTimeMsString);
    const displayedItemsAtOnce = TEMP__getNumberOrDefaultFromString(displayedItemsAtOnceString, 1);
    const itemHopsPerMove = TEMP__getNumberOrDefaultFromString(itemHopsPerMoveString, 1);

    const allRelatedItems = TEMP__getItemsToMoveBackwards(carouselItems, itemHopsPerMove, displayedItemsAtOnce);

    allRelatedItems.forEach((item, i) => {
        const offset = i - itemHopsPerMove;

        item.classList.remove(TEMP__activeItemClass);

        item.style.transition = "";
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    void carousel.offsetWidth;

    allRelatedItems.forEach((item, i) => {
        const offset = i;

        if (offset < displayedItemsAtOnce) {
            item.classList.add(TEMP__activeItemClass);
        }

        TEMP__setDefaultTransitionStyle(item, transitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    const firstActiveElement = allRelatedItems[0];

    TEMP__setActiveIndicatorForItem(carousel, displayedItemsAtOnce, carouselItems.indexOf(firstActiveElement));

    carousel.setAttribute("aria-activedescendant", firstActiveElement.id);
}

function TEMP__goTo(carouselId, firstItemIndex) {

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${TEMP__carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(TEMP__carouselTransitionTimeMsAttribute);
    const goToTransitionTimeMsString = carousel.getAttribute(TEMP__carouselGoToTransitionTimeMsAttribute);
    const displayedItemsAtOnceString = carousel.getAttribute(TEMP__carouselDisplayedItemsAtOnceAttribute);

    const transitionTimeMs = TEMP__getNumberOrDefaultFromString(transitionTimeMsString);
    const goToTransitionTimeMs = TEMP__getNumberOrDefaultFromString(goToTransitionTimeMsString);
    const displayedItemsAtOnce = TEMP__getNumberOrDefaultFromString(displayedItemsAtOnceString, 1);

    const actualTransitionTimeMs = goToTransitionTimeMs != 0 ? goToTransitionTimeMs : transitionTimeMs;

    if (carouselItems.length <= 1) return;

    const activeItems = TEMP__getActiveItemsOrderedLTR(carouselItems);

    activeItems.forEach((item) => {
        item.style.zIndex = 1
    });

    const itemsToShow = [];

    for (var i = 0; i < displayedItemsAtOnce; i++) {
        var itemIndex = firstItemIndex + i;

        if (itemIndex >= carouselItems.length) {
            itemIndex -= carouselItems.length;
        }

        itemsToShow[i] = carouselItems[itemIndex];
    }

    const firstActiveItemIndex = carouselItems.indexOf(activeItems[0]);

    const travelDistance = Math.abs(firstItemIndex - firstActiveItemIndex);

    var moveAmount = Math.min(travelDistance, displayedItemsAtOnce);

    var moveForward = firstItemIndex > firstActiveItemIndex;

    const itemCount = carouselItems.length;

    const pageCount = Math.ceil(itemCount / displayedItemsAtOnce);

    const currentPageIndex = Math.floor(firstActiveItemIndex / displayedItemsAtOnce);

    const targetPageIndex = Math.floor(firstItemIndex / displayedItemsAtOnce);

    const isForwardWrap = currentPageIndex === pageCount - 1 && targetPageIndex === 0;

    const isBackwardWrap = currentPageIndex === 0 && targetPageIndex === pageCount - 1;

    const itemToPageDifference = pageCount * displayedItemsAtOnce - itemCount;

    if (isForwardWrap) {
        moveForward = true;

        const activeItemsThatWillBeVisibleAfter = displayedItemsAtOnce - (itemCount - firstActiveItemIndex);

        moveAmount -= activeItemsThatWillBeVisibleAfter;
    }

    if (isBackwardWrap) {
        moveForward = false;

        const activeItemsThatWillBeVisibleAfter = itemToPageDifference - firstActiveItemIndex;

        moveAmount -= activeItemsThatWillBeVisibleAfter;
    }

    moveAmount = Math.min(moveAmount, displayedItemsAtOnce);
    
    if (moveForward) {

        itemsToShow.forEach((item, i) => {
            const offset = i + moveAmount;

            if (offset < displayedItemsAtOnce) return;

            item.style.zIndex = 2;
            item.style.transition = "";
            item.style.transform = `translateX(${offset * 100}%)`;
        });

        void carousel.offsetWidth;

        activeItems.forEach((item, i) => {
            const offset = i - moveAmount;

            item.classList.remove(TEMP__activeItemClass);

            TEMP__setDefaultTransitionStyle(item, actualTransitionTimeMs);
            item.style.transform = `translateX(${offset * 100}%)`;
        });

        itemsToShow.forEach((item, i) => {
            item.classList.add(TEMP__activeItemClass);

            TEMP__setDefaultTransitionStyle(item, actualTransitionTimeMs);
            item.style.transform = `translateX(${i * 100}%)`;
        });

        TEMP__setActiveIndicatorAt(carousel, targetPageIndex);

        return;
    }

    itemsToShow.forEach((item, i) => {
        const offset = i - moveAmount;

        if (offset > 0) return;

        item.style.zIndex = 2;
        item.style.transition = "";
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    void carousel.offsetWidth;

    activeItems.forEach((item, i) => {
        const offset = i + moveAmount;

        item.classList.remove(TEMP__activeItemClass);

        TEMP__setDefaultTransitionStyle(item, actualTransitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    itemsToShow.forEach((item, i) => {
        item.classList.add(TEMP__activeItemClass);

        TEMP__setDefaultTransitionStyle(item, actualTransitionTimeMs);
        item.style.transform = `translateX(${i * 100}%)`;
    });

    TEMP__setActiveIndicatorAt(carousel, targetPageIndex);
}

function TEMP__getActiveItemsOrderedLTR(carouselItems) {

    const items = [];

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(TEMP__activeItemClass)) {

            items.push(item);
        }
    }

    return items.sort(function (a, b)
    {
        const transformStyleA = a.style.transform;
        const transformStyleB = b.style.transform;

        const percentTransformA = transformStyleA.substring(transformStyleA.indexOf('(') + 1, transformStyleA.indexOf('%'));
        const percentTransformB = transformStyleB.substring(transformStyleB.indexOf('(') + 1, transformStyleB.indexOf('%'));

        return parseFloat(percentTransformA) - parseFloat(percentTransformB);
    });
}

function TEMP__getItemsToMoveForward(carouselItems, hopsInMove, visibleItems) {

    const items = [];

    const totalItemCount = visibleItems + hopsInMove;

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(TEMP__activeItemClass)) {

            items.push(item);
        }
    }

    items.sort(function (a, b) {
        const transformStyleA = a.style.transform;
        const transformStyleB = b.style.transform;

        const percentTransformA = transformStyleA.substring(transformStyleA.indexOf('(') + 1, transformStyleA.indexOf('%'));
        const percentTransformB = transformStyleB.substring(transformStyleB.indexOf('(') + 1, transformStyleB.indexOf('%'));

        return parseFloat(percentTransformA) - parseFloat(percentTransformB);
    });

    const remainingItems = totalItemCount - items.length;

    if (remainingItems <= 0) return items;

    const lastActive = items[items.length - 1];

    const lastItemIndex = carouselItems.indexOf(lastActive);

    for (var i = 1; i <= remainingItems; i++) {
        var nextItemIndex = lastItemIndex + i;

        if (nextItemIndex >= carouselItems.length) {
            nextItemIndex = nextItemIndex - carouselItems.length;
        }

        items.push(carouselItems[nextItemIndex]);
    }

    return items;
}

function TEMP__getItemsToMoveBackwards(carouselItems, hopsInMove, visibleItems) {

    const items = [];

    const totalItemCount = visibleItems + hopsInMove;

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(TEMP__activeItemClass)) {

            items.push(item);
        }
    }

    items.sort(function (a, b) {
        const transformStyleA = a.style.transform;
        const transformStyleB = b.style.transform;

        const percentTransformA = transformStyleA.substring(transformStyleA.indexOf('(') + 1, transformStyleA.indexOf('%'));
        const percentTransformB = transformStyleB.substring(transformStyleB.indexOf('(') + 1, transformStyleB.indexOf('%'));

        return parseFloat(percentTransformB) - parseFloat(percentTransformA);
    });

    const remainingItems = totalItemCount - items.length;

    if (remainingItems <= 0) return items.reverse();

    const lastActive = items[items.length - 1];

    const lastItemIndex = carouselItems.indexOf(lastActive);

    for (var i = 1; i <= remainingItems; i++) {
        var nextItemIndex = lastItemIndex - i;

        if (nextItemIndex < 0) {
            nextItemIndex = nextItemIndex + carouselItems.length;
        }

        items.push(carouselItems[nextItemIndex]);
    }

    return items.reverse();
}

function resizeCarousel(carouselId, newDisplayedItemsAtOnce) {

    if (isNaN(newDisplayedItemsAtOnce) || isNaN(parseInt(newDisplayedItemsAtOnce))) return;

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${TEMP__carouselItemClass}`)];

    const displayedItemsAtOnceString = carousel.getAttribute(TEMP__carouselDisplayedItemsAtOnceAttribute);

    const displayedItemsAtOnce = TEMP__getNumberOrDefaultFromString(displayedItemsAtOnceString, 1);

    if (displayedItemsAtOnce === newDisplayedItemsAtOnce) return;

    const differenceBetweenOldAndNewItems = newDisplayedItemsAtOnce - displayedItemsAtOnce;

    var activeItems = TEMP__getActiveItemsOrderedLTR(carouselItems);

    carousel.setAttribute(TEMP__carouselDisplayedItemsAtOnceAttribute, newDisplayedItemsAtOnce);

    if (differenceBetweenOldAndNewItems > 0) {

        const lastItem = activeItems[activeItems.length - 1];

        var lastItemIndex = carouselItems.indexOf(lastItem);

        for (var i = 1; i <= differenceBetweenOldAndNewItems; i++) {

            lastItemIndex += 1;

            if (lastItemIndex >= carouselItems.length) {
                lastItemIndex = lastItemIndex % carouselItems.length;
            }

            const newItem = carouselItems[lastItemIndex];

            newItem.classList.add(TEMP__activeItemClass);

            activeItems.push(newItem);
        }
    }
    else {

        for (var i = 0; i > differenceBetweenOldAndNewItems; i--) {

            const itemToRemoveIndex = activeItems.length - 1;

            activeItems[itemToRemoveIndex].classList.remove(TEMP__activeItemClass);

            activeItems.pop();
        }
    }

    const itemWidthPercent = 100 / newDisplayedItemsAtOnce;

    const itemWidthPercentStyle = `${itemWidthPercent}%`;

    for (var i = 0; i < carouselItems.length; i++) {
        const item = carouselItems[i];

        item.style.transition = "";
        item.style.width = itemWidthPercentStyle;
        item.style.transform = `translateX(${newDisplayedItemsAtOnce * 100}%)`;
    }

    for (var i = 0; i < activeItems.length; i++) {
        const item = activeItems[i];

        item.style.transform = `translateX(${i * 100}%)`;
    }

    const firstActiveItem = activeItems[0];

    const firstActiveItemIndex = carouselItems.indexOf(firstActiveItem);

    const carouselList = carousel.querySelector(`.${TEMP__carouselIndicatorsListClass}`);

    const indicators = [...carouselList.children];

    if (indicators == null || indicators.length === 0) return;

    const firstIndicator = indicators[0];

    const indicatorPagesCount = Math.ceil(carouselItems.length / newDisplayedItemsAtOnce);

    const targetPageIndex = Math.floor(firstActiveItemIndex / newDisplayedItemsAtOnce);

    firstIndicator.classList.remove(TEMP__activeIndicatorClass);

    carouselList.innerHTML = "";

    for (var i = 0; i < indicatorPagesCount; i++) {
        const indicator = firstIndicator.cloneNode(true);

        const startIndex = i * newDisplayedItemsAtOnce;

        if (i === targetPageIndex)
        {
            indicator.classList.add(TEMP__activeIndicatorClass);
        }

        indicator.id = `carousel-option-${i}`;

        indicator.onclick = null;

        indicator.addEventListener("click", function () {
            TEMP__goTo(carouselId, startIndex);
        });

        carouselList.appendChild(indicator);
    }
}

function TEMP__getCarouselIndicators(carousel)
{
    const carouselList = carousel.querySelector(`.${TEMP__carouselIndicatorsListClass}`);

    const carouselIndicators = [...carouselList.children];

    return carouselIndicators;
}

function TEMP__setActiveIndicatorForItem(carousel, displayedItemsAtOnce, index) {
    const targetPageIndex = Math.floor(index / displayedItemsAtOnce);

    TEMP__setActiveIndicatorAt(carousel, targetPageIndex);
}

function TEMP__setActiveIndicatorAt(carousel, index)
{
    const carouselIndicators = TEMP__getCarouselIndicators(carousel);

    if (index < 0 || index >= carouselIndicators.length) return;

    for (var i = 0; i < carouselIndicators.length; i++) {

        const indicator = carouselIndicators[i]

        if (indicator.classList.contains(TEMP__activeIndicatorClass))
        {
            indicator.classList.remove(TEMP__activeIndicatorClass);

            break;
        }
    }

    carouselIndicators[index].classList.add(TEMP__activeIndicatorClass);
}

function TEMP__getNumberOrDefaultFromString(stringValue, defaultValue = 0) {
    if (stringValue == null && stringValue != "") return defaultValue;

    var output = 0;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}