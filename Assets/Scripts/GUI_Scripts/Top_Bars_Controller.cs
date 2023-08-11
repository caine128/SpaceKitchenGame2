using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Top_Bars_Controller : HUDBarsController, IAlternative_HUDBarsController
{

    [SerializeField] protected GUI_LerpMethods_Movement[] alternativeBars_LerpScripts;
    [SerializeField] protected Image alternativeBarBG;
    private Action followingAction = null;


    public override sealed void PanelControllerConfig()
    {
        base.PanelControllerConfig();

        alternativeBarBG = alternativeBars_LerpScripts[0].GetComponentInChildren<Image>();
        foreach (var bar in alternativeBars_LerpScripts)
        {
            bar.gameObject.SetActive(false);
        }
    }


    protected override void SetTargetPositions()
    {
        targetPositions = new Vector2[bars.Length];
        for (int i = 0; i < bars.Length; i++)
        {
            targetPositions[i] = new Vector2(bars[i].OriginalPos.x + MathF.Abs(bars[i].Rect.sizeDelta.x), bars[i].OriginalPos.y);
        }
    }

    public void ArrangeBarsInitial()
    {

        bars[0].InitialCall(targetPos: targetPositions[1]);

        followingAction = SetupAlternativeBarProps;
        followingAction += () => alternativeBars_LerpScripts[0].InitialCall(Vector2.zero);

        bars[3].InitialCall(targetPos: targetPositions[1], followingAction: followingAction);

    }

    /*
    IEnumerator ArrangeBarsInitialroutine(float lerpDuration)
    {
        barLerpScripts[3].InitialCallWithDiff(barWidths[1], lerpDuration, moveDirection: moveDirection);


        switch (PanelManager.SelectedPanels.Peek().PanelName)
        {
            case Panel.Name.craftPanel:
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.researchPoint, initialValue_IN: Inventory.Instance.CheckAmountInInventory_Name(SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.name, GameItemType.Type.SpecialItem));
                alternativeBarBG.color = Color.green;
                break;
            case Panel.Name.inventoryPanel:
                alternativeBarBG.color = Color.red;
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.inventoryCapacity, initialValue_IN: Inventory.ExistingItemAmount, maxValue_IN: Inventory.InventoryCapacity);
                break;
            case Panel.Name.shopPanel:
                alternativeBarBG.color = Color.blue;
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.shopCapacity, initialValue_IN: ShopData.ShopUpgradesAmount, maxValue_IN: ShopData.ShopCapacity);
                break;

        }


        while (barLerpScripts[3].RunningCoroutine != null)
        {
            yield return null;
        }


        alternativeBars[0].gameObject.SetActive(true);
        alternativeBars_LerpScripts[0].InitialCallWithPos(Vector2.zero, lerpDuration);


        co = null;
    }
    */

    private void SetupAlternativeBarProps()
    {
        alternativeBars_LerpScripts[0].gameObject.SetActive(true);
        switch (PanelManager.SelectedPanels.Peek().MainPanel) //(PanelManager.SelectedPanels.Peek().PanelName)
        {
            case CraftPanel_Manager:
            case RecipeInfoPanel_Manager://Panel.Name.craftPanel:
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.researchPoint, initialValue_IN: Inventory.Instance.CheckAmountInInventory_Name(SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.name, GameItemType.Type.SpecialItem));
                alternativeBarBG.color = Color.green;
                break;
            case InventoryPanel_Manager:
            case GameItemInfoPanel_Manager:///Panel.Name.inventoryPanel:
                alternativeBarBG.color = Color.red;
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.inventoryCapacity, initialValue_IN: Inventory.ExistingItemAmount, maxValue_IN: Inventory.InventoryCapacity);
                break;
            case ShopPanel_Manager:
            case ShopUpgradesPanel_Manager:
                alternativeBarBG.color = Color.blue;
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.shopCapacity, initialValue_IN: ShopData.ShopUpgradesAmount, maxValue_IN: ShopData.ShopCapacityMax);
                break;
        }
    }



    public void ArrangeBarsFinal()
    {
        bars[0].FinalCall();

        if (alternativeBars_LerpScripts[0].gameObject.activeInHierarchy)
        {
            followingAction = () => bars[3].FinalCall();
            alternativeBars_LerpScripts[0].FinalCall(deactivateSelf: true, followingAction: followingAction);
        }
        else
        {
            bars[3].FinalCall();
        }


    }




    /*
    IEnumerator ArrangeBarsFinalRoutine(float lerpDuration, int indexNO)
    {
        alternativeBars_LerpScripts[indexNO].FinalCall(lerpDuration, deactivateSelf: true);
        while (alternativeBars_LerpScripts[indexNO].RunningCoroutine != null)
        {
            yield return null;
        }
        barLerpScripts[3].FinalCall(lerpDuration);

        co = null;
    }*/
}
