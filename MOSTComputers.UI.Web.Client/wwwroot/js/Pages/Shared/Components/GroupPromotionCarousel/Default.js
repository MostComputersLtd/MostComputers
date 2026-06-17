const activeTransformStyleMultiItem = "translateX(0%)";
const leftTransformStyleMultiItem = "translateX(-100%)";
const rightTransformStyleMultiItem = "translateX(100%)";

const carouselTransitionTimeMsAttributeMultiItem = "data-custom-carousel-transition-time-ms";
const carouselGoToTransitionTimeMsAttributeMultiItem = "data-custom-carousel-go-to-transition-time-ms";
const carouselAutoSlideAllowedAttributeMultiItem = "data-custom-carousel-auto-slide-allowed";
const carouselAutoSlideIntervalIdAttributeMultiItem = "data-custom-carousel-auto-slide-interval-id";
const carouselAutoSlideIntervalTimeMsAttributeMultiItem = "data-custom-carousel-auto-slide-interval-time-ms";
const carouselDisplayedItemsAtOnceAttributeMultiItem = "data-custom-carousel-items-displayed-at-once";
const carouselItemHopsPerMoveAttributeMultiItem = "data-custom-carousel-item-hops-per-move";

const carouselItemClassMultiItem = "custom-carousel-slide";
const activeItemClassMultiItem = "active-carousel-item";

const carouselIndicatorsListClassMultiItem = "custom-carousel-indicators";
const activeIndicatorClassMultiItem = "active-carousel-indicator";

function getLeftTransformStyleWithPercentMultiItem(percent) {
    return `translateX(-${percent}%)`
}

function getRightTransformStyleWithPercentMultiItem(percent) {
    return `translateX(${percent}%)`
}

function setDefaultTransitionStyleMultiItem(element, transitionTimeMs) {

    element.style.transitionProperty = "all"
    element.style.transitionTimingFunction = "ease"
    element.style.transitionDuration = `${transitionTimeMs}ms`
}

function configureCarouselAutoSlideOnLoad(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (carousel == null) return;

    carousel.addEventListener("mouseenter", onCarouselMouseEnter);
    carousel.addEventListener("mouseleave", onCarouselMouseLeave);

    startAutoSlideMultiItem(carousel.id);
}

function onCarouselMouseEnter(e) {
    const carousel = e.currentTarget;

    stopAutoSlideMultiItem(carousel.id);
}

function onCarouselMouseLeave(e) {
    const carousel = e.currentTarget;

    startAutoSlideMultiItem(carousel.id);
}

function startAutoSlideMultiItem(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const isAutoSlideAllowedString = carousel.getAttribute(carouselAutoSlideAllowedAttributeMultiItem);

    const isAutoSlideAllowed = isAutoSlideAllowedString === "true";

    if (!isAutoSlideAllowed) return;

    const existingIntervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttributeMultiItem);

    if (existingIntervalId != null) return;

    const intervalMs = carousel.getAttribute(carouselAutoSlideIntervalTimeMsAttributeMultiItem);

    if (intervalMs == null) return;

    const intervalId = setInterval(() => {
        slideInDirectionMultiItem(carouselId, false)
    },
    intervalMs);

    carousel.setAttribute(carouselAutoSlideIntervalIdAttributeMultiItem, intervalId);
}

function stopAutoSlideMultiItem(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const intervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttributeMultiItem);

    if (!intervalId) return;

    if (intervalId !== null) {
        clearInterval(intervalId);

        carousel.removeAttribute(carouselAutoSlideIntervalIdAttributeMultiItem);
    }
}

function slideInDirectionMultiItem(carouselId, goBackwards) {

    if (goBackwards) {
        prevMultiItem(carouselId);
    }
    else {
        nextMultiItem(carouselId);
    }
}

function nextMultiItem(carouselId) {
    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClassMultiItem}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttributeMultiItem);
    const displayedItemsAtOnceString = carousel.getAttribute(carouselDisplayedItemsAtOnceAttributeMultiItem);
    const itemHopsPerMoveString = carousel.getAttribute(carouselItemHopsPerMoveAttributeMultiItem);

    const transitionTimeMs = getNumberOrDefaultFromStringMultiItem(transitionTimeMsString);
    const displayedItemsAtOnce = getNumberOrDefaultFromStringMultiItem(displayedItemsAtOnceString, 1);
    const itemHopsPerMove = getNumberOrDefaultFromStringMultiItem(itemHopsPerMoveString, 1);

    const allRelatedItems = getItemsToMoveForwardMultiItem(carouselItems, itemHopsPerMove, displayedItemsAtOnce);

    allRelatedItems.forEach((item, i) => {
        const offset = i;

        item.classList.remove(activeItemClassMultiItem);

        item.style.transition = "";
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    void carousel.offsetWidth;

    allRelatedItems.forEach((item, i) => {
        const offset = i - itemHopsPerMove;

        if (offset >= 0) {
            item.classList.add(activeItemClassMultiItem);
        }

        setDefaultTransitionStyleMultiItem(item, transitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    })

    const firstActiveElement = allRelatedItems[itemHopsPerMove];

    setActiveIndicatorForItemMultiItem(carousel, displayedItemsAtOnce, carouselItems.indexOf(firstActiveElement));

    carousel.setAttribute("aria-activedescendant", firstActiveElement.id);
}

function prevMultiItem(carouselId) {
    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClassMultiItem}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttributeMultiItem);
    const displayedItemsAtOnceString = carousel.getAttribute(carouselDisplayedItemsAtOnceAttributeMultiItem);
    const itemHopsPerMoveString = carousel.getAttribute(carouselItemHopsPerMoveAttributeMultiItem);

    const transitionTimeMs = getNumberOrDefaultFromStringMultiItem(transitionTimeMsString);
    const displayedItemsAtOnce = getNumberOrDefaultFromStringMultiItem(displayedItemsAtOnceString, 1);
    const itemHopsPerMove = getNumberOrDefaultFromStringMultiItem(itemHopsPerMoveString, 1);

    const allRelatedItems = getItemsToMoveBackwardsMultiItem(carouselItems, itemHopsPerMove, displayedItemsAtOnce);

    allRelatedItems.forEach((item, i) => {
        const offset = i - itemHopsPerMove;

        item.classList.remove(activeItemClassMultiItem);

        item.style.transition = "";
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    void carousel.offsetWidth;

    allRelatedItems.forEach((item, i) => {
        const offset = i;

        if (offset < displayedItemsAtOnce) {
            item.classList.add(activeItemClassMultiItem);
        }

        setDefaultTransitionStyleMultiItem(item, transitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    const firstActiveElement = allRelatedItems[0];

    setActiveIndicatorForItemMultiItem(carousel, displayedItemsAtOnce, carouselItems.indexOf(firstActiveElement));

    carousel.setAttribute("aria-activedescendant", firstActiveElement.id);
}

function goToMultiItem(carouselId, firstItemIndex) {

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClassMultiItem}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttributeMultiItem);
    const goToTransitionTimeMsString = carousel.getAttribute(carouselGoToTransitionTimeMsAttributeMultiItem);
    const displayedItemsAtOnceString = carousel.getAttribute(carouselDisplayedItemsAtOnceAttributeMultiItem);

    const transitionTimeMs = getNumberOrDefaultFromStringMultiItem(transitionTimeMsString);
    const goToTransitionTimeMs = getNumberOrDefaultFromStringMultiItem(goToTransitionTimeMsString);
    const displayedItemsAtOnce = getNumberOrDefaultFromStringMultiItem(displayedItemsAtOnceString, 1);

    const actualTransitionTimeMs = goToTransitionTimeMs != 0 ? goToTransitionTimeMs : transitionTimeMs;

    if (carouselItems.length <= 1) return;

    const activeItems = getActiveItemsOrderedLTRMultiItem(carouselItems);

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

            item.classList.remove(activeItemClassMultiItem);

            setDefaultTransitionStyleMultiItem(item, actualTransitionTimeMs);
            item.style.transform = `translateX(${offset * 100}%)`;
        });

        itemsToShow.forEach((item, i) => {
            item.classList.add(activeItemClassMultiItem);

            setDefaultTransitionStyleMultiItem(item, actualTransitionTimeMs);
            item.style.transform = `translateX(${i * 100}%)`;
        });

        setActiveIndicatorAtMultiItem(carousel, targetPageIndex);

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

        item.classList.remove(activeItemClassMultiItem);

        setDefaultTransitionStyleMultiItem(item, actualTransitionTimeMs);
        item.style.transform = `translateX(${offset * 100}%)`;
    });

    itemsToShow.forEach((item, i) => {
        item.classList.add(activeItemClassMultiItem);

        setDefaultTransitionStyleMultiItem(item, actualTransitionTimeMs);
        item.style.transform = `translateX(${i * 100}%)`;
    });

    setActiveIndicatorAtMultiItem(carousel, targetPageIndex);
}

function getActiveItemsOrderedLTRMultiItem(carouselItems) {

    const items = [];

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(activeItemClassMultiItem)) {

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

function getItemsToMoveForwardMultiItem(carouselItems, hopsInMove, visibleItems) {

    const items = [];

    const totalItemCount = visibleItems + hopsInMove;

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(activeItemClassMultiItem)) {

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

function getItemsToMoveBackwardsMultiItem(carouselItems, hopsInMove, visibleItems) {

    const items = [];

    const totalItemCount = visibleItems + hopsInMove;

    for (var i = 0; i < carouselItems.length; i++) {

        const item = carouselItems[i];

        if (item.classList.contains(activeItemClassMultiItem)) {

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

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClassMultiItem}`)];

    const displayedItemsAtOnceString = carousel.getAttribute(carouselDisplayedItemsAtOnceAttributeMultiItem);

    const displayedItemsAtOnce = getNumberOrDefaultFromStringMultiItem(displayedItemsAtOnceString, 1);

    if (displayedItemsAtOnce === newDisplayedItemsAtOnce) return;

    const differenceBetweenOldAndNewItems = newDisplayedItemsAtOnce - displayedItemsAtOnce;

    var activeItems = getActiveItemsOrderedLTRMultiItem(carouselItems);

    carousel.setAttribute(carouselDisplayedItemsAtOnceAttributeMultiItem, newDisplayedItemsAtOnce);

    if (differenceBetweenOldAndNewItems > 0) {

        const lastItem = activeItems[activeItems.length - 1];

        var lastItemIndex = carouselItems.indexOf(lastItem);

        for (var i = 1; i <= differenceBetweenOldAndNewItems; i++) {

            lastItemIndex += 1;

            if (lastItemIndex >= carouselItems.length) {
                lastItemIndex = lastItemIndex % carouselItems.length;
            }

            const newItem = carouselItems[lastItemIndex];

            newItem.classList.add(activeItemClassMultiItem);

            activeItems.push(newItem);
        }
    }
    else {

        for (var i = 0; i > differenceBetweenOldAndNewItems; i--) {

            const itemToRemoveIndex = activeItems.length - 1;

            activeItems[itemToRemoveIndex].classList.remove(activeItemClassMultiItem);

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

    const carouselList = carousel.querySelector(`.${carouselIndicatorsListClassMultiItem}`);

    const indicators = [...carouselList.children];

    if (indicators == null || indicators.length === 0) return;

    const firstIndicator = indicators[0];

    const indicatorPagesCount = Math.ceil(carouselItems.length / newDisplayedItemsAtOnce);

    const targetPageIndex = Math.floor(firstActiveItemIndex / newDisplayedItemsAtOnce);

    firstIndicator.classList.remove(activeIndicatorClassMultiItem);

    carouselList.innerHTML = "";

    for (var i = 0; i < indicatorPagesCount; i++) {
        const indicator = firstIndicator.cloneNode(true);

        const startIndex = i * newDisplayedItemsAtOnce;

        if (i === targetPageIndex)
        {
            indicator.classList.add(activeIndicatorClassMultiItem);
        }

        indicator.id = `carousel-option-${i}`;

        indicator.onclick = null;

        indicator.addEventListener("click", function () {
            goToMultiItem(carouselId, startIndex);
        });

        carouselList.appendChild(indicator);
    }
}

function getCarouselIndicatorsMultiItem(carousel)
{
    const carouselList = carousel.querySelector(`.${carouselIndicatorsListClassMultiItem}`);

    const carouselIndicators = [...carouselList.children];

    return carouselIndicators;
}

function setActiveIndicatorForItemMultiItem(carousel, displayedItemsAtOnce, index) {
    const targetPageIndex = Math.floor(index / displayedItemsAtOnce);

    setActiveIndicatorAtMultiItem(carousel, targetPageIndex);
}

function setActiveIndicatorAtMultiItem(carousel, index)
{
    const carouselIndicators = getCarouselIndicatorsMultiItem(carousel);

    if (index < 0 || index >= carouselIndicators.length) return;

    for (var i = 0; i < carouselIndicators.length; i++) {

        const indicator = carouselIndicators[i]

        if (indicator.classList.contains(activeIndicatorClassMultiItem))
        {
            indicator.classList.remove(activeIndicatorClassMultiItem);

            break;
        }
    }

    carouselIndicators[index].classList.add(activeIndicatorClassMultiItem);
}

function getNumberOrDefaultFromStringMultiItem(stringValue, defaultValue = 0) {
    if (stringValue == null && stringValue != "") return defaultValue;

    var output = 0;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}