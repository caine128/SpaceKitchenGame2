using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildOptionsPanel_Manager : Panel_Base
{
    [SerializeField] private BuildOptionsButton[] buildOptionsButtons;
    [SerializeField] private TextMeshProUGUI propDisplayName;
    private RectTransform propDisplayTextField_Rt;
    [SerializeField] private ContentDisplay_JustSpriteAndText propIconContentDisplay;
    private RectTransform propIconContentDisplay_Rt;

    private void Awake()
    {
        propDisplayTextField_Rt = propDisplayName.GetComponent<RectTransform>();
        propIconContentDisplay_Rt = propIconContentDisplay.GetComponent<RectTransform>();
    }
    public void LoadPanel(Prop prop)
    {
        propIconContentDisplay.Unload();

        switch (prop.ShopUpgradeBluePrint) // TODO : Check if is a purchased one, maybe it's not the "fixed" parameter
        {
            case WorkStationUpgrade when !prop.IsPurchased:
                propDisplayName.text = prop.ShopUpgradeBluePrint.GetName();
                var spriteToLoad = ImageManager.SelectSprite("StarIconRed"); // TODO : Later to Load Proper Sprite for Level Background
                propIconContentDisplay.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: prop.ShopUpgradeBluePrint.GetLevel().ToString(), spriteRef_IN: spriteToLoad));
                GUI_CentralPlacement.PlaceImageWithText(textToplace: propDisplayName, textToplaceRect: propDisplayTextField_Rt,
                                                        imageToPlaceRect: propIconContentDisplay_Rt, isImageOnLeft: true);


                buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Rotate);
                buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);
                buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Apply);
                break;
            default:
                break;
        }
    

    }
}
