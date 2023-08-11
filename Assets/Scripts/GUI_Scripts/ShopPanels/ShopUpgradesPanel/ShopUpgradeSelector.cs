using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradeSelector : SelectorButton<ShopUpgradeType.Type>
{
    private IScrollablePanel<ShopUpgradeType.Type, Sort.Type> panel;
    public sealed override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<ShopUpgradeType.Type, Sort.Type>)panel;
    }

    public sealed override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter != this.gameObject)
        {
            panel.SortByEquipmentType(type, this);
        }
        else
        {
            if (panel.CheckActiveMainType(type))
            {
                return;
            }
            else
            {
                panel.SortByEquipmentType(type, this);                       //change the methodname 
            }
        }
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
    }
}
