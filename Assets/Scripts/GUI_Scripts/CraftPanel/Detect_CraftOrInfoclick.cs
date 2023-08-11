using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_CraftOrInfoclick : DetectClickRequest<ProductRecipe>, IMultiPanelInvokeButton
{
    [SerializeField] private Radial_CraftSlots_Crafter radial_CraftSlots_Crafter;

    public InvokablePanelController[] InvokablePanels { get { return _invokablePanels; } }
    [SerializeField] private InvokablePanelController[] _invokablePanels;

    public event Action<InvokablePanelController, Action> OnInvokeButtonPressed;

    public override async void OnPointerUp(PointerEventData eventData)
    {
        if (isValidClick && (Vector2.Distance(initialClickPosition,eventData.position) <= PanelManager.MAXCLICKOFFSET)) //initialClickPosition == eventData.position)
        {
            if (initialSelection is OpenRecipeInfoPanelButton openPopupPanelSelection)
            {
                var bluePrint = openPopupPanelSelection.parentContainer.bluePrint;

                if(_invokablePanels[0].MainPanel is RecipeInfoPanel_Manager)
                {
                    var panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                    PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[0], panelLoadAction_IN: () => RecipeInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
                }
                else
                {
                    Debug.Log("Wrong Panel Type Sent To Load");
                }

            }
            else if (initialSelection is RecipeContainer recipeContainerSelection)
            {
                recipeContainerSelection.Tintsize();
                await radial_CraftSlots_Crafter.TryStartCraftingAsync(recipeContainerSelection.bluePrint);
                /*if (recipeContainerSelection.bluePrint.IsUnlocked() == false && recipeContainerSelection.bluePrint.IsResearched() == false)
                {
                    if (recipeContainerSelection.bluePrint.recipeSpecs.unlockPrerequisite.Length > 0)
                    {
                        var bluePrint = recipeContainerSelection.bluePrint;
                        if(_invokablePanels[2].MainPanel is UnlockRecipePopupPanel)
                        {
                            var panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                            PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[2], panelLoadAction_IN: () => UnlockRecipePopupPanel.Instance.LoadPanel(panelLoadData));
                        }
                        else
                        {
                            Debug.Log("Wrong Panel Type Sent To Load");
                        }
                    }
                }
                else if (recipeContainerSelection.bluePrint.IsUnlocked() == true && recipeContainerSelection.bluePrint.IsResearched() == false)
                {
                    var bluePrint = recipeContainerSelection.bluePrint;
                    if(_invokablePanels[1].MainPanel is ResearchPopupPanel)
                    {
                        var panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                        PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[1], panelLoadAction_IN: () => ResearchPopupPanel.Instance.LoadPanel(panelLoadData));
                    }
                    else
                    {
                        Debug.Log("Wrong Panel Type Sent To Load");
                    }
                 
                }
                else
                {
                    await radial_CraftSlots_Crafter.TryStartCraftingAsync(recipeContainerSelection.bluePrint);
                }*/
            }

            initialSelection = null;
            initialClickPosition = default(Vector2);
            return;
        }

        initialSelection = null;
        initialClickPosition = default(Vector2);
    }



    public void InvokePanel(int invokablePanelIndex_IN, Action panelLoadAction)
    {
        OnInvokeButtonPressed?.Invoke(_invokablePanels[invokablePanelIndex_IN], panelLoadAction);
    }

}
