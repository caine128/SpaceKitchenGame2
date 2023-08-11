using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class AscensionLadderSelector_Button : SelectorButton<ProductType.Type> //, ISinglePanelInvokeButton
{
    private AscensionLadderPanel_Manager panel;

    //public InvokablePanelController PanelToInvoke => _panelToInvoke;
    //[SerializeField] private InvokablePanelController _panelToInvoke;

    public sealed override void AssignPanel(object panel)
    {
        this.panel = (AscensionLadderPanel_Manager)panel;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        var asceisonLadderPanelManager = PanelManager.InvokablePanels[typeof(AscensionLadderPanel_Manager)];
        PanelManager.ActivateAndLoad(invokablePanel_IN: asceisonLadderPanelManager,
                                     preLoadAction_IN: () => AscensionLadderPanel_Manager.activeSelection_MainType = type,
                                     panelLoadAction_IN: null);
        //PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN: () => panel.SortByEquipmentType(type,this));
        //if (eventData.pointerEnter != this.gameObject)
        //{
        //    panel.SortByEquipmentType(type, this);
        //}
        //else
        //{
        //    if (panel.CheckActiveMainType(type))
        //    {
        //        return;
        //    }
        //    else
        //    {

        //        panel.SortByEquipmentType(type, this);
        //    }
        //}

    }


    public override void OnPointerUp(PointerEventData eventData)
    {

    }
}
