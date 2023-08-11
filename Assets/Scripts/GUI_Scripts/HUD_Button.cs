
using UnityEngine;
using UnityEngine.EventSystems;

public class HUD_Button : PanelInvokeButton
{
    //public override void Awake()
    //{
    //    ScrollablePanel.OnPanelMoved += SetState_ImageRaycast;
    //}

    public sealed override void OnPointerDown(PointerEventData eventData)
    {

    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN: null);
    }

}
