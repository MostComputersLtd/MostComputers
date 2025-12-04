const selectedImageFileAttributeName = "data-selected-image-file";
const imageFileNameAttributeName = "data-image-file-name";

async function addNewImageFileToProduct(productId, fileInputElement, imageFilesPopupContentId)
{
    if (productId == null) return;

    const formBody = new FormData();

    formBody.append("fileInfo", fileInputElement.files[0]);

    const url = "/ProductEditor" + "?handler=AddNewImageFileToProduct" + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: "POST",
        headers:
        {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        data: formBody,
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = response.text();
    
    const imagesPopupModalContent = document.getElementById(imageFilesPopupContentId);

    imagesPopupModalContent.innerHTML = responseData;
}

async function update_productImageDisplay_ul_li_activeState(elementId, elementCheckBoxId, productId, fileName)
{
    if (displayOrder < 0) return;

    const element = document.getElementById(elementId);
    const elementCheckbox = document.getElementById(elementCheckBoxId);

    const checkboxIsChecked = elementCheckbox.checked;

    const url = "/ProductEditor" + "?handler=UpdateImageActiveStatus" + "&productId=" + productId + "&fileName=" + fileName + "&active=" + checkboxIsChecked;

    const response = await fetch(url,
    {
        method: "PUT",
        headers: {
            'Content-Type': "application/json",
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        data: null,
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const opacityOfLi = checkboxIsChecked ? 1 : 0.35;

    element.style.opacity = opacityOfLi;
}

function changeSelectedImageFile(imageFileElementId)
{
    const imageFileElement = document.getElementById(imageFileElementId);

    const previousSelectedElement = document.querySelector('[' + selectedImageFileAttributeName + ']');

    if (previousSelectedElement != null)
    {
        previousSelectedElement.removeAttribute(selectedImageFileAttributeName);
    }

    imageFileElement.setAttribute(selectedImageFileAttributeName, "true");
}

function copySelectedImageFileDataToClipboard(notificationBoxId = null)
{
    const selectedElement = document.querySelector('[' + selectedImageFileAttributeName + ']');

    if (selectedElement == null) return;
    
    const selectedImageData = selectedElement.src;

    if (selectedImageData == null) return;

    navigator.clipboard.writeText(selectedImageData);

    showNotificationWithText(notificationBoxId, "Copied!", "notificationBox-short-message", 2000);
}