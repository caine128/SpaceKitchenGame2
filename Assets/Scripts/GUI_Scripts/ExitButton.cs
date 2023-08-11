using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitButton : PanelInvokeButton
{
    public sealed override void OnPointerDown(PointerEventData eventData)
    {
    }

    public  override void OnPointerUp(PointerEventData eventData)
    {

        if (PanelToInvoke.MainPanel is PopupPanel popupPanel)
        {
            var buttons = popupPanel.PopupButtons;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].StimulateWhenPanelExit)
                {
                    ExecuteEvents.Execute(buttons[i].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                    return;
                }
            }
        }

        PanelManager.DeactivatePanel(PanelToInvoke, nextPanelLoadAction_IN: null);
    }

    protected sealed override void SetState_ImageRaycast(Panel_Base.PanelState panelState)
    {
        switch (panelState)
        {
            case ScrollablePanel.PanelState.Active:
                if (buttonImage_Adressable.raycastTarget != true) buttonImage_Adressable.raycastTarget = true;
                break;
            case ScrollablePanel.PanelState.Deactivating:
                if (buttonImage_Adressable.raycastTarget != false) buttonImage_Adressable.raycastTarget = false;
                break;
            case ScrollablePanel.PanelState.Activating:
            case ScrollablePanel.PanelState.Inactive:
            default:
                break;
        }
    }
}
