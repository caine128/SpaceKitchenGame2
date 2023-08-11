using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftPanel_Button : PanelInvokeButton //MonoBehaviour,ISinglePanelInvokeButton    // TODO : CHANGE TO OANELINVOKE BUTTON WE HAVE THIS 
{
    public sealed override void OnPointerDown(PointerEventData eventData)
    {

    }


    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN: null);
    }



}
