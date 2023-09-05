using System;
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

    private int requiredAmountOfButtons = 0;

    public IEnumerator[] CO { get => _co; }
    private IEnumerator[] _co = null;
    public GUI_LerpMethods PanelToAwait { get => _panelToAwait;}
    [SerializeField] private GUI_LerpMethods _panelToAwait;

    private void Awake()
    {
        propDisplayTextField_Rt = propDisplayName.GetComponent<RectTransform>();
        propIconContentDisplay_Rt = propIconContentDisplay.GetComponent<RectTransform>();

        for (int i = 0; i < buildOptionsButtons.Length; i++)
        {
            buildOptionsButtons[i].gameObject.transform.SetAsFirstSibling();
        }
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
                        requiredAmountOfButtons = 4; // TODO : should be 5 , now its 4 for test purposes 

                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Upgrade);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Content);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Skins);
                        buildOptionsButtons[3].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);
                        //buildOptionsButtons[4].SetupButton(ButtonFunctionType.BuildOptionsPanel.Edit); // TODO : Should be active, commented out for test purposes
                        break;
                    case false:
                        requiredAmountOfButtons = 5; // TODO : should be 3 , now its 5 for test purposes

                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Purchase);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Rotate);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);

                        buildOptionsButtons[3].SetupButton(ButtonFunctionType.BuildOptionsPanel.Upgrade);
                        buildOptionsButtons[4].SetupButton(ButtonFunctionType.BuildOptionsPanel.Content);
                        break;
                }
                break;

            default:
                break;
        }

        for (int i = 0; i < buildOptionsButtons.Length; i++)
        {
            buildOptionsButtons[i].gameObject.SetActive(i < requiredAmountOfButtons);
        }

    }

    public void DisplayContainers()
    {
        int mirroringMultiplier;
        int columnNo = 0;
        bool isIndexRound;
        float offsetBetween_X;
        float lastPosRoundX = 0; float lastPosOddX = 0;
        float finalPosX;
        if (requiredAmountOfButtons%2 == 0)
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0 || i == 1)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();

                /*isIndexRound = i % 2 == 0;
                mirroringMultiplier = isIndexRound ? 1 : -1;
                columnNo += isIndexRound ? 0 : 1;*/

                offsetBetween_X = i == 0 || i == 1
                                        ? buildOptionsButtons[i].RT.sizeDelta.x / 2
                                        : buildOptionsButtons[Mathf.Max(i - 2, 0)].RT.sizeDelta.x / 2 + buildOptionsButtons[i].RT.sizeDelta.x / 2;
                CalculatePositions(i);

                /*if (isIndexRound)
                {
                    finalPosX = lastPosRoundX + mirroringMultiplier * offsetBetween_X;
                    lastPosRoundX = finalPosX;
                }
                else
                {
                    finalPosX = lastPosOddX + mirroringMultiplier * offsetBetween_X;
                    lastPosOddX = finalPosX;
                }

                buildOptionsButtons[i].RT.anchoredPosition = new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y);*/

            }
        }
        else
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();

                /*isIndexRound = i % 2 == 0;
                mirroringMultiplier = isIndexRound ? 1 : -1;
                columnNo += isIndexRound ? 0 : 1;*/
                
                offsetBetween_X = i == 0 
                                        ? 0 
                                        : buildOptionsButtons[Mathf.Max(i - 2,0)].RT.sizeDelta.x/2 + buildOptionsButtons[i].RT.sizeDelta.x/2;

                CalculatePositions(i);

                /*if (isIndexRound)
                {
                    finalPosX = lastPosRoundX + mirroringMultiplier * offsetBetween_X;
                    lastPosRoundX = finalPosX;
                }
                else
                {
                    finalPosX = lastPosOddX + mirroringMultiplier * offsetBetween_X;
                    lastPosOddX = finalPosX;
                }

                buildOptionsButtons[i].RT.anchoredPosition = new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y);*/
            }
        }

        void CalculatePositions(int i)
        {
            isIndexRound = i % 2 == 0;
            mirroringMultiplier = isIndexRound ? 1 : -1;
            columnNo += isIndexRound ? 0 : 1;

            if (isIndexRound)
            {
                finalPosX = lastPosRoundX + mirroringMultiplier * offsetBetween_X;
                lastPosRoundX = finalPosX;
            }
            else
            {
                finalPosX = lastPosOddX + mirroringMultiplier * offsetBetween_X;
                lastPosOddX = finalPosX;
            }

            buildOptionsButtons[i].RT.anchoredPosition = new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y);
        }

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
