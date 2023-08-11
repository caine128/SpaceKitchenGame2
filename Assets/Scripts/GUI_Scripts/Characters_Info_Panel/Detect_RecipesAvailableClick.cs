using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_RecipesAvailableClick : DetectClickRequest<ProductRecipe>
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(isValidClick && (Vector2.Distance(initialClickPosition,eventData.position) <= PanelManager.MAXCLICKOFFSET))
        {
            if(initialSelection is RecipeContainer_Small recipeContainer_Small)
            {
                recipeContainer_Small.Tintsize();
                var blueprint = recipeContainer_Small.bluePrint;

                var panelToInvoke = PanelManager.InvokablePanels[typeof(RecipeInfoPanel_Manager)];
                var panelLoadData = new PanelLoadData(mainLoadInfo: blueprint, panelHeader: null, tcs_IN: null);

                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                             panelLoadAction_IN: () => RecipeInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
            }
        }
    }
}
