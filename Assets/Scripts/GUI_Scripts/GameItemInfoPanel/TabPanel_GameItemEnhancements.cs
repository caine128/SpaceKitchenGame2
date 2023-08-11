using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabPanel_GameItemEnhancements : TabPanel_Animated<Tab.GameItemInfoTabs>
{
    [SerializeField] private EnhanceButtons[] gameItemInfoPanelButtons;

    public override Tab.GameItemInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.GameItemInfoTabs _tabType;

    private void Awake()
    {
        co = new IEnumerator[1];
    }

    public override void LoadInfo()
    {
        if(GameItemInfoPanel_Manager.Instance.SelectedRecipe is IEnhanceable enhanceableBluePrint)
        {
            for (int i = 0; i < gameItemInfoPanelButtons.Length; i++)              // TODO : LATER TO ADD THE COOK STUFF AS A CONDITON AS WELL !!!
            {
                if (enhanceableBluePrint.CanEnhanceWith(gameItemInfoPanelButtons[i].EnhancementType))
                {
                    gameItemInfoPanelButtons[i].SetupButton(ButtonFunctionType.GameItemInfoPanel.SlotOpenToEnhancement);
                }
                else
                {
                    gameItemInfoPanelButtons[i].SetupButton(ButtonFunctionType.GameItemInfoPanel.SlotEnhanced);
                }

                gameItemInfoPanelButtons[i].ScaleDirect(isVisible: false,
                                                        finalValueOperations:null);
            }
        }
    }
    public override void HideContainers()
    {
        gameItemInfoPanelButtons.HideContainers();
    }

    public override void DisplayContainers()
    {
        gameItemInfoPanelButtons.SortContainers(customInitialValues:null, 
                                                secondaryInterpolations: null,
                                                amountToSort_IN: gameItemInfoPanelButtons.Length, 
                                                enumeratorIndex: 0, 
                                                parentPanel_IN: this,
                                                lerpSpeedModifiers: null);
    }


    public override void UnloadInfo()
    {
        for (int i = 0; i < gameItemInfoPanelButtons.Length; i++)
        {
            gameItemInfoPanelButtons[i].UnloadButton();
        }

        base.UnloadInfo();
    }


}
