using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GUI_TintScale))]
public class UseEnhancement_Button : PanelInvokeButton //, IReassigner
{
    //public IReassignablePanel ReassignablePanel { get{ return _reassignablePanel; } }
    private static IReassignablePanel _reassignablePanel;

    [SerializeField] protected GUI_TintScale gUI_TintScale;
    public override void Awake()                 // LATER TO TAKE UP
    {
        base.Awake();
        _reassignablePanel = (IReassignablePanel)PanelToInvoke.MainPanel;
    }


    public sealed override void OnPointerDown(PointerEventData eventData)
    {

    }
    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        gUI_TintScale.TintSize();
        var selectedEnhancement = GameItemInfoPanel_Manager.Instance.SelectedRecipe as Enhancement;
        _reassignablePanel.ReassignPanelLayout(GameItemType.Type.Product, selectedEnhancement.GetEnhancementType(), IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct);
        PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN: null);
    }
}
