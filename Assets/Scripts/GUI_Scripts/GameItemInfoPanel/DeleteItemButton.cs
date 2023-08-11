using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GUI_TintScale))]
public class DeleteItemButton : PanelInvokeButton
{
    [SerializeField] protected GUI_TintScale gUI_TintScale;

    public sealed override void OnPointerDown(PointerEventData eventData)
    {
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        gUI_TintScale.TintSize();
        var selectedItem = GameItemInfoPanel_Manager.Instance.SelectedRecipe;
 
        if(PanelToInvoke.MainPanel is DeleteItemPopupPanel)
        {
            var panelLoadData = new PanelLoadData(
                mainLoadInfo: selectedItem, 
                panelHeader: null, tcs_IN: null);

            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN:
                () => DeleteItemPopupPanel.Instance.LoadPanel(panelLoadData));

        }
    }

}
