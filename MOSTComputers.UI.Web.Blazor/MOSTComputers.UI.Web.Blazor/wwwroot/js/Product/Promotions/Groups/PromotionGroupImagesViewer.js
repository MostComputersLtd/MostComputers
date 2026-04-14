const promotionGroupsListId = "promotionGroupsList";
const promotionImagesContainerId = "promotionImagesContainer";
const promotionGroupItemName = "promotionGroupElement";

async function selectGroupAndShowData(promotionGroupId, promotionGroupElementId, alwaysActivePromotionGroupId = null) {
    var url = "/api/GetPromotionGroupImagesHtml?promotionGroupId=" + promotionGroupId;

    if (alwaysActivePromotionGroupId != null) {
        url += "&alwaysActivePromotionGroupId=" + alwaysActivePromotionGroupId;
    }

    const newImagesHtmlResponse = await fetch(url,
    {
        method: "GET",
        headers: {
            'Accept': 'application/html'
        }
    });

    if (!newImagesHtmlResponse.ok) return;

    const promotionGroupElement = document.getElementById(promotionGroupElementId);

    const promotionGroupsList = document.getElementById(promotionGroupsListId);

    const promotionGroupElements = promotionGroupsList.querySelectorAll("[name='" + promotionGroupItemName + "']");

    for (const promotionGroupElement of promotionGroupElements) {
        if (element !== promotionGroupElement) {
            element.classList.remove("active");

            continue;
        }

        element.classList.add("active");
    }

    const newImagesHtml = await newImagesHtmlResponse.text();

    const promotionImagesContainer = document.getElementById(promotionImagesContainerId); 

    promotionImagesContainer.innerHTML = newImagesHtml;
}