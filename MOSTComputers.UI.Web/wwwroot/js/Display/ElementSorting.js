function sortTable(tableId, columnIndex, isDescending, getValueFn = cell => cell.innerText.toLowerCase(), excludedFirstRowsCount = 0)
{
    const table = document.getElementById(tableId);

    const rows = Array.from(table.rows).slice(excludedFirstRowsCount);

    const tbody = table.tBodies[0];

    rows.sort((rowA, rowB) =>
    {
        const cellA = rowA.cells[columnIndex];
        const cellB = rowB.cells[columnIndex];

        const valueA = getValueFn(cellA);
        const valueB = getValueFn(cellB);

        if (valueA === null) return isDescending ? -1 : 1;
        if (valueB === null) return isDescending ? 1 : -1;

        if (valueA < valueB) return isDescending ? 1 : -1;
        if (valueA > valueB) return isDescending ? -1 : 1;

        return 0;
    });

    rows.forEach(row => tbody.insertBefore(row, null));
}

function sortTableWithCustomData(tableId, columnIndex, isDescending, getValueFn, extraSortingParams, excludedFirstRowsCount = 0)
{
    const table = document.getElementById(tableId);

    const rows = Array.from(table.rows).slice(excludedFirstRowsCount);

    const tbody = table.tBodies[0];

    rows.sort((rowA, rowB) =>
    {
        const cellA = rowA.cells[columnIndex];
        const cellB = rowB.cells[columnIndex];

        const valueA = getValueFn(cellA, extraSortingParams);
        const valueB = getValueFn(cellB, extraSortingParams);

        if (valueA === null) return isDescending ? 1 : -1;
        if (valueB === null) return isDescending ? -1 : 1;

        if (valueA < valueB) return isDescending ? 1 : -1;
        if (valueA > valueB) return isDescending ? -1 : 1;

        return 0;
    });

    rows.forEach(row => tbody.insertBefore(row, null));
}