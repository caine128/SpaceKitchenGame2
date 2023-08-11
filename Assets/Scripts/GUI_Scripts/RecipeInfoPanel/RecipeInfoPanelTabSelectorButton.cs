using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeInfoPanelTabSelectorButton : TabSelectorButton<Tab.RecipeInfoTabs>
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        //if (RecipeInfoPanel_Manager.Instance.activeTabType.Equals(_tabType))
        //{
        //    return;
        //}
        //else
        //{
        //    Debug.Log("LOADING THE TAB AGAIN");
            RecipeInfoPanel_Manager.Instance.PlaceTabPanel(_tabType, this);
        //}  
    }
}
