using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LadderContainer_Big : LadderContainer
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private ContentDisplay_JustSpriteAndText contentValue;
    [SerializeField] private ContentDisplay_JustSprite[] secondaryImageContainers;  //the background image and the primary image is defined on the Container<T> class

    private string[] descriptions = new string[]
    {
        "Increases Worker XP!!",
        "Reduce Surcharge Energy!",
        "More chance to MultiCraft!",
        "More Quality Items!",
        "Economy Of Ingredients!",
    };

    public override void LoadContainer(AscensionRewardState ascensionRewardState)
    {
        bluePrint = ascensionRewardState;
        header.text = ascensionRewardState.GetName();
        description.text = ascensionRewardState.GetDescription(); // GetAscensionTreeRewardDescription(ascensionRewardState.reward.ascensionTreeRewardType);
        //contentValue.text = ascensionRewardState.reward.GetAscensionTreeRewardValue();
        //_isUnlocked = ascensionRewardState.isUnlocked;
        //_isclaimed = ascensionRewardState.IsClaimed;
        SetContainerColor();


        float offsetDistance = secondaryImageContainers[0].RT.rect.height / 4;
        float lastVertexOfLastContainer;
        var ascensionRewardStateImageRefs = ascensionRewardState.GetAdressableImages();
        var ascensionRewardStateImageRefsCount = ascensionRewardStateImageRefs.Count();
        switch (ascensionRewardStateImageRefsCount)
        {
            case 1:
                if (mainImageContainer.gameObject.activeInHierarchy != true) mainImageContainer.gameObject.SetActive(true);
                mainImageContainer.LoadSprite(ascensionRewardStateImageRefs.First());
                GUI_CentralPlacement.DeactivateUnusedContainers(0, secondaryImageContainers);
                lastVertexOfLastContainer = mainImageContainer.rectTransform.rect.xMax;
                //contentValue.RT.anchoredPosition = new Vector2(lastVertexOfLastContainer + offsetDistance, contentValue.RT.anchoredPosition.y);
                contentValue.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: ascensionRewardState.reward.GetAscensionTreeRewardValue(),
                                                                          spriteRef_IN: ImageManager.SelectSprite("StarIconRed")));
                break;
            case > 1 and < 5:
                if(mainImageContainer.gameObject.activeInHierarchy !=false) mainImageContainer.gameObject.SetActive(false);
                secondaryImageContainers.PlaceContainers(requiredAmount: ascensionRewardStateImageRefsCount,
                                                         containerWidth: secondaryImageContainers[0].RT.rect.width,
                                                         isHorizontalPlacement: true,
                                                         offsetDistance_IN: offsetDistance,
                                                         shiftDistance: contentValue.RT.rect.width + offsetDistance);
                secondaryImageContainers.LoadContainers(loadData_IN: ascensionRewardStateImageRefs.ConvertEnumerable(converter: sprite =>
                                                                                                                                new ContentDisplayInfo_JustSprite(spriteRef_IN: sprite)),
                                                        hideAtInit: false);

                if (contentValue.RT.gameObject.activeInHierarchy != true) contentValue.RT.gameObject.SetActive(true);

                lastVertexOfLastContainer = secondaryImageContainers[ascensionRewardStateImageRefsCount - 1].RT.rect.xMax;
                //contentValue.RT.anchoredPosition = new Vector2(lastVertexOfLastContainer + offsetDistance, contentValue.RT.anchoredPosition.y);
                contentValue.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: ascensionRewardState.reward.GetAscensionTreeRewardValue(),
                                                                          spriteRef_IN: ImageManager.SelectSprite("StarIconRed")));


                break;
            default:
                Debug.LogError("shouldnt ve working");
                break;
        }
    }

    public override void MatchContainerDynamicInfo() // later to remove this is not necessary here 
    {
        Debug.LogError("shouldnt ve working");
        throw new System.NotImplementedException();
    }

    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        contentValue.Unload();
        foreach (var secondaryImageContainer in secondaryImageContainers)
        {
            secondaryImageContainer.Unload();
        }
       //_isUnlocked = _isclaimed = false;
    }

    public override void SetContainerColor()
    {
        containerImage.color = bluePrint.IsClaimed
                                    ? Color.blue
                                    : Color.red;
    }
}
