function highlightAllElementsTextsWhereValueIsPresent(elements, value, selectionColor, customSpanName)
{
    for (var i = 0; i < elements.length; i++)
    {
        var element = elements[i];

        element.innerHTML = element.innerHTML.replace(new RegExp(value, "g"), "<span name='" + customSpanName + "'style='line-height: normal; background-color:" + selectionColor + "'>" + value + "</span>");
    }
}

function removeHighlightAllElementsTextsWhereValueIsPresent(elements, value)
{
    for (var i = 0; i < elements.length; i++)
    {
        var element = elements[i];

        var stop = false;

        var startIndex = 0;

        while (!stop)
        {
            var startIndexOfSpan = element.innerHTML.indexOf("<span", startIndex);

            if (startIndexOfSpan < 0
                || startIndexOfSpan === null)
            {
                stop = true;

                break;
            }

            var endIndexOfSpan = element.innerHTML.indexOf("</span>") + 7;

            var substringBetweenSpan = element.innerHTML.substring(startIndexOfSpan, endIndexOfSpan);

            var dataWithExtra = substringBetweenSpan.substring(substringBetweenSpan.indexOf(">", 7));

            var indexOfSpanEnd = dataWithExtra.length - 7;

            var valueInSpan = dataWithExtra.substring(1, indexOfSpanEnd);

            if (valueInSpan === value)
            {
                element.innerHTML = element.innerHTML.replace(new RegExp(substringBetweenSpan, "g"), value);

                stop = true;

                break;
            }

            startIndex = endIndexOfSpan;
        }
    }
}