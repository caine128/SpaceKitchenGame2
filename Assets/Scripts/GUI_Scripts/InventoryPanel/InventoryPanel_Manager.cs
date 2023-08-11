using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel_Manager : ScrollablePanel<GameObject, GameItemType.Type, Sort.Type>, IReassignablePanel 
{
    //public bool IsPanelReassigned { get; private set; } = false;
    public IReassignablePanel.AssignedState panelAssignedState { get; private set; } = IReassignablePanel.AssignedState.Default;
    public (Enum maintypeSelector, Enum subtypeSelector) ReassignedSelectorType { get; private set; }

    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 11;
    protected override void Start()
    {
        base.Start();

        activeSelection_MainType = GameItemType.Type.Product;// activeSelection.active_MainType = GameItemType.Type.Product;
        activeSelection_SubtType = Sort.Type.None;// activeSelection.active_SubtType = Sort.Type.None;

        ExecuteEvents.Execute(mainType_Selector_Buttons[1].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }


    protected override List<GameObject> CreateSortList() //GameItemType.Type mainSelector, Sort.Type subSelector)
    {
        var mainSelector = activeSelection_MainType; // var mainSelector = activeSelection.active_MainType;
        var subSelector = activeSelection_SubtType;  //var subSelector = activeSelection.active_SubtType;
        List<GameObject> listToSort = new List<GameObject>();

        if (panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromProductToEnhance)
        {
                foreach (Enhancement enhancement in Inventory.InventoryIterationDict[GameItemType.Type.Enhancement])
                {
                    if (enhancement.GetEnhancementType().Equals(ReassignedSelectorType.subtypeSelector))
                    {
                        listToSort.Add(enhancement);
                    }
                }
            DefineSortType(Sort.Type.Level, listToSort);
        }
        else if (panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct)
        {
            var enhancementTypeSubSelector = (EnhancementType.Type)ReassignedSelectorType.subtypeSelector;
            foreach (Product product in Inventory.InventoryIterationDict[GameItemType.Type.Product])
            {
                if (product.CanEnhanceWith(enhancementTypeSubSelector))
                {
                    listToSort.Add(product);
                }
            }
        }

        else
        {
            listToSort = Inventory.InventoryIterationDict[mainSelector];
            DefineSortType(subSelector, listToSort);
        }

        return listToSort;
    }


    public void RefreshPanel()
    {
        var listToSort = CreateSortList();//activeSelection.active_MainType, activeSelection.active_SubtType);
        ArrangeAndSort(listToSort);
    }


    public void ReassignPanelLayout<T_Type, T_SubType>(T_Type mainTypeSelection_IN, T_SubType subtypeSelection_IN, IReassignablePanel.AssignedState assignedState_IN)
        where T_Type : System.Enum
        where T_SubType : System.Enum
    {
        switch (assignedState_IN)
        {
            case IReassignablePanel.AssignedState.Default:
                AssignToOriginalPanelLayout();
                break;
            case IReassignablePanel.AssignedState.Inventory_FromProductToEnhance:
                panelAssignedState = assignedState_IN;
                ReassignedSelectorType = (mainTypeSelection_IN, subtypeSelection_IN);
                break;
            case IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct:
                ReassignedSelectorType = (mainTypeSelection_IN, subtypeSelection_IN);
                panelAssignedState = assignedState_IN;
                break;
            //case IReassignablePanel.AssignedState.Inventory_ForReplacingEnhancemnet:
            //    ReassignedSelectorType = (mainTypeSelection_IN, subtypeSelection_IN);
            //    panelAssignedState = assignedState_IN;
            //    break;
        }



        foreach (var mainTypeSelectorButtons in mainType_Selector_Buttons)
        {
            mainTypeSelectorButtons.SetButtonImageVisibility(false);
        }

        foreach (var subTypeSelectorButtons in subType_SelectorButtons)
        {
            subTypeSelectorButtons.SetButtonImageVisibility(false);
        }
    }

    public void AssignToOriginalPanelLayout()
    {
        foreach (var mainTypeSelectorButtons in mainType_Selector_Buttons)
        {
            mainTypeSelectorButtons.SetButtonImageVisibility(true);
        }

        foreach (var subTypeSelectorButtons in subType_SelectorButtons)
        {
            subTypeSelectorButtons.SetButtonImageVisibility(true);
        }

        panelAssignedState = IReassignablePanel.AssignedState.Default;
        ReassignedSelectorType = (null, null);
    }


    /// <summary>
    /// IS THIS REALLY NECESSARY COMING FROM PARENT ??
    /// </summary>
    /// <param name="displayBuluPrint_In"></param>
    /// <exception cref="NotImplementedException"></exception>

    public override void ScrollToSelection(GameObject displayBuluPrint_In, bool markSelection)
    {
        //var equalityComparer = new prod();
        //float targetForwardPos = 0;
        int selectedContainerIndex = 0;
        for (int i = 0; i < RequestedBluePrints.Count; i++)
        {
            if (displayBuluPrint_In.Equals(RequestedBluePrints[i]))
            {
                selectedContainerIndex = i;
                //targetForwardPos = (CardWidth / 2 * i) + (CardWidth / 2 * (i + 1)) + (OffsetDistance * (i + 1));
                break;
            }
        }

        CalculateForwardPosAndScroll(selectedContainerIndex, markSelection);
    }
}
