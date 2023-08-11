using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GUI_TintScale))]
public class InvokeRecipeButton : PanelInvokeButton // WHY NOT TAKING THE TINT TO PARENT ?? 
{
    [SerializeField] protected GUI_TintScale gUI_TintScale;

    public sealed override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        gUI_TintScale.TintSize();
        var selectedCraftable = GameItemInfoPanel_Manager.Instance.SelectedRecipe as ICraftable;

        if(PanelToInvoke.MainPanel is RecipeInfoPanel_Manager)
        {
            var panelLoadData = new PanelLoadData(mainLoadInfo: selectedCraftable.GetProductRecipe(), 
                                                  panelHeader: null, 
                                                  tcs_IN: null);

            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelToInvoke, panelLoadAction_IN:
            () => RecipeInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
        }
        else
        {
            Debug.LogError("Wrong Panel Format");
        }
    }


   
}
