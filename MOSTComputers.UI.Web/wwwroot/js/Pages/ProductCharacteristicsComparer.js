const tableSortedColumnSessionStorageKey = "table_sorted_column";

const tableSortableSingleItemAttributeName = "data-sortable-single-item";
const tableSortableMultipleItemPartAttributeName = "data-sortable-multiple-item-part";

const selectableItemRelationshipIndexAttributeName = 'data-selectable-item-in-relationship-with-index';
const selectableItemIndexAttributeName = "data-selectable-item-in-relationship-index";
const selectableItemTypeAttributeName = 'data-selectable-item-in-relationship-type';

const selectedItemAttributeName = 'data-product-characteristics-compare-selected-item';
const selectedItemClassname = "product-characteristics-compare-selected-item";

const relationshipAllowsDropAttributeName = "data-relationship-allows-drop-with-index";

const relationshipDragEnterClassname = "product-characteristics-compare-relationship-allowing-drop-drag-over";

function sortRelationshipTable(
    relationshipTableId,
    sortByCharacteristicsOrProperties,
    isDescending,
    innerColumnIndex)
{
    extraSortParams = [innerColumnIndex];

    sortTable(relationshipTableId, sortByCharacteristicsOrProperties, isDescending,
        (cell) => sortRelationshipsByCharacteristicColumn(cell, extraSortParams), 2);

    sessionStorage.setItem(tableSortedColumnSessionStorageKey, sortByCharacteristicsOrProperties + "," + innerColumnIndex);
}

function sortRelationshipTableWithSessionStorageData(relationshipTableId)
{
    const storedSortingData = sessionStorage.getItem(tableSortedColumnSessionStorageKey);

    if (storedSortingData == null) return;

    const indexOfCommaSeparator = storedSortingData.indexOf(',');

    const storedCharacteristicOrPropertySortIndexAsString = storedSortingData.substring(0, indexOfCommaSeparator);
    const columnToSortIndexAsString = storedSortingData.substring(indexOfCommaSeparator + 1);

    const storedCharacteristicOrPropertySortIndex = parseInt(storedCharacteristicOrPropertySortIndexAsString);
    const columnToSortIndex = parseInt(columnToSortIndexAsString);

    sortRelationshipTable(
        relationshipTableId,
        storedCharacteristicOrPropertySortIndex,
        false,
        columnToSortIndex)
}

function sortRelationshipsByCharacteristicColumn(cell, extraSortParams)
{
    const innerColumnIndex = extraSortParams[0];

    const singleCharacteristicItem = cell.querySelector(`[${tableSortableSingleItemAttributeName}]`);
    const multipleCharacteristicItem = cell.querySelector(`[${tableSortableMultipleItemPartAttributeName}]`);

    var cellValue = null;

    if (singleCharacteristicItem != null)
    {
        const firstRow = Array.from(singleCharacteristicItem.rows)[0];

        cellValue = firstRow.cells[innerColumnIndex].innerText;
    }
    else if (multipleCharacteristicItem != null)
    {
        cellValue = multipleCharacteristicItem.cells[innerColumnIndex].innerText;
    }

    const valueAsInt = parseFloat(cellValue);

    if (!isNaN(valueAsInt))
    {
        return valueAsInt
    }

    return (cellValue != null) ? cellValue.trim() : null;
}

async function compareExternalAndLocalDataForCategory(
    categoryIdSelectId,
    characteristicsRelationshipTablePartialViewContainerId,
    characteristicsRelationshipTableId,
    compareDataInCategoryButtonId,
    loaderElementId = null)
{
    const categoryIdSelect = document.getElementById(categoryIdSelectId);

    const categoryId = categoryIdSelect.value;

    if (categoryId == null || isNaN(parseInt(categoryId))) return;

    const url = "/ProductCharacteristicsComparer" + "?handler=CompareExternalAndLocalDataForCategory"
        + "&categoryId=" + categoryId;

    const promise = fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    const response = await awaitWithCallbacks(promise,
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); },
        function () { toggleViews(compareDataInCategoryButtonId, loaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    const characteristicsRelationshipTablePartialViewContainer = document.getElementById(characteristicsRelationshipTablePartialViewContainerId);

    characteristicsRelationshipTablePartialViewContainer.innerHTML = responseData.characteristicsRelationshipTablePartialViewText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

function isElementSelectable(element)
{
    return element.hasAttribute(selectableItemRelationshipIndexAttributeName);
}

function isElementSelected(element)
{
    const selectedAttribute = element.getAttribute(selectedItemAttributeName);

    if (selectedAttribute == null) return false;

    return selectedAttribute;
}

function doesRelationshipAllowDrop(element)
{
    return element.hasAttribute(relationshipAllowsDropAttributeName);
}

function getRelationshipToDropInIndex(element)
{
    return element.getAttribute(relationshipAllowsDropAttributeName);
}

function getRelationshipIndex(element)
{
    if (!isElementSelectable(element)) return null;

    return element.getAttribute(selectableItemRelationshipIndexAttributeName);
}

function getSelectableElementIndex(element)
{
    if (!isElementSelectable(element)) return null;

    return element.getAttribute(selectableItemIndexAttributeName);
}

function getSelectableElementType(element)
{
    return element.getAttribute(selectableItemTypeAttributeName);
}

function unselectElementsFromOtherRelationships(currentElement)
{
    const currentIndex = getRelationshipIndex(currentElement);
    const currentElementType = getSelectableElementType(currentElement);

    document.querySelectorAll('[' + selectedItemAttributeName + '="true"]')
        .forEach(element =>
        {
            if (getRelationshipIndex(element) !== currentIndex
                || getSelectableElementType(element) !== currentElementType)
            {
                element.removeAttribute(selectedItemAttributeName);

                element.classList.remove(selectedItemClassname);
            }
        });
}

function toggleElementSelectionState(element)
{
    if (element.getAttribute(selectedItemAttributeName) === 'true')
    {
        element.removeAttribute(selectedItemAttributeName);

        element.classList.remove(selectedItemClassname);
    }
    else
    {
        element.setAttribute(selectedItemAttributeName, 'true');

        element.classList.add(selectedItemClassname);
    }
}

function toggleElementSelect(elementId)
{
    const element = document.getElementById(elementId);

    if (element == null
        || !isElementSelectable(element)) return;

    if (!isElementSelected(element))
    {
        unselectElementsFromOtherRelationships(element);
    }

    toggleElementSelectionState(element);
}

async function onClickToggleElementSelectOrRelationshipMatch(elementId)
{
    toggleElementSelect(elementId);
}

function onStartDraggingRelationshipElements(event, elementId)
{
    const element = document.getElementById(elementId);

    if (!isElementSelected(element))
    {
        toggleElementSelect(elementId);
    }

    const selectedItems = document.querySelectorAll('[' + selectedItemAttributeName + '="true"]');

    var data =
    {
        relationshipIndex: getRelationshipIndex(element),
        elementsType: getSelectableElementType(element),
        elementData: []
    };

    selectedItems.forEach(item =>
    {
        const elementIndex = getSelectableElementIndex(item);

        data.elementData.push(
        {
            elementId: item.id,
            elementIndex: elementIndex
        });
    });

    event.dataTransfer.setData("application/json", JSON.stringify(data));
}

function onDragEnterRelationshipElements(event, relationshipRowElementId)
{
    event.preventDefault();

    var relationshipRowElement = document.getElementById(relationshipRowElementId);

    relationshipRowElement.classList.add(relationshipDragEnterClassname);
}

function onDragOverRelationshipElements(event)
{
    event.preventDefault();
}

function onDragLeaveRelationshipElements(event, relationshipRowElementId)
{
    event.preventDefault();

    var relationshipRowElement = document.getElementById(relationshipRowElementId);

    if (!relationshipRowElement.contains(event.relatedTarget))
    {
        relationshipRowElement.classList.remove(relationshipDragEnterClassname);
    }
}

async function onDropRelationshipElements(
    event,
    relationshipToDropInId,
    productCharacteristicsCompareTableContainerId,
    characteristicsRelationshipTableId)
{
   event.preventDefault();

    const relationshipToDropIn = document.getElementById(relationshipToDropInId);

    if (!doesRelationshipAllowDrop(relationshipToDropIn)) return;

    relationshipToDropIn.classList.remove(relationshipDragEnterClassname);

    const relationshipToMoveToIndex = getRelationshipToDropInIndex(relationshipToDropIn);

    const dataAsString = event.dataTransfer.getData("application/json");

    if (dataAsString == null || dataAsString === "") return;

    const data = JSON.parse(dataAsString);

    if (data.relationshipIndex === relationshipToMoveToIndex) return;

    const indexesOfElements = data.elementData.map(item => item.elementIndex);

    const response = await moveItemsFromOneRelationshipToAnother(data.relationshipIndex, relationshipToMoveToIndex, data.elementsType, indexesOfElements);

    redirectIfResponseIsRedirected(response);

    if (response.status != 200) return;

    const responseText = await response.text();

    if (responseText == null
        || responseText.length <= 0) return;

    const productCharacteristicsCompareTableContainer = document.getElementById(productCharacteristicsCompareTableContainerId);

    productCharacteristicsCompareTableContainer.outerHTML = responseText;

    sortRelationshipTableWithSessionStorageData(characteristicsRelationshipTableId);
}

function moveItemsFromOneRelationshipToAnother(
    indexOfRelationshipToMoveFrom,
    indexOfRelationshipToMoveTo,
    moveCharacteristicsOrProperties,
    indexesOfItemsToMove)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=MoveItemsFromOneRelationshipToAnother"
        + "&indexOfRelationshipToMoveFrom=" + indexOfRelationshipToMoveFrom
        + "&indexOfRelationshipToMoveTo=" + indexOfRelationshipToMoveTo
        + "&moveCharacteristicsOrProperties=" + moveCharacteristicsOrProperties;

    return fetch(url,
    {
        method: 'PUT',
        body: JSON.stringify(indexesOfItemsToMove),
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });
}

async function saveAllRelationships(characteristicsRelationshipTablePartialViewContainerId)
{
    const url = "/ProductCharacteristicsComparer" + "?handler=SaveAllRelationships";

    const response = await fetch(url,
    {
        method: 'POST',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    const characteristicsRelationshipTablePartialViewContainer = document.getElementById(characteristicsRelationshipTablePartialViewContainerId);

    characteristicsRelationshipTablePartialViewContainer.innerHTML = responseData.characteristicsRelationshipTablePartialViewText;
}