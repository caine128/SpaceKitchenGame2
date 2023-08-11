using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReassignablePanel : IRefreshablePanel
{
    public enum AssignedState
    {
        Default,
        Inventory_FromProductToEnhance,
        Inventory_FromEnhanceToProduct,
        //Inventory_ForReplacingEnhancemnet,
    }

    public AssignedState panelAssignedState { get; }

   // bool IsPanelReassigned { get;  }
    (System.Enum maintypeSelector, System.Enum subtypeSelector) ReassignedSelectorType { get;  }

    void ReassignPanelLayout<T_Type,T_SubType>(T_Type mainTypeSelection, T_SubType subtypeSelection, AssignedState assignedState) 
        where T_Type : System.Enum
        where T_SubType : System.Enum;
    void AssignToOriginalPanelLayout();
}
