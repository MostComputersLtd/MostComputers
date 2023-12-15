function showNotificationWithText(notificationBoxId, text, elementClassList)
{
    let notificationLi = document.createElement("li");

    notificationLi.classList = elementClassList;
    notificationLi.innerHTML = text;

    document.getElementById(notificationBoxId).appendChild(notificationLi);

    setTimeout(() =>
    {
        document.getElementById(notificationBoxId).removeChild(notificationLi);
    },
    1200);
}