namespace MOSTComputers.UI.Web.StaticUtilities;

public static class ImageFileNameInfoToImageDataUtils
{
    public static int? GetImageIdFromImageFileNameInfoName(string fileName)
    {
        int endIndexOfIdOfImageFromNameOfFileInfo = fileName.IndexOf('.');

        string idOfImageAsString = fileName[..endIndexOfIdOfImageFromNameOfFileInfo];

        bool succeededGettingIdFromFileInfoName = int.TryParse(idOfImageAsString, out int idOfImage);

        if (!succeededGettingIdFromFileInfoName) return null;

        return idOfImage;
    }
}
