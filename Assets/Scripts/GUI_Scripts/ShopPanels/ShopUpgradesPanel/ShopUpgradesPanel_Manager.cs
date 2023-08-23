using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradesPanel_Manager : ScrollablePanel<ShopUpgrade, ShopUpgradeType.Type, Sort.Type>, IRefreshablePanel 
{
    public override int IndiceIndex => indiceIndex;
    private readonly int indiceIndex = 11;
    protected override void Start()
    {
        base.Start();

        activeSelection_MainType = ShopUpgradeType.Type.WorkstationUpgrades;
        activeSelection_SubtType = Sort.Type.None;  

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
    protected override List<ShopUpgrade> CreateSortList()
    {
        var mainSelector = activeSelection_MainType;
        var listToSort = ShopUpgradesManager.shopUpgradesAvilable_Dict[mainSelector];

        return listToSort;
    }

    public override void ScrollToSelection(ShopUpgrade displayBuluPrint_In, bool markSelection)
    {
        var equalityComparer = new ShopUpgradeEqualityComparer_ByName();
        int selectedContainerIndex = 0;
        for (int i = 0; i < RequestedBluePrints.Count; i++)
        {
            if (equalityComparer.Equals(RequestedBluePrints[i], displayBuluPrint_In))
            {
                selectedContainerIndex = i;
                break;
            }
        }

        CalculateForwardPosAndScroll(selectedContainerIndex, markSelection);
        
    }


    public void RefreshPanel()
    {
        var listToSort = CreateSortList();
        ArrangeAndSort(listToSort);
    }


}
