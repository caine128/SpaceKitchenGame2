using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradesSortSelector : SelectorButton<Sort.Type>
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
            panel.SortBySubType(type, this);
        }
        else
        {
            if (panel.CheckActiveSubType(type))
            {
                return;
            }
            else
            {
                panel.SortBySubType(type, this);
            }
        }
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
    }
}
