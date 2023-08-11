using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradesPanel_Manager : ScrollablePanel<ShopUpgrade, ShopUpgradeType.Type, Sort.Type>, IRefreshablePanel //, IDeallocatablePanel
{
    public override int IndiceIndex => indiceIndex;
    private readonly int indiceIndex = 11;
    protected override void Start()
    {
        base.Start();

        activeSelection_MainType = ShopUpgradeType.Type.WorkstationUpgrades;// activeSelection.active_MainType = ShopUpgradeType.Type.WorkstationUpgrades;
        activeSelection_SubtType = Sort.Type.None;  //activeSelection.active_SubtType = Sort.Type.None;

        ExecuteEvents.Execute(mainType_Selector_Buttons[0].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }

    protected override void Initialize_SubSelectionButtons()
    {
        base.Initialize_SubSelectionButtons();
        foreach ( SelectorButton<Sort.Type> subselectorButton in subType_SelectorButtons)
        {
            subselectorButton.SetButtonImageVisibility(isVisible:false);
        }
    }
    protected override List<ShopUpgrade> CreateSortList()//ShopUpgradeType.Type mainSelector, Sort.Type subSelector)
    {
        var mainSelector = activeSelection_MainType;// var mainSelector = activeSelection.active_MainType;
        var listToSort = ShopUpgradesManager.shopUpgradesAvilable_Dict[mainSelector];

        //DefineSortType(subSelector, listToSort);
        return listToSort;
    }

    public override void ScrollToSelection(ShopUpgrade displayBuluPrint_In, bool markSelection)
    {
        var equalityComparer = new ShopUpgradeEqualityComparer_ByName();
        //float targetForwardPos = 0;
        int selectedContainerIndex = 0;
        for (int i = 0; i < RequestedBluePrints.Count; i++)
        {
            if (equalityComparer.Equals(RequestedBluePrints[i], displayBuluPrint_In))
            {
                //targetForwardPos = (CardWidth / 2 * i) + (CardWidth / 2 * (i + 1)) + (OffsetDistance * (i + 1));
                selectedContainerIndex = i;
                break;
            }
        }

        CalculateForwardPosAndScroll(selectedContainerIndex, markSelection);
        
    }

    //protected override void SortByQuantity(List<ShopUpgrade> listToSort_IN)
    //{
    //    throw new System.NotImplementedException();
    //}

    public void RefreshPanel()
    {
        var listToSort = CreateSortList();// activeSelection.active_MainType, activeSelection.active_SubtType);
        ArrangeAndSort(listToSort);
    }


}
