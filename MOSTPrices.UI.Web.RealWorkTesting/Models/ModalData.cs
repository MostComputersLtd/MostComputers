namespace MOSTComputers.UI.Web.RealWorkTesting.Models;

public readonly struct ModalData
{
    public ModalData()
    {
        
    }

    public ModalData(string modalId, string modalDialogId, string modalContentId)
    {
        ModalId = modalId;
        ModalDialogId = modalDialogId;

        ModalContentId = modalContentId;
    }

    public string? ModalId { get; init; }
    public string? ModalDialogId { get; init; }
    public string? ModalContentId { get; init; }
}