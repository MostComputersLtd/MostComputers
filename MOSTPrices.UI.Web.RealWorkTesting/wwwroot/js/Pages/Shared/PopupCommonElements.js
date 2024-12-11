function openModal(modalId, modalDialogId)
{
    let dialog = document.getElementById(modalDialogId);

    dialog.style.height = window.innerHeight;

    $("#" + modalId).modal("show");
}

function openModalWithData(modalId, modalDialogId, modalContentId, data)
{
    const dialog = document.getElementById(modalDialogId);
    const content = document.getElementById(modalContentId);

    dialog.style.height = window.innerHeight;

    if (data == null) return;

    content.innerHTML = data;

    $("#" + modalId).modal("show");
}

function openModalWithDataFunc(modalId, modalDialogId, modalContentId, getDataFunc)
{
    const dialog = document.getElementById(modalDialogId);
    const content = document.getElementById(modalContentId);

    dialog.style.height = window.innerHeight;

    const data = getDataFunc();

    if (data == null) return;

    content.innerHTML = data;

    $("#" + modalId).modal("show");
}

async function openModalWithDataFuncAsync(modalId, modalDialogId, modalContentId, getDataAsyncFunc)
{
    const dialog = document.getElementById(modalDialogId);
    const content = document.getElementById(modalContentId);

    dialog.style.height = window.innerHeight;

    const data = await getDataAsyncFunc();

    if (data == null) return;

    content.innerHTML = data;

    $("#" + modalId).modal("show");
}

function closeModal(modalId)
{
    $("#" + modalId).modal("toggle");
}