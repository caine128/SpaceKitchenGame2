using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildOptionsPanel_Manager : Panel_Base ,IDeallocatable , IAanimatedPanelController
{
    [SerializeField] private BuildOptionsButton[] buildOptionsButtons;
    [SerializeField] private TextMeshProUGUI propDisplayName;
    private RectTransform propDisplayTextField_Rt;
    [SerializeField] private ContentDisplay_JustSpriteAndText propIconContentDisplay;
    private RectTransform propIconContentDisplay_Rt;

    public IEnumerator[] CO { get => _co; }
    private IEnumerator[] _co = null;
    public GUI_LerpMethods PanelToAwait { get => _panelToAwait;}
    [SerializeField] private GUI_LerpMethods _panelToAwait;

    private void Awake()
    {
        propDisplayTextField_Rt = propDisplayName.GetComponent<RectTransform>();
        propIconContentDisplay_Rt = propIconContentDisplay.GetComponent<RectTransform>();
    }
    public void LoadPanel(Prop prop)
    {
        propIconContentDisplay.Unload();

        switch (prop.ShopUpgradeBluePrint) 
        {
            case WorkStationUpgrade:

                propDisplayName.text = prop.ShopUpgradeBluePrint.GetName();
                var spriteToLoad = ImageManager.SelectSprite("StarIconRed"); // TODO : Later to Load Proper Sprite for Level Background
                propIconContentDisplay.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: prop.ShopUpgradeBluePrint.GetLevel().ToString(), spriteRef_IN: spriteToLoad));
                GUI_CentralPlacement.PlaceImageWithText(textToplace: propDisplayName, textToplaceRect: propDisplayTextField_Rt,
                                                        imageToPlaceRect: propIconContentDisplay_Rt, isImageOnLeft: true);
                


                switch (prop.IsPurchased)
                {
                    case true:
                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Skins);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Upgrade);
                        buildOptionsButtons[3].SetupButton(ButtonFunctionType.BuildOptionsPanel.Content);
                        buildOptionsButtons[4].SetupButton(ButtonFunctionType.BuildOptionsPanel.Edit);
                        break;
                    case false:

                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Rotate);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Purchase);
                        break;
                }
                break;

            default:
                break;
        }


    }

    public void DisplayContainers()
    {
        Debug.LogWarning("DisplayContainers are being called should be implemented ");
    }

    public void HideContainers()
    {
        Debug.LogWarning("HideContainers are being called should be implemented ");

    }


    public void UnloadAndDeallocate()
    {
        propIconContentDisplay.Unload();
        foreach (var button in buildOptionsButtons)
        {
            button.UnloadButton();
        }
    }


}
