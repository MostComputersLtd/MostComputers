namespace MOSTComputers.UI.Web.Blazor.Endpoints;

public static class PromotionPictureSource
{
    public static string? GetPromotionPictureSource(int? promotionPictureId)
    {
        return promotionPictureId switch
        {
            1 => "/img/ProductInfo/PL_1.gif",
            2 => "/img/ProductInfo/PL_2.gif",
            3 => "/img/ProductInfo/PL_3.gif",
            4 => "/img/ProductInfo/PL_4.gif",
            5 => "/img/ProductInfo/PL_5.gif",
            6 => "/img/ProductInfo/PL_6.gif",
            7 => "/img/ProductInfo/PL_7.gif",
            8 => "/img/ProductInfo/PL_8.gif",
            9 => "/img/ProductInfo/PL_9.gif",
            10 => "/img/ProductInfo/PL_10.gif",
            11 => "/img/ProductInfo/PL_11.gif",
            12 => "/img/ProductInfo/PL_12.gif",
            _ => null
        };
    }
}