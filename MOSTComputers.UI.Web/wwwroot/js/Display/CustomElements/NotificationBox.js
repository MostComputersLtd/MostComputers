function showNotificationWithText(notificationBoxId, text, elementClassList, timeToRemoveInMilliseconds = 1200)
{
    if (notificationBoxId == null) return;

    let notificationLi = document.createElement("li");

    notificationLi.classList = elementClassList;
    notificationLi.innerHTML = text;

    document.getElementById(notificationBoxId).appendChild(notificationLi);

    setTimeout(() =>
    {
        document.getElementById(notificationBoxId).removeChild(notificationLi);
    },
    timeToRemoveInMilliseconds);
}