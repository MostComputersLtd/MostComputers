export function toggleElementDisplay(elementId)
{
    const element = document.getElementById(elementId);

    if (element.style.display === "none")
    {
        element.style.display = "";
    }
    else
    {
        element.style.display = "none";
    }
}